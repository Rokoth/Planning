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
    public partial class ProjectDetailWindow : Window
    {
        private AddEditMode _mode = AddEditMode.Add;        
        
        private IServiceProvider _serviceProvider;       
        private ILogger _logger;

        private Guid? _id = null;
        private Guid? parentId = null;       

        public event EventHandler<ChangeProjectArgs> OnProjectChanged;

        public ProjectDetailWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;           
            _logger = _serviceProvider.GetRequiredService<ILogger<ProjectAddEditWindow>>();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }       
    }  
}
