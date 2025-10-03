using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models.QuanTriHeThong;

namespace Com.Gosol.INOUT.DAL.HeThong.Auth
{
    public class AuthRepository : IAuthRepository
    {
        //private readonly DataContext _context;

        #region implement
        public async Task<User> Login(string username, string password)
        {
            //   var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username); // Get user from database.
            var user = new User();
            if (user == null)
                return null; // nếu user ko tồn tại thì return null - User does not exist.

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return null; // nếu mật khẩu không đúng thì return null
            return user;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            //await _context.Users.AddAsync(user); // Adding the user to context of users.
            //await _context.SaveChangesAsync(); // Save changes to database.

            return user;
        }

        public async Task<bool> UserExists(string username)
        {
          //  if (await _context.Users.AnyAsync(x => x.Username == username)) // kiểm tra tồn tại của user trong db
                return true;
           // return false;
        }
        #endregion

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // Tạo mật khẩu bằng hàm băm và khóa Salt - Create hash using password salt.
                for (int i = 0; i < computedHash.Length; i++)
                { // Lặp qua tất cả phần từ của mảng mật khẩu - Loop through the byte array
                    if (computedHash[i] != passwordHash[i]) return false; // nếu có 1 ký tự không đúng thì return false - if mismatch
                }
            }
            return true; // Nếu tất cả đều trùng khớp thì trả về true - if no mismatches.
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}
