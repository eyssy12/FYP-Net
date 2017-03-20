namespace CMS.Web.Configuration.Data
{
    using CMS.Web.Configuration.Data.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Data.Entity;
    using Microsoft.Data.Entity.Metadata;

    public class Entities : IdentityDbContext<ApplicationUser>
    {
        public Entities() : base()
        {
        }

        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Lecturer> Lecturers { get; set; }
        public virtual DbSet<GcmMobileClient> MobileClients { get; set; }
        public virtual DbSet<StudentModule> StudentModules { get; set; }
        public virtual DbSet<LecturerModule> LecturerModules { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Timetable> Timetables { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<StudentPerson> StudentPersons { get; set; }
        public virtual DbSet<LecturerPerson> LecturerPersons { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<NewsPost> NewsPosts { get; set; }
        public virtual DbSet<CancelledEvent> CancelledEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connString = "Server=tcp:fypcms.database.windows.net,1433;Database=fypcms;User ID=fypcms_user@fypcms;Password=Fyp_Cms_1q2w3e62878;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            optionsBuilder.UseSqlServer(connString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.SetupPrimaryKeys(modelBuilder);
            this.SetupIdentityUser(modelBuilder);

            this.BuildPersonEntity(modelBuilder);
            this.BuildStudentEntity(modelBuilder);
            this.BuildLecturerEntity(modelBuilder);
            this.BuildGcmMobileClientEntity(modelBuilder);
            this.BuildSemesterEntity(modelBuilder);
            this.BuildCourseEntity(modelBuilder);
            this.BuildClassEntity(modelBuilder);
            this.BuildModuleEntity(modelBuilder);
            this.BuildDepartmentEntity(modelBuilder);
            this.BuildEventEntity(modelBuilder);
            this.BuildTimetableEntity(modelBuilder);
            this.BuildStudentPersonEntity(modelBuilder);
            this.BuildLecturerPersonEntity(modelBuilder);
            this.BuildRefreshTokenEntity(modelBuilder);
            this.BuildNewsPostEntity(modelBuilder);
            this.BuildCancelledEventEntity(modelBuilder);

            this.SetupManyToManyEntities(modelBuilder);
        }

        private void BuildCancelledEventEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CancelledEvent>()
                .HasOne(s => s.CancellationEvent)
                .WithOne()
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void BuildNewsPostEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NewsPost>()
                .HasOne(s => s.ApplicationUser)
                .WithMany(s => s.NewsPosts)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void BuildRefreshTokenEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>()
                .HasOne(s => s.User)
                .WithOne()
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void BuildLecturerPersonEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LecturerPerson>().Property(p => p.Id).UseSqlServerIdentityColumn();
        }

        private void BuildStudentPersonEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentPerson>().Property(p => p.Id).UseSqlServerIdentityColumn();
        }

        private void BuildModuleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Module>().Property(e => e.Credits).IsRequired();
            modelBuilder.Entity<Module>().Property(e => e.Name).IsRequired();
            modelBuilder.Entity<Module>().Property(e => e.ModuleType).IsRequired();
        }

        private void BuildTimetableEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Timetable>().Property(e => e.Name).IsRequired();

            modelBuilder.Entity<Timetable>()
                .HasMany(p => p.Events)
                .WithOne(p => p.Timetable)
                .IsRequired();

            modelBuilder.Entity<Timetable>()
                .HasOne(p => p.Class)
                .WithOne(p => p.Timetable)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void BuildEventEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().Property(e => e.Title).IsRequired();
            modelBuilder.Entity<Event>().Property(e => e.Start).IsRequired();
            modelBuilder.Entity<Event>().Property(e => e.End).IsRequired();

            modelBuilder.Entity<Event>()
                .HasOne(p => p.Timetable)
                .WithMany(p => p.Events)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void SetupIdentityUser(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // this will remove the error of 'the type ApplicationUser requires a key to be defined'

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(p => p.Person)
                .WithOne(p => p.ApplicationUser)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void SetupPrimaryKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasKey(p => p.Id);
            modelBuilder.Entity<Student>().HasKey(s => s.Id); 
            modelBuilder.Entity<Lecturer>().HasKey(l => l.Id);
            modelBuilder.Entity<Course>().HasKey(p => p.Id);
            modelBuilder.Entity<GcmMobileClient>().HasKey(p => p.Id);
            modelBuilder.Entity<Module>().HasKey(p => p.Id);
            modelBuilder.Entity<Department>().HasKey(p => p.Id);
            modelBuilder.Entity<Class>().HasKey(p => p.Id);
            modelBuilder.Entity<Timetable>().HasKey(p => p.Id);
            modelBuilder.Entity<Event>().HasKey(p => p.Id);
            modelBuilder.Entity<RefreshToken>().HasKey(p => p.Id);
            modelBuilder.Entity<NewsPost>().HasKey(p => p.Id);
            modelBuilder.Entity<CancelledEvent>().HasKey(p => p.Id);

            modelBuilder.Entity<StudentPerson>().HasKey(sp => new { sp.PersonId, sp.StudentId });
            modelBuilder.Entity<LecturerPerson>().HasKey(sp => new { sp.PersonId, sp.LecturerId });
            modelBuilder.Entity<StudentModule>().HasKey(sm => new { sm.StudentId, sm.ModuleId });
            modelBuilder.Entity<LecturerModule>().HasKey(lm => new { lm.LecturerId, lm.ModuleId });
        }

        private void BuildPersonEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.LastName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.BirthDate).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.FirstName).HasMaxLength(40);
            modelBuilder.Entity<Person>().Property(p => p.LastName).HasMaxLength(40);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.MobileClients)
                .WithOne(p => p.Person)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void BuildStudentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOne(p => p.StudentPerson)
                .WithOne(p => p.Student)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Student>()
                .HasMany(p => p.StudentModules)
                .WithOne(p => p.Student)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void BuildLecturerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lecturer>()
                .HasOne(p => p.LecturerPerson)
                .WithOne(p => p.Lecturer)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lecturer>()
                .HasMany(p => p.LecturerModules)
                .WithOne(p => p.Lecturer)
                .HasForeignKey(p => p.LecturerId);
        }

        private void BuildGcmMobileClientEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GcmMobileClient>().Property(p => p.Token).IsRequired();
            modelBuilder.Entity<GcmMobileClient>().Property(p => p.Token).HasMaxLength(200);
        }

        private void BuildSemesterEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Semester>()
                .HasMany(p => p.Modules)
                .WithOne(p => p.Semester);

            modelBuilder.Entity<Semester>()
                .HasMany(p => p.Classes)
                .WithOne(p => p.Semester);
        }

        private void BuildCourseEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasMany(p => p.Semesters)
                .WithOne(p => p.Course)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Course>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Course>().Property(p => p.Name).HasMaxLength(100);
            modelBuilder.Entity<Course>().Property(p => p.AwardType).IsRequired();
        }

        private void BuildClassEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>()
                .HasMany(p => p.Students)
                .WithOne(p => p.Class);

            modelBuilder.Entity<Class>()
                .HasOne(p => p.Timetable)
                .WithOne(p => p.Class)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void BuildDepartmentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Department>().Property(p => p.Name).HasMaxLength(100);

            modelBuilder.Entity<Department>() // department name and course name must be the same length
                .HasMany(p => p.Courses)
                .WithOne(p => p.Department)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void SetupManyToManyEntities(ModelBuilder modelBuilder)
        {
            // many-to-many student->module->student
            modelBuilder.Entity<StudentModule>()
                .HasOne(p => p.Student)
                .WithMany(p => p.StudentModules)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentModule>()
                .HasOne(p => p.Module)
                .WithMany(p => p.StudentModules)
                .HasForeignKey(p => p.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            // many-to-many lecturer->module->lecturer
            modelBuilder.Entity<LecturerModule>()
                .HasOne(p => p.Lecturer)
                .WithMany(p => p.LecturerModules)
                .HasForeignKey(p => p.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LecturerModule>()
                .HasOne(p => p.Module)
                .WithMany(p => p.LecturerModules)
                .HasForeignKey(p => p.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}