using QL_MVALab.ViewModel;
using System.Windows.Controls;

namespace QL_MVALab.View
{
    /// <summary>
    /// Interaction logic for Views_HocPhiView.xaml
    /// </summary>
    public partial class Views_HocPhiView : UserControl
    {
        public Views_HocPhiView()
        {
            InitializeComponent();
            this.DataContext = new HocPhiViewModels();
        }
    }
}
