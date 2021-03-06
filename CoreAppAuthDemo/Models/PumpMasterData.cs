using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Models
{
    public class PumpMasterData
    {
        public string PumpModel { get; set; }
        public string CustomerModel { get; set; }
        public string CustomerName { get; set; }
        public string SalesUnit { get; set; }
        public string PackagingType { get; set; }
        public string PackingBoxNumber { get; set; }
        public int? PerBoxPumpQty { get; set; }
        public string PumpType { get; set; }
        public string BoxDestination { get; set; }
    }
}
