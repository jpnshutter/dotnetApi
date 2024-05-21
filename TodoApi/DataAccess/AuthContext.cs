using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.DataAccess  {
public class AuthContext : DbContext {

    public AuthContext(DbContextOptions<AuthContext> options) : base(options){
    }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => {
                entity.ToTable("user");
                entity.HasKey(p=>p.Id);
                entity.Property(p=>p.Id).ValueGeneratedOnAdd();
            });
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User>User{get;set;}
    
}
}