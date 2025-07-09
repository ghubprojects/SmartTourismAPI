using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO.Esri;
using SmartTourismAPI.Domain.Entities;
using SmartTourismAPI.Infrastructure.DataContext;
using SmartTourismAPI.Infrastructure.Utils;
using System.Text.Json;

namespace SmartTourismAPI.Infrastructure.DataSeeding;

public class AppDbSeeder(AppDbContext context) {
    private readonly AppDbContext _context = context;
    private readonly Random _random = new();

    public async Task InitialiseAsync() {
        if (_context.Database.IsRelational())
            await _context.Database.MigrateAsync();
    }

    public async Task SeedAsync() {
        _context.Database.EnsureCreated();

        await SeedFileAsync();
        await SeedUserAsync();
        await SeedPlaceTypeAsync();
        await SeedPlaceAsync();
        await SeedPhotoAndReviewAsync();

        _context.ChangeTracker.Clear();
    }

    private async Task SeedFileAsync() {
        if (await _context.MFiles.AnyAsync())
            return;

        var avatarFiles = SeedUtil.GetAvatarUrls().Select(url => new MFile {
            FilePath = url,
            FileName = new Uri(url).Segments[^1],
        });
        var placePhotos = SeedUtil.GetImageUrls().Select(url => new MFile {
            FilePath = url,
            FileName = new Uri(url).Segments[^1],
        });

        await _context.MFiles.AddRangeAsync(avatarFiles);
        await _context.MFiles.AddRangeAsync(placePhotos);
        await _context.SaveChangesAsync();
    }

    private async Task SeedUserAsync() {
        if (await _context.MUsers.AnyAsync())
            return;

        var files = await _context.MFiles.AsNoTracking().ToListAsync();
        var users = Enumerable.Range(1, 20).Select(i => new MUser {
            Username = $"user{i:D2}",
            Email = $"user{i:D2}@gmail.com",
            PasswordHash = $"pass{i:D6}",
            AvatarId = i
        }).ToList();

        await _context.MUsers.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }

    private async Task SeedPlaceTypeAsync() {
        if (await _context.MPlaceTypes.AnyAsync())
            return;

        var fclassList = new List<string>();
        foreach (var feature in Shapefile.ReadAllFeatures(@"C:\Users\Adminis\Downloads\hanoi_pois.shp")) {
            fclassList.Add(feature.Attributes["fclass"]?.ToString() ?? string.Empty);
        }
        fclassList = fclassList.Distinct().ToList();

        var placeTypes = fclassList.Select(fclass => new MPlaceType {
            Fclass = fclass,
            TypeName = SeedUtil.GetPlaceType(fclass)
        }).ToList();

        await _context.MPlaceTypes.AddRangeAsync(placeTypes);
        await _context.SaveChangesAsync();
    }

    private async Task SeedPlaceAsync() {
        if (await _context.MPlaceDetails.AnyAsync())
            return;

        var placeDetails = new List<MPlaceDetail>();
        var _httpClient = new HttpClient();
        var _semaphore = new SemaphoreSlim(1, 1);
        var i = 0;
        foreach (var feature in Shapefile.ReadAllFeatures(@"C:\Users\Adminis\Downloads\hanoi_pois.shp")) {
            i++;
            var fclass = feature.Attributes["fclass"].ToString();
            var type = _context.MPlaceTypes.FirstOrDefault(t => t.Fclass == fclass);
            var nameAttr = feature.Attributes["name"]?.ToString();
            var name = !string.IsNullOrEmpty(nameAttr)
                ? nameAttr
                : (type?.TypeName is not null
                    ? char.ToUpper(type.TypeName[0]) + type.TypeName.Substring(1)
                    : string.Empty);

            string address = "";
            try {
                await _semaphore.WaitAsync();
                var nominatimBaseUrl = "https://nominatim.openstreetmap.org/reverse?format=jsonv2";
                string url = FormattableString.Invariant(
                    $"{nominatimBaseUrl}&lat={feature.Geometry.Coordinate.Y}&lon={feature.Geometry.Coordinate.X}");

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "YourAppName/1.0 (your@email.com)");
                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode) {
                    string json = await response.Content.ReadAsStringAsync();
                    var res = JsonSerializer.Deserialize<NominatimResponse>(json);
                    if (res != null) {
                        address = res.display_name;
                        if (string.IsNullOrEmpty(nameAttr) && !string.IsNullOrEmpty(res.name)) {
                            name = res.name;
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            } finally {
                await Task.Delay(1000);
                _semaphore.Release();
            }

            var placeDetail = new MPlaceDetail {
                OsmId = feature.Attributes["osm_id"].ToString(),
                Name = name,
                Description = SeedUtil.GetDescription(fclass, name),
                OpeningHours = SeedUtil.GetOpeningHours(fclass),
                TypeId = type.TypeId,
                Address = address,
            };
            placeDetails.Add(placeDetail);
        }

        await _context.MPlaceDetails.AddRangeAsync(placeDetails);
        await _context.SaveChangesAsync();
    }

    private async Task SeedPhotoAndReviewAsync() {
        if (await _context.TPlacePhotos.AnyAsync() || await _context.TPlaceReviews.AnyAsync())
            return;

        var files = await _context.MFiles.AsNoTracking().ToListAsync();
        var placePhotos = new List<TPlacePhoto>();
        var placeReviews = new List<TPlaceReview>();

        var placeDetails = await _context.MPlaceDetails
                                                        .Include(d => d.Type)
                                                        .AsNoTracking()
                                                        .ToListAsync();
        var users = await _context.MUsers
                                            .AsNoTracking()
                                            .ToListAsync();

        foreach (var placeDetail in placeDetails) {
            var placeType = placeDetail.Type.Fclass;
            var imageUrls = SeedUtil.GetImageUrls(placeType);

            placePhotos.AddRange(imageUrls.Select(url => new TPlacePhoto {
                DetailId = placeDetail.DetailId,
                FileId = files.First(f => f.FilePath == url).FileId
            }));

            placeReviews.AddRange(SeedUtil.GetComments(placeType).Select(comment => new TPlaceReview {
                DetailId = placeDetail.DetailId,
                UserId = users[_random.Next(users.Count)].UserId,
                Comment = comment,
                Rating = _random.Next(1, 6),
            }));
        }

        await _context.TPlacePhotos.AddRangeAsync(placePhotos);
        await _context.TPlaceReviews.AddRangeAsync(placeReviews);
        await _context.SaveChangesAsync();
    }
}

public class NominatimResponse {
    public string display_name { get; set; }
    public string name { get; set; }
}