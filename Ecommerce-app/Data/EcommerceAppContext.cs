using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce_app.Models.Products;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Ecommerce_app.Areas.Identity.Models;
using Ecommerce_app.Areas.Admin.Models.ViewModels;
using Ecommerce_app.Models.ViewModels;
using Ecommerce_app.Models.Orders;

namespace Ecommerce_app.Data
{
    public class EcommerceAppContext : DbContext
    {
        public EcommerceAppContext (DbContextOptions<EcommerceAppContext> options)
            : base(options)
        {
        }

        public DbSet<Ecommerce_app.Models.Products.Department> Department { get; set; } = default!;
        public DbSet<Ecommerce_app.Models.Products.Category> Category { get; set; } = default!;
        public DbSet<Ecommerce_app.Models.Products.Product> Product { get; set; } = default!;
        public DbSet<Ecommerce_app.Models.Products.Option> Option { get; set; } = default!;
        public DbSet<Ecommerce_app.Models.Products.OptionValue> OptionValue { get; set; } = default!;
        public DbSet<Ecommerce_app.Models.Products.Variant> Variant { get; set; } = default!;
        public DbSet<Ecommerce_app.Models.Products.VariantValue> VariantValue { get; set; } = default!;
        
        public DbSet<Ecommerce_app.Models.Orders.Order> Order { get; set; } = default!;
        public DbSet<Ecommerce_app.Models.Orders.OrderItem> OrderItem { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Department>()
                .HasMany(e => e.Categories)
                .WithOne()
                .HasForeignKey(e => e.DepartmentId)
                .IsRequired();
            modelBuilder.Entity<Department>()
                .HasMany(e => e.Products)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .IsRequired();
            modelBuilder.Entity<Category>()
               .HasMany(e => e.Products)
               .WithOne(e => e.Category)
               .HasForeignKey(e => e.CategoryId)
               .IsRequired();

            modelBuilder.Entity<Option>()
                .HasMany(e => e.OptionValues)
                .WithOne(e => e.Option)
                .HasForeignKey(e => e.OptionId);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Comments)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);
            modelBuilder.Entity<Product>()
                .HasMany(e => e.Options)
                .WithMany()
                .UsingEntity<ProductOption>();
            modelBuilder.Entity<Product>()
                .HasMany(e => e.Variants)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);

            modelBuilder.Entity<Variant>()
                .HasMany(e => e.VariantValues)
                .WithOne(e => e.Variant)
                .HasForeignKey(e => e.VariantId);
            /*modelBuilder.Entity<VariantValue>()
                .HasOne(e => e.Option)
                .WithOne()
                .HasForeignKey<VariantValue>(e => e.OptionId);
            modelBuilder.Entity<VariantValue>()
                .HasOne(e => e.OptionValue)
                .WithOne()
                .HasForeignKey<VariantValue>(e => e.OptionValueId);*/

           modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderItems)
                .WithOne()
                .HasForeignKey(e => e.OrderId)
                .HasPrincipalKey(e => e.OrderId);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var AddedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList();

            AddedEntities.ForEach(E =>
            {
                E.Property("Created_at").CurrentValue = DateTime.Now;
                E.Property("Modiftied_at").CurrentValue = DateTime.Now;
            });

            var EditedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();

            EditedEntities.ForEach(E =>
            {
                E.Property("Created_at").IsModified = false;
                E.Property("Modiftied_at").CurrentValue = DateTime.Now;
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        //public DbSet<Ecommerce_app.Models.ViewModels.ProductViewModel> ProductViewModel { get; set; } = default!;

        /*
        public DbSet<Ecommerce_app.Areas.Admin.Models.ViewModels.ProductViewModel> ProductViewModel { get; set; } = default!;
        public DbSet<Ecommerce_app.Areas.Admin.Models.ViewModels.VariantViewModel> VariantViewModel { get; set; } = default!;
        public DbSet<Ecommerce_app.Areas.Admin.Models.ViewModels.VariantListViewModel> VariantListViewModel { get; set; } = default!;
        */
    }
}
