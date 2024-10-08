﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PermissionsAPI.Data;

#nullable disable

namespace PermissionsAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240925064210_Initial Create")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PermissionsAPI.Models.PermissionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApellidoEmpleado")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FechaPermiso")
                        .HasColumnType("datetime2");

                    b.Property<string>("NombreEmpleado")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("TipoPermiso")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TipoPermiso");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("PermissionsAPI.Models.PermissionType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PermissionTypes");
                });

            modelBuilder.Entity("PermissionsAPI.Models.PermissionEntity", b =>
                {
                    b.HasOne("PermissionsAPI.Models.PermissionType", "PermissionType")
                        .WithMany()
                        .HasForeignKey("TipoPermiso");

                    b.Navigation("PermissionType");
                });
#pragma warning restore 612, 618
        }
    }
}
