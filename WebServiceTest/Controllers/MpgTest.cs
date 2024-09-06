using Microsoft.AspNetCore.Mvc;
using Moq;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Controllers;
using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Response.Wrapper;
using WebServiceTest.Data;

namespace WebServiceTest.Controllers {

    public class MpgTest {
        private readonly Mock<IMpgService> _service;
        private readonly MpgController _controller;

        public MpgTest() {
            _service = new Mock<IMpgService>();
            _controller = new MpgController(_service.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _service.Setup(x => x.GetQCPail()).Returns(() => {
                var result = DataCreator.CreateQcDtO();

                return Task.FromResult(ServiceResponse<PailQCDto>.Ok(result));
            });

            _service.Setup(x => x.GetAvailablePail(It.IsAny<string>())).Returns(() => {
                var result = DataCreator.CreatePailDto();

                return Task.FromResult(ServiceResponse<PailDto>.Ok(result));
            });

            _service.Setup(x => x.GetMaterials(It.IsAny<string>())).Returns(() => {
                var result = DataCreator.CreateListOfMaterials();

                return Task.FromResult(ServiceResponse<IList<MaterialDto>>.Ok(result));
            });

            _service.Setup(x => x.GetLabel(It.IsAny<string>())).Returns(() => {
                var result = DataCreator.CreateLabel();

                return Task.FromResult(ServiceResponse<LabelDto>.Ok(result));
            });

            _service.Setup(x => x.GetQcLabel(It.IsAny<string>(), It.IsAny<int>())).Returns(() => {
                var result = DataCreator.GetQcLabel();

                return Task.FromResult(ServiceResponse<QcLabelDto>.Ok(result));
            });

            _service.Setup(x => x.GetCorrections(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(() => {
                var result = DataCreator.CreateListOfMaterials();

                return Task.FromResult(ServiceResponse<IList<MaterialDto>>.Ok(result));
            });

            _service.Setup(x => x.SaveDosageMaterials(It.IsAny<POConsumption>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _service.Setup(x => x.SaveCorrection(It.IsAny<POConsumption>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _service.Setup(x => x.ChangeStatus(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _service.Setup(x => x.GetCoefficients()).Returns(() => {
                var data = new List<CoefficientDto> { };

                return Task.FromResult(ServiceResponse<IList<CoefficientDto>>.Ok(data));
            });

            _service.Setup(x => x.UpdateReserveQuantities(It.IsAny<ReserveTank[]>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });
        }

        [Fact]
        public async void Should_Get_QC_Pail() {
            var result = await _controller.GetQCPail() as OkObjectResult;
            var data = result?.Value as ServiceResponse<PailQCDto>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.NotNull(data.Data);
        }

        [Fact]
        public async void Should_Get_Pail() {
            var parameter = String.Empty;
            var result = await _controller.GetPail(parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<PailDto>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.NotNull(data.Data);
        }

        [Fact]
        public async void Should_Get_Label() {
            var start = String.Empty;
            var response = await _controller.GetLabel(start) as OkObjectResult;
            var data = response?.Value as ServiceResponse<LabelDto>;

            Assert.NotNull(response);
            Assert.NotNull(data);
            Assert.NotNull(data.Data);
        }

        [Fact]
        public async void Should_Get_Materials() {
            var parameter = String.Empty;
            var result = await _controller.GetMaterials(parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<IList<MaterialDto>>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal(0, data.Data.Count);
        }

        [Fact]
        public async void Should_Get_Qc_Label() {
            var paramter = new QcDetails { };
            var result = await _controller.SetQCStatus(paramter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<QcLabelDto>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.NotNull(data.Data);
        }

        [Fact]
        public async void Should_Get_Correction() {
            var parameter = new QcDetails { };
            var result = await _controller.GetCorrection(parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<IList<MaterialDto>>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal(0, data.Data.Count);
        }

        [Fact]
        public async void Should_Save_Dosage_Materials() {
            var parameter = new POConsumption { };
            var result = await _controller.SaveDosageMaterials(parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }

        [Fact]
        public async void Should_Save_Correction_Materials() {
            var parameter = new POConsumption { };
            var result = await _controller.SaveCorrectionMaterials(parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }

        [Fact]
        public async void Should_Change_Status() {
            var parameter = String.Empty;
            var result = await _controller.SetPailStatus(parameter, parameter, parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }

        [Fact]
        public async void Should_Get_Coefficients() {
            var result = await _controller.GetHeadsCoefficients() as OkObjectResult;
            var data = result?.Value as ServiceResponse<IList<CoefficientDto>>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal(0, data.Data.Count);
        }

        [Fact]
        public async void Should_Save_Reserved_Quantities() {
            var parameter = Array.Empty<ReserveTank>();
            var result = await _controller.SetReservedQuantity(parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }
    }
}
