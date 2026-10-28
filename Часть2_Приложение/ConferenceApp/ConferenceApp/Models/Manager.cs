using System.Windows.Controls;
namespace ConferenceApp.Models
{
    public class Manager
    {
        /// <summary>Фрейм, в котором отображаются Page</summary>
        public static Frame MainFrame { get; set; }
        /// <summary>Текущий пользователь системы</summary>
        public static User CurrentUser { get; set; }
    }
}
