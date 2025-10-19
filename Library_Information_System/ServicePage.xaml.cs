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

            // Поиск
            currentProducts = currentProducts.Where(p =>
                (p.Istoch_lit_Name.ToLower().Contains(TBoxSearch.Text.ToLower()) ||
                 p.Izdatelstvo.ToLower().Contains(TBoxSearch.Text.ToLower()))
            ).ToList();

            // Фильтрация
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
            _currentPage = 1; // Сбрасываем на первую страницу при каждом обновлении
            ChangePage();
        }

        private void ChangePage()
        {
            if (_filteredProducts == null) return;

            int totalItems = _filteredProducts.Count;
            _maxPages = (int)Math.Ceiling((double)totalItems / PageSize);

            if (_maxPages == 0) _maxPages = 1; // Если записей нет, все равно будет 1 страница
            if (_currentPage > _maxPages) _currentPage = _maxPages;

            // Получаем записи для текущей страницы
            var pageItems = _filteredProducts.Skip((_currentPage - 1) * PageSize).Take(PageSize).ToList();
            ProductListView.ItemsSource = pageItems;

            // Обновляем информацию о страницах и количестве записей
            PageInfoTextBlock.Text = $"Страница {_currentPage}/{_maxPages}";
            RecordCountTextBlock.Text = $"Показано: {pageItems.Count} из {totalItems}";

            // Управляем доступностью кнопок
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

        // Эти методы пока остаются пустыми
        private void Delete_Click(object sender, RoutedEventArgs e) { }
        private void Redact_Click(object sender, RoutedEventArgs e) { }
    }
}