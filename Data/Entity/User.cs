using Common.Enums;
using Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class User
{
    [Key]
    public int UserID { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [JsonIgnore]

    public virtual ICollection<Keyword> SubscribedKeywords { get; set; } = new HashSet<Keyword>();
    [JsonIgnore]

    public virtual ICollection<News> SavedNews { get; set; } = new HashSet<News>();
    [JsonIgnore]

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new HashSet<UserSubscription>();
    [JsonIgnore]

    public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    [JsonIgnore]

    public virtual ICollection<Report> Reports { get; set; } = new HashSet<Report>();
    [JsonIgnore]

    public virtual ICollection<NewsLikeDislike> NewsLikeDislikes { get; set; } = new HashSet<NewsLikeDislike>();
}