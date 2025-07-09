using FluentValidation;
using MediatR;
using SmartTourismAPI.Application.Abstractions.Services;
using SmartTourismAPI.Application.DTOs;

namespace SmartTourismAPI.Application.Features.Places;

public class GetPlaceRequest : IRequest<List<PlaceDetailDto>> {
    public double CenterLongitude { get; set; }
    public double CenterLatitude { get; set; }
    public double RadiusMeters { get; set; }
    public string? Type { get; set; }
}

public class GetPlacesRequestHandler(IPlaceService placeService) : IRequestHandler<GetPlaceRequest, List<PlaceDetailDto>> {
    private readonly IPlaceService _placeService = placeService;

    public async Task<List<PlaceDetailDto>> Handle(GetPlaceRequest request, CancellationToken cancellationToken) {
        await new GetPlaceRequestValidator().ValidateAndThrowAsync(request, cancellationToken);
        return await _placeService.GetPlacesAsync(request.CenterLongitude, request.CenterLatitude, request.RadiusMeters, request.Type);
    }
}

public class GetPlaceRequestValidator : AbstractValidator<GetPlaceRequest> {
    public GetPlaceRequestValidator() {
        RuleFor(x => x.Type)
            .MaximumLength(255).WithMessage("Loại địa điểm không được vượt quá 255 ký tự.");

        RuleFor(x => x.CenterLatitude)
            .InclusiveBetween(-90, 90).WithMessage("Vĩ độ phải nằm trong khoảng từ -90 đến 90.");

        RuleFor(x => x.CenterLongitude)
            .InclusiveBetween(-180, 180).WithMessage("Kinh độ phải nằm trong khoảng từ -180 đến 180.");

        RuleFor(x => x.RadiusMeters / 1000)
            .GreaterThan(0).WithMessage("Bán kính tìm kiếm phải lớn hơn 0.")
            .LessThanOrEqualTo(10).WithMessage("Bán kính tìm kiếm không được vượt quá 10 km.");
    }
}