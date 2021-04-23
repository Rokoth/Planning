using System;
using System.Windows.Controls;

namespace Project
{
    /// <summary>
    /// Логика взаимодействия для ProjectRow.xaml
    /// </summary>
    public partial class ProjectRow : UserControl
    {
        private PlanProject project;

        public ProjectRow(PlanProject project)
        {
            InitializeComponent();
            this.project = project;
            this.DataContext = project;
        }

        private void Edit_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }

    public class PlanProject
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTimeOffset DateBegin { get; set; }
        public DateTimeOffset DateEnd { get; set; }
    }
}
