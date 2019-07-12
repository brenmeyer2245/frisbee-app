using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Claims;
using System.Web;
using UFL.API.DB;
using UFL.API.Models;

namespace UFL.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UFLDbContext _db;
        public AuthService(UFLDbContext db)
        {
            _db = db;
        }


        public async Task<User>  Register(User user, string password){
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<User> Login(string username, string password){
            var user = await _db.Users.FirstOrDefaultAsync(_ => _.Username == username);
            if (user == null) return null;

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt)) return null;
            return user;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt){
           using (var hmac = new System.Security.Cryptography.HMACSHA512()){
               passwordSalt = hmac.Key;
               passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
           }
        }


        /*
        Takes in the submitted password, stored hash and salt.
        Then it uses the salt to compute a new hash from the submitted password
        And compares each character for the hashes
         */
        public bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt){
                using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                    var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    for (var i = 0; i < computedHash.Length; i++){
                        if (computedHash[i] != passwordHash[i]) return false;
                    }
                }
                return true;
        }

        public async Task<bool> UserExists(string username){
            return await _db.Users.AnyAsync(_ => _.Username == username);
        }


    }
}
