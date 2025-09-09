using System;

namespace QL_MVALab.Model
{
    public class BuoiHocModel
    {
        public int Id { get; set; }
        public int LopHocId { get; set; }
        public string BuoiThu { get; set; } = "";
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string ChuDe { get; set; } = "";
        public string LinkBuoiHoc { get; set; } = "";

        // Thông tin hiển thị từ join với các bảng khác
        public string TenLop { get; set; } = ""; // Từ bảng LopHoc
        public string KhoaHocId { get; set; } = ""; // Từ bảng KhoaHoc (TenKhoa)
        public string GiangVienId { get; set; } = ""; // Từ bảng GiangVien (HoTen)
    }
}