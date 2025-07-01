using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class ReportResponseDto
    {
        public int ReportID { get; set; }
        public int ReporterID { get; set; }
        public string ReporterName { get; set; }
        public int NewsID { get; set; }
        public string NewsTitle { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsResolved { get; set; }
    }
}
