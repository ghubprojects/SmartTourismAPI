#nullable disable

namespace SmartTourismAPI.Domain.Entities;

public partial class MPlaceType {
    public int TypeId { get; set; }

    public string TypeName { get; set; }

    public string Fclass { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public string LastModifiedBy { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual ICollection<MPlaceDetail> MPlaceDetails { get; set; } = new List<MPlaceDetail>();
}
