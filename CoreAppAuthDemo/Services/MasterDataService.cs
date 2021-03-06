using CoreAppAuthDemo.Models;
using CoreAppAuthDemo.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Services
{
    public class MasterDataService : IMasterDataServiceRepo
    {
        private readonly ApplicationDataContext _context;
        private readonly ILogger<MasterDataService> _logger;

        public MasterDataService(ApplicationDataContext context, ILogger<MasterDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            try
            {
                return await _context.Employees.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<PumpMasterData>> GetPumpMasterData()
        {
            try
            {
                return await _context.PumpMasterData.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> InsertPumpMasterData(PumpMasterData data)
        {
            try
            {
                await _context.PumpMasterData.AddAsync(data);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public IEnumerable<ProcessParametersModel> GetProcessParameterDetails()
        {
            List<ProcessParametersModel> processParameters = new List<ProcessParametersModel>();
            ProcessParametersModel parametersModel = new ProcessParametersModel();
            parametersModel.MachineID = "GRINDING-01";
            parametersModel.ProcessParamData = new List<ProcessParameterData> { new ProcessParameterData { BackColor = "Green", BackColorForReference = "", chartContainerID = "", ChartType = "Bar", GraphHighLowVisibility = "visible", HeaderName = "X-Axis Load", HighValue = "28", LowValue = "20", MachineID = "GRINDING-01", NextLineVisibility = "visible", Template1Visibility = "visible", Template2Visibility = "hidden", TemplateType = "Text", TrubleshootIconVisibility = "visible", UpdatedTS = "2021-03-06 15:00:00", Value = "24" }, new ProcessParameterData { BackColor = "Red", BackColorForReference = "", chartContainerID = "", ChartType = "Bar", GraphHighLowVisibility = "visible", HeaderName = "X-Axis Load", HighValue = "28", LowValue = "20", MachineID = "GRINDING-01", NextLineVisibility = "visible", Template1Visibility = "visible", Template2Visibility = "hidden", TemplateType = "Text", TrubleshootIconVisibility = "visible", UpdatedTS = "2021-03-06 15:00:00", Value = "24" } };
            processParameters.Add(parametersModel);
            return processParameters.AsEnumerable();
        }

        public IEnumerable<ProcessParameterData> GetProcessParamData()
        {
            List<ProcessParameterData> parametersDataList = new List<ProcessParameterData>();
            parametersDataList = new List<ProcessParameterData> { new ProcessParameterData { BackColor = "Green", BackColorForReference = "", chartContainerID = "", ChartType = "Bar", GraphHighLowVisibility = "visible", HeaderName = "X-Axis Load", HighValue = "28", LowValue = "20", MachineID = "GRINDING-01", NextLineVisibility = "visible", Template1Visibility = "visible", Template2Visibility = "hidden", TemplateType = "Text", TrubleshootIconVisibility = "visible", UpdatedTS = "2021-03-06 15:00:00", Value = "24" }, new ProcessParameterData { BackColor = "Red", BackColorForReference = "", chartContainerID = "", ChartType = "Bar", GraphHighLowVisibility = "visible", HeaderName = "Y-Axis Load", HighValue = "28", LowValue = "20", MachineID = "GRINDING-01", NextLineVisibility = "visible", Template1Visibility = "visible", Template2Visibility = "hidden", TemplateType = "Text", TrubleshootIconVisibility = "visible", UpdatedTS = "2021-03-06 15:00:00", Value = "24" }, new ProcessParameterData { BackColor = "Red", BackColorForReference = "", chartContainerID = "", ChartType = "Bar", GraphHighLowVisibility = "visible", HeaderName = "Z-Axis Load", HighValue = "28", LowValue = "20", MachineID = "CNC-01", NextLineVisibility = "visible", Template1Visibility = "visible", Template2Visibility = "hidden", TemplateType = "Text", TrubleshootIconVisibility = "visible", UpdatedTS = "2021-03-06 15:00:00", Value = "24" }, new ProcessParameterData { BackColor = "green", BackColorForReference = "", chartContainerID = "", ChartType = "Bar", GraphHighLowVisibility = "visible", HeaderName = "X-Axis Temp", HighValue = "28", LowValue = "20", MachineID = "CNC-01", NextLineVisibility = "visible", Template1Visibility = "visible", Template2Visibility = "hidden", TemplateType = "Text", TrubleshootIconVisibility = "visible", UpdatedTS = "2021-03-06 15:00:00", Value = "24" }, new ProcessParameterData { BackColor = "green", BackColorForReference = "", chartContainerID = "", ChartType = "Bar", GraphHighLowVisibility = "visible", HeaderName = "Y-Axis Temp", HighValue = "28", LowValue = "20", MachineID = "CNC-01", NextLineVisibility = "visible", Template1Visibility = "visible", Template2Visibility = "hidden", TemplateType = "Text", TrubleshootIconVisibility = "visible", UpdatedTS = "2021-03-06 15:00:00", Value = "24" }, new ProcessParameterData { BackColor = "red", BackColorForReference = "", chartContainerID = "", ChartType = "Bar", GraphHighLowVisibility = "visible", HeaderName = "Z-Axis Temp", HighValue = "28", LowValue = "20", MachineID = "GRINDING-01", NextLineVisibility = "visible", Template1Visibility = "visible", Template2Visibility = "hidden", TemplateType = "Text", TrubleshootIconVisibility = "visible", UpdatedTS = "2021-03-06 15:00:00", Value = "24" } };
            return parametersDataList.AsEnumerable();
        }
    }
}
