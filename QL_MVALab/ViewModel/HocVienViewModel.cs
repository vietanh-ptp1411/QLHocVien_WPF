using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using QL_MVALab.ConnectDatabase;
using QL_MVALab.Model;

namespace QL_MVALab.ViewModel
{
    public class HocVienViewModels : BaseViewModel
    {
        // Danh sách / lọc
        public ObservableCollection<HocVienModel> HocVienList { get; private set; } = new();
        private ObservableCollection<HocVienModel> _filtered = new();
        public ObservableCollection<HocVienModel> FilteredHocVienList
        {
            get => _filtered; set { _filtered = value; OnPropertyChanged(); }
        }

        // Bản ghi đang chọn + các ô nhập
        private HocVienModel? _selected;
        public HocVienModel? SelectedHocVien
        {
            get => _selected;
            set
            {
                _selected = value; OnPropertyChanged();
                if (value == null) return;
                Id = value.Id;
                MaHocVien = value.MaHocVien;
                HoTen = value.HoTen;
                NamSinh = value.NamSinh;
                Email = value.Email;
                DienThoai = value.DienThoai;
                DiaChi = value.DiaChi;
                NgayTao = value.NgayTao;
            }
        }

        public int? Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        private int? _id;

        public string MaHocVien { get => _ma; set { _ma = value; OnPropertyChanged(); } }
        private string _ma = "";

        public string HoTen { get => _ten; set { _ten = value; OnPropertyChanged(); } }
        private string _ten = "";

        public string? NamSinh { get => _nam; set { _nam = value; OnPropertyChanged(); } }
        private string? _nam;

        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        private string _email = "";

        public string? DienThoai { get => _dt; set { _dt = value; OnPropertyChanged(); } }
        private string? _dt;

        public string? DiaChi { get => _dc; set { _dc = value; OnPropertyChanged(); } }
        private string? _dc;

        public DateTime? NgayTao { get => _nt; set { _nt = value; OnPropertyChanged(); } }
        private DateTime? _nt;

        // Tìm kiếm
        private string? _search;
        public string? SearchText { get => _search; set { _search = value; OnPropertyChanged(); Search(); } }

        // Command
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand RefreshCommand { get; }

        public HocVienViewModels()
        {
            AddCommand = new RelayCommand<object>(_ => CanAdd(), _ => Add());
            UpdateCommand = new RelayCommand<object>(_ => CanUpdate(), _ => Update());
            DeleteCommand = new RelayCommand<object>(_ => CanDelete(), _ => Delete());
            SearchCommand = new RelayCommand<object>(_ => true, _ => Search());
            ClearCommand = new RelayCommand<object>(_ => true, _ => Clear());
            RefreshCommand = new RelayCommand<object>(_ => true, _ => LoadData());
            LoadData();
        }

        // ===== CRUD =====
        private void LoadData()
        {
            try
            {
                var dt = Connect.DataTransport(
                    "SELECT Id, MaHocVien, HoTen, NamSinh, Email, DienThoai, DiaChi, NgayTao " +
                    "FROM dbo.HocVien ORDER BY Id ASC");

                HocVienList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    HocVienList.Add(new HocVienModel
                    {
                        Id = r.Field<int>("Id"),
                        MaHocVien = r["MaHocVien"]?.ToString() ?? "",
                        HoTen = r["HoTen"]?.ToString() ?? "",
                        NamSinh = r["NamSinh"]?.ToString(),
                        Email = r["Email"]?.ToString() ?? "",
                        DienThoai = r["DienThoai"]?.ToString(),
                        DiaChi = r["DiaChi"]?.ToString(),
                        NgayTao = r.Field<DateTime>("NgayTao")
                    });
                }
                FilteredHocVienList = new ObservableCollection<HocVienModel>(HocVienList);
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}"); }
        }

        private bool CanAdd()
            => !string.IsNullOrWhiteSpace(MaHocVien)
            && !string.IsNullOrWhiteSpace(HoTen)
            && !string.IsNullOrWhiteSpace(Email)
            && !HocVienList.Any(x => x.MaHocVien.Equals(MaHocVien, StringComparison.OrdinalIgnoreCase));

        private void Add()
        {
            try
            {
                // Id & NgayTao DB tự sinh; dùng NULLIF để đẩy NULL khi người dùng để trống
                string sql = $@"
                        INSERT INTO dbo.HocVien (MaHocVien, HoTen, NamSinh, Email, DienThoai, DiaChi)
                        VALUES (N'{Esc(MaHocVien)}', N'{Esc(HoTen)}',
                        NULLIF(N'{Esc(NamSinh)}',''), N'{Esc(Email)}',
                        NULLIF(N'{Esc(DienThoai)}',''), NULLIF(N'{Esc(DiaChi)}',''));";
                                int n = Connect.DataExcution(sql);
                                if (n > 0) { LoadData(); Clear(); MessageBox.Show("Đã thêm học viên."); }
                                else MessageBox.Show("Không thể thêm học viên.");
                            }
                            catch (Exception ex) { MessageBox.Show($"Lỗi thêm: {ex.Message}"); }
        }

        private bool CanUpdate()
            => SelectedHocVien != null && Id.HasValue
            && !string.IsNullOrWhiteSpace(MaHocVien)
            && !string.IsNullOrWhiteSpace(HoTen)
            && !string.IsNullOrWhiteSpace(Email);

        private void Update()
        {
            if (!Id.HasValue) return;
            try
            {
                string sql = $@"
                                UPDATE dbo.HocVien SET
                                MaHocVien = N'{Esc(MaHocVien)}',
                                HoTen     = N'{Esc(HoTen)}',
                                NamSinh   = NULLIF(N'{Esc(NamSinh)}',''),
                                Email     = N'{Esc(Email)}',
                                DienThoai = NULLIF(N'{Esc(DienThoai)}',''),
                                DiaChi    = NULLIF(N'{Esc(DiaChi)}','')
                                WHERE Id = {Id.Value};";
                                            int n = Connect.DataExcution(sql);
                                            if (n > 0) { LoadData(); MessageBox.Show("Đã cập nhật."); }
                                            else MessageBox.Show("Không thể cập nhật.");
                                        }
                                        catch (Exception ex) { MessageBox.Show($"Lỗi cập nhật: {ex.Message}"); }
        }

        private bool CanDelete() => SelectedHocVien != null && Id.HasValue;

        private void Delete()
        {
            if (!Id.HasValue) return;
            if (MessageBox.Show("Xóa học viên đã chọn?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                int n = Connect.DataExcution($"DELETE FROM dbo.HocVien WHERE Id = {Id.Value};");
                if (n > 0) { LoadData(); Clear(); MessageBox.Show("Đã xóa."); }
                else MessageBox.Show("Không thể xóa.");
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi xóa: {ex.Message}"); }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredHocVienList = new ObservableCollection<HocVienModel>(HocVienList);
                return;
            }
            string kw = SearchText.Trim().ToLower();
            var rs = HocVienList.Where(h =>
                    (h.MaHocVien?.ToLower().Contains(kw) ?? false) ||
                    (h.HoTen?.ToLower().Contains(kw) ?? false) ||
                    (h.NamSinh?.ToLower().Contains(kw) ?? false) ||
                    (h.Email?.ToLower().Contains(kw) ?? false) ||
                    (h.DienThoai?.ToLower().Contains(kw) ?? false) ||
                    (h.DiaChi?.ToLower().Contains(kw) ?? false))
                .ToList();
            FilteredHocVienList = new ObservableCollection<HocVienModel>(rs);
        }

        private void Clear()
        {
            Id = null;
            MaHocVien = HoTen = Email = "";
            NamSinh = DienThoai = DiaChi = null;
            NgayTao = null;
            SelectedHocVien = null;
        }

        private static string Esc(string? s) => (s ?? string.Empty).Replace("'", "''");
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}