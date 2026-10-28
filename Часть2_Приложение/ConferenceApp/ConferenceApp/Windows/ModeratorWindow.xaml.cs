using ConferenceApp.Models;
using System;
using System.Windows;
namespace ConferenceApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для ModeratorWindow.xaml
    /// </summary>
    public partial class ModeratorWindow : Window
    {
        public ModeratorWindow()
        {
            InitializeComponent();
        }
        // событие происходит после закрытия окна
        private void Window_Closed(object sender, EventArgs e)
        {
            Manager.CurrentUser = null; // забываем пользователя
            Owner.Show();               // показываем главную форму
        }
        // если пользователь выберет Cancel, окно не закроется
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult x = MessageBox.Show("Вы действительно хотите выйти из системы?",
                "Выйти", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (x == MessageBoxResult.Cancel)
                e.Cancel = true;
        }
    }
}
