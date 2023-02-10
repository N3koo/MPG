using DataEntity.Model.Input;
using MES_Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MES_Service.Repository.Command {

    public class SapCommandRepository : ICommandRepository {
        public ActionResult<bool> BlockCommand(string POID) {
            throw new NotImplementedException();
        }

        public bool CheckPriority(string POID, string Priority) {
            throw new NotImplementedException();
        }

        public bool CheckPriority(string Priority) {
            throw new NotImplementedException();
        }

        public ActionResult<bool> CloseCommand(string POID) {
            throw new NotImplementedException();
        }

        public ActionResult<bool> DownloadMaterials() {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductionOrder> GetCommands(DateTime startDate, DateTime endDate) {
            throw new NotImplementedException();
        }

        public ProductionOrder GetCommands(string POID) {
            throw new NotImplementedException();
        }

        public string GetQC(string POID) {
            throw new NotImplementedException();
        }

        public ActionResult<bool> PartialProduction(string pOID) {
            throw new NotImplementedException();
        }

        public void StartCommand(string POID) {
            throw new NotImplementedException();
        }
    }
}
