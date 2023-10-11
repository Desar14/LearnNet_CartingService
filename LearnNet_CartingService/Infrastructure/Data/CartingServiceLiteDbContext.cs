using LearnNet_CartingService.Core.Interfaces;
using LiteDB;
using Microsoft.Extensions.Options;

namespace LearnNet_CartingService.Infrastructure.Data
{
    public class CartingServiceLiteDbContext : ILiteDbContext
    {
        public LiteDatabase Database { get; }

        public CartingServiceLiteDbContext(IOptions<LiteDbOptions> options)
        {
            Database = new LiteDatabase(options.Value.DatabaseConnectionString);
        }
    }
}
