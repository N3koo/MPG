using DataEntity.Model.Input;

using System.Collections.Generic;
using System;

using Microsoft.AspNetCore.Mvc;

namespace MES_Service.Interface {
    public interface ICommandRepository {

        IEnumerable<ProductionOrder> GetCommands(DateTime startDate, DateTime endDate);

        ProductionOrder GetCommands(string POID);

        void StartCommand(string POID);

        bool CheckPriority(string Priority);

        //void SetPailStatus(string POID, int Pail, string Status);

        /*void GetCorrections(string POID, int Pail);

        void UploadCorrections();/**/

        string GetQC(string POID);

        ActionResult<bool> BlockCommand(string POID);

        ActionResult<bool> CloseCommand(string POID);

        ActionResult<bool> PartialProduction(string POID);

        ActionResult<bool> DownloadMaterials();
    }
}
