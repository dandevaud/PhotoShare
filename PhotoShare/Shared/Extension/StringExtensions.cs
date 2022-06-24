using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShare.Shared.Extension
{
    public static class StringExtensions
    {

        public static string HashString(this string input)
        {
            var destination = new Span<byte>();
            if(!SHA256.TryHashData(Encoding.UTF8.GetBytes(input), destination, out var written))
            {
                throw new CryptographicException("Error when trying to hash string");
            }
            return Encoding.UTF8.GetString(destination);
        }
    }
}
