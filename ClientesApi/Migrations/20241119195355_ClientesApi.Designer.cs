﻿// <auto-generated />
using ClientesApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ClientesApi.Migrations
{
    [DbContext(typeof(DataBaseContext))]
    [Migration("20241119195355_ClientesApi")]
    partial class ClientesApi
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ClientesApi.Models.ClienteModel", b =>
                {
                    b.Property<string>("CodCliente")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Apellido_Cliente")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre_Cliente")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CodCliente");

                    b.ToTable("Clientes");
                });
#pragma warning restore 612, 618
        }
    }
}
