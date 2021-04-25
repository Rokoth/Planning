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
        public MainWindow()
        {
            InitializeComponent();


            foreach (var i in Enumerable.Range(1, 15))
            {
                var row = new ProjectRow(new PlanProject()
                {
                    DateBegin = DateTimeOffset.Now,
                    DateEnd = DateTimeOffset.Now,
                    Id = Guid.NewGuid(),
                    Name = "Row" + i,
                    ParentId = Guid.NewGuid(),
                    Path = $@"C:\Project{i}"
                });
                var top = i * 55;
                row.Margin = new Thickness(5, top, 0, 0);
                ContentInlineGrid.Children.Add(row);
                ContentInlineGrid.Height = (i + 1) * 55;
            }
        }
    }
}
