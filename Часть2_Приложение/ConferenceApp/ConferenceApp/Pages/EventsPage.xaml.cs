using ConferenceApp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace ConferenceApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для EventsPage.xaml
    /// </summary>
    public partial class EventsPage : Page
    {
        int _itemcount = 0; // количество записей в таблице Event
        bool _loaded = false;
        public EventsPage()
        {
            InitializeComponent();
            LoadAndInitData();
        }
        /// <summary>Загрузка и подготовка данных страницы</summary>
        void LoadAndInitData()
        {
            try
            {
                // загрузка мероприятий в ListBox, сортировка по дате
                ListBoxEvents.ItemsSource = ConferenceEntities.GetContext().Events
                    .OrderBy(p => p.Date).ToList();
                _itemcount = ListBoxEvents.Items.Count;
                // список направлений
                var courses = ConferenceEntities.GetContext().Courses
                    .OrderBy(p => p.EventTitle).ToList();
                courses.Insert(0, new Course { EventTitle = "Все типы", CourseTitle = "Все типы" });
                ComboCourse.ItemsSource = courses;
                ComboCourse.SelectedIndex = 0;
                TextBlockInfo.Text = $" Результат запроса: {_itemcount} записей из {_itemcount}";
                _loaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к БД DEMO2023:\n" + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PageIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // обновление данных после каждой активации страницы
            if (Visibility == Visibility.Visible && _loaded)
            {
                ConferenceEntities.GetContext().ChangeTracker.Entries().ToList()
                    .ForEach(p => p.Reload());
                UpdateData();
            }
        }
        private void TBoxSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateData();
        }
        /// <summary>Фильтрация и сортировка данных</summary>
        private void UpdateData()
        {
            if (!_loaded) return;
            var currentEvents = ConferenceEntities.GetContext().Events
                .OrderBy(p => p.Date).ToList();
            // фильтр по направлению
            if (ComboCourse.SelectedIndex > 0)
                currentEvents = currentEvents
                    .Where(p => p.CourseId == (ComboCourse.SelectedItem as Course).Id).ToList();
            // фильтр по дате
            if (DatePickerCurrentDate.SelectedDate != null)
                currentEvents = currentEvents
                    .Where(p => p.Date == DatePickerCurrentDate.SelectedDate.Value.Date).ToList();
            // поиск по названию
            currentEvents = currentEvents
                .Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            ListBoxEvents.ItemsSource = currentEvents;
            TextBlockInfo.Text = $" Результат запроса: {currentEvents.Count} записей из {_itemcount}";
        }
        private void ComboCourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }
        private void DatePickerCurrentDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }
    }
}
