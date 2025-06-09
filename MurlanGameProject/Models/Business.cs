using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MuranGameProject.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("UsersTbl");
            modelBuilder.Entity<User>()
                .Property(u => u.Id).HasColumnName("user_id");
            modelBuilder.Entity<User>()
                .Property(u => u.Name).HasColumnName("user_name");
            modelBuilder.Entity<User>()
                .Property(u => u.Surname).HasColumnName("user_surname");
            modelBuilder.Entity<User>()
                .Property(u => u.Email).HasColumnName("user_email");
            modelBuilder.Entity<User>()
                .Property(u => u.Password).HasColumnName("user_password");
            modelBuilder.Entity<User>()
                .Property(u => u.RegistrationDate).HasColumnName("registration_date");
            modelBuilder.Entity<User>()
                .Property(u => u.ProfilePicture).HasColumnName("profile_picture");
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? ProfilePicture { get; set; }
    }

    public class Business
    {
        private readonly UserContext _context;

        public Business(UserContext context)
        {
            _context = context;
        }

        public async Task<bool> Authenticate(string email, string pass)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == pass);
            return user != null;
        }

        public async Task<bool> Register(string email, string name, string surname, string pass, string confPass)
        {
            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") || pass != confPass)
                return false;

            if (await _context.Users.AnyAsync(u => u.Email == email))
                return false;

            var user = new User
            {
                Email = email,
                Name = name,
                Surname = surname,
                Password = pass,
                RegistrationDate = DateTime.UtcNow,
                ProfilePicture = "/images/profile1.png"
            };
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<User> GetUserInfo(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UpdateProfilePicture(string email, string picturePath)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            user.ProfilePicture = picturePath;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangePassword(string email, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.Password != currentPassword)
                return false;

            // Basic validation: ensure new password is at least 8 characters
            if (newPassword.Length < 8)
                return false;

            user.Password = newPassword;
            return await _context.SaveChangesAsync() > 0;
        }
    }

}