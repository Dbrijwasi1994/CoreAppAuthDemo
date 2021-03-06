using CoreAppAuthDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Repositories
{
    public interface IMasterDataServiceRepo
    {
        Task<IEnumerable<PumpMasterData>> GetPumpMasterData();
        Task<IEnumerable<Employee>> GetEmployees();
        Task<bool> InsertPumpMasterData(PumpMasterData data);
        IEnumerable<ProcessParametersModel> GetProcessParameterDetails();
        IEnumerable<ProcessParameterData> GetProcessParamData();
    }
}
