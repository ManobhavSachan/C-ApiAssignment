using Microsoft.EntityFrameworkCore;
using Try_pls.Models;

namespace Try_pls.Data
{
    public class ContactsdBcontext : DbContext
    {
        public ContactsdBcontext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Contacts> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Contacts entity
            modelBuilder.Entity<Contacts>(entity =>
            {
                entity.HasKey(c => c.Id); // Define primary key for Contacts entity

                // Configure Address as a complex type
                entity.OwnsMany(c => c.Addresses, a =>
                {
                    a.WithOwner().HasForeignKey("ContactsId");
                    a.Property<int>("Id").ValueGeneratedOnAdd();
                    a.HasKey("Id");
                });
                entity.OwnsMany(c => c.Dates, d =>
                {
                    d.Property(d => d.DateType);
                    d.Property(d => d.DateValue);
                });
                entity.OwnsMany(c => c.Names, n =>
                {
                    n.Property(n => n.FirstName);
                    n.Property(n => n.MiddleName);
                    n.Property(n => n.Surname);
                });
                // Add other configurations as needed
            });

            // Add other configurations as needed

            base.OnModelCreating(modelBuilder);
        }
    }
}
