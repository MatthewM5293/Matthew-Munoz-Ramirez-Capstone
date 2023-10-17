using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Echoes_v0._1.Models
{
    public class LikeModel
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("UserID")]
        public Guid UserId { get; set; }

        [ForeignKey("PostId")]
        public Guid PostId { get; set; }

        public LikeModel()
        {
        }
    }
}
