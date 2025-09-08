//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace QL_MVALab.Model
//{
//    public class HocPhiModel 
//    {
//        public int Id { get; set; }
//        public int DangKyId { get; set; } 
//        public int KyThu  { get; set; }
//        public int SoBuoi { get; set; } 
//        public decimal SoTien  { get; set; } 
//        public DateTime NgayDong  { get; set; } 
//        public string GhiChu { get; set; } = "";

//        //Thong tin từ dangKy để hiển thị
//        public string HocVienId { get; set; } = "";
//        public string LopHocId { get; set; } = "";

//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_MVALab.Model
{
    public class HocPhiModel
    {
        public int Id { get; set; }
        public int DangKyId { get; set; }
        public int KyThu { get; set; }
        public int SoBuoi { get; set; }
        public decimal SoTien { get; set; }
        public DateTime NgayDong { get; set; }
        public string GhiChu { get; set; } = "";

        // Thông tin hiển thị từ join bảng khác
        public string HoTen { get; set; } = ""; // Tên học viên từ bảng HocVien
        public string TenLop { get; set; } = ""; // Tên lớp từ bảng LopHoc
    }
}