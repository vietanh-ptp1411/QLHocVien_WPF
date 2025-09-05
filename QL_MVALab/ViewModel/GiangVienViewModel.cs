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
    public class GiangVienViewModels : BaseViewModel
    {
        // Danh sách / lọc
        public ObservableCollection<GiangVienModel> GiangVienList { get; private set; } = new();
        private ObservableCollection<GiangVienModel> _filtered = new();
        public ObservableCollection<GiangVienModel> FilteredGiangVienList
        {
            get => _filtered; set { _filtered = value; OnPropertyChanged(); }
        }

        // Bản ghi đang chọn + các ô nhập
        private GiangVienModel? _selected;
        public GiangVienModel? SelectedGiangVien
        {
            get => _selected;
            set
            {
                _selected = value; OnPropertyChanged();
                if (value == null) return;
                Id = value.Id;
                HoTen = value.HoTen;
                Email = value.Email;
                DienThoai = value.DienThoai;
                ChuyenMon = value.ChuyenMon;
                TrangThai = value.TrangThai;
            }
        }

        public int? Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        private int? _id;

        public string HoTen { get => _ten; set { _ten = value; OnPropertyChanged(); } }
        private string _ten = "";

        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        private string _email = "";

        public string? DienThoai { get => _dt; set { _dt = value; OnPropertyChanged(); } }
        private string? _dt;

        public string? ChuyenMon { get => _cm; set { _cm = value; OnPropertyChanged(); } }
        private string? _cm;

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

        public GiangVienViewModels()
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
                    "SELECT Id , HoTen , Email, DienThoai, ChuyenMon, TrangThai " +
                    "FROM dbo.GiangVien ORDER BY Id ASC");

                GiangVienList.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    GiangVienList.Add(new GiangVienModel
                    {
                        Id = r.Field<int>("Id"),
                        HoTen = r["HoTen"]?.ToString() ?? "",
                        Email = r["Email"]?.ToString() ?? "",
                        DienThoai = r["DienThoai"]?.ToString(),
                        ChuyenMon = r["ChuyenMon"]?.ToString(),
                        TrangThai = r.Field<bool>("TrangThai")
                    });
                }
                FilteredGiangVienList = new ObservableCollection<GiangVienModel>(GiangVienList);
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}"); }
        }

        private bool CanAdd()
            => !string.IsNullOrWhiteSpace(HoTen)
            && !string.IsNullOrWhiteSpace(Email)
            && !GiangVienList.Any(x => x.HoTen.Equals(HoTen, StringComparison.OrdinalIgnoreCase));

        private void Add()
        {
            try
            {
                // Id & NgayTao DB tự sinh; dùng NULLIF để đẩy NULL khi người dùng để trống
                string sql = $@"
                        INSERT INTO dbo.GiangVien (HoTen, Email, DienThoai, ChuyenMon , TrangThai)
                        VALUES (N'{Esc(HoTen)}',  N'{Esc(Email)}',
                        NULLIF(N'{Esc(DienThoai)}',''), NULLIF(N'{Esc(ChuyenMon)}','') ,{(TrangThai ? 1 : 0)} );";
                int n = Connect.DataExcution(sql);
                if (n > 0) { LoadData(); Clear(); MessageBox.Show("Đã thêm giảng viên."); }
                else MessageBox.Show("Không thể thêm giảng viên.");
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi thêm: {ex.Message}"); }
        }

        private bool CanUpdate()
            => SelectedGiangVien != null && Id.HasValue
            && !string.IsNullOrWhiteSpace(HoTen)
            && !string.IsNullOrWhiteSpace(Email);

        private void Update()
        {
            if (!Id.HasValue) return;
            try
            {
                string sql = $@"
                                UPDATE dbo.GiangVien SET
                                HoTen     = N'{Esc(HoTen)}',
                                Email     = N'{Esc(Email)}',
                                DienThoai = NULLIF(N'{Esc(DienThoai)}',''),
                                ChuyenMon    = NULLIF(N'{Esc(ChuyenMon)}',''),
                                TrangThai = {(TrangThai ? 1 : 0)}
                                WHERE Id = {Id.Value};";
                int n = Connect.DataExcution(sql);
                if (n > 0) { LoadData(); MessageBox.Show("Đã cập nhật."); }
                else MessageBox.Show("Không thể cập nhật.");
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi cập nhật: {ex.Message}"); }
        }

        private bool CanDelete() => SelectedGiangVien != null && Id.HasValue;

        private void Delete()
        {
            if (!Id.HasValue) return;
            if (MessageBox.Show("Xóa giảng viên đã chọn?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                int n = Connect.DataExcution($"DELETE FROM dbo.GiangVien WHERE Id = {Id.Value};");
                if (n > 0) { LoadData(); Clear(); MessageBox.Show("Đã xóa."); }
                else MessageBox.Show("Không thể xóa.");
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi xóa: {ex.Message}"); }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredGiangVienList = new ObservableCollection<GiangVienModel>(GiangVienList);
                return;
            }
            string kw = SearchText.Trim().ToLower();
            var rs = GiangVienList.Where(h =>
                    (h.HoTen?.ToLower().Contains(kw) ?? false) ||
                    (h.Email?.ToLower().Contains(kw) ?? false) ||
                    (h.DienThoai?.ToLower().Contains(kw) ?? false) ||
                    (h.ChuyenMon?.ToLower().Contains(kw) ?? false))
                .ToList();
            FilteredGiangVienList = new ObservableCollection<GiangVienModel>(rs);
        }

        private void Clear()
        {
            Id = null;
            HoTen = Email = "";
            DienThoai = ChuyenMon = null;
            TrangThai = true;
            SelectedGiangVien = null;
        }

        private static string Esc(string? s) => (s ?? string.Empty).Replace("'", "''x");
    }
}