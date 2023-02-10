using MES_Service.DTO;
using System;
using System.Collections.Generic;

namespace MES_Service.Interface {

    public interface IReportRepository {

        IEnumerable<ReportCommandDto> GetReport(DateTime start, DateTime end);

        IEnumerable<ReportMaterialDto> GetMaterialsForCommand(string POID);

        IEnumerable<ReportMaterialDto> GetMaterialsForPail(string POID, int pail);
    }
}
