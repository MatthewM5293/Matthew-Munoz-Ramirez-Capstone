using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Echoes_v0._1.Models;

/// <summary>
/// 
/// </summary>
public class PostModel
{
    /*
     * PostID DONE
     * UserID (User it belongs to) DONE
     *  User's Profile picture URL 
     * 
     * Media (Image or Video) 
     * Caption or Description DONE
     * Date of Post Done
     * Date Edited Done
     * Comments Done
     */

    [Key]
    public Guid PostId { get; set; }

    [ForeignKey("UserID")]
    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public string? ProfilePicture { get; set; }

    [MaxLength(2000)]
    public string? Caption { get; set; }

    ///temp solution for images
    [DataType(DataType.ImageUrl)]
    public string? ImageUrl { get; set; } //temporaily uses a url as the image source

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime PostDate { get; set; } = DateTime.Now; //sets Posted time to now

    [DataType(DataType.DateTime)]
    public DateTime? EditDate { get; set; } //can be null as a post will not always be edited.

    //Toggle Comments feature
    public bool CommentsEnabled { get; set; } = true;

    //Likes
    public int LikeCount { get; set; } = 0;

    public List<CommentModel>? Comments { get; set; } //Comments filtered by ID 
    public List<LikeModel>? LikedBy { get; set; }

    //Empty Constructor 
    public PostModel()
    {
    }
}