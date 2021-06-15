using Microsoft.Extensions.DependencyInjection;
using PlanningClient;
using System;
using System.Linq;
using System.Windows;

namespace Project
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IServiceProvider _serviceProvider;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            var win = _serviceProvider.GetRequiredService<ProjectWindow>();
            
            win.ShowDialog();
            
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
