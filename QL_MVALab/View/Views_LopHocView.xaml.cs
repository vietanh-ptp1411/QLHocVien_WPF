using System.Windows.Controls;
using QL_MVALab.ViewModel;

namespace QL_MVALab.View
{
    /// <summary>
    /// Interaction logic for Views_LopHocView.xaml
    /// </summary>
    public partial class Views_LopHocView : UserControl
    {
        public Views_LopHocView()
        {
            InitializeComponent();
            this.DataContext = new LopHocViewModels();
        }
    }
}