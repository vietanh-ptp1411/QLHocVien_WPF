namespace QL_MVALab.Model
{
    public class DiemDanhModel
    {
        public int Id { get; set; }
        public int BuoiHocId { get; set; }
        public int HocVienId { get; set; }
        public bool CoMat { get; set; }

        // Thông tin hiển thị từ join bảng khác
        public string HoTen { get; set; } = ""; // Tên học viên từ bảng HocVien
        public string TenLop { get; set; } = ""; // Tên lớp từ bảng LopHoc
        public DateTime? ThoiGianBatDau { get; set; } // Ngày học từ bảng BuoiHoc
        public int? BuoiThu { get; set; } // Buổi thứ mấy từ bảng BuoiHoc
        public string TrangThaiText => CoMat ? "Có mặt" : "Vắng mặt";
    }
}