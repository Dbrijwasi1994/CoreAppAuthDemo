using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Models
{
    public class ProcessParametersModel
    {
        public string MachineID { get; set; }
        public List<ProcessParameterData> ProcessParamData { get; set; }
    }

    public class ProcessParameterData
    {
        public string Value { get; set; }
        public string ChartType { get; set; }
        public string HighValue { get; set; }
        public string LowValue { get; set; }
        public string TemplateType { get; set; }
        public string BackColor { get; set; }
        public string BackColorForReference { get; set; }
        public string HeaderName { get; set; }
        public string NextLineVisibility { get; set; }
        public string Template1Visibility { get; set; }
        public string Template2Visibility { get; set; }
        public string chartContainerID { get; set; }
        public string MachineID { get; set; }
        public string GraphHighLowVisibility { get; set; }
        public string TrubleshootIconVisibility { get; set; }
        public string UpdatedTS { get; set; }
    }
}
