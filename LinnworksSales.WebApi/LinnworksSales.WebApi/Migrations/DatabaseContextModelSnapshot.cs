﻿// <auto-generated />
using System;
using LinnworksSales.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LinnworksSales.WebApi.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LinnworksSales.Data.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int?>("RegionId");

                    b.HasKey("Id");

                    b.HasIndex("RegionId");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("LinnworksSales.Data.Models.ItemType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ItemTypes");
                });

            modelBuilder.Entity("LinnworksSales.Data.Models.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("LinnworksSales.Data.Models.Sale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CountryId");

                    b.Property<int?>("ItemTypeId");

                    b.Property<DateTime>("OrderDate");

                    b.Property<long>("OrderId");

                    b.Property<string>("OrderPriority")
                        .IsRequired();

                    b.Property<string>("SalesChanel")
                        .IsRequired();

                    b.Property<DateTime>("ShipDate");

                    b.Property<decimal>("UnitCost");

                    b.Property<decimal>("UnitPrice");

                    b.Property<int>("UnitsSold");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("ItemTypeId");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("LinnworksSales.Data.Models.Country", b =>
                {
                    b.HasOne("LinnworksSales.Data.Models.Region", "Region")
                        .WithMany("Countries")
                        .HasForeignKey("RegionId");
                });

            modelBuilder.Entity("LinnworksSales.Data.Models.Sale", b =>
                {
                    b.HasOne("LinnworksSales.Data.Models.Country", "Country")
                        .WithMany("Sales")
                        .HasForeignKey("CountryId");

                    b.HasOne("LinnworksSales.Data.Models.ItemType", "ItemType")
                        .WithMany("Sales")
                        .HasForeignKey("ItemTypeId");
                });
#pragma warning restore 612, 618
        }
    }
}
