
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using trading_app.models;

namespace trading_app.data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){
                   
        }
        
        public required DbSet<Trade> Trades {get;set;}
        public required DbSet<Wire> Wires {get;set;}

         protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);
              modelBuilder.Entity<ApplicationUser>()
                          .HasKey(i=>i.Id);

             modelBuilder.Entity<Wire>()
                         .Property(i => i.Amount)
                         .HasColumnType("decimal(20,2)");
                         
              modelBuilder.Entity<ApplicationUser>()
                        .HasMany<Wire>(w => w.Wires)
                        .WithOne(user => user.applicationUser)
                        .HasForeignKey(key => key.UserId)
                        .IsRequired();

              modelBuilder.Entity<ApplicationUser>()
                        .HasMany<Trade>(w => w.Trades)
                        .WithOne(user => user.applicationUser)
                        .HasForeignKey(key => key.UserId);  
        }
    }
}