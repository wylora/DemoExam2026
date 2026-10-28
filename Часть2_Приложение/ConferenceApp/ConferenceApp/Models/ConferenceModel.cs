//------------------------------------------------------------------------------
// Модель БД DEMO2023 (аналог классов, которые генерирует ADO.NET EDM).
// Дополнительная логика - в папке Partials.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ConferenceApp.Models
{
    public partial class Activity
    {
        public Activity() { Marks = new HashSet<Mark>(); }
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; }
        public byte Day { get; set; }
        public TimeSpan Time { get; set; }
        public int? ModeratorId { get; set; }

        public virtual Event Event { get; set; }
        public virtual User User { get; set; }            // модератор
        public virtual ICollection<Mark> Marks { get; set; }
    }

    public partial class City
    {
        public City() { Events = new HashSet<Event>(); }
        public int Id { get; set; }
        public string Title { get; set; }
        public string CoatOfArms { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }

    public partial class Country
    {
        public Country() { Users = new HashSet<User>(); }
        public int Id { get; set; }
        public string Title { get; set; }
        public string EnglishTitle { get; set; }
        public string Code { get; set; }
        public int Code2 { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public partial class Course
    {
        public Course() { Events = new HashSet<Event>(); Users = new HashSet<User>(); }
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string CourseTitle { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public partial class Event
    {
        public Event() { Activities = new HashSet<Activity>(); }
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public byte Days { get; set; }
        public int CityId { get; set; }
        public int CourseId { get; set; }
        public int? WinnerId { get; set; }
        public string Logo { get; set; }

        public virtual City City { get; set; }
        public virtual Course Course { get; set; }
        public virtual User User { get; set; }            // победитель
        public virtual ICollection<Activity> Activities { get; set; }
    }

    public partial class Gender
    {
        public Gender() { Users = new HashSet<User>(); }
        public byte Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public partial class Mark
    {
        public int Id { get; set; }
        public int JuryId { get; set; }
        public int ActivityId { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual User User { get; set; }            // член жюри
    }

    public partial class Role
    {
        public Role() { Users = new HashSet<User>(); }
        public byte Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public partial class User
    {
        public User()
        {
            Activities = new HashSet<Activity>();
            Events = new HashSet<Event>();
            Marks = new HashSet<Mark>();
        }
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public int? CountryId { get; set; }
        public int IdNumber { get; set; }
        public string Password { get; set; }
        public string Photo { get; set; }
        public byte GenderId { get; set; }
        public byte RoleId { get; set; }
        public int? CourseId { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
        public virtual Country Country { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual ICollection<Mark> Marks { get; set; }
        public virtual Role Role { get; set; }
    }
}
