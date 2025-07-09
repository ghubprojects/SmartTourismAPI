﻿#nullable disable

namespace SmartTourismAPI.Domain.Entities;

public partial class MUser {
    public int UserId { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public int AvatarId { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public string LastModifiedBy { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual MFile Avatar { get; set; } = null!;

    public virtual ICollection<TPlaceReview> TPlaceReviews { get; set; } = new List<TPlaceReview>();
}
