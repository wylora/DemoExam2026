using ConferenceApp.Models;
using ConferenceApp.Pages;
using ConferenceApp.Windows;
using System;
using System.Windows;
namespace ConferenceApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }
        /// <summary>загружаем данные и инициализируем переменные</summary>
        void LoadData()
        {
            MainFrame.Navigate(new EventsPage());
            Manager.MainFrame = MainFrame;
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        // если пользователь выберет Cancel, то форму не закроем
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult x = MessageBox.Show("Вы действительно хотите выйти?", "Выйти",
                MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (x == MessageBoxResult.Cancel)
                e.Cancel = true;
        }
        // авторизация
        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AutorizationWindow window = new AutorizationWindow();
                window.Owner = this;
                // открытие формы в режиме модального окна
                if (window.ShowDialog() == true)
                {
                    int role = Manager.CurrentUser.RoleId;
                    Window next = null;
                    if (role == 1) next = new ParticipantWindow(); // участник
                    if (role == 2) next = new ModeratorWindow();   // модератор
                    if (role == 3) next = new OrganizerWindow();   // организатор
                    if (role == 4) next = new JuryWindow();        // жюри
                    if (next != null)
                    {
                        next.Owner = this;
                        this.Hide(); // скрываем главную форму
                        next.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
