using QL_MVALab.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace QL_MVALab.View
{
    /// <summary>
    /// Interaction logic for Views_HocPhiView.xaml
    /// </summary>
    public partial class Views_HocVienView : UserControl
    {
        public Views_HocVienView()
        {
            InitializeComponent();
            this.DataContext = new HocVienViewModels();

        }
        private void CopyInfo_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as HocVienViewModels;
            if (viewModel?.SelectedHocVien != null)
            {
                var hv = viewModel.SelectedHocVien;
                var info =
                    $"Họ tên: {hv.HoTen}\n" +
                    $"Năm sinh: {hv.NamSinh}\n" +
                    $"SĐT: {hv.DienThoai}\n" +
                    $"Email: {hv.Email}\n" +
                    $"Địa chỉ: {hv.DiaChi}\n" +
                    $"Ngày tạo: {hv.NgayTao:yyyy-MM-dd}";


                Clipboard.SetText(info);
                MessageBox.Show("Đã sao chép thông tin học viên vào clipboard!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}