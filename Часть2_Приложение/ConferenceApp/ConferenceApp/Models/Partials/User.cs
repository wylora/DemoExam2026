using System.IO;
namespace ConferenceApp.Models
{
    public partial class User
    {
        /// <summary>
        /// Возвращает абсолютный путь к фотографии пользователя
        /// </summary>
        public string GetPhoto
        {
            get
            {
                string dir = Directory.GetCurrentDirectory() + @"\Images\";
                // если поле фото пустое, отобразим картинку-заглушку
                if (string.IsNullOrWhiteSpace(Photo))
                    return dir + "picture.png";
                string folder = "Events";
                if (RoleId == 1) folder = "Participants"; // участник
                if (RoleId == 2) folder = "Moderators";   // модератор
                if (RoleId == 3) folder = "Organizers";   // организатор
                if (RoleId == 4) folder = "Jury";         // жюри
                string path = dir + folder + @"\" + Photo.Trim();
                return File.Exists(path) ? path : dir + "picture.png";
            }
        }
    }
}
