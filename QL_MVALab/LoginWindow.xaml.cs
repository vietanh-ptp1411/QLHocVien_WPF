using System;
using System.Collections.Generic;
using System.Data;
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

namespace QL_MVALab
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // đóng form
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            //trước khi khiểm tra thì phải xem người dùng nhập đủ tk mk chưa
            if (!AllowLogin())//nếu hàm này là falsse thì thoát luôn không thực hiện các chức năng bên dưới
            {
                return;
            }

            //bắt đầu lấy dữ liệu trong bảng user để ss dữ liệu người dùng và hiện thông báo

            DataTable dtData = ConnectDatabase.Connect.DataTransport("Select * From Users");
            for (int i = 0; i < dtData.Rows.Count; i++) 
            {
                if (txtUser.Text.ToLower().Trim() == Convert.ToString(dtData.Rows[i]["TaiKhoan"]).ToLower().Trim()) 
                { 
                    //Nếu kiểm tra trùng khớp thì tiếp tục kiểm tra mật khẩu
                    if(txtPass.Password == Convert.ToString(dtData.Rows[i]["MatKhau"]).ToLower().Trim())
                    {
                        MessageBox.Show("Đăng nhập thành công.", "Chúc mừng", MessageBoxButton.OK, MessageBoxImage.Information);
                        var main = new MainWindow { WindowState = WindowState.Maximized  };
                        main.Show();
                        Close();
                    }
                    else
                    {
                        lblError.Text = "Mật khẩu không chính xác.";
                        txtPass.Focus();
                        return;
                    }
                }
                else
                {
                    lblError.Text = "Tài khoản bạn nhập không chính xác";
                    txtUser.Focus();
                    return;
                }
            }
            

        }
        private bool AllowLogin()
        {
            if(txtUser.Text.Trim() == "")
            {
                lblError.Text = "Vui lòng nhập tài khoản.";
                txtUser.Focus();
                return false;
            }
            if (txtPass.Password.Trim() == "")
            {
                lblError.Text = "Vui lòng nhập mật khẩu.";
                txtPass.Focus();
                return false;
            }
            return true;
        }
    }
}
