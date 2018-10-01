using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace bytme.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<bytme.Models.Item> Items { get; set; }
        public DbSet<bytme.Models.ItemCategories> ItemCategories { get; set; }
        public DbSet<bytme.Models.UserModel> UserModels { get; set; }
        public DbSet<bytme.Models.OrderHistory> OrderHistories { get; set; }
        public DbSet<bytme.Models.OrderMain> OrderMains { get; set; }
        public DbSet<bytme.Models.OrderStatus> OrderStatuses { get; set; }
        public DbSet<bytme.Models.WishlistModel> WishlistModels { get; set; }
        public DbSet<bytme.Models.ShoppingCartModel> ShoppingCartModels { get; set; }
    }
}
