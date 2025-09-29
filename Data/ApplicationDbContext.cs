using CompanyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentLines> DocumentLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Receipt relationships
            modelBuilder.Entity<Document>()
                .HasOne(r => r.User)
                .WithMany(u => u.Receipts)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Document>()
                .HasOne(r => r.Branch)
                .WithMany(b => b.Receipts)
                .HasForeignKey(r => r.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Document>()
                .HasOne(r => r.Company)
                .WithMany(c => c.Receipts)
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            // ReceiptItem relationships
            modelBuilder.Entity<DocumentLines>()
                .HasOne(ri => ri.Receipt)
                .WithMany(r => r.ReceiptItems)
                .HasForeignKey(ri => ri.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentLines>()
                .HasOne(ri => ri.Product)
                .WithMany()
                .HasForeignKey(ri => ri.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Company-Branch relationship
            //modelBuilder.Entity<Company>()
            //    .HasMany(c => c.Branches)
            //    .WithOne(b => b.Company)
            //    .HasForeignKey(b => b.CompanyId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // Add some indexes for better performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Branch>()
                .HasIndex(b => b.Name);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            modelBuilder.Entity<Document>()
                .HasIndex(r => r.ReceiptNumber)
                .IsUnique();




            // Apply global filter for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                        ?.MakeGenericMethod(entityType.ClrType);

                    method?.Invoke(null, [modelBuilder]);
                }
            }
        }

        public override int SaveChanges()
        {
            HandleSoftDelete();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSoftDelete();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void HandleSoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                }
            }
        }


        private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseEntity
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
