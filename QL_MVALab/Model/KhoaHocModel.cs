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
        public string TenKhoaHoc { get; set; } = "";
        public string MoTa { get; set; } = "";
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal HocPhi { get; set; }

        // Display property for ComboBox
        public string DisplayText => $"{Id} - {TenKhoaHoc}";

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
