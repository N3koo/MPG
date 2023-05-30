using MpgWebService.Presentation.Request;

using System.Collections.Generic;
using System.Threading.Tasks;
using MpgWebService.Business.Data.DTO;

namespace MpgWebService.Repository.Interface {

    public interface IReportRepository {

        Task<List<ReportCommand>> GetReport(Period period);

        Task<IList<ReportMaterial>> GetMaterialsForCommand(string POID);

        Task<IList<ReportMaterial>> GetMaterialsForPail(string POID, int pail);
    }
}
