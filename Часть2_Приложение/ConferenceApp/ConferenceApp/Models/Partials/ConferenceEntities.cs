using System.Data.Entity; // нужно добавить для контекста
// пространство имен такое же как и у ConferenceEntities из папки Models
namespace ConferenceApp.Models
{
    // partial - частичный, т.е. его часть в другом файле
    public partial class ConferenceEntities : DbContext
    {
        private static ConferenceEntities _context;
        /// <summary>
        /// Метод возвращающий контекст подключения (Singleton)
        /// </summary>
        public static ConferenceEntities GetContext()
        {
            if (_context == null)
                _context = new ConferenceEntities();
            return _context;
        }
    }
}
