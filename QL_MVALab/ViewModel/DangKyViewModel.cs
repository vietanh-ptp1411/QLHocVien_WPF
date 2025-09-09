using QL_MVALab.ConnectDatabase;
using QL_MVALab.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QL_MVALab.ViewModel
{
    public class DangKyViewModels : BaseViewModel
    {
        #region Properties - Collections
        // Danh sách đăng ký
        public ObservableCollection<DangKyModel> DangKyList { get; private set; } = new();

        private ObservableCollection<DangKyModel> _filtered = new();
        public ObservableCollection<DangKyModel> FilteredDangKyList
        {
            get => _filtered;
            set { _filtered = value; OnPropertyChanged(); }
        }

        // Danh sách cho ComboBox
        public ObservableCollection<HocVienModel> HocVienList { get; private set; } = new();
        public ObservableCollection<LopHocModel> LopHocList { get; private set; } = new();

        // Danh sách đã lọc cho ComboBox học viên
        private ObservableCollection<HocVienModel> _filteredHocVien = new();
        public ObservableCollection<HocVienModel> FilteredHocVienList
        {
            get => _filteredHocVien;
            set { _filteredHocVien = value; OnPropertyChanged(); }
        }
        #endregion

        #region Properties - Selected Item
        // Bản ghi đang chọn
        private DangKyModel? _selected;
        public DangKyModel? SelectedDangKy
        {
            get => _selected;
            set
            {
                _selected = value; OnPropertyChanged();
                if (value == null) return;
                Id = value.Id;

                // Set selected items in ComboBoxes
                SelectedHocVien = HocVienList.FirstOrDefault(k => k.Id == value.HocVienID);
                SelectedLopHoc = LopHocList.FirstOrDefault(g => g.Id == value.LopHocID);

                // Cập nhật text search cho học viên
                HocVienSearchText = SelectedHocVien?.HoTen ?? "";

                NgayDangKy = value.NgayDangKy;
                TrangThai = value.TrangThai;
            }
        }

        // Selected items for ComboBoxes
        private HocVienModel? _selectedHocVien;
        public HocVienModel? SelectedHocVien
        {
            get => _selectedHocVien;
            set
            {
                _selectedHocVien = value;
                OnPropertyChanged();
                // Cập nhật text khi chọn từ dropdown
                if (value != null && !_isUpdatingFromText)
                {
                    _isUpdatingFromSelection = true;
                    HocVienSearchText = value.HoTen;
                    _isUpdatingFromSelection = false;
                }
            }
        }

        private LopHocModel? _selectedLopHoc;
        public LopHocModel? SelectedLopHoc
        {
            get => _selectedLopHoc;
            set { _selectedLopHoc = value; OnPropertyChanged(); }
        }
        #endregion

        #region Properties - Form Fields
        private int? _id;
        public int? Id { get => _id; set { _id = value; OnPropertyChanged(); } }

        private DateTime _ngayDangKy = DateTime.Now;
        public DateTime NgayDangKy { get => _ngayDangKy; set { _ngayDangKy = value; OnPropertyChanged(); } }

        public bool TrangThai { get => _tt; set { _tt = value; OnPropertyChanged(); } }
        private bool _tt = true;

        // Tìm kiếm - FIXED: bỏ comment
        private string? _search;
        public string? SearchText { get => _search; set { _search = value; OnPropertyChanged(); Search(); } }

        // Text search cho học viên trong ComboBox
        private string _hocVienSearchText = "";
        private bool _isUpdatingFromText = false;
        private bool _isUpdatingFromSelection = false;

        public string HocVienSearchText
        {
            get => _hocVienSearchText;
            set
            {
                _hocVienSearchText = value;
                OnPropertyChanged();
                if (!_isUpdatingFromSelection)
                {
                    _isUpdatingFromText = true;
                    FilterHocVien();
                    // Tự động chọn nếu có match chính xác
                    var exactMatch = HocVienList.FirstOrDefault(h =>
                        string.Equals(h.HoTen, value, StringComparison.OrdinalIgnoreCase));
                    if (exactMatch != null)
                    {
                        SelectedHocVien = exactMatch;
                    }
                    else if (string.IsNullOrWhiteSpace(value))
                    {
                        SelectedHocVien = null;
                    }
                    _isUpdatingFromText = false;
                }
            }
        }
        #endregion

        #region Commands
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand RefreshCommand { get; }
        #endregion

        #region Constructor
        public DangKyViewModels()
        {
            AddCommand = new RelayCommand<object>(_ => CanAdd(), _ => Add());
            UpdateCommand = new RelayCommand<object>(_ => CanUpdate(), _ => Update());
            DeleteCommand = new RelayCommand<object>(_ => CanDelete(), _ => Delete());
            SearchCommand = new RelayCommand<object>(_ => true, _ => Search());
            ClearCommand = new RelayCommand<object>(_ => true, _ => Clear());
            RefreshCommand = new RelayCommand<object>(_ => true, _ => LoadData());
            LoadData();
        }
        #endregion

        #region Filter Methods
        private void FilterHocVien()
        {
            if (string.IsNullOrWhiteSpace(HocVienSearchText))
            {
                FilteredHocVienList = new ObservableCollection<HocVienModel>(HocVienList);
                return;
            }

            string searchText = HocVienSearchText.ToLower().Trim();
            var filtered = HocVienList.Where(h =>
                h.HoTen.ToLower().Contains(searchText)).ToList();

            FilteredHocVienList = new ObservableCollection<HocVienModel>(filtered);
        }
        #endregion

        #region CRUD Methods
        private void LoadData()
        {
            try
            {
                // Load Học viên trước
                LoadHocVien();

                // Load Lớp học  
                LoadLopHoc();

                // Kiểm tra xem bảng có tồn tại không trước
                string checkTablesSQL = @"
                    SELECT COUNT(*) as TableCount 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME IN ('LopHoc', 'HocVien', 'DangKy')";

                var tableCheck = Connect.DataTransport(checkTablesSQL);
                if (tableCheck.Rows[0]["TableCount"].ToString() != "3")
                {
                    MessageBox.Show("Một hoặc nhiều bảng ('LopHoc', 'HocVien', 'DangKy') không tồn tại trong database!",
                        "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Load Đăng ký
                string simpleSQL = "SELECT Id, HocVienID, LopHocID, NgayDangKy, TrangThai FROM dbo.DangKy ORDER BY Id ASC";
                var dt = Connect.DataTransport(simpleSQL);

                DangKyList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    var dangky = new DangKyModel
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        HocVienID = Convert.ToInt32(r["HocVienID"]),
                        LopHocID = Convert.ToInt32(r["LopHocID"]),
                        NgayDangKy = Convert.ToDateTime(r["NgayDangKy"]),
                        TrangThai = Convert.ToBoolean(r["TrangThai"])
                    };

                    // Tìm tên học viên và tên lớp từ collections đã load
                    var hocVien = HocVienList.FirstOrDefault(k => k.Id == dangky.HocVienID);
                    var lopHoc = LopHocList.FirstOrDefault(g => g.Id == dangky.LopHocID);

                    // FIXED: Gán tên vào model
                    dangky.HoTen = hocVien?.HoTen ?? "Không tìm thấy";
                    dangky.TenLop = lopHoc?.TenLop ?? "Không tìm thấy";

                    DangKyList.Add(dangky);
                }

                FilteredDangKyList = new ObservableCollection<DangKyModel>(DangKyList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu đăng ký: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadHocVien()
        {
            try
            {
                string sql = "SELECT Id, HoTen FROM dbo.HocVien ORDER BY HoTen";
                var dt = Connect.DataTransport(sql);

                HocVienList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    HocVienList.Add(new HocVienModel
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        HoTen = r["HoTen"]?.ToString() ?? ""
                    });
                }

                // Khởi tạo danh sách đã lọc
                FilteredHocVienList = new ObservableCollection<HocVienModel>(HocVienList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách học viên: {ex.Message}\n\nVui lòng kiểm tra bảng HocVien có tồn tại với cột Id, HoTen",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLopHoc()
        {
            try
            {
                string sql = "SELECT Id, TenLop FROM dbo.LopHoc ORDER BY TenLop";
                var dt = Connect.DataTransport(sql);

                LopHocList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    LopHocList.Add(new LopHocModel
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        TenLop = r["TenLop"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách lớp học: {ex.Message}\n\nVui lòng kiểm tra bảng LopHoc có tồn tại với cột Id, TenLop",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAdd() =>
            SelectedHocVien != null
            && SelectedLopHoc != null
            && !(DangKyList?.Any(l => l.LopHocID == SelectedLopHoc.Id && l.HocVienID == SelectedHocVien.Id) ?? false);

        private void Add()
        {
            try
            {
                if (SelectedHocVien == null || SelectedLopHoc == null) return;

                string sql = $@"
                            INSERT INTO dbo.DangKy (HocVienID, LopHocID, NgayDangKy, TrangThai)
                            VALUES ({SelectedHocVien.Id}, {SelectedLopHoc.Id},
                                    '{NgayDangKy:yyyy-MM-dd}', {(TrangThai ? 1 : 0)})";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã đăng ký thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể đăng ký.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // FIXED: Sửa lại method CanUpdate() và Update()
        private bool CanUpdate()
            => SelectedDangKy != null && Id.HasValue
            && SelectedHocVien != null
            && SelectedLopHoc != null;

        private void Update()
        {
            if (!Id.HasValue || SelectedHocVien == null || SelectedLopHoc == null) return;
            try
            {
                string sql = $@"
                                UPDATE dbo.DangKy SET
                                HocVienID = {SelectedHocVien.Id},
                                LopHocID = {SelectedLopHoc.Id},
                                NgayDangKy = '{NgayDangKy:yyyy-MM-dd}',
                                TrangThai = {(TrangThai ? 1 : 0)}
                                WHERE Id = {Id.Value};";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    MessageBox.Show("Đã cập nhật đăng ký.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật đăng ký.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanDelete() => SelectedDangKy != null && Id.HasValue;

        // FIXED: Sửa lại method Delete()
        private void Delete()
        {
            if (!Id.HasValue || SelectedDangKy == null) return;

            var hocVienTen = HocVienList.FirstOrDefault(h => h.Id == SelectedDangKy.HocVienID)?.HoTen ?? "Không xác định";
            var lopHocTen = LopHocList.FirstOrDefault(l => l.Id == SelectedDangKy.LopHocID)?.TenLop ?? "Không xác định";

            if (MessageBox.Show($"Xóa đăng ký của học viên '{hocVienTen}' vào lớp '{lopHocTen}'?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                int n = Connect.DataExcution($"DELETE FROM dbo.DangKy WHERE Id = {Id.Value};");
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã xóa đăng ký.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể xóa đăng ký.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Search & Helper Methods
        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredDangKyList = new ObservableCollection<DangKyModel>(DangKyList);
                return;
            }

            string kw = SearchText.Trim().ToLower();
            var rs = DangKyList.Where(d =>
                    (d.HoTen?.ToLower().Contains(kw) ?? false) ||
                    (d.TenLop?.ToLower().Contains(kw) ?? false) ||
                    d.Id.ToString().Contains(kw) ||
                    d.NgayDangKy.ToString("dd/MM/yyyy").Contains(kw))
                .ToList();
            FilteredDangKyList = new ObservableCollection<DangKyModel>(rs);
        }

        private void Clear()
        {
            Id = null;
            SelectedHocVien = null;
            SelectedLopHoc = null;
            HocVienSearchText = "";
            NgayDangKy = DateTime.Now;
            TrangThai = true;
            SelectedDangKy = null;
            FilteredHocVienList = new ObservableCollection<HocVienModel>(HocVienList);
        }

        private static string Esc(string? s) => (s ?? string.Empty).Replace("'", "''");
        #endregion
    }
}