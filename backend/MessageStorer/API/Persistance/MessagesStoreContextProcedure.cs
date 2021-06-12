using API.Persistance.Entity.Procedure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Persistance
{
    public partial class MessagesStoreContext
    {
        public virtual DbSet<RowNumber> RowNumbers { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RowNumber>().HasNoKey();
        }
        public async Task<RowNumber> GetRowNumber(int messageId, int aliasId)
        {
            var query = "SELECT * FROM FindRowNumber({0}, {1})";
            return await RowNumbers
                .FromSqlRaw(query, messageId, aliasId)
                .FirstOrDefaultAsync();

        }
        public async Task RemoveMessagesWithImportId(int importId)
        {
            await Database.ExecuteSqlRawAsync(
                "SELECT * FROM DeleteMessagesWithImportId({0});", 
                importId);
        }
        public async Task RemoveEmptyContacts(int appUserId)
        {
            await Database.ExecuteSqlRawAsync("SELECT * FROM DeleteEmptyContacts({0});",
                appUserId);
        }
    }
}
