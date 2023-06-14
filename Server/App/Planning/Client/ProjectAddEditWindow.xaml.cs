using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Client.ClientHttpClient;
using Planning.Contract.Model;
using System;
using System.Windows;

namespace PlanningClient
{
    /// <summary>
    /// Логика взаимодействия для ProjectAddEditWindow.xaml
    /// </summary>
    public partial class ProjectAddEditWindow : Window
    {
        private AddEditMode _mode = AddEditMode.Add;        
        
        private IServiceProvider _serviceProvider;
        private IDataService _dataService;
        private ILogger _logger;

        private Guid? _id = null;
        private Guid? parentId = null;       

        public event EventHandler<ChangeProjectArgs> OnProjectChanged;

        public ProjectAddEditWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _dataService = _serviceProvider.GetRequiredService<IDataService>();
            _logger = _serviceProvider.GetRequiredService<ILogger<ProjectAddEditWindow>>();
        }

        public async void ShowDialog(AddEditMode mode, Guid? id)
        {
            _mode = mode;
            _id = id;

            switch (mode)
            {
                case AddEditMode.Add:
                    this.Title = "Добавление проекта";
                    break;
                case AddEditMode.Edit:
                    _id = id;
                    this.Title = "Редактирование проекта";
                    var project = await _dataService.GetProject(_id.Value);
                    NameTextBox.Text = project.Name;
                    PathTextBox.Text = project.Path;
                    if(project.ParentId.HasValue)
                        SetParent(project.ParentId.Value);
                    PeriodTextBox.Text = project.Period.ToString();
                    PriorityTextBox.Text = project.Priority.ToString();
                    break;               
            }
            
            ShowDialog();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private async void Save()
        {
            uint priority = 0;
            uint period = 0;

            if (!ValidateFields(out priority, out period)) return;

            switch (_mode)
            {
                case AddEditMode.Add:
                    try
                    {
                        var result = await _dataService.AddProject(new ProjectCreator()
                        {
                            ParentId = parentId,
                            Path = PathTextBox.Text,
                            Name = NameTextBox.Text,
                            Period = (int)period,
                            Priority = (int)priority
                        });
                        if (result == null)
                            MessageBox.Show("Неизвестная ошибка при сохранении проекта");
                        else
                        {
                            if (OnProjectChanged != null)
                                OnProjectChanged(this, new ChangeProjectArgs()
                                {
                                    Id = result.Id
                                });
                            Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Ошибка при сохранении проекта: {ex.Message} {ex.StackTrace}");
                        MessageBox.Show($"Ошибка при сохранении проекта: {ex.Message}");
                    }

                    break;
                case AddEditMode.Edit:                    
                    try
                    {
                        var result = await _dataService.UpdateProject(new ProjectUpdater()
                        {             
                            Id = _id.Value,
                            ParentId = parentId,
                            Path = PathTextBox.Text,
                            Name = NameTextBox.Text,
                            Period = (int)period,
                            Priority = (int)priority
                        });
                        if (result == null)
                            MessageBox.Show("Неизвестная ошибка при сохранении проекта");
                        else
                        {
                            if (OnProjectChanged != null)
                                OnProjectChanged(this, new ChangeProjectArgs()
                                {
                                    Id = result.Id
                                });
                            Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Ошибка при сохранении проекта: {ex.Message} {ex.StackTrace}");
                        MessageBox.Show($"Ошибка при сохранении проекта: {ex.Message}");
                    }
                    break;               
            }
        }

        private bool ValidateFields(out uint priority, out uint period)
        {
            priority = 0;
            period = 0;
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                MessageBox.Show("Не задано наименование");
                return false;
            }

            if (string.IsNullOrEmpty(PathTextBox.Text))
            {
                MessageBox.Show("Не задан путь");
                return false;
            }

            if (string.IsNullOrEmpty(PriorityTextBox.Text))
            {
                MessageBox.Show("Не задан приоритет");
                return false;
            }

            if (string.IsNullOrEmpty(PeriodTextBox.Text))
            {
                MessageBox.Show("Не задан период");
                return false;
            }

            if (!uint.TryParse(PriorityTextBox.Text, out priority) || priority > 10000)
            {
                MessageBox.Show("Приоритет должен быть задан целым положительным числом не более 10000");
                return false;
            }

            if (!uint.TryParse(PeriodTextBox.Text, out period))
            {
                MessageBox.Show("Период должен быть задан целым положительным числом");
                return false;
            }

            return true;
        }

        private void ParentButton_Click(object sender, RoutedEventArgs e)
        {
            var listWindow = _serviceProvider.GetRequiredService<ProjectSelectWindow>();
            listWindow.OnElementSelected += ParentWindow_OnParentSelected;
            listWindow.ShowDialog();
        }

        private void ParentWindow_OnParentSelected(object sender, ElementSelectedArgs e)
        {
            SetParent(e.Id);
        }

        private async void SetParent(Guid id)
        {
            parentId = id;
            var parent = await _dataService.GetProject(id);
            ParentTextBox.Text = parent.Name;
        }
    }

    public class ChangeProjectArgs
    {
        public Guid Id { get; set; }
    }
}
