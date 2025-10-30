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
    public partial class ServicePage : Page
    {
        private int _currentPage = 1;
        private const int PageSize = 10;
        private int _maxPages;
        private List<Пополнение_фонда> _filteredProducts;

        public ServicePage()
        {
            InitializeComponent();
            UpdateProducts();
        }

        public void UpdateProducts()
        {
            var currentProducts = Library_Information_SystemEntities.getInstance().Пополнение_фонда.ToList();

            currentProducts = currentProducts.Where(p =>
                (p.Istoch_lit_Name.ToLower().Contains(TBoxSearch.Text.ToLower()) ||
                 p.Izdatelstvo.ToLower().Contains(TBoxSearch.Text.ToLower()))
            ).ToList();

            switch (FiltrComboBox.SelectedIndex)
            {
                case 1:
                    currentProducts = currentProducts.Where(p => p.Izdatelstvo.Contains("Издательский дом «Республика Башкортостан»")).ToList();
                    break;
                case 2:
                    currentProducts = currentProducts.Where(p => p.Izdatelstvo.Contains("Башкирское книжное издательство")).ToList();
                    break;
                case 3:
                    currentProducts = currentProducts.Where(p => p.Izdatelstvo.Contains("Издательство УГНТУ")).ToList();
                    break;
                case 4:
                    currentProducts = currentProducts.Where(p => p.Izdatelstvo.Contains("Уфа")).ToList();
                    break;
                case 5:
                    currentProducts = currentProducts.Where(p => p.Izdatelstvo.Contains("Эксмо")).ToList();
                    break;
                case 6:
                    currentProducts = currentProducts.Where(p => p.Izdatelstvo.Contains("Китап")).ToList();
                    break;
                case 7:
                    currentProducts = currentProducts.Where(p => p.Izdatelstvo.Contains("Детская и юношеская книга")).ToList();
                    break;
            }

            // Сортировка
            if (SortComboBox.IsChecked == true)
            {
                currentProducts = currentProducts.OrderBy(p => p.Kolvo_ekz).ToList();
            }
            if (SortComboBox1.IsChecked == true)
            {
                currentProducts = currentProducts.OrderByDescending(p => p.Kolvo_ekz).ToList();
            }

            _filteredProducts = currentProducts;
            _currentPage = 1;
            ChangePage();
        }

        private void ChangePage()
        {
            if (_filteredProducts == null) return;

            int totalItems = _filteredProducts.Count;
            _maxPages = (int)Math.Ceiling((double)totalItems / PageSize);

            if (_maxPages == 0) _maxPages = 1;
            if (_currentPage > _maxPages) _currentPage = _maxPages;

            var pageItems = _filteredProducts.Skip((_currentPage - 1) * PageSize).Take(PageSize).ToList();
            ProductListView.ItemsSource = pageItems;

            PageInfoTextBlock.Text = $"Страница {_currentPage}/{_maxPages}";
            RecordCountTextBlock.Text = $"Показано: {pageItems.Count} из {totalItems}";

            PrevPageButton.IsEnabled = _currentPage > 1;
            NextPageButton.IsEnabled = _currentPage < _maxPages;
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ChangePage();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _maxPages)
            {
                _currentPage++;
                ChangePage();
            }
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e) => UpdateProducts();
        private void FiltrComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateProducts();
        private void SortComboBox_Checked(object sender, RoutedEventArgs e) => UpdateProducts();
        private void SortComboBox1_Checked(object sender, RoutedEventArgs e) => UpdateProducts();

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Пополнение_фонда itemToDelete)
            {
                var context = Library_Information_SystemEntities.getInstance();

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить источник \"{itemToDelete.Istoch_lit_Name}\"?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var existingItem = context.Пополнение_фонда.Find(itemToDelete.ID);

                        if (existingItem != null)
                        {
                            context.Пополнение_фонда.Remove(existingItem);
                            context.SaveChanges();

                            MessageBox.Show("Источник успешно удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            UpdateProducts();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}\nВозможно, этот источник используется в другой таблице.",
                                        "Ошибка",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Redact_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var selectedItem = button.DataContext as Пополнение_фонда;
            if (selectedItem != null && NavigationService != null)
            {
                NavigationService.Navigate(new AddEditPage(selectedItem));
            }
        }

        private void BtnAddRecord_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
            {
                NavigationService.Navigate(new AddEditPage(null));
            }
        }
    }
}