using MpgWebService.Presentation.Request;
using MpgWebService.Properties;

using DataEntity.Model.Output;
using DataEntity.Model.Types;

using NHibernate;

using System.Collections.Generic;
using System.Linq;
using System;

namespace MpgWebService.Business.Data.Extension {
    public static class InputDataExtension {
        public static List<ProductionOrderPailStatus> CreatePails(this InputData data, ISession session, StartCommand command) {
            List<ProductionOrderPailStatus> pails = new();
            DateTime now = DateTime.Now;

            int size = (int)data.Order.PlannedQtyBUC;
            data.Order.Status = Resources.CMD_STARTED;
            data.Order.Priority = command.Priority.ToString();

            Enumerable.Range(1, size).ToList().ForEach(index => {
                var pail = new ProductionOrderPailStatus {
                    CreationDate = now,
                    PailNumber = $"{index}",
                    POID = data.Order.POID,
                    PailStatus = Resources.CMD_SEND,
                    NetWeight = data.OrderFinalItem[0].ItemQty / data.Order.PlannedQtyBUC,
                    GrossWeight = 0,
                    QC = command.QC[index - 1],
                    Timeout = Resources.MAXIMUM_DOSAGE_TIME,
                    StartDate = now,
                    EndDate = now,
                    MPGRowUpdated = now
                };

                pails.Add(pail);

                session.Save(pail);
            });

            return pails;
        }
    }
}
