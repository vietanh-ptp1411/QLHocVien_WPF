//using System;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows.Input;
//using System.Windows;
//using System.Data;
//using QL_MVALab.Model;
//using QL_MVALab.ConnectDatabase;
//using System.Runtime.CompilerServices;

//namespace QL_MVALab.ViewModel
//{
//    public class HocVienViewModels : BaseViewModel
//    {
//        #region Properties
//        private ObservableCollection<HocVienModel> _hocVienList;
//        public ObservableCollection<HocVienModel> HocVienList
//        {
//            get { return _hocVienList; }
//            set { _hocVienList = value; OnPropertyChanged(); }
//        }

//        private ObservableCollection<HocVienModel> _filteredHocVienList;
//        public ObservableCollection<HocVienModel> FilteredHocVienList
//        {
//            get { return _filteredHocVienList; }
//            set { _filteredHocVienList = value; OnPropertyChanged(); }
//        }

//        private HocVienModel _selectedHocVien;
//        public HocVienModel SelectedHocVien
//        {
//            get { return _selectedHocVien; }
//            set
//            {
//                _selectedHocVien = value;
//                OnPropertyChanged();
//                if (value != null)
//                {
//                    MaHocVien = value.MaHocVien;
//                    TenHocVien = value.TenHocVien;
//                    NgaySinh = value.NgaySinh;
//                    GioiTinh = value.GioiTinh;
//                    DiaChi = value.DiaChi;
//                    SoDienThoai = value.SoDienThoai;
//                    Email = value.Email;
//                    NgayDangKy = value.NgayDangKy;
//                }
//            }
//        }

//        // Properties for form input
//        private string _maHocVien = "";
//        public string MaHocVien
//        {
//            get { return _maHocVien; }
//            set { _maHocVien = value; OnPropertyChanged(); }
//        }

//        private string _tenHocVien = "";
//        public string TenHocVien
//        {
//            get { return _tenHocVien; }
//            set { _tenHocVien = value; OnPropertyChanged(); }
//        }

//        private DateTime _ngaySinh = DateTime.Now;
//        public DateTime NgaySinh
//        {
//            get { return _ngaySinh; }
//            set { _ngaySinh = value; OnPropertyChanged(); }
//        }

//        private string _gioiTinh = "";
//        public string GioiTinh
//        {
//            get { return _gioiTinh; }
//            set { _gioiTinh = value; OnPropertyChanged(); }
//        }

//        private string _diaChi = "";
//        public string DiaChi
//        {
//            get { return _diaChi; }
//            set { _diaChi = value; OnPropertyChanged(); }
//        }

//        private string _soDienThoai = "";
//        public string SoDienThoai
//        {
//            get { return _soDienThoai; }
//            set { _soDienThoai = value; OnPropertyChanged(); }
//        }

//        private string _email = "";
//        public string Email
//        {
//            get { return _email; }
//            set { _email = value; OnPropertyChanged(); }
//        }

//        private DateTime _ngayDangKy = DateTime.Now;
//        public DateTime NgayDangKy
//        {
//            get { return _ngayDangKy; }
//            set { _ngayDangKy = value; OnPropertyChanged(); }
//        }

//        // Search property
//        private string _searchText = "";
//        public string SearchText
//        {
//            get { return _searchText; }
//            set
//            {
//                _searchText = value;
//                OnPropertyChanged();
//                SearchCommand.Execute(null);
//            }
//        }
//        #endregion

//        #region Commands
//        public ICommand AddCommand { get; set; }
//        public ICommand UpdateCommand { get; set; }
//        public ICommand DeleteCommand { get; set; }
//        public ICommand SearchCommand { get; set; }
//        public ICommand ClearCommand { get; set; }
//        public ICommand RefreshCommand { get; set; }
//        #endregion

//        #region Constructor
//        public HocVienViewModels()
//        {
//            HocVienList = new ObservableCollection<HocVienModel>();
//            FilteredHocVienList = new ObservableCollection<HocVienModel>();

//            InitializeCommands();
//            LoadData();
//        }
//        #endregion

//        #region Methods
//        private void InitializeCommands()
//        {
//            AddCommand = new RelayCommand<object>((p) => CanAdd(), (p) => Add());
//            UpdateCommand = new RelayCommand<object>((p) => CanUpdate(), (p) => Update());
//            DeleteCommand = new RelayCommand<object>((p) => CanDelete(), (p) => Delete());
//            SearchCommand = new RelayCommand<object>((p) => true, (p) => Search());
//            ClearCommand = new RelayCommand<object>((p) => true, (p) => Clear());
//            RefreshCommand = new RelayCommand<object>((p) => true, (p) => LoadData());
//        }

//        // Load data from database
//        private void LoadData()
//        {
//            try
//            {
//                string sql = "SELECT * FROM HocVien ORDER BY NgayDangKy DESC";
//                DataTable dt = Connect.DataTransport(sql);

//                HocVienList.Clear();

//                foreach (DataRow row in dt.Rows)
//                {
//                    var hocVien = new HocVienModel
//                    {
//                        MaHocVien = row["MaHocVien"]?.ToString() ?? "",
//                        TenHocVien = row["TenHocVien"]?.ToString() ?? "",
//                        NgaySinh = Convert.ToDateTime(row["NgaySinh"]),
//                        GioiTinh = row["GioiTinh"]?.ToString() ?? "",
//                        DiaChi = row["DiaChi"]?.ToString() ?? "",
//                        SoDienThoai = row["SoDienThoai"]?.ToString() ?? "",
//                        Email = row["Email"]?.ToString() ?? "",
//                        NgayDangKy = Convert.ToDateTime(row["NgayDangKy"])
//                    };

//                    HocVienList.Add(hocVien);
//                }

//                FilteredHocVienList = new ObservableCollection<HocVienModel>(HocVienList);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        #region Add Methods
//        private bool CanAdd()
//        {
//            return !string.IsNullOrWhiteSpace(TenHocVien) &&
//                   !string.IsNullOrWhiteSpace(MaHocVien) &&
//                   !IsHocVienExists(MaHocVien);
//        }

//        private void Add()
//        {
//            try
//            {
//                string sql = $@"INSERT INTO HocVien (MaHocVien, TenHocVien, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, NgayDangKy) 
//                               VALUES (N'{MaHocVien}', N'{TenHocVien}', '{NgaySinh:yyyy-MM-dd}', N'{GioiTinh}', 
//                                      N'{DiaChi}', '{SoDienThoai}', '{Email}', '{NgayDangKy:yyyy-MM-dd}')";

//                int result = Connect.DataExcution(sql);

//                if (result > 0)
//                {
//                    // Add to ObservableCollection
//                    var hocVienModel = new HocVienModel
//                    {
//                        MaHocVien = MaHocVien,
//                        TenHocVien = TenHocVien,
//                        NgaySinh = NgaySinh,
//                        GioiTinh = GioiTinh,
//                        DiaChi = DiaChi,
//                        SoDienThoai = SoDienThoai,
//                        Email = Email,
//                        NgayDangKy = NgayDangKy
//                    };

//                    HocVienList.Add(hocVienModel);
//                    FilteredHocVienList.Add(hocVienModel);

//                    MessageBox.Show("Thêm học viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
//                    Clear();
//                }
//                else
//                {
//                    MessageBox.Show("Không thể thêm học viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Lỗi khi thêm học viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//        #endregion

//        #region Update Methods
//        private bool CanUpdate()
//        {
//            return SelectedHocVien != null &&
//                   !string.IsNullOrWhiteSpace(TenHocVien) &&
//                   !string.IsNullOrWhiteSpace(MaHocVien);
//        }

//        private void Update()
//        {
//            try
//            {
//                string sql = $@"UPDATE HocVien SET 
//                               TenHocVien = N'{TenHocVien}',
//                               NgaySinh = '{NgaySinh:yyyy-MM-dd}',
//                               GioiTinh = N'{GioiTinh}',
//                               DiaChi = N'{DiaChi}',
//                               SoDienThoai = '{SoDienThoai}',
//                               Email = '{Email}',
//                               NgayDangKy = '{NgayDangKy:yyyy-MM-dd}'
//                               WHERE MaHocVien = '{SelectedHocVien.MaHocVien}'";

//                int result = Connect.DataExcution(sql);

//                if (result > 0)
//                {
//                    // Update ObservableCollection
//                    var existingHocVien = HocVienList.FirstOrDefault(h => h.MaHocVien == SelectedHocVien.MaHocVien);
//                    if (existingHocVien != null)
//                    {
//                        existingHocVien.TenHocVien = TenHocVien;
//                        existingHocVien.NgaySinh = NgaySinh;
//                        existingHocVien.GioiTinh = GioiTinh;
//                        existingHocVien.DiaChi = DiaChi;
//                        existingHocVien.SoDienThoai = SoDienThoai;
//                        existingHocVien.Email = Email;
//                        existingHocVien.NgayDangKy = NgayDangKy;
//                    }

//                    MessageBox.Show("Cập nhật học viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
//                    Clear();
//                }
//                else
//                {
//                    MessageBox.Show("Không thể cập nhật học viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Lỗi khi cập nhật học viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//        #endregion

//        #region Delete Methods
//        private bool CanDelete()
//        {
//            return SelectedHocVien != null;
//        }

//        private void Delete()
//        {
//            try
//            {
//                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa học viên {SelectedHocVien.TenHocVien}?",
//                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

//                if (result == MessageBoxResult.Yes)
//                {
//                    string sql = $"DELETE FROM HocVien WHERE MaHocVien = '{SelectedHocVien.MaHocVien}'";

//                    int deleteResult = Connect.DataExcution(sql);

//                    if (deleteResult > 0)
//                    {
//                        // Remove from ObservableCollection
//                        HocVienList.Remove(SelectedHocVien);
//                        FilteredHocVienList.Remove(SelectedHocVien);

//                        MessageBox.Show("Xóa học viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
//                        Clear();
//                    }
//                    else
//                    {
//                        MessageBox.Show("Không thể xóa học viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Lỗi khi xóa học viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//        #endregion

//        #region Search Methods
//        private void Search()
//        {
//            try
//            {
//                if (string.IsNullOrWhiteSpace(SearchText))
//                {
//                    FilteredHocVienList = new ObservableCollection<HocVienModel>(HocVienList);
//                }
//                else
//                {
//                    var searchResults = HocVienList.Where(h =>
//                        (h.MaHocVien?.ToLower().Contains(SearchText.ToLower()) == true) ||
//                        (h.TenHocVien?.ToLower().Contains(SearchText.ToLower()) == true) ||
//                        (h.SoDienThoai?.Contains(SearchText) == true) ||
//                        (h.Email?.ToLower().Contains(SearchText.ToLower()) == true) ||
//                        (h.DiaChi?.ToLower().Contains(SearchText.ToLower()) == true)
//                    ).ToList();

//                    FilteredHocVienList = new ObservableCollection<HocVienModel>(searchResults);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//        #endregion

//        #region Helper Methods
//        private void Clear()
//        {
//            MaHocVien = "";
//            TenHocVien = "";
//            NgaySinh = DateTime.Now;
//            GioiTinh = "";
//            DiaChi = "";
//            SoDienThoai = "";
//            Email = "";
//            NgayDangKy = DateTime.Now;
//            SelectedHocVien = null;
//        }

//        private bool IsHocVienExists(string maHocVien)
//        {
//            return HocVienList.Any(h => h.MaHocVien?.Equals(maHocVien, StringComparison.OrdinalIgnoreCase) == true);
//        }
//        #endregion
//    }

//    public class BaseViewModel : INotifyPropertyChanged
//    {
//        public event PropertyChangedEventHandler PropertyChanged;

//        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//    #endregion
//}
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