using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace Echoesv2.Models;

/// <summary>
/// 
/// </summary>
public class PostModel
{
    /*
     * PostID
     * UserID (User it belongs to)
     *  User's Profile picture URL
     * 
     * Media (Image or Video)
     * Caption or Description
     * Date of Post
     * Date Edited
     * Comments
     */
    
    [Key] 
    public Guid PostId { get; set; }

    [ForeignKey("UserID")]
    public Guid UserId { get; set; }

    [Required]
    public string Caption { get; set; }
    
    ///Images or Videos go here once I figure out how to allow videos to display
    
    ///temp solution for images
    public String? ImageUrl { get; set; } //temporaily uses a url as the image source


    
    public DateTime PostDate { get; set; } = DateTime.Now; //sets Posted time to now
    public DateTime? EditDate { get; set; } //can be null as a post will not always be edited.

    ///Comments may go here but Filtering using which Post the comments belong to could be simpler
    //public List<CommentModel> Comments { get; set; }

    ///Toggle Comments feature
    //public bool CommentsEnabled { get; set; } = true;


    //Empty Constructor 
    public PostModel()
    {
    }
}