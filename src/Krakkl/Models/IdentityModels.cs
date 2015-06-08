using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace Krakkl.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Pseudonym { get; set; }
        public string EditorLanguage { get; set; }
        public string AboutMe { get; set; }
        public string AuthorProfile { get; set; }
        public string GravatarImage => $"https://secure.gravatar.com/avatar/{GetMd5Hash(Email)}?s=240&d=identicon";
        public string GravatarProfile => $"https://secure.gravatar.com/{GetMd5Hash(Email)}";

        private string GetMd5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }

    public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private static bool _created;

        public ApplicationDbContext()
        {
            // Create the database and schema if it doesn't exist
            if (!_created)
            {
                Database.AsRelational().ApplyMigrations();
                _created = true;
            }
        }
    }
}
