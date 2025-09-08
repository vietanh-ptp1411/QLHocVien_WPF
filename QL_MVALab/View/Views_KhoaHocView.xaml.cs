using QL_MVALab.ViewModel;
using System.Windows.Controls;

namespace QL_MVALab.View
{
    /// <summary>
    /// Interaction logic for Views_KhoaHocView.xaml
    /// </summary>
    public partial class Views_KhoaHocView : UserControl
    {
        public Views_KhoaHocView()
        {
            InitializeComponent();
            this.DataContext = new KhoaHocViewModels();
        }
    }
}
