using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Helpers
{
    public class UserPasswordHashHelper
    {
        public static byte[] CreateHash(byte[] salt, string valueToHash)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToHash));
            }
        }

        public static byte[] GenerateSalt()
        {
            using (var hmac = new HMACSHA512())
            {
                return hmac.Key;
            }
        }

        public static bool VerifyHash(string password, byte[] salt, byte[] actualPassword)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(actualPassword);
            }
        }
    }
}
