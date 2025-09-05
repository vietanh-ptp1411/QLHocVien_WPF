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
    public class LopHocViewModels : BaseViewModel
    {
        #region Properties - Collections
        // Danh sách lớp học
        public ObservableCollection<LopHocModel > LopHocList { get; private set; } = new();

        private ObservableCollection<LopHocModel> _filtered = new();
        public ObservableCollection<LopHocModel> FilteredLopHocList
        {
            get => _filtered;
            set { _filtered = value; OnPropertyChanged(); }
        }

        // Danh sách cho ComboBox
        public ObservableCollection<KhoaHocModel> KhoaHocList { get; private set; } = new();
        public ObservableCollection<GiangVienModel> GiangVienList { get; private set; } = new();
        #endregion

        #region Properties - Selected Item
        // Bản ghi đang chọn
        private LopHocModel ? _selected;
        public LopHocModel? SelectedLopHoc
        {
            get => _selected;
            set
            {
                _selected = value; OnPropertyChanged();
                if (value == null) return;
                Id = value.Id;
                TenLop = value.TenLop;

                // Set selected items in ComboBoxes
                SelectedKhoaHoc = KhoaHocList.FirstOrDefault(k => k.Id == value.KhoaHocId);
                SelectedGiangVien = GiangVienList.FirstOrDefault(g => g.Id == value.GiangVienId);

                NgayBatDau = value.NgayBatDau;
                NgayKetThuc = value.NgayKetThuc;
                SiSo = value.SiSo;
                LichHoc = value.LichHoc;
            }
        }

        // Selected items for ComboBoxes
        private KhoaHocModel? _selectedKhoaHoc;
        public KhoaHocModel? SelectedKhoaHoc
        {
            get => _selectedKhoaHoc;
            set { _selectedKhoaHoc = value; OnPropertyChanged(); }
        }

        private GiangVienModel? _selectedGiangVien;
        public GiangVienModel? SelectedGiangVien
        {
            get => _selectedGiangVien;
            set { _selectedGiangVien = value; OnPropertyChanged(); }
        }
        #endregion

        #region Properties - Form Fields
        private int? _id;
        public int? Id { get => _id; set { _id = value; OnPropertyChanged(); } }

        private string _tenLop = "";
        public string TenLop { get => _tenLop; set { _tenLop = value; OnPropertyChanged(); } }

        private DateTime _ngayBatDau = DateTime.Now;
        public DateTime NgayBatDau { get => _ngayBatDau; set { _ngayBatDau = value; OnPropertyChanged(); } }

        private DateTime _ngayKetThuc = DateTime.Now.AddMonths(3);
        public DateTime NgayKetThuc { get => _ngayKetThuc; set { _ngayKetThuc = value; OnPropertyChanged(); } }

        private int _siSo = 1;
        public int SiSo { get => _siSo; set { _siSo = value; OnPropertyChanged(); } }

        private string _LichHoc = "";
        public string LichHoc { get => _LichHoc; set { _LichHoc = value; OnPropertyChanged(); } }

        // Tìm kiếm
        private string? _search;
        public string? SearchText { get => _search; set { _search = value; OnPropertyChanged(); Search(); } }
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
        public LopHocViewModels()
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

        #region CRUD Methods
        private void LoadData()
        {
            try
            {
                // Load Khóa học trước
                LoadKhoaHoc();

                // Load Giảng viên  
                LoadGiangVien();

                // Kiểm tra xem bảng có tồn tại không trước
                string checkTablesSQL = @"
                    SELECT COUNT(*) as TableCount 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME IN ('LopHoc', 'KhoaHoc', 'GiangVien')";

                var tableCheck = Connect.DataTransport(checkTablesSQL);
                if (tableCheck.Rows[0]["TableCount"].ToString() != "3")
                {
                    MessageBox.Show("Một hoặc nhiều bảng (LopHoc, KhoaHoc, GiangVien) không tồn tại trong database!",
                        "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Load Lớp học - thử không dùng JOIN trước
                string simpleSQL = "SELECT Id, TenLop, KhoaHocID, GiangVienID, NgayBatDau, NgayKetThuc, SiSo, LichHoc FROM dbo.LopHoc ORDER BY Id ASC";
                var dt = Connect.DataTransport(simpleSQL);

                LopHocList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    var lopHoc = new LopHocModel
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        TenLop = r["TenLop"]?.ToString() ?? "",
                        KhoaHocId = Convert.ToInt32(r["KhoaHocID"]),
                        GiangVienId = Convert.ToInt32(r["GiangVienID"]),
                        NgayBatDau = Convert.ToDateTime(r["NgayBatDau"]),
                        NgayKetThuc = Convert.ToDateTime(r["NgayKetThuc"]),
                        SiSo = Convert.ToInt32(r["SiSo"]),
                        LichHoc = r["LichHoc"]?.ToString() ?? ""
                    };

                    // Tìm tên khóa học và giảng viên từ collections đã load
                    var khoaHoc = KhoaHocList.FirstOrDefault(k => k.Id == lopHoc.KhoaHocId);
                    var giangVien = GiangVienList.FirstOrDefault(g => g.Id == lopHoc.GiangVienId);

                    lopHoc.TenKhoaHoc = khoaHoc?.TenKhoa ?? "Không tìm thấy";
                    lopHoc.TenGiangVien = giangVien?.HoTen ?? "Không tìm thấy";

                    LopHocList.Add(lopHoc);
                }

                FilteredLopHocList = new ObservableCollection<LopHocModel>(LopHocList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu lớp học: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadKhoaHoc()
        {
            try
            {
                // Thử câu SQL đơn giản trước
                string sql = "SELECT Id, TenKhoa FROM dbo.KhoaHoc ORDER BY TenKhoa";
                var dt = Connect.DataTransport(sql);

                KhoaHocList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    KhoaHocList.Add(new KhoaHocModel
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        TenKhoa = r["TenKhoa"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách khóa học: {ex.Message}\n\nVui lòng kiểm tra bảng KhoaHoc có tồn tại với cột Id, TenKhoa",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGiangVien()
        {
            try
            {
                // Thử câu SQL đơn giản trước
                string sql = "SELECT Id, HoTen FROM dbo.GiangVien ORDER BY HoTen";
                var dt = Connect.DataTransport(sql);

                GiangVienList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    GiangVienList.Add(new GiangVienModel
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        HoTen = r["HoTen"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách giảng viên: {ex.Message}\n\nVui lòng kiểm tra bảng GiangVien có tồn tại với cột Id, HoTen",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                // Thêm dữ liệu mẫu để test
                GiangVienList.Add(new GiangVienModel { Id = 1, HoTen = "Giảng viên mẫu" });
            }
        }

        private bool CanAdd() =>
            !string.IsNullOrWhiteSpace(TenLop)
            && SelectedKhoaHoc != null
            && SelectedGiangVien != null
            && NgayKetThuc >= NgayBatDau
            && SiSo > 0
            && !(LopHocList?.Any(l =>
                   string.Equals(l.TenLop ?? "", TenLop, StringComparison.OrdinalIgnoreCase)
                   && l.KhoaHocId == SelectedKhoaHoc.Id) ?? false);

        private void Add()
        {
            try
            {
                if (SelectedKhoaHoc == null || SelectedGiangVien == null) return;

                string sql = $@"
                            INSERT INTO dbo.LopHoc (TenLop, KhoaHocID, GiangVienID, NgayBatDau, NgayKetThuc, SiSo, LichHoc)
                            VALUES (N'{Esc(TenLop)}', {SelectedKhoaHoc.Id}, {SelectedGiangVien.Id},
                                    '{NgayBatDau:yyyy-MM-dd}', '{NgayKetThuc:yyyy-MM-dd}',
                                    {SiSo},N'{Esc(LichHoc)}')";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã thêm lớp học.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể thêm lớp học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanUpdate()
            => SelectedLopHoc != null && Id.HasValue
            && !string.IsNullOrWhiteSpace(TenLop)
            && SelectedKhoaHoc != null
            && SelectedGiangVien != null
            && NgayKetThuc >= NgayBatDau
            && SiSo > 0;

        private void Update()
        {
            if (!Id.HasValue || SelectedKhoaHoc == null || SelectedGiangVien == null) return;
            try
            {
                string sql = $@"
                                UPDATE dbo.LopHoc SET
                                TenLop = N'{Esc(TenLop)}',
                                KhoaHocID = {SelectedKhoaHoc.Id},
                                GiangVienID = {SelectedGiangVien.Id},
                                NgayBatDau = '{NgayBatDau:yyyy-MM-dd}',
                                NgayKetThuc = '{NgayKetThuc:yyyy-MM-dd}',
                                SiSo = {SiSo},
                                LichHoc =  N'{Esc(LichHoc)}'
                                WHERE Id = {Id.Value};";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    MessageBox.Show("Đã cập nhật lớp học.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật lớp học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanDelete() => SelectedLopHoc != null && Id.HasValue;

        private void Delete()
        {
            if (!Id.HasValue) return;
            if (MessageBox.Show($"Xóa lớp học '{TenLop}' đã chọn?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                int n = Connect.DataExcution($"DELETE FROM dbo.LopHoc WHERE Id = {Id.Value};");
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã xóa lớp học.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể xóa lớp học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                FilteredLopHocList = new ObservableCollection<LopHocModel>(LopHocList);
                return;
            }

            string kw = SearchText.Trim().ToLower();
            var rs = LopHocList.Where(h =>
                    (h.TenLop?.ToLower().Contains(kw) ?? false) ||
                    (h.TenKhoaHoc?.ToLower().Contains(kw) ?? false) ||
                    (h.TenGiangVien?.ToLower().Contains(kw) ?? false) ||
                    (h.LichHoc?.ToLower().Contains(kw) ?? false)||
                    h.Id.ToString().Contains(kw) ||
                    h.SiSo.ToString().Contains(kw))          
                .ToList();
            FilteredLopHocList = new ObservableCollection<LopHocModel>(rs);
        }

        private void Clear()
        {
            Id = null;
            TenLop = "";
            SelectedKhoaHoc = null;
            SelectedGiangVien = null;
            NgayBatDau = DateTime.Now;
            NgayKetThuc = DateTime.Now.AddMonths(3);
            SiSo = 1;
            LichHoc = "";
            SelectedLopHoc = null;
        }

        private static string Esc(string? s) => (s ?? string.Empty).Replace("'", "''");
        #endregion
    }
}