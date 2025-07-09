#nullable disable

namespace SmartTourismAPI.Domain.Entities;

public partial class MPlaceDetail {
    public int DetailId { get; set; }

    public string OsmId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string OpeningHours { get; set; }

    public string Address { get; set; }

    public int TypeId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public int LastModifiedBy { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual MPlaceType Type { get; set; } = null!;

    public virtual ICollection<TPlacePhoto> TPlacePhotos { get; set; } = new List<TPlacePhoto>();

    public virtual ICollection<TPlaceReview> TPlaceReviews { get; set; } = new List<TPlaceReview>();
}
