using QL_MVALab.ViewModel;
using System.Windows.Controls;

namespace QL_MVALab.View
{
    /// <summary>
    /// Interaction logic for View_BuoiHocView.xaml
    /// </summary>
    public partial class Views_BuoiHocView : UserControl
    {
        public Views_BuoiHocView()
        {
            InitializeComponent();
            this.DataContext = new BuoiHocViewModels();
        }
    }
}
