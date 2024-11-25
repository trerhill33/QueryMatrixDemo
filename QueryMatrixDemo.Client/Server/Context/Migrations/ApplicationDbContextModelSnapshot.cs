﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using QueryMatrixDemo.Client.Server.Context;

#nullable disable

namespace QueryMatrixDemo.Client.Server.Context.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Electronics"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Clothing"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Books"
                        });
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Customers");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "john@example.com",
                            Name = "John Doe"
                        },
                        new
                        {
                            Id = 2,
                            Email = "jane@example.com",
                            Name = "Jane Smith"
                        });
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.InventoryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<int>("WarehouseId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("WarehouseId");

                    b.ToTable("InventoryItems");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ProductId = 1,
                            Quantity = 30,
                            WarehouseId = 1
                        },
                        new
                        {
                            Id = 2,
                            ProductId = 1,
                            Quantity = 20,
                            WarehouseId = 2
                        },
                        new
                        {
                            Id = 3,
                            ProductId = 2,
                            Quantity = 50,
                            WarehouseId = 1
                        });
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ShippingAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShippingMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("InventoryQuantity")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CategoryId = 1,
                            Description = "High-end laptop",
                            InventoryQuantity = 50,
                            Name = "Laptop",
                            Price = 999.99m
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 2,
                            Description = "Cotton t-shirt",
                            InventoryQuantity = 100,
                            Name = "T-Shirt",
                            Price = 19.99m
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 3,
                            Description = "C# Programming",
                            InventoryQuantity = 30,
                            Name = "Programming Book",
                            Price = 49.99m
                        });
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Warehouse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Warehouses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Location = "New York",
                            Name = "Main Warehouse"
                        },
                        new
                        {
                            Id = 2,
                            Location = "Los Angeles",
                            Name = "Secondary Warehouse"
                        });
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.InventoryItem", b =>
                {
                    b.HasOne("QueryMatrixDemo.Shared.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QueryMatrixDemo.Shared.Entities.Warehouse", "Warehouse")
                        .WithMany("InventoryItems")
                        .HasForeignKey("WarehouseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Warehouse");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Order", b =>
                {
                    b.HasOne("QueryMatrixDemo.Shared.Entities.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.OrderItem", b =>
                {
                    b.HasOne("QueryMatrixDemo.Shared.Entities.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QueryMatrixDemo.Shared.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Payment", b =>
                {
                    b.HasOne("QueryMatrixDemo.Shared.Entities.Order", "Order")
                        .WithOne("Payment")
                        .HasForeignKey("QueryMatrixDemo.Shared.Entities.Payment", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Product", b =>
                {
                    b.HasOne("QueryMatrixDemo.Shared.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Customer", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Order", b =>
                {
                    b.Navigation("OrderItems");

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("QueryMatrixDemo.Shared.Entities.Warehouse", b =>
                {
                    b.Navigation("InventoryItems");
                });
#pragma warning restore 612, 618
        }
    }
}
