using System;
using System.Collections.Generic;

namespace otherServices.Models;

public partial class Post
{
    public long PostId { get; set; }

    public long LandlordId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Price { get; set; }

    public string Location { get; set; } = null!;

    public string RentalStatus { get; set; } = null!;

    public long FlagWaitingPost { get; set; }
    public string ImagePath { get; set; }

    public DateTime DatePost { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User Landlord { get; set; } = null!;

    public virtual ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();

    public virtual ICollection<User> Tenants { get; set; } = new List<User>();

    public ICollection<Proposal> Proposals { get; set; }
}
