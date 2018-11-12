using System;
using System.Collections.Generic;
using System.Text;
using bytme.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace bytme.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<bytme.Models.Item> Items { get; set; }
        public DbSet<bytme.Models.ItemCategories> ItemCategories { get; set; }
        public DbSet<bytme.Models.UserModel> UserModels { get; set; }
        public DbSet<bytme.Models.OrderHistory> OrderHistories { get; set; }
        public DbSet<bytme.Models.OrderMain> OrderMains { get; set; }
        public DbSet<bytme.Models.OrderLines> OrderLines { get; set; }
        public DbSet<bytme.Models.OrderStatus> OrderStatuses { get; set; }
        public DbSet<bytme.Models.WishlistModel> WishlistModels { get; set; }
        public DbSet<bytme.Models.ShoppingCartModel> ShoppingCartModels { get; set; }
    }
}
