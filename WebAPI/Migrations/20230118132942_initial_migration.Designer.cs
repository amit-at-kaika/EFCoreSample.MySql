﻿// <auto-generated />
using EFCoreSample.MySql.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace WebAPI.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    [Migration("20230118132942_initial_migration")]
    partial class initialmigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EFCoreSample.MySql.Models.Student", b =>
                {
                    b.Property<int>("RollNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Class")
                        .HasColumnType("int");

                    b.Property<string>("Division")
                        .IsRequired()
                        .HasColumnType("varchar(1)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("RollNo");

                    b.ToTable("Students");
                });
#pragma warning restore 612, 618
        }
    }
}