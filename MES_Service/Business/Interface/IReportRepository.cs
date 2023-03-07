using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MpgWebService.Repository.Interface {

    public interface IReportRepository {

        Task<List<ReportCommand>> GetReport(DateTime start, DateTime end);

        Task<IList<ReportMaterial>> GetMaterialsForCommand(string POID);

        Task<IList<ReportMaterial>> GetMaterialsForPail(string POID, int pail);
    }
}
