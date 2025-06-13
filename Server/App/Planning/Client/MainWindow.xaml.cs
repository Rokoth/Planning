using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Client.ClientHttpClient;
using Planning.Contract.Model;
using PlanningClient;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PlanningClient
{
    public class ErrorEventArgs
    {
        public string Error { get; set; }
    }

    public class ServerConnectEventArgs
    {
        public string ServerAddress { get; set; }
    }

    public enum AddEditMode
    { 
       Add = 0,
       Edit = 2
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        public MainWindow(IServiceProvider serviceProvider)
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

        private void EditSelected()
        {
            var addTreeWindow = _serviceProvider.GetService<ScheduleAddEditWindow>();
            var row = DataGridMain.SelectedItem;
            if (row != null)
            {
                addTreeWindow.ShowDialog(AddEditMode.Edit, ((Schedule)row).Id);
            }

            FillTable();
        }

        private async void FillTable()
        {
            try
            {
                var perPage = int.Parse(CountTextBox.Text);
                DataGridMain.Items.Clear();
                var result = await _dataService.GetSchedules(FilterTextBox.Text, page, perPage, null);
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
                    logger.LogError($"Ошибка при получении списка деревьев: {message} {stack}");
                    MessageBox.Show($"Ошибка при получении списка деревьев: {message}");
                }
                else
                {
                    logger.LogError($"Ошибка при получении списка деревьев: {ex.Message} {ex.StackTrace}");
                    MessageBox.Show($"Ошибка при получении списка деревьев: {ex.Message}");
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditSelected();
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

        private void MainWindow_OnServerConnected(object sender, ServerConnectEventArgs e)
        {
            MessageBox.Show($"Подключение к серверу {e.ServerAddress} успешно ");
            _dbService.SaveSettings("ServerAddress", e.ServerAddress);
        }

        private void MainWindow_OnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show("Произошла ошибка: " + e.Error);
        }

        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            var win = _serviceProvider.GetRequiredService<ProjectWindow>(); 
            win.ShowDialog();
            FillTable();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
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

        private void FormulasButton_Click(object sender, RoutedEventArgs e)
        {
            var win = _serviceProvider.GetRequiredService<FormulaWindow>();
            win.ShowDialog();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            FillTable();
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var win = _serviceProvider.GetRequiredService<ProjectAddEditWindow>();
            win.ShowDialog(AddEditMode.Add, null);
        }

        private void AddFormulaButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            var win = _serviceProvider.GetRequiredService<ScheduleAddEditWindow>();
            win.ShowDialog(AddEditMode.Add, null);
            FillTable();
        }

        private void CountTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void CountTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }
    }
}
