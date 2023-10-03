using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace Echoes_v0._1.Models;

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
    public string Description { get; set; }
    
    //Images or Videos go here once I figure out how to allow videos to display
    
    //temp images
    public String? ImageUrl { get; set; }
    
    public DateTime PostDate { get; set; }
    public DateTime? EditDate { get; set; }
    
    //Comments may go here but Filtering using which Post the comments belong to could be simpler
    
    
    //Empty Constructor 
    public PostModel()
    {
    }
}