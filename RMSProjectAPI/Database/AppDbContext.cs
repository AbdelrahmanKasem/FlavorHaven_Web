using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database.Entity;

namespace RMSProjectAPI.Database
{
    public class AppDbContext : IdentityDbContext<User,IdentityRole<Guid>, Guid>
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Extra> Extras { get; set; }
        public DbSet<ContactForm> ContactForms { get; set; }
        public DbSet<FavoriteMeal> FavoriteMeals { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemSize> MenuItemSizes { get; set; }
        public DbSet<MenuItemSuggestion> MenuItemSuggestions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemExtra> OrderItemExtras { get; set; }
        public DbSet<OrderLog> OrderLogs { get; set; }
        public DbSet<Table> Tables { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Waiter)
                .WithMany()
                .HasForeignKey(o => o.WaiterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryPerson)
                .WithMany()
                .HasForeignKey(o => o.DeliveryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItemSuggestion>()
                .HasOne(ms => ms.MenuItem)
                .WithMany(m => m.Suggestions)
                .HasForeignKey(ms => ms.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItemSuggestion>()
                .HasOne(ms => ms.SuggestedItem)
                .WithMany()
                .HasForeignKey(ms => ms.SuggestedItemId)
                .OnDelete(DeleteBehavior.Restrict);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }
}
