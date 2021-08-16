using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Client.ClientHttpClient;
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
using System.Windows.Shapes;

namespace PlanningClient
{
    /// <summary>
    /// Логика взаимодействия для ProjectWindow.xaml
    /// </summary>
    public partial class ProjectWindow : Window
    {
        private IServiceProvider _serviceProvider;
        private readonly IDataService _dataService;
        private readonly ILogger logger;
        private readonly IClientHttpClient httpClient;
        private readonly IDbService _dbService;

        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<ServerConnectEventArgs> OnServerConnected;

        private bool isLoaded = false;

        private int page = 0;
        private int allPages = 1;
        private bool needRefresh = false;


        public ProjectWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            _dataService = _serviceProvider.GetRequiredService<IDataService>();

            logger = _serviceProvider.GetRequiredService<ILogger<MainWindow>>();
            httpClient = _serviceProvider.GetRequiredService<IClientHttpClient>();
            OnError += MainWindow_OnError;
            OnServerConnected += MainWindow_OnServerConnected;

            Task.Factory.StartNew(RunTimer, TaskCreationOptions.LongRunning);
            DataGridMain.MouseDoubleClick += EditButton_Click;

            isLoaded = true;
        }

        private void MainWindow_OnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show("Произошла ошибка: " + e.Error);
        }

        private void MainWindow_OnServerConnected(object sender, ServerConnectEventArgs e)
        {
            MessageBox.Show($"Подключение к серверу {e.ServerAddress} успешно ");
            _dbService.SaveSettings("ServerAddress", e.ServerAddress);
        }

        private async Task RunTimer()
        {
            while (true)
            {
                if (needRefresh)
                {
                    Dispatcher.Invoke(() => FillTable());
                    //FillTable();
                    needRefresh = false;
                }
                await Task.Delay(1000);
            }
        }

        private async void FillTable()
        {
            try
            {
                var perPage = int.Parse(CountTextBox.Text);
                DataGridMain.Items.Clear();
                var result = await _dataService.GetProjects(FilterTextBox.Text, page, perPage, null);
                allPages = (result.Count % perPage == 0) ? (result.Count / perPage) : ((result.Count / perPage) + 1);
                foreach (var item in result.Items)
                {
                    DataGridMain.Items.Add(item);
                }

                CountLabel.Content = $"Страница {page + 1} из {allPages}";
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aex)
                {
                    var message = "";
                    var stack = "";
                    foreach (var exs in aex.InnerExceptions)
                    {
                        message += exs.Message + "\r\n";
                        stack += exs.StackTrace + "\r\n";
                    }
                    logger.LogError($"Ошибка при получении списка проектов: {message} {stack}");
                    MessageBox.Show($"Ошибка при получении списка проектов: {message}");
                }
                else
                {
                    logger.LogError($"Ошибка при получении списка проектов: {ex.Message} {ex.StackTrace}");
                    MessageBox.Show($"Ошибка при получении списка проектов: {ex.Message}");
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddProject();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditSelected();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Удалить проект?", "Удаление проекта", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                var row = DataGridMain.SelectedItem;
                if (row != null)
                {
                    if (await _dataService.DeleteProject(((Planning.Contract.Model.Project)row).Id))
                    {
                        FillTable();
                    }                    
                }                
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }     

        private void CountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }               

        private void DataGridCell_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void DataGridCell_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditSelected();
        }

        private void DataGridMain_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void DataGridMain_Loaded(object sender, RoutedEventArgs e)
        {
            FillTable();
        }

        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            IncPage(false, true);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            IncPage(false, false);
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            IncPage(true, false);
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            IncPage(true, true);
        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FillTable();
        }

        private void EditSelected()
        {
            var addTreeWindow = _serviceProvider.GetRequiredService<ProjectAddEditWindow>();
            var row = DataGridMain.SelectedItem;
            if (row != null)
            {
                addTreeWindow.ShowDialog(AddEditMode.Edit, ((Planning.Contract.Model.Project)row).Id);
            }

            FillTable();
        }

        private void AddProject()
        {
            var addTreeWindow = _serviceProvider.GetRequiredService<ProjectAddEditWindow>();
            addTreeWindow.ShowDialog(AddEditMode.Add, null);

            FillTable();
        }

        private void IncPage(bool inc, bool end)
        {
            bool changed = false;
            if (inc)
            {
                if (page + 1 < allPages)
                {
                    if (end)
                    {
                        page = allPages - 1;
                    }
                    else
                    {
                        page++;
                    }
                    changed = true;
                }
            }
            else
            {
                if (page > 0)
                {
                    if (end)
                    {
                        page = 0;
                    }
                    else
                    {
                        page--;
                    }
                    changed = true;
                }
            }
            if (changed)
            {
                FillTable();
            }
        }

    }
}
