using Echoes_v0._1.Models;

namespace Echoes_v0._1.Interfaces
{
    public interface IDataAccessLayer
    {
        //user methods
        public void EditUser(ApplicationUser au);
        public void DeleteUser(ApplicationUser au);

        public ApplicationUser GetUser(string? id);

        public string GetUserName(string userName);
        public bool IsValidUserName(string userName);

        public IEnumerable<ApplicationUser> GetUsers();

        public IEnumerable<PostModel> GetUserPosts(string? id);

        public PostModel GetUserPost(int? id);

        //post stuff
        //IEnumerable<PostModel> GetPosts();
        //void AddPost(PostModel Post);

        //void RemovePost(int? id);

        //void EditPost(PostModel post);

        //PostModel GetPost(int? id);



        //comments
        //public void AddComment(CommentModel Comment);

        //public void RemoveComment(int? id);

        //public void EditComment(CommentModel Comment);

        //public CommentModel GetComment(int? id);

        //IEnumerable<CommentModel> GetComments();

        //IEnumerable<CommentModel> GetPostComments(int? id);
    }
}
