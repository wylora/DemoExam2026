using System.IO;
namespace ConferenceApp.Models
{
    public partial class Event
    {
        /// <summary>
        /// Возвращает абсолютный путь к логотипу мероприятия
        /// </summary>
        public string GetLogo
        {
            get
            {
                string noPhoto = Directory.GetCurrentDirectory() + @"\Images\picture.png";
                if (string.IsNullOrWhiteSpace(Logo))
                    return noPhoto;
                string path = Directory.GetCurrentDirectory() + @"\Images\Events\" + Logo.Trim();
                return File.Exists(path) ? path : noPhoto;
            }
        }
    }
}
