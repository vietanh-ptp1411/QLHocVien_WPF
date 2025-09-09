using System;

namespace QL_MVALab.Model
{
    public class DiemDanhModel : BaseModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private int _buoiHocId;
        public int BuoiHocId
        {
            get => _buoiHocId;
            set { _buoiHocId = value; OnPropertyChanged(); }
        }

        private int _hocVienId;
        public int HocVienId
        {
            get => _hocVienId;
            set { _hocVienId = value; OnPropertyChanged(); }
        }

        private bool _coMat;
        public bool CoMat
        {
            get => _coMat;
            set { _coMat = value; OnPropertyChanged(); OnPropertyChanged(nameof(TrangThaiText)); }
        }

        // Thông tin hiển thị từ các bảng liên quan
        private string? _hoTen;
        public string? HoTen
        {
            get => _hoTen;
            set { _hoTen = value; OnPropertyChanged(); }
        }

        private string? _tenLop;
        public string? TenLop
        {
            get => _tenLop;
            set { _tenLop = value; OnPropertyChanged(); }
        }

        private DateTime? _ngayHoc;
        public DateTime? NgayHoc
        {
            get => _ngayHoc;
            set { _ngayHoc = value; OnPropertyChanged(); }
        }

        private string? _buoiThu;
        public string? BuoiThu
        {
            get => _buoiThu;
            set { _buoiThu = value; OnPropertyChanged(); }
        }

        private DateTime? _thoiGianBatDau;
        public DateTime? ThoiGianBatDau
        {
            get => _thoiGianBatDau;
            set { _thoiGianBatDau = value; OnPropertyChanged(); }
        }

        private DateTime? _thoiGianKetThuc;
        public DateTime? ThoiGianKetThuc
        {
            get => _thoiGianKetThuc;
            set { _thoiGianKetThuc = value; OnPropertyChanged(); }
        }

        private string? _chuDe;
        public string? ChuDe
        {
            get => _chuDe;
            set { _chuDe = value; OnPropertyChanged(); }
        }

        // Computed property để hiển thị trạng thái
        public string TrangThaiText => CoMat ? "Có mặt" : "Vắng mặt";

        // Constructor
        public DiemDanhModel()
        {
            CoMat = true; // Mặc định là có mặt
        }
    }
}