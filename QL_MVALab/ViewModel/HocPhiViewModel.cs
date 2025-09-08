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
    public class HocPhiViewModels : BaseViewModel
    {
        // Danh sách / lọc
        public ObservableCollection<HocPhiModel> HocPhiList { get; private set; } = new();
        private ObservableCollection<HocPhiModel> _filtered = new();
        public ObservableCollection<HocPhiModel> FilteredHocPhiList
        {
            get => _filtered;
            set { _filtered = value; OnPropertyChanged(); }
        }

        // Bản ghi đang chọn + các ô nhập
        private HocPhiModel? _selected;
        public HocPhiModel? SelectedHocPhi
        {
            get => _selected;
            set
            {
                _selected = value; OnPropertyChanged();
                if (value == null) return;
                Id = value.Id;
                DangKyId = value.DangKyId;
                KyThu = value.KyThu;
                SoBuoi = value.SoBuoi;
                SoTien = value.SoTien;
                NgayDong = value.NgayDong;
                GhiChu = value.GhiChu;
            }
        }

        public int? Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        private int? _id;

        public int? DangKyId { get => _dkid; set { _dkid = value; OnPropertyChanged(); LoadThongTinFromDangKyId(); } }
        private int? _dkid;

        public int KyThu { get => _KyThu; set { _KyThu = value; OnPropertyChanged(); } }
        private int _KyThu;

        public int? SoBuoi { get => _sb; set { _sb = value; OnPropertyChanged(); } }
        private int? _sb;

        public decimal SoTien { get => _st; set { _st = value; OnPropertyChanged(); } }
        private decimal _st;

        public string? GhiChu { get => _gc; set { _gc = value; OnPropertyChanged(); } }
        private string? _gc;

        public DateTime? NgayDong { get => _nd; set { _nd = value; OnPropertyChanged(); } }
        private DateTime? _nd;

        // Thông tin hiển thị (chỉ đọc)
        public string? HoTen { get => _hoTen; set { _hoTen = value; OnPropertyChanged(); } }
        private string? _hoTen = "";

        public string? TenLop { get => _tenLop; set { _tenLop = value; OnPropertyChanged(); } }
        private string? _tenLop = "";

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

        public HocPhiViewModels()
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
                // Join với bảng DangKy, HocVien và LopHoc để lấy thông tin đầy đủ
                var dt = Connect.DataTransport(@"
                    SELECT hp.Id, hp.DangKyID, hp.KyThu, hp.SoBuoi, hp.SoTien, hp.NgayDong, hp.GhiChu,
                           hv.HoTen, lh.TenLop
                    FROM dbo.HocPhi hp
                    LEFT JOIN dbo.DangKy dk ON hp.DangKyID = dk.Id
                    LEFT JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
                    LEFT JOIN dbo.LopHoc lh ON dk.LopHocId = lh.Id
                    ORDER BY hp.Id ASC");

                HocPhiList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    HocPhiList.Add(new HocPhiModel
                    {
                        Id = r.Field<int>("Id"),
                        DangKyId = r.Field<int>("DangKyID"),
                        KyThu = r.Field<int>("KyThu"),
                        SoBuoi = r.Field<int?>("SoBuoi") ?? 0,
                        SoTien = r.Field<decimal>("SoTien"),
                        GhiChu = r.Field<string>("GhiChu") ?? "",
                        NgayDong = r.Field<DateTime?>("NgayDong") ?? DateTime.Now,
                        HoTen = r.Field<string>("HoTen") ?? "",
                        TenLop = r.Field<string>("TenLop") ?? ""
                    });
                }
                FilteredHocPhiList = new ObservableCollection<HocPhiModel>(HocPhiList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private bool CanAdd()
            => DangKyId != null && DangKyId > 0
            && KyThu > 0
            && SoTien > 0
            && !HocPhiList.Any(x => x.DangKyId == DangKyId && x.KyThu == KyThu);

        private void Add()
        {
            try
            {
                string sql = $@"
                    INSERT INTO dbo.HocPhi (DangKyID, KyThu, SoBuoi, SoTien, NgayDong, GhiChu)
                    VALUES ({DangKyId}, {KyThu}, {SoBuoi ?? 0}, {SoTien}, 
                    {(NgayDong.HasValue ? $"'{NgayDong.Value:yyyy-MM-dd}'" : "GETDATE()")}, 
                    N'{Esc(GhiChu)}');";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã thêm học phí.");
                }
                else MessageBox.Show("Không thể thêm học phí.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm: {ex.Message}");
            }
        }

        private bool CanUpdate()
            => SelectedHocPhi != null && Id.HasValue
            && DangKyId != null && DangKyId > 0
            && KyThu > 0
            && SoTien > 0;

        private void Update()
        {
            if (!Id.HasValue) return;
            try
            {
                string sql = $@"
                    UPDATE dbo.HocPhi SET
                    DangKyID = {DangKyId},
                    KyThu = {KyThu},
                    SoBuoi = {SoBuoi ?? 0},
                    SoTien = {SoTien},
                    NgayDong = {(NgayDong.HasValue ? $"'{NgayDong.Value:yyyy-MM-dd}'" : "GETDATE()")},
                    GhiChu = N'{Esc(GhiChu)}'
                    WHERE Id = {Id.Value};";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    MessageBox.Show("Đã cập nhật học phí.");
                }
                else MessageBox.Show("Không thể cập nhật học phí.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật: {ex.Message}");
            }
        }

        private bool CanDelete() => SelectedHocPhi != null && Id.HasValue;

        private void Delete()
        {
            if (!Id.HasValue) return;
            if (MessageBox.Show("Xóa học phí đã chọn?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                int n = Connect.DataExcution($"DELETE FROM dbo.HocPhi WHERE Id = {Id.Value};");
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã xóa học phí.");
                }
                else MessageBox.Show("Không thể xóa học phí.");
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
                FilteredHocPhiList = new ObservableCollection<HocPhiModel>(HocPhiList);
                return;
            }

            string kw = SearchText.Trim().ToLower();
            var rs = HocPhiList.Where(h =>
                    (h.HoTen?.ToLower().Contains(kw) ?? false) ||
                    (h.TenLop?.ToLower().Contains(kw) ?? false) ||
                    (h.GhiChu?.ToLower().Contains(kw) ?? false) ||
                    h.KyThu.ToString().Contains(kw) ||
                    h.DangKyId.ToString().Contains(kw) ||
                    h.SoTien.ToString("N0").Contains(kw))
                .ToList();
            FilteredHocPhiList = new ObservableCollection<HocPhiModel>(rs);
        }

        private void Clear()
        {
            Id = null;
            DangKyId = null;
            KyThu = 0;
            SoBuoi = null;
            SoTien = 0;
            NgayDong = null;
            GhiChu = "";
            HoTen = "";
            TenLop = "";
            SelectedHocPhi = null;
        }

        private void LoadThongTinFromDangKyId()
        {
            if (!DangKyId.HasValue || DangKyId <= 0)
            {
                HoTen = "";
                TenLop = "";
                return;
            }

            try
            {
                var dt = Connect.DataTransport($@"
                    SELECT hv.HoTen, lh.TenLop
                    FROM dbo.DangKy dk
                    LEFT JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
                    LEFT JOIN dbo.LopHoc lh ON dk.LopHocId = lh.Id
                    WHERE dk.Id = {DangKyId.Value};");

                if (dt.Rows.Count > 0)
                {
                    HoTen = dt.Rows[0].Field<string>("HoTen") ?? "";
                    TenLop = dt.Rows[0].Field<string>("TenLop") ?? "";
                }
                else
                {
                    HoTen = "Không tìm thấy thông tin đăng ký";
                    TenLop = "Không tìm thấy thông tin đăng ký";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin Đăng Ký: {ex.Message}");
                HoTen = "Lỗi tải dữ liệu";
                TenLop = "Lỗi tải dữ liệu";
            }
        }

        private static string Esc(string? s) => (s ?? string.Empty).Replace("'", "''");
    }
}