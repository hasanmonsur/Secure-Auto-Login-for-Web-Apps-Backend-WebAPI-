using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace AuthWebApi.Data
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var connectionString = "mongodb://admin:admin123@127.0.0.1:27017/?authSource=admin"; // Hardcode or read from config
            var databaseName = "AuthDb";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseMongoDB(client, databaseName);

            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}
