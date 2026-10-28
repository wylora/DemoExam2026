using ConferenceApp.Models;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
namespace ConferenceApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrganizerWindow.xaml
    /// </summary>
    public partial class OrganizerWindow : Window
    {
        public OrganizerWindow()
        {
            InitializeComponent();
            InitData();
        }
        // начальные данные формы
        void InitData()
        {
            // приветствие в зависимости от времени суток
            DateTime time = DateTime.Now;
            int m = time.Hour * 60 + time.Minute; // минут с начала суток
            string s = "Доброй ночи!";
            if (m >= 9 * 60 && m <= 11 * 60)
                s = "Доброе утро!";      // 9:00 - 11:00
            else if (m > 11 * 60 && m <= 18 * 60)
                s = "Добрый день!";      // 11:01 - 18:00
            else if (m > 18 * 60)
                s = "Добрый вечер!";     // 18:01 - 24:00
            TextBlockDayTime.Text = s;

            if (Manager.CurrentUser.GenderId == 1)
                TextBlockUserName.Text = $"Ms {Manager.CurrentUser.Name} {Manager.CurrentUser.Patronymic}";
            else
                TextBlockUserName.Text = $"Mr {Manager.CurrentUser.Name} {Manager.CurrentUser.Patronymic}";

            try
            {
                ImagePhoto.Source = new BitmapImage(new Uri(Manager.CurrentUser.GetPhoto, UriKind.Absolute));
            }
            catch { /* фото не найдено */ }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Manager.CurrentUser = null;
            Owner.Show();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult x = MessageBox.Show("Вы действительно хотите выйти из системы?",
                "Выйти", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (x == MessageBoxResult.Cancel)
                e.Cancel = true;
        }
        // Открытие формы регистрации жюри/модератора
        private void ButtonAddJuryModerator_Click(object sender, RoutedEventArgs e)
        {
            AddJuryModeratorWindow addJuryModeratorWindow = new AddJuryModeratorWindow();
            addJuryModeratorWindow.Owner = this;
            addJuryModeratorWindow.ShowDialog();
        }
    }
}
