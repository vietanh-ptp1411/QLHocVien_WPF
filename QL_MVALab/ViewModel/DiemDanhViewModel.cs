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
    public class DiemDanhViewModels : BaseViewModel
    {
        // Danh sách / lọc
        public ObservableCollection<DiemDanhModel> DiemDanhList { get; private set; } = new();
        private ObservableCollection<DiemDanhModel> _filtered = new();
        public ObservableCollection<DiemDanhModel> FilteredDiemDanhList
        {
            get => _filtered;
            set { _filtered = value; OnPropertyChanged(); }
        }

        // Bản ghi đang chọn + các ô nhập
        private DiemDanhModel? _selected;
        public DiemDanhModel? SelectedDiemDanh
        {
            get => _selected;
            set
            {
                _selected = value; OnPropertyChanged();
                if (value == null) return;
                Id = value.Id;
                BuoiHocId = value.BuoiHocId;
                HocVienId = value.HocVienId;
                CoMat = value.CoMat;
            }
        }

        public int? Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        private int? _id;

        public int? BuoiHocId { get => _buoiHocId; set { _buoiHocId = value; OnPropertyChanged(); LoadThongTinFromBuoiHocId(); } }
        private int? _buoiHocId;

        public int? HocVienId { get => _hocVienId; set { _hocVienId = value; OnPropertyChanged(); LoadThongTinFromHocVienId(); } }
        private int? _hocVienId;

        public bool CoMat { get => _coMat; set { _coMat = value; OnPropertyChanged(); } }
        private bool _coMat = true;

        // Thông tin hiển thị (chỉ đọc)
        public string? HoTen { get => _hoTen; set { _hoTen = value; OnPropertyChanged(); } }
        private string? _hoTen = "";

        public string? TenLop { get => _tenLop; set { _tenLop = value; OnPropertyChanged(); } }
        private string? _tenLop = "";

        public DateTime? NgayHoc { get => _ngayHoc; set { _ngayHoc = value; OnPropertyChanged(); } }
        private DateTime? _ngayHoc;

        public string? BuoiThu { get => _buoiThu; set { _buoiThu = value; OnPropertyChanged(); } }
        private string? _buoiThu;

        public DateTime? ThoiGianBatDau { get => _thoiGianBatDau; set { _thoiGianBatDau = value; OnPropertyChanged(); } }
        private DateTime? _thoiGianBatDau;

        public DateTime? ThoiGianKetThuc { get => _thoiGianKetThuc; set { _thoiGianKetThuc = value; OnPropertyChanged(); } }
        private DateTime? _thoiGianKetThuc;

        public string? ChuDe { get => _chuDe; set { _chuDe = value; OnPropertyChanged(); } }
        private string? _chuDe;

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

        public DiemDanhViewModels()
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
                // Join với bảng DiemDanh, BuoiHoc, HocVien, LopHoc để lấy thông tin hiển thị
                var dt = Connect.DataTransport(@"
                    SELECT dd.Id, dd.BuoiHocId, dd.HocVienId, dd.CoMat,
                           bh.BuoiThu, bh.ThoiGianBatDau, bh.ThoiGianKetThuc, bh.ChuDe,
                           hv.HoTen, lh.TenLop
                    FROM dbo.DiemDanh dd
                    LEFT JOIN dbo.BuoiHoc bh ON dd.BuoiHocId = bh.Id
                    LEFT JOIN dbo.HocVien hv ON dd.HocVienId = hv.Id
                    LEFT JOIN dbo.LopHoc lh ON bh.LopHocId = lh.Id
                    ORDER BY bh.ThoiGianBatDau DESC, hv.HoTen ASC");

                DiemDanhList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    DiemDanhList.Add(new DiemDanhModel
                    {
                        Id = r.Field<int>("Id"),
                        BuoiHocId = r.Field<int>("BuoiHocId"),
                        HocVienId = r.Field<int>("HocVienId"),
                        CoMat = r.Field<bool>("CoMat"),
                        BuoiThu = r.Field<string>("BuoiThu") ?? "",
                        ThoiGianBatDau = r.Field<DateTime?>("ThoiGianBatDau"),
                        ThoiGianKetThuc = r.Field<DateTime?>("ThoiGianKetThuc"),
                        ChuDe = r.Field<string>("ChuDe") ?? "",
                        HoTen = r.Field<string>("HoTen") ?? "",
                        TenLop = r.Field<string>("TenLop") ?? "",
                        NgayHoc = r.Field<DateTime?>("ThoiGianBatDau")?.Date
                    });
                }
                FilteredDiemDanhList = new ObservableCollection<DiemDanhModel>(DiemDanhList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private bool CanAdd()
            => BuoiHocId != null && BuoiHocId > 0
            && HocVienId != null && HocVienId > 0
            && !DiemDanhList.Any(x => x.BuoiHocId == BuoiHocId && x.HocVienId == HocVienId);

        private void Add()
        {
            try
            {
                string sql = $@"
                    INSERT INTO dbo.DiemDanh (BuoiHocId, HocVienId, CoMat)
                    VALUES ({BuoiHocId}, {HocVienId}, {(CoMat ? 1 : 0)});";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã thêm điểm danh.");
                }
                else MessageBox.Show("Không thể thêm điểm danh.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm: {ex.Message}");
            }
        }

        private bool CanUpdate()
            => SelectedDiemDanh != null && Id.HasValue
            && BuoiHocId != null && BuoiHocId > 0
            && HocVienId != null && HocVienId > 0;

        private void Update()
        {
            if (!Id.HasValue) return;
            try
            {
                string sql = $@"
                    UPDATE dbo.DiemDanh SET
                    BuoiHocId = {BuoiHocId},
                    HocVienId = {HocVienId},
                    CoMat = {(CoMat ? 1 : 0)}
                    WHERE Id = {Id.Value};";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    MessageBox.Show("Đã cập nhật điểm danh.");
                }
                else MessageBox.Show("Không thể cập nhật điểm danh.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật: {ex.Message}");
            }
        }

        private bool CanDelete() => SelectedDiemDanh != null && Id.HasValue;

        private void Delete()
        {
            if (!Id.HasValue) return;
            if (MessageBox.Show("Xóa điểm danh đã chọn?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                int n = Connect.DataExcution($"DELETE FROM dbo.DiemDanh WHERE Id = {Id.Value};");
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã xóa điểm danh.");
                }
                else MessageBox.Show("Không thể xóa điểm danh.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xóa: {ex.Message}");
            }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredDiemDanhList = new ObservableCollection<DiemDanhModel>(DiemDanhList);
                return;
            }

            string kw = SearchText.Trim().ToLower();
            var rs = DiemDanhList.Where(d =>
                    (d.HoTen?.ToLower().Contains(kw) ?? false) ||
                    (d.TenLop?.ToLower().Contains(kw) ?? false) ||
                    d.BuoiHocId.ToString().Contains(kw) ||
                    d.HocVienId.ToString().Contains(kw) ||
                    (d.BuoiThu?.ToString().Contains(kw) ?? false) ||
                    d.TrangThaiText.ToLower().Contains(kw) ||
                    (d.ChuDe?.ToLower().Contains(kw) ?? false))
                .ToList();
            FilteredDiemDanhList = new ObservableCollection<DiemDanhModel>(rs);
        }

        private void Clear()
        {
            Id = null;
            BuoiHocId = null;
            HocVienId = null;
            CoMat = true;
            HoTen = "";
            TenLop = "";
            NgayHoc = null;
            BuoiThu = "";
            ThoiGianBatDau = null;
            ThoiGianKetThuc = null;
            ChuDe = "";
            SelectedDiemDanh = null;
        }

        private void LoadThongTinFromBuoiHocId()
        {
            if (!BuoiHocId.HasValue || BuoiHocId <= 0)
            {
                TenLop = "";
                NgayHoc = null;
                BuoiThu = "";
                ThoiGianBatDau = null;
                ThoiGianKetThuc = null;
                ChuDe = "";
                return;
            }

            try
            {
                var dt = Connect.DataTransport($@"
                    SELECT lh.TenLop, bh.BuoiThu, bh.ThoiGianBatDau, bh.ThoiGianKetThuc, bh.ChuDe
                    FROM dbo.BuoiHoc bh
                    LEFT JOIN dbo.LopHoc lh ON bh.LopHocId = lh.Id
                    WHERE bh.Id = {BuoiHocId.Value};");

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    TenLop = row["TenLop"]?.ToString() ?? "";
                    BuoiThu = row["BuoiThu"]?.ToString() ?? "";
                    ThoiGianBatDau = row["ThoiGianBatDau"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ThoiGianBatDau"]);
                    ThoiGianKetThuc = row["ThoiGianKetThuc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ThoiGianKetThuc"]);
                    ChuDe = row["ChuDe"]?.ToString() ?? "";
                    NgayHoc = ThoiGianBatDau?.Date;
                }
                else
                {
                    TenLop = "Không tìm thấy thông tin buổi học";
                    NgayHoc = null;
                    BuoiThu = "";
                    ThoiGianBatDau = null;
                    ThoiGianKetThuc = null;
                    ChuDe = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin Buổi học: {ex.Message}");
                TenLop = "Lỗi tải dữ liệu";
                NgayHoc = null;
                BuoiThu = "";
                ThoiGianBatDau = null;
                ThoiGianKetThuc = null;
                ChuDe = "";
            }
        }

        private void LoadThongTinFromHocVienId()
        {
            if (!HocVienId.HasValue || HocVienId <= 0)
            {
                HoTen = "";
                return;
            }

            try
            {
                var dt = Connect.DataTransport($@"
                    SELECT HoTen
                    FROM dbo.HocVien
                    WHERE Id = {HocVienId.Value};");

                if (dt.Rows.Count > 0)
                {
                    HoTen = dt.Rows[0]["HoTen"]?.ToString() ?? "";
                }
                else
                {
                    HoTen = "Không tìm thấy thông tin học viên";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin Học viên: {ex.Message}");
                HoTen = "Lỗi tải dữ liệu";
            }
        }
    }
}