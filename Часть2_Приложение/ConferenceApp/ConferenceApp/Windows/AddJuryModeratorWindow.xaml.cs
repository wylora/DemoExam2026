using ConferenceApp.Models;
using Microsoft.Win32;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
namespace ConferenceApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddJuryModeratorWindow.xaml
    /// </summary>
    public partial class AddJuryModeratorWindow : Window
    {
        private User _currentUser = new User();   // новый пользователь
        private string _filePath = null;          // путь к выбранной фотографии
        private string _photoName = null;         // имя файла
        private static string _currentModeratorDirectory = Directory.GetCurrentDirectory() + @"\Images\Moderators\";
        private static string _currentJuryDirectory = Directory.GetCurrentDirectory() + @"\Images\Jury\";

        public AddJuryModeratorWindow()
        {
            InitializeComponent();
            LoadAndInitData();
        }
        // загрузка и начальные значения полей
        void LoadAndInitData()
        {
            var db = ConferenceEntities.GetContext();
            // новый IdNumber = максимальный + 1
            int numberid = 1;
            if (db.Users.Any())
                numberid = db.Users.Max(x => x.IdNumber) + 1;
            _currentUser.IdNumber = numberid;
            TextBoxIdNumber.Text = numberid.ToString();
            // выпадающие списки
            ComboCountry.ItemsSource = db.Countries.OrderBy(c => c.Title).ToList();
            ComboCourse.ItemsSource = db.Courses.ToList();
            ComboEvent.ItemsSource = db.Events.OrderBy(e => e.Title).ToList();
            // пока не отмечен флажок "прикрепить к мероприятию" - списки недоступны
            ComboEvent.IsEnabled = false;
            ComboActivity.IsEnabled = false;
        }
        /// <summary>
        /// Проверка пароля: не менее 6 символов, заглавные и строчные буквы,
        /// не менее одного спецсимвола и одной цифры
        /// </summary>
        bool CheckPassword(string password)
        {
            if (password.Length < 6)
                return false;
            if (password.ToLower() == password || password.ToUpper() == password)
                return false;
            int count = 0, spec = 0;
            foreach (char c in password)
            {
                if (char.IsDigit(c)) count++;
                if (!char.IsLetterOrDigit(c)) spec++;
            }
            return count > 0 && spec > 0;
        }
        string Password => CheckBoxIsVisiblePassword.IsChecked == true ? TextBoxPasswordFirst.Text : PasswordBoxFirst.Password;
        string PasswordAgain => CheckBoxIsVisiblePassword.IsChecked == true ? TextBoxPasswordAgain.Text : PasswordBoxAgain.Password;

        /// <summary>Проверка полей ввода</summary>
        private StringBuilder CheckFields()
        {
            StringBuilder s = new StringBuilder();
            if (string.IsNullOrWhiteSpace(TextBoxSurname.Text)) s.AppendLine("Укажите фамилию");
            if (string.IsNullOrWhiteSpace(TextBoxName.Text)) s.AppendLine("Укажите имя");
            if (DatePickerBirthday.SelectedDate == null) s.AppendLine("Укажите дату рождения");
            if (ComboBoxGender.SelectedIndex == -1) s.AppendLine("Выберите пол");
            if (ComboCountry.SelectedIndex == -1) s.AppendLine("Выберите страну");
            if (ComboBoxRole.SelectedIndex == -1) s.AppendLine("Выберите роль");
            if (string.IsNullOrWhiteSpace(TextBoxEmail.Text)) s.AppendLine("Укажите Email");
            if (MaskedTextBoxPhone.IsMaskCompleted == false) s.AppendLine("Укажите телефон полностью");
            if (ComboCourse.SelectedIndex == -1) s.AppendLine("Выберите направление");
            if (_filePath is null) s.AppendLine("Загрузите фотографию");
            if (string.IsNullOrWhiteSpace(Password))
                s.AppendLine("Укажите пароль");
            else if (Password != PasswordAgain)
                s.AppendLine("Пароли отличаются");
            else if (!CheckPassword(Password))
                s.AppendLine("Пароль должен соответствовать требованиям:\n - не менее 6 символов;" +
                             "\n - заглавные и строчные буквы;\n - не менее одного спецсимвола;\n - не менее одной цифры.");

            if (CheckBoxAttachToEvent.IsChecked == true && ComboActivity.SelectedIndex == -1)
                s.AppendLine("Выберите мероприятие и активность");

            if (CheckBoxAttachToEvent.IsChecked == true && ComboActivity.SelectedIndex != -1)
            {
                Activity activity = ConferenceEntities.GetContext().Activities.Find(ComboActivity.SelectedValue);
                // у активности может быть только один модератор
                if (ComboBoxRole.SelectedIndex == 0 && activity.ModeratorId != null)
                    s.AppendLine("Выберите другую активность, модератор уже назначен");
                // жюри - не более пяти человек
                if (ComboBoxRole.SelectedIndex == 1)
                {
                    int count = ConferenceEntities.GetContext().Marks.Count(x => x.ActivityId == activity.Id);
                    if (count > 4)
                        s.AppendLine("Выберите другую активность, количество членов жюри не более пяти человек");
                }
            }
            return s;
        }
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = CheckFields();
            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var db = ConferenceEntities.GetContext();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    _currentUser.IdNumber = Convert.ToInt32(TextBoxIdNumber.Text);
                    _currentUser.CourseId = Convert.ToInt32(ComboCourse.SelectedValue);
                    _currentUser.CountryId = Convert.ToInt32(ComboCountry.SelectedValue);
                    _currentUser.Surname = TextBoxSurname.Text.Trim();
                    _currentUser.Name = TextBoxName.Text.Trim();
                    _currentUser.Patronymic = TextBoxPatronymic.Text.Trim();
                    _currentUser.Email = TextBoxEmail.Text.Trim();
                    _currentUser.Birthday = DatePickerBirthday.SelectedDate.Value.Date;
                    _currentUser.Phone = MaskedTextBoxPhone.Text;
                    _currentUser.Password = Password;
                    _currentUser.GenderId = (byte)(ComboBoxGender.SelectedIndex == 1 ? 2 : 1);
                    _currentUser.RoleId = (byte)(ComboBoxRole.SelectedIndex == 1 ? 4 : 2);

                    string dir = _currentUser.RoleId == 4 ? _currentJuryDirectory : _currentModeratorDirectory;
                    Directory.CreateDirectory(dir);
                    string photo = ChangePhotoName(dir);
                    File.Copy(_filePath, dir + photo);
                    _currentUser.Photo = photo;

                    db.Users.Add(_currentUser);
                    db.SaveChanges();

                    if (CheckBoxAttachToEvent.IsChecked == true)
                    {
                        Activity activity = db.Activities.Find(ComboActivity.SelectedValue);
                        if (_currentUser.RoleId == 2)
                            activity.ModeratorId = _currentUser.Id;
                        if (_currentUser.RoleId == 4)
                            db.Marks.Add(new Mark { ActivityId = activity.Id, JuryId = _currentUser.Id });
                        db.SaveChanges();
                    }
                    transaction.Commit();
                    MessageBox.Show($"Запись добавлена. IdNumber для входа: {_currentUser.IdNumber}");
                    DialogResult = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // убираем несохранённые изменения из контекста
                    foreach (var entry in db.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList())
                    {
                        if (entry.State == EntityState.Added) entry.State = EntityState.Detached;
                        else entry.Reload();
                    }
                    _currentUser = new User();
                    MessageBox.Show(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
            }
        }
        private void ButtonLoadPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Выберите фотографию";
                op.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif";
                if (op.ShowDialog() == true)
                {
                    ImageUserPhoto.Source = new BitmapImage(new Uri(op.FileName));
                    _photoName = op.SafeFileName;
                    _filePath = op.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _filePath = null;
            }
        }
        // подбор свободного имени файла
        string ChangePhotoName(string currentDirectory)
        {
            string x = currentDirectory + _photoName;
            string photoname = _photoName;
            int i = 0;
            while (File.Exists(x))
            {
                i++;
                x = currentDirectory + i.ToString() + _photoName;
            }
            if (i > 0) photoname = i.ToString() + _photoName;
            return photoname;
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void CheckBoxAttachToEvent_Checked(object sender, RoutedEventArgs e)
        {
            ComboEvent.IsEnabled = true;
            ComboActivity.IsEnabled = true;
        }
        private void CheckBoxAttachToEvent_Unchecked(object sender, RoutedEventArgs e)
        {
            ComboEvent.IsEnabled = false;
            ComboActivity.IsEnabled = false;
        }
        // активности выбранного мероприятия
        private void ComboEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int eventId = Convert.ToInt32(ComboEvent.SelectedValue);
            ComboActivity.ItemsSource = ConferenceEntities.GetContext().Activities
                .Where(x => x.EventId == eventId).ToList();
        }
        // видимость пароля: в ячейке лежат PasswordBox и TextBox, показываем один из них
        private void CheckBoxIsVisiblePassword_Checked(object sender, RoutedEventArgs e)
        {
            TextBoxPasswordFirst.Text = PasswordBoxFirst.Password;
            PasswordBoxFirst.Visibility = Visibility.Collapsed;
            TextBoxPasswordFirst.Visibility = Visibility.Visible;
            TextBoxPasswordAgain.Text = PasswordBoxAgain.Password;
            PasswordBoxAgain.Visibility = Visibility.Collapsed;
            TextBoxPasswordAgain.Visibility = Visibility.Visible;
        }
        private void CheckBoxIsVisiblePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBoxFirst.Password = TextBoxPasswordFirst.Text;
            PasswordBoxFirst.Visibility = Visibility.Visible;
            TextBoxPasswordFirst.Visibility = Visibility.Collapsed;
            PasswordBoxAgain.Password = TextBoxPasswordAgain.Text;
            PasswordBoxAgain.Visibility = Visibility.Visible;
            TextBoxPasswordAgain.Visibility = Visibility.Collapsed;
        }
    }
}
