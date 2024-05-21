using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.DataAccess  {
public class AppicationContext : DbContext {

    public AppicationContext(DbContextOptions<AppicationContext> options) : base(options){
    }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity => {
                entity.ToTable("Product");
                entity.HasKey(p=>p.Id);
                entity.Property(p=>p.Id).ValueGeneratedOnAdd();
            });
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Product>Products{get;set;}
    
}
}