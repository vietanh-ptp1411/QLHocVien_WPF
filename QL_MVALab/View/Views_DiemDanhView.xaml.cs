using QL_MVALab.ViewModel;
using System.Windows.Controls;

namespace QL_MVALab.View
{
    /// <summary>
    /// Interaction logic for View_DiemDanhView.xaml
    /// </summary>
    public partial class Views_DiemDanhView : UserControl
    {
        public Views_DiemDanhView()
        {
            InitializeComponent();
            this.DataContext = new DiemDanhViewModels();
        }
    }
}
