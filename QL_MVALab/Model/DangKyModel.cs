using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_MVALab.Model
{
    public class DangKyModel 
    {
        public int Id { get; set; }
        public int HocVienID { get; set; } 
        public int LopHocID  { get; set; } 
        public DateTime NgayDangKy { get; set; } 
        public bool TrangThai { get; set; } = true;

        public string HoTen { get; set; } = ""; //này là tên học viên để hiển thị cho dễ ( thay cho học viên id)
        public string TenLop { get; set; } = ""; //này là tên lớp học để hiển thị cho dễ ( thay cho lớp học id)
    }
}
