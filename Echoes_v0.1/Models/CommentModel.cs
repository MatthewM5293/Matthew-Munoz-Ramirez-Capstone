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
     * Message (can be null)
     * PostDate
     * EditDate
     */

    [Key] 
    public Guid CommentId;
    
    [ForeignKey("PostID")]
    public Guid PostId;
    
    [ForeignKey("UserID")]
    public Guid UserId;

    public string Username;
    public string Message;
    public DateTime PostDate;
    public DateTime EditDate;
    
    public CommentModel()
    {
    }
}