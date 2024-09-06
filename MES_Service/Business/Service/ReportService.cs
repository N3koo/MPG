using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Report;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MpgWebService.Business.Service {

    public class ReportService : IReportService {

        private readonly IReportRepository repository;

        public ReportService(IReportRepository repository) {
            this.repository = repository;
        }

        public async Task<ServiceResponse<IList<ReportMaterialDto>>> GetMaterialsForCommand(string POID) =>
            await repository.GetMaterialsForCommand(POID);

        public async Task<ServiceResponse<IList<ReportMaterialDto>>> GetMaterialsForPail(string POID, int pail) =>
            await repository.GetMaterialsForPail(POID, pail);

        public async Task<ServiceResponse<IList<ReportCommandDto>>> GetReport(Period period) =>
            await repository.GetReport(period);

    }
}
