using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using CMS.Web.Configuration.Data;

namespace CMSWebConfigurationData.Migrations
{
    [DbContext(typeof(Entities))]
    partial class EntitiesModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.CancelledEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CancellationEventId");

                    b.Property<string>("CancelledBy");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Class", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EnrollmentStage");

                    b.Property<string>("Name");

                    b.Property<int?>("SemesterId");

                    b.Property<DateTime>("YearCommenced");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AwardType");

                    b.Property<string>("Code");

                    b.Property<int?>("DepartmentId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BackgroundColor");

                    b.Property<string>("BackgroundColorDescriptive");

                    b.Property<string>("BorderColor");

                    b.Property<DateTime>("End");

                    b.Property<int?>("ModuleId");

                    b.Property<bool>("Repeatable");

                    b.Property<string>("Room");

                    b.Property<DateTime>("Start");

                    b.Property<int?>("TimetableId")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.GcmMobileClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("PersonId");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Lecturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("HireDate");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.LecturerModule", b =>
                {
                    b.Property<int>("LecturerId");

                    b.Property<int>("ModuleId");

                    b.Property<int>("Id");

                    b.HasKey("LecturerId", "ModuleId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.LecturerPerson", b =>
                {
                    b.Property<int>("PersonId");

                    b.Property<int>("LecturerId");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("PersonId", "LecturerId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Module", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Credits");

                    b.Property<int>("ModuleType");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("SemesterId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.NewsPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("Body");

                    b.Property<string>("PostedBy");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("Title");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("ApplicationUserId");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 40);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 40);

                    b.Property<string>("MobilePhone");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ExpiryDate");

                    b.Property<string>("Token");

                    b.Property<string>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Semester", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CourseId");

                    b.Property<int>("Number");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClassId");

                    b.Property<DateTime>("EnrollmentDate");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.StudentModule", b =>
                {
                    b.Property<int>("StudentId");

                    b.Property<int>("ModuleId");

                    b.HasKey("StudentId", "ModuleId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.StudentPerson", b =>
                {
                    b.Property<int>("PersonId");

                    b.Property<int>("StudentId");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("PersonId", "StudentId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Timetable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClassId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.CancelledEvent", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Event")
                        .WithOne()
                        .HasForeignKey("CMS.Web.Configuration.Data.Models.CancelledEvent", "CancellationEventId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Class", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Semester")
                        .WithMany()
                        .HasForeignKey("SemesterId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Course", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Event", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Module")
                        .WithMany()
                        .HasForeignKey("ModuleId");

                    b.HasOne("CMS.Web.Configuration.Data.Models.Timetable")
                        .WithMany()
                        .HasForeignKey("TimetableId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.GcmMobileClient", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Person")
                        .WithMany()
                        .HasForeignKey("PersonId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.LecturerModule", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Lecturer")
                        .WithMany()
                        .HasForeignKey("LecturerId");

                    b.HasOne("CMS.Web.Configuration.Data.Models.Module")
                        .WithMany()
                        .HasForeignKey("ModuleId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.LecturerPerson", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Lecturer")
                        .WithOne()
                        .HasForeignKey("CMS.Web.Configuration.Data.Models.LecturerPerson", "LecturerId");

                    b.HasOne("CMS.Web.Configuration.Data.Models.Person")
                        .WithMany()
                        .HasForeignKey("PersonId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Module", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Semester")
                        .WithMany()
                        .HasForeignKey("SemesterId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.NewsPost", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Person", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.ApplicationUser")
                        .WithOne()
                        .HasForeignKey("CMS.Web.Configuration.Data.Models.Person", "ApplicationUserId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.RefreshToken", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.ApplicationUser")
                        .WithOne()
                        .HasForeignKey("CMS.Web.Configuration.Data.Models.RefreshToken", "UserId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Semester", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Course")
                        .WithMany()
                        .HasForeignKey("CourseId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Student", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Class")
                        .WithMany()
                        .HasForeignKey("ClassId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.StudentModule", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Module")
                        .WithMany()
                        .HasForeignKey("ModuleId");

                    b.HasOne("CMS.Web.Configuration.Data.Models.Student")
                        .WithMany()
                        .HasForeignKey("StudentId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.StudentPerson", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Person")
                        .WithMany()
                        .HasForeignKey("PersonId");

                    b.HasOne("CMS.Web.Configuration.Data.Models.Student")
                        .WithOne()
                        .HasForeignKey("CMS.Web.Configuration.Data.Models.StudentPerson", "StudentId");
                });

            modelBuilder.Entity("CMS.Web.Configuration.Data.Models.Timetable", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.Class")
                        .WithOne()
                        .HasForeignKey("CMS.Web.Configuration.Data.Models.Timetable", "ClassId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("CMS.Web.Configuration.Data.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("CMS.Web.Configuration.Data.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
