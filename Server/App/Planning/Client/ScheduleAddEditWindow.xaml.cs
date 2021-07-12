using Project;
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
    /// Логика взаимодействия для ScheduleAddEditWindow.xaml
    /// </summary>
    public partial class ScheduleAddEditWindow : Window
    {
        public ScheduleAddEditWindow()
        {
            InitializeComponent();
        }

        internal void ShowDialog(AddEditMode edit, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
