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
    public class BuoiHocViewModels : BaseViewModel
    {
        // Danh sách / lọc
        public ObservableCollection<BuoiHocModel> BuoiHocList { get; private set; } = new();
        private ObservableCollection<BuoiHocModel> _filtered = new();
        public ObservableCollection<BuoiHocModel> FilteredBuoiHocList
        {
            get => _filtered;
            set { _filtered = value; OnPropertyChanged(); }
        }

        // Bản ghi đang chọn + các ô nhập
        private BuoiHocModel? _selected;
        public BuoiHocModel? SelectedBuoiHoc
        {
            get => _selected;
            set
            {
                _selected = value; OnPropertyChanged();
                if (value == null) return;
                Id = value.Id;
                LopHocId = value.LopHocId;
                BuoiThu = value.BuoiThu;
                ThoiGianBatDau = value.ThoiGianBatDau;
                ThoiGianKetThuc = value.ThoiGianKetThuc;
                ChuDe = value.ChuDe;
                LinkBuoiHoc = value.LinkBuoiHoc;
                TenLop = value.TenLop;
                KhoaHocId = value.KhoaHocId;
                GiangVienId = value.GiangVienId;
            }
        }

        public int? Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        private int? _id;

        public int? LopHocId { get => _lhid; set { _lhid = value; OnPropertyChanged(); LoadThongTinFromLopHocId(); } }
        private int? _lhid;

        public string BuoiThu { get => _bt; set { _bt = value; OnPropertyChanged(); } }
        private string _bt = "";

        public DateTime ThoiGianBatDau { get => _tgbd; set { _tgbd = value; OnPropertyChanged(); } }
        private DateTime _tgbd ;

        public DateTime ThoiGianKetThuc { get => _tgkt; set { _tgkt = value; OnPropertyChanged(); } }
        private DateTime _tgkt ;

        public string ChuDe { get => _cd; set { _cd = value; OnPropertyChanged(); } }
        private string _cd = "";

        public string LinkBuoiHoc { get => _lk; set { _lk = value; OnPropertyChanged(); } }
        private string _lk = "";

        // Thông tin hiển thị từ LopHoc
        public string TenLop { get => _tl; set { _tl = value; OnPropertyChanged(); } }
        private string _tl = "";

        public string KhoaHocId { get => _khid; set { _khid = value; OnPropertyChanged(); } }
        private string _khid = "";

        public string GiangVienId { get => _gvid; set { _gvid = value; OnPropertyChanged(); } }
        private string _gvid = "";

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

        public BuoiHocViewModels()
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
                // Join với bảng LopHoc, KhoaHoc, GiangVien để lấy thông tin hiển thị
                var dt = Connect.DataTransport(@"
                    SELECT bh.Id, bh.LopHocId, bh.BuoiThu, bh.ThoiGianBatDau, bh.ThoiGianKetThuc, bh.ChuDe, bh.LinkBuoiHoc,
                           lh.TenLop, kh.TenKhoa as KhoaHocId, gv.HoTen as GiangVienId
                    FROM dbo.BuoiHoc bh
                    LEFT JOIN dbo.LopHoc lh ON bh.LopHocId = lh.Id
                    LEFT JOIN dbo.KhoaHoc kh ON lh.KhoaHocId = kh.Id
                    LEFT JOIN dbo.GiangVien gv ON lh.GiangVienId = gv.Id
                    ORDER BY bh.Id ASC");

                BuoiHocList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    BuoiHocList.Add(new BuoiHocModel
                    {
                        Id = r.Field<int>("Id"),
                        LopHocId = r.Field<int>("LopHocId"),
                        BuoiThu = r.Field<string>("BuoiThu") ?? "",
                        ThoiGianBatDau = r.Field<DateTime>("ThoiGianBatDau") ,
                        ThoiGianKetThuc = r.Field<DateTime>("ThoiGianKetThuc") ,
                        ChuDe = r.Field<string>("ChuDe") ?? "",
                        LinkBuoiHoc = r.Field<string>("LinkBuoiHoc") ?? "",
                        TenLop = r.Field<string>("TenLop") ?? "",
                        KhoaHocId = r.Field<string>("KhoaHocId") ?? "",
                        GiangVienId = r.Field<string>("GiangVienId") ?? ""
                    });
                }
                FilteredBuoiHocList = new ObservableCollection<BuoiHocModel>(BuoiHocList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private bool CanAdd()
            => LopHocId != null && LopHocId > 0
            && !string.IsNullOrWhiteSpace(BuoiThu)
            && ThoiGianBatDau != null
            && ThoiGianKetThuc != null;

        private void Add()
        {
            try
            {
                string sql = $@"
                    INSERT INTO dbo.BuoiHoc (LopHocId, BuoiThu, ThoiGianBatDau, ThoiGianKetThuc, ChuDe, LinkBuoiHoc)
                    VALUES ({LopHocId}, N'{Esc(BuoiThu)}', {ThoiGianBatDau:yyyy-MM-dd}, 
                    {ThoiGianKetThuc:yyyy-MM-dd}, N'{Esc(ChuDe)}' , N'{Esc(LinkBuoiHoc)}');";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã thêm buổi học.");
                }
                else MessageBox.Show("Không thể thêm buổi học.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm: {ex.Message}");
            }
        }

        private bool CanUpdate()
            => SelectedBuoiHoc != null && Id.HasValue
            && LopHocId != null && LopHocId > 0
            && !string.IsNullOrWhiteSpace(BuoiThu)
            && ThoiGianBatDau!= null
            && ThoiGianKetThuc!=null;

        private void Update()
        {
            if (!Id.HasValue) return;
            try
            {
                string sql = $@"
                    UPDATE dbo.BuoiHoc SET
                    LopHocId = {LopHocId},
                    BuoiThu = N'{Esc(BuoiThu)}',
                    ThoiGianBatDau = '{ThoiGianBatDau:yyyy-MM-dd}',
                    ThoiGianKetThuc ='{ThoiGianKetThuc:yyyy-MM-dd}',
                    ChuDe = N'{Esc(ChuDe)}',
                    LinkBuoiHoc = N'{Esc(LinkBuoiHoc)}'   
                    WHERE Id = {Id.Value};";

                int n = Connect.DataExcution(sql);
                if (n > 0)
                {
                    LoadData();
                    MessageBox.Show("Đã cập nhật buổi học.");
                }
                else MessageBox.Show("Không thể cập nhật buổi học.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật: {ex.Message}");
            }
        }

        private bool CanDelete() => SelectedBuoiHoc != null && Id.HasValue;

        private void Delete()
        {
            if (!Id.HasValue) return;
            if (MessageBox.Show("Xóa buổi học đã chọn?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                int n = Connect.DataExcution($"DELETE FROM dbo.BuoiHoc WHERE Id = {Id.Value};");
                if (n > 0)
                {
                    LoadData();
                    Clear();
                    MessageBox.Show("Đã xóa buổi học.");
                }
                else MessageBox.Show("Không thể xóa buổi học.");
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
                FilteredBuoiHocList = new ObservableCollection<BuoiHocModel>(BuoiHocList);
                return;
            }

            string kw = SearchText.Trim().ToLower();
            var rs = BuoiHocList.Where(h =>
                    (h.TenLop?.ToLower().Contains(kw) ?? false) ||
                    (h.BuoiThu?.ToLower().Contains(kw) ?? false) ||
                    (h.ChuDe?.ToLower().Contains(kw) ?? false) ||
                    (h.KhoaHocId?.ToLower().Contains(kw) ?? false) ||
                    (h.GiangVienId?.ToLower().Contains(kw) ?? false) ||
                    h.LopHocId.ToString().Contains(kw))
                .ToList();
            FilteredBuoiHocList = new ObservableCollection<BuoiHocModel>(rs);
        }

        private void Clear()
        {
            Id = null;
            LopHocId = null;
            BuoiThu = "";
            ThoiGianBatDau = DateTime.Now;
            ThoiGianKetThuc = DateTime.Now;
            ChuDe = "";
            LinkBuoiHoc = "";
            TenLop = "";
            KhoaHocId = "";
            GiangVienId = "";
            SelectedBuoiHoc = null;
        }

        // Load thông tin lớp học khi nhập LopHocId
        private void LoadThongTinFromLopHocId()
        {
            if (!LopHocId.HasValue || LopHocId <= 0)
            {
                TenLop = "";
                KhoaHocId = "";
                GiangVienId = "";
                return;
            }

            try
            {
                var dt = Connect.DataTransport($@"
                    SELECT lh.TenLop, kh.TenKhoa, gv.HoTen
                    FROM dbo.LopHoc lh 
                    LEFT JOIN dbo.KhoaHoc kh ON lh.KhoaHocId = kh.Id
                    LEFT JOIN dbo.GiangVien gv ON lh.GiangVienId = gv.Id
                    WHERE lh.Id = {LopHocId.Value}");

                if (dt.Rows.Count > 0)
                {
                    string tenLop = dt.Rows[0].Field<string>("TenLop") ?? "";
                    string tenKhoa = dt.Rows[0].Field<string>("TenKhoa") ?? "";
                    string hoTenGV = dt.Rows[0].Field<string>("HoTen") ?? "";

                    TenLop = tenLop;
                    KhoaHocId = tenKhoa;
                    GiangVienId = hoTenGV;
                }
                else
                {
                    TenLop = "⚠️ Không tìm thấy lớp học này";
                    KhoaHocId = "";
                    GiangVienId = "";
                }
            }
            catch (Exception ex)
            {
                TenLop = $"❌ Lỗi: {ex.Message}";
                KhoaHocId = "";
                GiangVienId = "";
            }
        }

        private static string Esc(string? s) => (s ?? string.Empty).Replace("'", "''");
    }
}