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
            var query = $"SELECT * FROM FindRowNumber({messageId}, {aliasId});";
            return await RowNumbers
                .FromSqlRaw(query)
                .FirstOrDefaultAsync();

        }
        public async Task RemoveMessagesWithImportId(int importId)
        {
            await Database.ExecuteSqlRawAsync($"SELECT * FROM DeleteMessagesWithImportId({importId});");
        }
    }
}
