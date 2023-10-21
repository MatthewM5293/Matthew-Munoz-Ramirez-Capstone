using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Echoes_v0._1.Models;

public class CommentModel
{
    /*
     * CommentID
     * PostID
     * UserID
     * 
     * Username
     * Message (can't be null)
     * PostDate
     * EditDate
     */

    [Key]
    public Guid CommentId { get; set; }

    [ForeignKey("PostID")]
    public Guid PostId { get; set; }

    [ForeignKey("UserID")]
    public Guid UserId { get; set; }

    public string Username { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string Message { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime PostDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? EditDate { get; set; }

    public CommentModel()
    {
    }
}