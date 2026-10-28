//------------------------------------------------------------------------------
// Модель БД DEMO2023 (аналог классов, которые генерирует ADO.NET EDM).
// Дополнительная логика - в папке Partials.
//------------------------------------------------------------------------------

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ConferenceApp.Models
{
    public partial class ConferenceEntities : DbContext
    {
        static ConferenceEntities()
        {
            // БД уже создана скриптом - EF не должен её создавать/менять
            Database.SetInitializer<ConferenceEntities>(null);
        }

        public ConferenceEntities() : base("name=ConferenceEntities") { }

        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Mark> Marks { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            // имена таблиц в БД в единственном числе: User, Event ...
            mb.Conventions.Remove<PluralizingTableNameConvention>();

            mb.Entity<Activity>().Property(p => p.Time).HasColumnType("time").HasPrecision(0);
            mb.Entity<Activity>().HasRequired(a => a.Event).WithMany(e => e.Activities).HasForeignKey(a => a.EventId);
            mb.Entity<Activity>().HasOptional(a => a.User).WithMany(u => u.Activities).HasForeignKey(a => a.ModeratorId);

            mb.Entity<Event>().HasRequired(e => e.City).WithMany(c => c.Events).HasForeignKey(e => e.CityId);
            mb.Entity<Event>().HasRequired(e => e.Course).WithMany(c => c.Events).HasForeignKey(e => e.CourseId);
            mb.Entity<Event>().HasOptional(e => e.User).WithMany(u => u.Events).HasForeignKey(e => e.WinnerId);

            mb.Entity<Mark>().HasRequired(m => m.Activity).WithMany(a => a.Marks).HasForeignKey(m => m.ActivityId);
            mb.Entity<Mark>().HasRequired(m => m.User).WithMany(u => u.Marks).HasForeignKey(m => m.JuryId);

            mb.Entity<User>().HasOptional(u => u.Country).WithMany(c => c.Users).HasForeignKey(u => u.CountryId);
            mb.Entity<User>().HasOptional(u => u.Course).WithMany(c => c.Users).HasForeignKey(u => u.CourseId);
            mb.Entity<User>().HasRequired(u => u.Gender).WithMany(g => g.Users).HasForeignKey(u => u.GenderId);
            mb.Entity<User>().HasRequired(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId);
            mb.Entity<User>().Property(u => u.Email).IsUnicode(false);
            mb.Entity<User>().Property(u => u.Phone).IsUnicode(false);
            mb.Entity<Country>().Property(c => c.EnglishTitle).IsUnicode(false);
        }
    }
}
