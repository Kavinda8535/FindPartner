using System;
using System.Threading.Tasks;
using FindPartner.API.Model;
using Microsoft.EntityFrameworkCore;

namespace FindPartner.API.Data
{
    
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
           var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);
           
           if(user == null)
            return null;

            if(!VeryfyPasswardHash(password, user.UserName, user.PassowrdHash, user.PasswordSalt))
                return null;

           return user;
        }

        private bool VeryfyPasswardHash(string password, string userName, byte[] passowrdHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var ComputeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i=0; i < ComputeHash.Length; i++)
                {
                    if(ComputeHash[i] != passowrdHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHas, passwordSalt;
            CreatePasswordHash(password, out passwordHas, out passwordSalt);

            user.PassowrdHash = passwordHas;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHas, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHas = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.UserName == username))
                return true;

            return false;
            
        }
    }
}