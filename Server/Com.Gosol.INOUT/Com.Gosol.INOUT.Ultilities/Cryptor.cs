using System;
using System.Security.Cryptography;
using System.Text;

namespace Com.Gosol.INOUT.Ultilities
{
    public static class Cryptor
    {
        static string hash = "f0xle@rn";
        /// <summary>
        /// Encrypt password to insert into database 
        /// </summary>
        /// <param name="UserName">UserName is a salt</param>
        /// <param name="Password">Password</param>
        /// <returns></returns>
        public static string EncryptPasswordUser(string UserName, string Password)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, GetMd5Hash(md5Hash, Password + UserName.ToLower()) + UserName.ToLower());
            }

        }
        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        private static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            string hashOfInput = GetMd5Hash(md5Hash, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static string key { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";

        public static string DecryptPasswordUser(string Password)
        {

            try
            {
                byte[] data = Convert.FromBase64String(Password);
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                    using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 })
                    {
                        ICryptoTransform transform = tripleDES.CreateDecryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        var a = UTF8Encoding.UTF8.GetString(results);
                        return a;
                    }
                }

            }
            catch (Exception ex)
            {
                return Password;
                throw ex;
            }
        }
        public static string PasswordToBase64(string Password)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(Password);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 })
                {
                    ICryptoTransform transform = tripleDES.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    //var a = UTF8Encoding.UTF8.GetString(results);
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }
    }
}
