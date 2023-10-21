using Echoes_v0._1.Interfaces;
using Echoes_v0._1.Models;

namespace Echoes_v0._1.Data
{
    public class EchoesDBDAL : IDataAccessLayer
    {
        private ApplicationDbContext EchoesDB;

        public EchoesDBDAL(ApplicationDbContext indb)
        {
            EchoesDB = indb;
        }

        public void EditUser(ApplicationUser au)
        {
            EchoesDB.ApplicationUsers.Update(au);
            EchoesDB.SaveChanges();
            //throw new NotImplementedException();
        }

        public void DeleteUser(ApplicationUser au)
        {
            //log them out, then remove (still in progress)
            EchoesDB.Remove(au);
            EchoesDB.SaveChanges();
            //throw new NotImplementedException();
        }

        public ApplicationUser GetUser(string? id)
        {
            return EchoesDB.ApplicationUsers.Where(user => user.Id.Equals(id)).FirstOrDefault();
            //throw new NotImplementedException();
        }

        public string GetUserName(string userName)
        {
            //check if Username is taken
            throw new NotImplementedException();
            //var results = EchoesDB.ApplicationUsers.Where(user => user.Uname.Equals(userName)).FirstOrDefault();
        }

        public bool IsValidUserName(string userName)
        {
            //check if Username is taken
            var result = EchoesDB.ApplicationUsers.Any(user => user.Uname == userName);
            return result;
        }

        public PostModel GetUserPost(int? id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PostModel> GetUserPosts(string? id)
        {
            //return a list of posts where the PostOwnerID mathes UserID
            throw new NotImplementedException();
        }

        public IEnumerable<ApplicationUser> GetUsers()
        {
            return EchoesDB.ApplicationUsers.ToList();
        }
    }
}
