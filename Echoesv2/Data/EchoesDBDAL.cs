﻿using Echoesv2.Interfaces;
using Echoesv2.Models;

namespace Echoesv2.Data
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
        public ApplicationUser GetUserByEmail(string? email)
        {
            return EchoesDB.ApplicationUsers.Where(user => user.Email.ToLower().Equals(email)).FirstOrDefault();
            //throw new NotImplementedException();
        }

        public string GetUserName(string userId)
        {
            return EchoesDB.ApplicationUsers.Where(user => user.Id.Equals(userId)).FirstOrDefault().Name;
            //throw new NotImplementedException();
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
