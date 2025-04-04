using AuthWebApi.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace AuthWebApi.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.ToCollection("users");
                b.HasKey(u => u.Id);
                b.Property(u => u.Email).IsRequired();
            });

            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.ToCollection("refresh_tokens");
                b.HasKey(rt => rt.Id);
                b.Property(rt => rt.Token).IsRequired();
            });
        }

        //public static AuthDbContext Create(IMongoDatabase database) =>
        //    new(new DbContextOptionsBuilder<AuthDbContext>()
        //        .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
        //        .Options);


    }
}
