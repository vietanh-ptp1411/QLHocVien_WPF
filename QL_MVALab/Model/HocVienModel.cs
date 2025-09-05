using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace QL_MVALab.Model
{
    public class HocVienModel
    {
        public int Id { get; set; }                 // identity
        public string MaHocVien { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string? NamSinh { get; set; }
        public string Email { get; set; } = "";
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateTime NgayTao { get; set; }       // DEFAULT GETDATE()
    }
}