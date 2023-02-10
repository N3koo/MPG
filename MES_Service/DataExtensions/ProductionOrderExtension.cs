using DataEntity.Model.Input;

using MES_Service.DTO;

namespace MES_Service.DataExtensions {

    public static class ProductionOrderExtension {

        public static ProductionOrderDto AsDto(this ProductionOrder po) {
            return new ProductionOrderDto {
                PODescription = po.PODescription,
                POID = po.POID,
                MaterialID = po.MaterialID,
                PlantID = po.PlantID,
                Status = po.Status,
                PlannedQtyBUC = po.PlannedQtyBUC,
                PlannedQtyBUCUom = po.PlannedQtyBUCUom,
                KoberLot = po.KoberLot,
                ProfitCenter = po.ProfitCenter,
                Priority = po.Priority,
                PlannedStartDate = po.PlannedStartDate,
                PlannedStartHour = po.PlannedStartHour,
                PlannedEndDate = po.PlannedEndDate,
                PlannedEndHour = po.PlannedEndHour
            };
        }
    }
}
