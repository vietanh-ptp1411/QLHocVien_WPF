using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace QL_MVALab.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<NavItem> MenuItems { get; } = new();

        [ObservableProperty] private object? currentViewModel;
        [ObservableProperty] private NavItem? selectedItem;

        [ObservableProperty] private DateTime now = DateTime.Now;   // để hiển thị giờ/ngày

        public MainViewModel()
        {
            // Menu
            MenuItems.Add(new NavItem("Trang chủ", "\uE80F", () => new WelcomeViewModel()));
            MenuItems.Add(new NavItem("Học viên", "\uE77B", () => new HocVienViewModel()));
            MenuItems.Add(new NavItem("Lớp học", "\uE8FD", () => new LopHocViewModel()));
            MenuItems.Add(new NavItem("Khóa học", "\uE160", () => new KhoaHocViewModel()));
            MenuItems.Add(new NavItem("Đăng ký", "\uE8C8", () => new DangKyViewModel()));
            MenuItems.Add(new NavItem("Học phí", "\uE1D3", () => new HocPhiViewModel()));
            MenuItems.Add(new NavItem("Giảng viên", "\uED7D", () => new GiangVienViewModel()));

            CurrentViewModel = new WelcomeViewModel();
            SelectedItem = MenuItems[0];

            // Cập nhật đồng hồ mỗi giây
            var t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            t.Tick += (_, __) => Now = DateTime.Now;
            t.Start();
        }

        partial void OnSelectedItemChanged(NavItem? value)
        {
            if (value != null) CurrentViewModel = value.CreateViewModel();
        }
    }

    // Các VM còn lại (nếu chưa có, để tạm thế này)
    public partial class WelcomeViewModel : ObservableObject { }
    public partial class HocVienViewModel : ObservableObject { }
    public partial class LopHocViewModel : ObservableObject { }
    public partial class KhoaHocViewModel : ObservableObject { }
    public partial class DangKyViewModel : ObservableObject { }
    public partial class HocPhiViewModel : ObservableObject { }
    public partial class GiangVienViewModel : ObservableObject { }
}
