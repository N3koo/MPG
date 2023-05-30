using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using MpgWebService.Business.Data.DTO;

namespace MpgWebService.Business.Service {
    public class ReportService : IReportService {
        private readonly IReportRepository repository;

        public ReportService() {
            repository = new ReportRepository();
        }

        public async Task<IEnumerable<ReportMaterial>> GetMaterialsForCommand(string POID) {
            return await repository.GetMaterialsForCommand(POID);
        }

        public async Task<IEnumerable<ReportMaterial>> GetMaterialsForPail(string POID, int pail) {
            return await repository.GetMaterialsForPail(POID, pail);
        }

        public async Task<IEnumerable<ReportCommand>> GetReport(Period period) {
            return await repository.GetReport(period);
        }
    }
}
