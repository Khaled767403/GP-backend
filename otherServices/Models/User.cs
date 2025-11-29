using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace otherServices.Models;

public partial class User
{
    public long UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FName { get; set; } = null!;

    public string LName { get; set; } = null!;

    public int FlagWaitingUser { get; set; }

    public string RoleName { get; set; } = null!;

    public string Pass { get; set; } = null!;
    public string? FilePath { get; set; }


    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [JsonIgnore]

    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    [JsonIgnore]
    public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();

    public virtual ICollection<Post> PostsNavigation { get; set; } = new List<Post>();

    public ICollection<Proposal> Proposals { get; set; }



}
