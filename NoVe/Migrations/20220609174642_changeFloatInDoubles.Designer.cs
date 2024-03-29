﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NoVe.Migrations
{
    [DbContext(typeof(DatabaseHelper))]
    [Migration("20220609174642_changeFloatInDoubles")]
    partial class changeFloatInDoubles
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NoVe.Models.Beruf", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Berufs");
                });

            modelBuilder.Entity("NoVe.Models.Domains", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AllowedDomains")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("NoVe.Models.Fach", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Gewichtung")
                        .HasColumnType("int");

                    b.Property<int>("KompetenzbereichId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Rundung")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Fachs");
                });

            modelBuilder.Entity("NoVe.Models.Klasse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BerufId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDatum")
                        .HasColumnType("datetime2");

                    b.Property<string>("KlasseName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("KlassenInviteCode")
                        .HasColumnType("int");

                    b.Property<DateTime>("Startdatum")
                        .HasColumnType("datetime2");

                    b.Property<bool>("archived")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Klasses");
                });

            modelBuilder.Entity("NoVe.Models.Kompetenzbereich", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BerufId")
                        .HasColumnType("int");

                    b.Property<int>("Gewichtung")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Rundung")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Kompetenzbereichs");
                });

            modelBuilder.Entity("NoVe.Models.Note", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FachId")
                        .HasColumnType("int");

                    b.Property<int>("FachbereichId")
                        .HasColumnType("int");

                    b.Property<double>("Notenwert")
                        .HasColumnType("float");

                    b.Property<int>("Semester")
                        .HasColumnType("int");

                    b.Property<int>("StudentAlreadyChanged")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("NoVe.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AdminVerification")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Firma")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("KlasseId")
                        .HasColumnType("int");

                    b.Property<string>("LehrmeisterEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LoginFailedCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("LoginFailedFrom")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nachname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PasswordForgottenValidFrom")
                        .HasColumnType("datetime2");

                    b.Property<int>("PasswordForgottenVerifyKey")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VerificationKey")
                        .HasColumnType("int");

                    b.Property<int>("VerificationStatus")
                        .HasColumnType("int");

                    b.Property<string>("Vorname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("archived")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("KlasseId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("NoVe.Models.User", b =>
                {
                    b.HasOne("NoVe.Models.Klasse", "Klasse")
                        .WithMany("Users")
                        .HasForeignKey("KlasseId");
                });
#pragma warning restore 612, 618
        }
    }
}
