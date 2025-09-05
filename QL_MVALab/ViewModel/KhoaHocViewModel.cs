using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using QL_MVALab.ConnectDatabase;
using QL_MVALab.Model;

namespace QL_MVALab.ViewModel
{
    public class KhoaHocViewModels : BaseViewModel
    {
        // Danh sách / lọc
        public ObservableCollection<KhoaHocModel> KhoaHocList { get; private set; } = new();
        private ObservableCollection<KhoaHocModel> _filtered = new();
        public ObservableCollection<KhoaHocModel> FilteredKhoaHocList
        {
            get => _filtered; set { _filtered = value; OnPropertyChanged(); }
        }

        // Bản ghi đang chọn + các ô nhập
        private KhoaHocModel? _selected;
        public KhoaHocModel? SelectedKhoaHoc
        {
            get => _selected;
            set
            {
                _selected = value; OnPropertyChanged();
                if (value == null) return;
                Id = value.Id;
                TenKhoa = value.TenKhoa;
                MoTa = value.MoTa;
                DonGia10Buoi = value.DonGia10Buoi;
                TongSoBuoi = value.TongSoBuoi;
                TrangThai = value.TrangThai;
            }
        }

        public int? Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        private int? _id;

        public string TenKhoa { get => _ten; set { _ten = value; OnPropertyChanged(); } }
        private string _ten = "";

        public string MoTa { get => _MoTa; set { _MoTa = value; OnPropertyChanged(); } }
        private string _MoTa  = "";

        public Decimal? DonGia10Buoi { get => _dg; set { _dg = value; OnPropertyChanged(); } }
        private Decimal? _dg;

        public int? TongSoBuoi { get => _tsb; set { _tsb = value; OnPropertyChanged(); } }
        private int? _tsb;

        public bool TrangThai { get => _tt; set { _tt = value; OnPropertyChanged(); } }
        private bool _tt = true;

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

        public KhoaHocViewModels()
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
                    "SELECT Id , TenKhoa , MoTa, DonGia10Buoi, TongSoBuoi, TrangThai " +
                    "FROM dbo.KhoaHoc ORDER BY Id ASC");

                KhoaHocList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    KhoaHocList.Add(new KhoaHocModel
                    {
                        Id = r.Field<int>("Id"),
                        TenKhoa = r.Field<string>("TenKhoa") ?? "",
                        MoTa = r["MoTa"]?.ToString() ?? "",
                        DonGia10Buoi = r.Field<Decimal>("DonGia10Buoi"),
                        TongSoBuoi = r.Field<int>("TongSoBuoi"),
                        TrangThai = r.Field<bool>("TrangThai")
                    });
                }
                FilteredKhoaHocList = new ObservableCollection<KhoaHocModel>(KhoaHocList);
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}"); }
        }

        private bool CanAdd()
            => !string.IsNullOrWhiteSpace(TenKhoa)
            && DonGia10Buoi != null && DonGia10Buoi >= 0
            && TongSoBuoi != null && TongSoBuoi > 0
            && !KhoaHocList.Any(x => x.TenKhoa.Equals(TenKhoa, StringComparison.OrdinalIgnoreCase));

        private void Add()
        {
            try
            {
                // Id & NgayTao DB tự sinh; dùng NULLIF để đẩy NULL khi người dùng để trống
                string sql = $@"
                        INSERT INTO dbo.KhoaHoc ( TenKhoa , MoTa, DonGia10Buoi, TongSoBuoi, TrangThai)
                        VALUES (N'{Esc(TenKhoa)}',  N'{Esc(MoTa)}',
                        {DonGia10Buoi}, {TongSoBuoi} ,{(TrangThai ? 1 : 0)} );";
                int n = Connect.DataExcution(sql);
                if (n > 0) { LoadData(); Clear(); MessageBox.Show("Đã thêm khóa học."); }
                else MessageBox.Show("Không thể thêm khóa học.");
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi thêm: {ex.Message}"); }
        }

        private bool CanUpdate()
            => SelectedKhoaHoc != null && Id.HasValue
            && !string.IsNullOrWhiteSpace(TenKhoa);

        private void Update()
        {
            if (!Id.HasValue) return;
            try
            {
                string sql = $@"
                                UPDATE dbo.KhoaHoc SET
                                TenKhoa     = N'{Esc(TenKhoa)}',
                                MoTa     = N'{Esc(MoTa)}',
                                DonGia10Buoi = {DonGia10Buoi},
                                TongSoBuoi    = {TongSoBuoi},
                                TrangThai = {(TrangThai ? 1 : 0)}
                                WHERE Id = {Id.Value};";
                int n = Connect.DataExcution(sql);
                if (n > 0) { LoadData(); MessageBox.Show("Đã cập nhật."); }
                else MessageBox.Show("Không thể cập nhật.");
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi cập nhật: {ex.Message}"); }
        }

        private bool CanDelete() => SelectedKhoaHoc != null && Id.HasValue;

        private void Delete()
        {
            if (!Id.HasValue) return;
            if (MessageBox.Show("Xóa khóa học đã chọn?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                int n = Connect.DataExcution($"DELETE FROM dbo.KhoaHoc WHERE Id = {Id.Value};");
                if (n > 0) { LoadData(); Clear(); MessageBox.Show("Đã xóa."); }
                else MessageBox.Show("Không thể xóa.");
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi xóa: {ex.Message}"); }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredKhoaHocList = new ObservableCollection<KhoaHocModel>(KhoaHocList);
                return;
            }
            string kw = SearchText.Trim().ToLower();
            var rs = KhoaHocList.Where(h =>
                    (h.TenKhoa?.ToLower().Contains(kw) ?? false) )
                .ToList();
            FilteredKhoaHocList = new ObservableCollection<KhoaHocModel>(rs);
        }

        private void Clear()
        {
            Id = null;
            TenKhoa = MoTa = "";
            DonGia10Buoi = TongSoBuoi = null;
            TrangThai = true;
        }

        private static string Esc(string? s) => (s ?? string.Empty).Replace("'", "''x");
    }
}