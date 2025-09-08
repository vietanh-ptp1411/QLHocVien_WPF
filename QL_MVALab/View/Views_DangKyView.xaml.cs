using QL_MVALab.ViewModel;
using System.Windows.Controls;

namespace QL_MVALab.View
{
    /// <summary>
    /// Interaction logic for Views_DangKyView.xaml
    /// </summary>
    public partial class Views_DangKyView : UserControl
    {
        public Views_DangKyView()
        {
            InitializeComponent();
            this.DataContext = new DangKyViewModels();
        }
    }
}
