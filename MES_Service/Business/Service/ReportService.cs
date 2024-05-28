using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Report;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace MpgWebService.Business.Service {

    public class ReportService : IReportService {

        private readonly IReportRepository repository;

        public ReportService() {
            repository = new ReportRepository();
        }

        public async Task<IEnumerable<ReportMaterialDto>> GetMaterialsForCommand(string POID) =>
            await repository.GetMaterialsForCommand(POID);

        public async Task<IEnumerable<ReportMaterialDto>> GetMaterialsForPail(string POID, int pail) =>
            await repository.GetMaterialsForPail(POID, pail);

        public async Task<IEnumerable<ReportCommandDto>> GetReport(Period period) =>
            await repository.GetReport(period);

    }
}
