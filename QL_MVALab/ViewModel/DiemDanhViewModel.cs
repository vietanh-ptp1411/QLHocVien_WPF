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
                // Cập nhật selected items dựa trên IDs
                SelectedLopHoc = LopHocList.FirstOrDefault(l => DiemDanhList.Any(d => d.BuoiHocId == BuoiHocId && d.TenLop == l.TenLop));
                SelectedBuoiHoc = BuoiHocList.FirstOrDefault(b => b.Id == BuoiHocId);
                SelectedHocVien = HocVienList.FirstOrDefault(h => h.Id == HocVienId);
            }
        }

        public int? Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        private int? _id;

        public int? BuoiHocId { get => _buoiHocId; set { _buoiHocId = value; OnPropertyChanged(); } }
        private int? _buoiHocId;

        public int? HocVienId { get => _hocVienId; set { _hocVienId = value; OnPropertyChanged(); } }
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

        // Danh sách cho ComboBox
        public ObservableCollection<LopHocModel> LopHocList { get; private set; } = new();
        private LopHocModel? _selectedLopHoc;
        public LopHocModel? SelectedLopHoc
        {
            get => _selectedLopHoc;
            set
            {
                _selectedLopHoc = value; OnPropertyChanged();
                if (value == null)
                {
                    BuoiHocList.Clear();
                    HocVienList.Clear();
                    TenLop = "";
                    ClearBuoiHocInfo();
                    return;
                }
                TenLop = value.TenLop;
                LoadBuoiHocList(value.Id);
                LoadHocVienList(value.Id);
            }
        }

        public ObservableCollection<BuoiHocModel> BuoiHocList { get; private set; } = new();
        private BuoiHocModel? _selectedBuoiHoc;
        public BuoiHocModel? SelectedBuoiHoc
        {
            get => _selectedBuoiHoc;
            set
            {
                _selectedBuoiHoc = value; OnPropertyChanged();
                if (value == null)
                {
                    BuoiHocId = null;
                    ClearBuoiHocInfo();
                    return;
                }
                BuoiHocId = value.Id;
                BuoiThu = value.BuoiThu;
                ThoiGianBatDau = value.ThoiGianBatDau;
                ThoiGianKetThuc = value.ThoiGianKetThuc;
                ChuDe = value.ChuDe;
                NgayHoc = value.ThoiGianBatDau.Date;
            }
        }

        public ObservableCollection<HocVienModel> HocVienList { get; private set; } = new();
        private HocVienModel? _selectedHocVien;
        public HocVienModel? SelectedHocVien
        {
            get => _selectedHocVien;
            set
            {
                _selectedHocVien = value; OnPropertyChanged();
                if (value == null)
                {
                    HocVienId = null;
                    HoTen = "";
                    return;
                }
                HocVienId = value.Id;
                HoTen = value.HoTen;
            }
        }

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
                LoadLopHocList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void LoadLopHocList()
        {
            try
            {
                var dt = Connect.DataTransport(@"
                    SELECT Id, TenLop
                    FROM dbo.LopHoc
                    ORDER BY TenLop ASC");

                LopHocList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    LopHocList.Add(new LopHocModel
                    {
                        Id = r.Field<int>("Id"),
                        TenLop = r.Field<string>("TenLop") ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách lớp học: {ex.Message}");
            }
        }

        private void LoadBuoiHocList(int lopHocId)
        {
            try
            {
                var dt = Connect.DataTransport($@"
                    SELECT Id, BuoiThu, ThoiGianBatDau, ThoiGianKetThuc, ChuDe
                    FROM dbo.BuoiHoc
                    WHERE LopHocId = {lopHocId}
                    ORDER BY ThoiGianBatDau ASC");

                BuoiHocList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    BuoiHocList.Add(new BuoiHocModel
                    {
                        Id = r.Field<int>("Id"),
                        BuoiThu = r.Field<string>("BuoiThu") ?? "",
                        ThoiGianBatDau = r.Field<DateTime>("ThoiGianBatDau"),
                        ThoiGianKetThuc = r.Field<DateTime>("ThoiGianKetThuc"),
                        ChuDe = r.Field<string>("ChuDe") ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách buổi học: {ex.Message}");
            }
        }

        private void LoadHocVienList(int lopHocId)
        {
            try
            {
                var dt = Connect.DataTransport($@"
                    SELECT DISTINCT hv.Id, hv.HoTen
                    FROM dbo.HocVien hv
                    INNER JOIN dbo.DangKy dk ON hv.Id = dk.HocVienId
                    WHERE dk.LopHocId = {lopHocId}
                    ORDER BY hv.HoTen ASC");

                HocVienList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    HocVienList.Add(new HocVienModel
                    {
                        Id = r.Field<int>("Id"),
                        HoTen = r.Field<string>("HoTen") ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách học viên: {ex.Message}");
            }
        }

        private void ClearBuoiHocInfo()
        {
            NgayHoc = null;
            BuoiThu = "";
            ThoiGianBatDau = null;
            ThoiGianKetThuc = null;
            ChuDe = "";
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
            SelectedLopHoc = null;
            SelectedBuoiHoc = null;
            SelectedHocVien = null;
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
    }
}