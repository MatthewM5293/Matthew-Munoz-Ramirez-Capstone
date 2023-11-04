namespace Echoes_v0._1.Models
{
    public class HomeModel
    {
        public List<PostModel> Posts { get; set; }
        public List<PostModel> FollowedPosts { get; set; }

        public PostModel Post { get; set; }
    }
}
