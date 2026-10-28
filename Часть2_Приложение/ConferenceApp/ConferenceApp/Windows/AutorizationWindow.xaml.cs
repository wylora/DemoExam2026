using ConferenceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
namespace ConferenceApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для AutorizationWindow.xaml
    /// </summary>
    public partial class AutorizationWindow : Window
    {
        // флаг: пользователь не смог войти с первого раза -> показываем капчу
        bool b = false;
        int count = 0;
        DispatcherTimer timer = new DispatcherTimer();
        int seconds = 0;     // секунды блокировки
        string captcha = ""; // текст капчи

        public AutorizationWindow()
        {
            InitializeComponent();
            LoadAndInitData();
        }
        /// <summary>Загружает и инициализирует данные</summary>
        void LoadAndInitData()
        {
            this.Height = 200;
            timer.Tick += timer_Tick;
            // скрываем строки с капчой
            RowCaptcha1.Height = new GridLength(0);
            RowCaptcha2.Height = new GridLength(0);
        }
        // срабатывает каждую секунду
        void timer_Tick(object sender, EventArgs e)
        {
            seconds -= 1;
            TextBlockTime.Text = $"Осталось {seconds} секунд до разблокировки";
            if (seconds == 0) // останавливаем таймер, разблокируем кнопку
            {
                BtnOk.IsEnabled = true;
                TextBlockTime.Text = "";
                timer.Stop();
            }
        }
        private void BtnOkClick(object sender, RoutedEventArgs e)
        {
            try
            {
                List<User> users = ConferenceEntities.GetContext().Users.ToList();
                User user = users.FirstOrDefault(p => p.Password == TbPass.Password
                                                   && p.IdNumber.ToString() == TbLogin.Text.Trim());
                // удачный вход: логин+пароль, либо логин+пароль+капча
                if ((user != null && !b) || (user != null && b && TbCaptcha.Text == captcha))
                {
                    Manager.CurrentUser = user;
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль" + (b ? " или капча" : ""));
                    count++;
                    if (count >= 3)
                    {
                        b = true;
                        this.Height = 350;
                        ShowCaptcha();
                        // блокируем кнопку на 10 секунд
                        timer.Interval = TimeSpan.FromSeconds(1);
                        BtnOk.IsEnabled = false;
                        seconds = 10;
                        TextBlockTime.Text = $"Осталось {seconds} секунд до разблокировки";
                        timer.Start();
                        RowCaptcha1.Height = new GridLength(34);
                        RowCaptcha2.Height = new GridLength(1, GridUnitType.Star);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void BtnRenewCaptcha_Click(object sender, RoutedEventArgs e)
        {
            ShowCaptcha();
        }
        /// <summary>отображает капчу</summary>
        void ShowCaptcha()
        {
            var tuple = MakeCaptcha.CreateImage(300, 100, 4);
            ImageCaptcha.Source = tuple.image;
            captcha = tuple.captcha;
        }
    }
}
