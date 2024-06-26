﻿using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Response.Report;

using DataEntity.Model.Output;
using DataEntity.Model.Input;

using System.Globalization;

namespace MpgWebService.Data.Extension {

    public static class PailStatusExtensions {

        public static ProductionDto AsProductionDto(this ProductionOrderPailStatus pail) {
            return new ProductionDto {
                POID = $"{pail.POID}_{pail.PailNumber}",
                POID_ID = pail.POID,
                Quantity = pail.NetWeight,
                BonPredare = pail.Ticket,
                Consumption = pail.Consumption,
                Status = pail.PailStatus,
                Unit = "KG",
                Date = pail.StartDate.ToString(CultureInfo.InvariantCulture)
            };
        }

        public static ReportCommandDto AsReportDto(this ProductionOrderPailStatus pail, ProductionOrder order) {
            return new ReportCommandDto {
                POID = $"{pail.POID}_{pail.PailNumber}",
                POID_ID = pail.POID,
                Quantity = pail.GrossWeight,
                EndDate = order.PlannedEndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                StartDate = order.PlannedStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                ExecuteDate = pail.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Status = pail.PailStatus,
                QC = pail.QC,
                UOM = "KG",
            };
        }
    }
}
