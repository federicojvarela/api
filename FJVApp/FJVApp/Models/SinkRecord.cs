using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FJVApp.Models
{
    public class SinkRecord
    {
        public SinkRecord()
        {
            id = string.Empty;
            kind = String.Empty;
        }
        public string id { get; set; }
        public string kind { get; set; }
    }
}
