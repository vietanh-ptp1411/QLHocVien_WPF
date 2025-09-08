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

        // Thông tin từ LopHoc để hiển thị
        public string TenLop { get; set; } = ""; // Tên lớp học
        public string KhoaHocId { get; set; } = ""; // Mã khóa học
        public string GiangVienId { get; set; } = ""; // Tên giảng viên
    }
}