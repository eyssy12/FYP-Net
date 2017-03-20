namespace CMS.Configuration.Data
{
    using System.Data.Entity;
    using Models;

    public partial class IdentityEntities : DbContext
    {
        public IdentityEntities()
            : base("Server=tcp:fypcms.database.windows.net,1433;Database=fypcms;User ID=fypcms_user@fypcms;Password=Fyp_Cms_1q2w3e62878;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
        {
        }

        public virtual DbSet<C__EFMigrationsHistory> C__EFMigrationsHistory { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<CancelledEvent> CancelledEvents { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<GcmMobileClient> GcmMobileClients { get; set; }
        public virtual DbSet<Lecturer> Lecturers { get; set; }
        public virtual DbSet<LecturerModule> LecturerModules { get; set; }
        public virtual DbSet<LecturerPerson> LecturerPersons { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<NewsPost> NewsPosts { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentPerson> StudentPersons { get; set; }
        public virtual DbSet<Timetable> Timetables { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetRoleClaims)
                .WithRequired(e => e.AspNetRole)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.NewsPosts)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.ApplicationUserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.People)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.ApplicationUserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.RefreshTokens)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetRoles)
                .WithMany(e => e.AspNetUsers)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("UserId").MapRightKey("RoleId"));

            modelBuilder.Entity<Event>()
                .HasMany(e => e.CancelledEvents)
                .WithOptional(e => e.Event)
                .HasForeignKey(e => e.CancellationEventId);

            modelBuilder.Entity<Lecturer>()
                .HasMany(e => e.LecturerModules)
                .WithRequired(e => e.Lecturer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Module>()
                .HasMany(e => e.LecturerModules)
                .WithRequired(e => e.Module)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Module>()
                .HasMany(e => e.Students)
                .WithMany(e => e.Modules)
                .Map(m => m.ToTable("StudentModule").MapLeftKey("ModuleId").MapRightKey("StudentId"));
        }
    }
}
