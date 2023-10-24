using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Echoes_v0._1.Models
{
    public class UserFollowModel
    {

        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// User being followed
        /// </summary>
        public Guid FollowingUserId { get; set; }
        
        /// <summary>
        /// User Clicking the follow Button
        /// </summary>
        public Guid CurrentUserId { get; set; }

        public UserFollowModel()
        {
        }
    }
}
