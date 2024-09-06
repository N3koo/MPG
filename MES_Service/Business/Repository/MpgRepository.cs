using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Clients;
using MpgWebService.Repository.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Command {

    public class MpgRepository : IMpgRepository {

        private readonly MpgClient mpg;

        private readonly MesClient mes;

        public MpgRepository(MpgClient mpg, MesClient mes) {
            this.mpg = mpg;
            this.mes = mes;
        }

        public async Task<ServiceResponse<bool>> ChangeStatus(string POID, string indexPail, string status) {
            var mpgResponse = await mpg.ChangeStatus(POID, indexPail, status);
            var mesReponse = await mes.ChangeStatus(POID, indexPail, status);

            return ServiceResponse<bool>.CombineResponses(mpgResponse, mesReponse);
        }

        public async Task<ServiceResponse<bool>> SaveCorrection(POConsumption materials) {
            var mesResponse = await mes.SaveCorrection(materials);
            var mpgResponse = await mpg.SaveCorrection(materials, mesResponse.Data);

            return mpgResponse;
        }

        public async Task<ServiceResponse<bool>> SaveDosageMaterials(POConsumption materials) {
            var mpgResponse = await mpg.SaveDosageMaterials(materials);
            var mesResponse = await mes.SaveDosageMaterials(mpgResponse.Data);

            return mesResponse;
        }

        public async Task<ServiceResponse<QcLabelDto>> GetQcLabel(string POID, int pailNumber) {
            var mesResult = await mes.SetQcStatus(POID, pailNumber);
            var mpgResult = await mpg.SetQC(mesResult.Data);

            if (mpgResult.Errors.Count > 0) {
                mesResult.AddErros(mpgResult.Errors);
            }

            return mesResult;
        }

        public async Task<ServiceResponse<IList<CoefficientDto>>> GetCoefficients() {
            var data = await mes.GetCoefficients();
            var dtos = data.Data.Select(p => CoefficientDto.FromStockVessel(p)).ToList();

            return ServiceResponse<IList<CoefficientDto>>.Ok(dtos);
        }

        public async Task<ServiceResponse<PailQCDto>> GetQCPail() =>
           await mpg.GetFirstPail();

        public async Task<ServiceResponse<PailDto>> GetAvailablePail(string POID) =>
            await mpg.GetAvailablePail(POID);

        public async Task<ServiceResponse<IList<MaterialDto>>> GetCorrections(string POID, int pailNumber, string opNo) =>
            await mes.GetCorrections(POID, pailNumber, opNo);

        public async Task<ServiceResponse<LabelDto>> GetLabel(string POID) =>
            await mpg.GetLabelData(POID);

        public async Task<ServiceResponse<IList<MaterialDto>>> GetMaterials(string POID) =>
            await mpg.GetMaterials(POID);

        public async Task<ServiceResponse<bool>> UpdateReserveQuantities(ReserveTank[] quatities) =>
            await mes.ReserveQuantities(quatities);
    }
}
