using QL_MVALab.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QL_MVALab.View
{
    /// <summary>
    /// Interaction logic for Views_GiangVienView.xaml
    /// </summary>
    public partial class Views_GiangVienView : UserControl
    {
        public Views_GiangVienView()
        {
            InitializeComponent();
            this.DataContext = new GiangVienViewModels();
        }
        private void CopyInfo_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as GiangVienViewModels;
            if (viewModel?.SelectedGiangVien != null)
            {
                var gv = viewModel.SelectedGiangVien;
                var info =
                    $"Họ tên: {gv.HoTen}\n" +
                    $"Email: {gv.Email}\n" +
                    $"SĐT: {gv.DienThoai}\n" +
                    $"Chuyên môn: {gv.ChuyenMon}\n" +
                    $"Trạng thái: {gv.TrangThai}";


                Clipboard.SetText(info);
                MessageBox.Show("Đã sao chép thông tin học viên vào clipboard!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
