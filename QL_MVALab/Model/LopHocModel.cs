using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_MVALab.Model
{
    public class LopHocModel
    {
        public int Id { get; set; }
        public string TenLop { get; set; } = "";
        public int KhoaHocId { get; set; }
        public int GiangVienId { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int SiSo { get; set; }
        public string LichHoc { get; set; } = "";
        public string TenKhoaHoc { get; set; } = "";
        public string TenGiangVien { get; set; } = "";

    }
}
