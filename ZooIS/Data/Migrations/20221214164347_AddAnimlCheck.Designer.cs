﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZooIS.Data;

namespace ZooIS.Data.Migrations
{
    [DbContext(typeof(ZooISContext))]
    [Migration("20221214164347_AddAnimlCheck")]
    partial class AddAnimlCheck
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AnimalAnimal", b =>
                {
                    b.Property<Guid>("ChildrenGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ParentsGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ChildrenGuid", "ParentsGuid");

                    b.HasIndex("ParentsGuid");

                    b.ToTable("AnimalAnimal");
                });

            modelBuilder.Entity("AnimalFood", b =>
                {
                    b.Property<Guid>("EatersGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FavouriteFoodGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("EatersGuid", "FavouriteFoodGuid");

                    b.HasIndex("FavouriteFoodGuid");

                    b.ToTable("AnimalFood");
                });

            modelBuilder.Entity("CharacterTagCharacterization", b =>
                {
                    b.Property<Guid>("CharacterizationsGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TagsWord")
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("CharacterizationsGuid", "TagsWord");

                    b.HasIndex("TagsWord");

                    b.ToTable("CharacterTagCharacterization");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.Property<string>("RolesId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UsersId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("RolesId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("RoleUser");
                });

            modelBuilder.Entity("ZooIS.Models.Alert", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("isRead")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ts")
                        .HasColumnType("datetime2");

                    b.HasKey("Guid");

                    b.HasIndex("UserId");

                    b.ToTable("Alerts");
                });

            modelBuilder.Entity("ZooIS.Models.Animal", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("PicturePath")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("Sex")
                        .HasColumnType("int");

                    b.Property<Guid>("SpeciesGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("SpeciesGuid");

                    b.ToTable("Animals");

                    b.HasCheckConstraint("BirthDate_NoFuture", "BirthDate <= CURRENT_TIMESTAMP");
                });

            modelBuilder.Entity("ZooIS.Models.CharacterTag", b =>
                {
                    b.Property<string>("Word")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Word");

                    b.ToTable("CharacterTag");
                });

            modelBuilder.Entity("ZooIS.Models.Characterization", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AnimalGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("ts")
                        .HasColumnType("datetime2");

                    b.HasKey("Guid");

                    b.HasIndex("AnimalGuid");

                    b.HasIndex("AuthorGuid");

                    b.ToTable("Characterization");
                });

            modelBuilder.Entity("ZooIS.Models.Employee", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EnrollDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Guid");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("ZooIS.Models.Food", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AdditionalInfo")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<long>("Demand")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<long?>("Supply")
                        .HasColumnType("bigint");

                    b.HasKey("Guid");

                    b.ToTable("Food");
                });

            modelBuilder.Entity("ZooIS.Models.Log", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Action")
                        .HasColumnType("int");

                    b.Property<string>("PageTitle")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("ts")
                        .HasColumnType("datetime2");

                    b.HasKey("Guid");

                    b.HasIndex("UserId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("ZooIS.Models.Report", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Params")
                        .IsRequired()
                        .HasColumnType("xml");

                    b.Property<string>("RequesterId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Result")
                        .IsRequired()
                        .HasColumnType("xml");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime>("ts")
                        .HasColumnType("datetime2");

                    b.HasKey("Guid");

                    b.HasIndex("RequesterId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("ZooIS.Models.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Display")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("ZooIS.Models.Taxon", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ParentGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<string>("ScientificName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("VernacularName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Guid");

                    b.HasIndex("ParentGuid");

                    b.ToTable("Taxons");
                });

            modelBuilder.Entity("ZooIS.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<Guid?>("EmployeeGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("AnimalAnimal", b =>
                {
                    b.HasOne("ZooIS.Models.Animal", null)
                        .WithMany()
                        .HasForeignKey("ChildrenGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZooIS.Models.Animal", null)
                        .WithMany()
                        .HasForeignKey("ParentsGuid")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AnimalFood", b =>
                {
                    b.HasOne("ZooIS.Models.Animal", null)
                        .WithMany()
                        .HasForeignKey("EatersGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZooIS.Models.Food", null)
                        .WithMany()
                        .HasForeignKey("FavouriteFoodGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CharacterTagCharacterization", b =>
                {
                    b.HasOne("ZooIS.Models.Characterization", null)
                        .WithMany()
                        .HasForeignKey("CharacterizationsGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZooIS.Models.CharacterTag", null)
                        .WithMany()
                        .HasForeignKey("TagsWord")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("ZooIS.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ZooIS.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ZooIS.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("ZooIS.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZooIS.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("ZooIS.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.HasOne("ZooIS.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZooIS.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ZooIS.Models.Alert", b =>
                {
                    b.HasOne("ZooIS.Models.User", "User")
                        .WithMany("Alerts")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZooIS.Models.Animal", b =>
                {
                    b.HasOne("ZooIS.Models.Taxon", "Species")
                        .WithMany("Animals")
                        .HasForeignKey("SpeciesGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("ZooIS.Models.Status", "Status", b1 =>
                        {
                            b1.Property<Guid>("AnimalGuid")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Health")
                                .HasColumnType("int");

                            b1.HasKey("AnimalGuid");

                            b1.ToTable("Animals");

                            b1.WithOwner()
                                .HasForeignKey("AnimalGuid");
                        });

                    b.Navigation("Species");

                    b.Navigation("Status")
                        .IsRequired();
                });

            modelBuilder.Entity("ZooIS.Models.Characterization", b =>
                {
                    b.HasOne("ZooIS.Models.Animal", null)
                        .WithMany("Characterization")
                        .HasForeignKey("AnimalGuid");

                    b.HasOne("ZooIS.Models.Employee", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("ZooIS.Models.Employee", b =>
                {
                    b.HasOne("ZooIS.Models.User", "User")
                        .WithOne("Employee")
                        .HasForeignKey("ZooIS.Models.Employee", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("ZooIS.Models.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("EmployeeGuid")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("FamilyName")
                                .IsRequired()
                                .HasMaxLength(60)
                                .HasColumnType("nvarchar(60)");

                            b1.Property<string>("GivenName")
                                .IsRequired()
                                .HasMaxLength(60)
                                .HasColumnType("nvarchar(60)");

                            b1.Property<string>("ThirdName")
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.HasKey("EmployeeGuid");

                            b1.ToTable("Employees");

                            b1.WithOwner("Employee")
                                .HasForeignKey("EmployeeGuid");

                            b1.Navigation("Employee");
                        });

                    b.Navigation("Name")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZooIS.Models.Log", b =>
                {
                    b.HasOne("ZooIS.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZooIS.Models.Report", b =>
                {
                    b.HasOne("ZooIS.Models.User", "Requester")
                        .WithMany("Reports")
                        .HasForeignKey("RequesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Requester");
                });

            modelBuilder.Entity("ZooIS.Models.Taxon", b =>
                {
                    b.HasOne("ZooIS.Models.Taxon", "Parent")
                        .WithMany("Taxons")
                        .HasForeignKey("ParentGuid");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("ZooIS.Models.Animal", b =>
                {
                    b.Navigation("Characterization");
                });

            modelBuilder.Entity("ZooIS.Models.Taxon", b =>
                {
                    b.Navigation("Animals");

                    b.Navigation("Taxons");
                });

            modelBuilder.Entity("ZooIS.Models.User", b =>
                {
                    b.Navigation("Alerts");

                    b.Navigation("Employee");

                    b.Navigation("Reports");
                });
#pragma warning restore 612, 618
        }
    }
}
