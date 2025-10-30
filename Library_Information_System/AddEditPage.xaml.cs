using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library_Information_System
{
    public partial class AddEditPage : Page
    {
        private Пополнение_фонда _currentItem;

        public AddEditPage(Пополнение_фонда selectedItem)
        {
            InitializeComponent();

            string[] allowedFunds = { "Основной книжный фонд", "Электронные ресурсы" };
            var allFunds = Library_Information_SystemEntities.getInstance().Фонд_Библиотеки.ToList();
            var allSotrudniki = Library_Information_SystemEntities.getInstance().Сотрудники.ToList();
            var allTipLits = Library_Information_SystemEntities.getInstance().Тип_литературы.ToList();

            CBoxFond.ItemsSource = allFunds
                .Where(f => allowedFunds.Contains(f.Name_Fond))
                .ToList();
            CBoxSotrudnik.ItemsSource = allSotrudniki;
            CBoxTipLit.ItemsSource = allTipLits;

            if (selectedItem != null)
            {
                _currentItem = selectedItem;

                CBoxFond.SelectedItem = (CBoxFond.ItemsSource as List<Фонд_Библиотеки>)
                                        .FirstOrDefault(f => f.FondID == _currentItem.FondID);
                CBoxSotrudnik.SelectedItem = allSotrudniki
                                        .FirstOrDefault(s => s.Sotrudnik_ID == _currentItem.Sotrudnik_ID);
                CBoxTipLit.SelectedItem = allTipLits
                                        .FirstOrDefault(t => t.Tip_LitID == _currentItem.Tip_LitID);
            }
            else
            {
                _currentItem = new Пополнение_фонда();
            }
            DataContext = _currentItem;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentItem.Istoch_lit_Name))
                errors.AppendLine("Укажите название источника");
            if (string.IsNullOrWhiteSpace(_currentItem.Izdatelstvo))
                errors.AppendLine("Укажите издательство");

            if (_currentItem.Data_izdania <= 1000 || _currentItem.Data_izdania > DateTime.Now.Year)
                errors.AppendLine("Укажите корректный год издания");
            if (_currentItem.Kolvo_ekz < 0)
                errors.AppendLine("Количество экземпляров не может быть отрицательным");

            if (_currentItem.FondID == 0)
                errors.AppendLine("Выберите фонд библиотеки (Основной книжный фонд или Электронные ресурсы)");
            if (_currentItem.Sotrudnik_ID == 0)
                errors.AppendLine("Выберите сотрудника");
            if (_currentItem.Tip_LitID == 0)
                errors.AppendLine("Выберите тип литературы");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var context = Library_Information_SystemEntities.getInstance();

                if (_currentItem.ID > 0)
                {
                    var existingItem = context.Пополнение_фонда.Find(_currentItem.ID);

                    if (existingItem != null)
                    {
                        existingItem.Istoch_lit_Name = _currentItem.Istoch_lit_Name;
                        existingItem.Izdatelstvo = _currentItem.Izdatelstvo;
                        existingItem.Data_izdania = _currentItem.Data_izdania;
                        existingItem.Kolvo_ekz = _currentItem.Kolvo_ekz;
                        existingItem.FondID = _currentItem.FondID;
                        existingItem.Sotrudnik_ID = _currentItem.Sotrudnik_ID;
                        existingItem.Tip_LitID = _currentItem.Tip_LitID;
                    }
                }
                else
                {
                    context.Пополнение_фонда.Add(_currentItem);
                }

                context.SaveChanges();
                MessageBox.Show("Информация успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}