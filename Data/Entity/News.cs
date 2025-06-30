using Data.Entity;
using System.ComponentModel.DataAnnotations;

public class News
{
    [Key]
    [Required]
    public int NewsID { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    [Url]
    [MaxLength(500)]
    public string Url { get; set; }

    [Url]
    [MaxLength(500)]
    public string ImageUrl { get; set; }

    [MaxLength(5000)]
    public string Content { get; set; }

    [MaxLength(200)]
    public string Source { get; set; }

    [MaxLength(200)]
    public string Author { get; set; }

    public DateTime PublishedAt { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(10)]
    public string Language { get; set; }

    public bool IsDisabled { get; set; } = false;

    public virtual ICollection<Keyword> Keywords { get; set; } = new HashSet<Keyword>();
    public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    public virtual ICollection<Report> Reports { get; set; } = new HashSet<Report>();
    public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    public virtual ICollection<NewsLikeDislike> NewsLikeDislikes { get; set; } = new HashSet<NewsLikeDislike>();
}
