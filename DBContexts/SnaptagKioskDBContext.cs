using Microsoft.EntityFrameworkCore;
using SnaptagOwnKioskInternalBackend.DBContexts.DBModels;

namespace SnaptagOwnKioskInternalBackend.DBContexts
{
    public class SnaptagKioskDBContext : DbContext
    {
        public DbSet<PurchaseHistoryModel> PurchaseHistories { get; set; }

        public SnaptagKioskDBContext(DbContextOptions<SnaptagKioskDBContext> options) : base(options)
        {

        }
    }
}