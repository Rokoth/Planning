using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Client.ClientHttpClient;
using Planning.Contract.Model;
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
                    //var tree = await _dataService.GetTree(_id.Value);
                    //NameTextBox.Text = tree.Name;
                    //DescriptionTextBox.Text = tree.Description;
                    //SetFormula(tree.FormulaId);
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
            switch (_mode)
            {
                case AddEditMode.Add:
                    if (string.IsNullOrEmpty(NameTextBox.Text))
                    {
                        MessageBox.Show("Не задано наименование");
                    }      
                    else if (string.IsNullOrEmpty(PriorityTextBox.Text))
                    {
                        MessageBox.Show("Не задан приоритет");
                    }
                    else
                    {
                        try
                        {
                            var result = await _dataService.AddProject(new ProjectCreator()
                            {
                                ParentId = parentId,
                                Path = PathTextBox.Text,
                                Name = NameTextBox.Text,
                                Period = int.Parse(PeriodTextBox.Text),
                                Priority = int.Parse(PriorityTextBox.Text)
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
                    }
                    break;
                case AddEditMode.Edit:
                    //if (string.IsNullOrEmpty(NameTextBox.Text))
                    //{
                    //    MessageBox.Show("Не задано наименование");
                    //}
                    //else if (formulaId == null)
                    //{
                    //    MessageBox.Show("Не задана формула");
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        var result = await _dataService.UpdateTree(new TreeUpdater()
                    //        {
                    //            Description = DescriptionTextBox.Text,
                    //            FormulaId = formulaId.Value,
                    //            Name = NameTextBox.Text,
                    //            Id = _id.Value
                    //        });
                    //        if (result == null)
                    //            MessageBox.Show("Неизвестная ошибка при сохранении дерева");
                    //        else
                    //        {
                    //            if (OnTreeAdded != null)
                    //                OnTreeAdded(this, new ChangeTreeArgs()
                    //                {
                    //                    Id = result.Id
                    //                });
                    //            Close();
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        _logger.LogError($"Ошибка при сохранении дерева: {ex.Message} {ex.StackTrace}");
                    //        MessageBox.Show($"Ошибка при сохранении дерева: {ex.Message}");
                    //    }
                    //}
                    break;               
            }
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
