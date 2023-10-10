using Echoesv2.Models;

namespace Echoesv2.Interfaces
{
    public interface IDataAccessLayer
    {
        //user methods
        public void EditUser(ApplicationUser au);
        public void DeleteUser(ApplicationUser au);

        public ApplicationUser GetUser(string? id);
        public ApplicationUser GetUserByEmail(string? email);

        public string GetUserName(string userId);

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
