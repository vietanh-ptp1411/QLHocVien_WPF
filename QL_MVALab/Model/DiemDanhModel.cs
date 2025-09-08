using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_MVALab.Model
{
    public class DiemDanhModel
    {
        public int Id { get; set; }
        public int BuoiHocId { get; set; }
        public int HocVienId { get; set; }
        public bool CoMat { get; set; }
    }
}