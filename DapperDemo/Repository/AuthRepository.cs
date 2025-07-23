using Dapper;
using DapperDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperDemo.Repository
{
    public class AuthRepository
    {
        private readonly string _connectionString;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public bool Register(UserDto userDto)
        {
            using var db = Connection;

            var existing = db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { userDto.Email });
            if (existing != null) return false;

            var user = new User { Email = userDto.Email, Role = "Client" };
            user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);

            string sql = "INSERT INTO Users (Email, PasswordHash, Role) VALUES (@Email, @PasswordHash, @Role)";
            db.Execute(sql, user);
            return true;
        }

        public bool Login(UserDto userDto)
        {
            using var db = Connection;

            var user = db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { userDto.Email });
            if (user == null) return false;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);
            return result == PasswordVerificationResult.Success;
        }

        public User? GetUserByEmail(string email)
        {
            using var db = Connection;
            return db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
        }

        public bool ChangePassword(string email, string currentPassword, string newPassword)
        {
            using var db = Connection;

            var user = db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
            if (user == null) return false;

            var hasher = new PasswordHasher<User>();
            var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (verify != PasswordVerificationResult.Success) return false;

            string newHash = hasher.HashPassword(user, newPassword);
            db.Execute("UPDATE Users SET PasswordHash = @PasswordHash WHERE Email = @Email",
                       new { PasswordHash = newHash, Email = email });

            return true;
        }
        public User? AuthenticateUser(string email, string password)
        {
            using var db = Connection;

            var user = db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }


    }
}
