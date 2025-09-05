using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_MVALab.Model
{
    public class GiangVienModel
    {
        public int Id { get; set; }
        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string DienThoai { get; set; } = "";
        public string ChuyenMon { get; set; } = "";
        public bool TrangThai { get; set; } = true;

        // Display property for ComboBox
        public string DisplayText => $"{Id} - {HoTen}";

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
