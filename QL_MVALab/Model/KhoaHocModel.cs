using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_MVALab.Model
{
    public class KhoaHocModel
    {
        public int Id { get; set; }
        public string TenKhoa { get; set; } = "";
        public string MoTa { get; set; } = "";
        public decimal DonGia10Buoi { get; set; }
        public int TongSoBuoi { get; set; }
        public bool TrangThai { get; set; } = true;

        // Display property for ComboBox
        public string DisplayText => $"{Id} - {TenKhoa}";

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
