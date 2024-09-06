using Moq;
using MpgWebService.Business.Service;
using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;

namespace WebServiceTest.Service {
    public class MpgServiceTest {

        public readonly Mock<IMpgRepository> _repository;
        public readonly MpgService _service;

        public MpgServiceTest() {
            _repository = new Mock<IMpgRepository>();
            _service = new MpgService(_repository.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _repository.Setup(x => x.GetAvailablePail(It.IsAny<string>())).Returns(() => {
                var data = new PailDto { };
                return Task.FromResult(ServiceResponse<PailDto>.Ok(data));
            });

            _repository.Setup(x => x.GetQCPail()).Returns(() => {
                var data = new PailQCDto { };
                return Task.FromResult(ServiceResponse<PailQCDto>.Ok(data));
            });

            _repository.Setup(x => x.GetLabel(It.IsAny<string>())).Returns(() => {
                var data = new LabelDto { };
                return Task.FromResult(ServiceResponse<LabelDto>.Ok(data));
            });

            _repository.Setup(x => x.GetMaterials(It.IsAny<string>())).Returns(() => {
                var data = new List<MaterialDto>();
                return Task.FromResult(ServiceResponse<IList<MaterialDto>>.Ok(data));
            });

            _repository.Setup(x => x.GetQcLabel(It.IsAny<string>(), It.IsAny<int>())).Returns(() => {
                var data = new QcLabelDto { };
                return Task.FromResult(ServiceResponse<QcLabelDto>.Ok(data));
            });

            _repository.Setup(x => x.GetCorrections(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(() => {
                var data = new List<MaterialDto> { };
                return Task.FromResult(ServiceResponse<IList<MaterialDto>>.Ok(data));
            });

            _repository.Setup(x => x.SaveCorrection(It.IsAny<POConsumption>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _repository.Setup(x => x.SaveDosageMaterials(It.IsAny<POConsumption>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _repository.Setup(x => x.ChangeStatus(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _repository.Setup(x => x.GetCoefficients()).Returns(() => {
                var data = new List<CoefficientDto> { };
                return Task.FromResult(ServiceResponse<IList<CoefficientDto>>.Ok(data));
            });

            _repository.Setup(x => x.UpdateReserveQuantities(It.IsAny<ReserveTank[]>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });
        }

        [Fact]
        public async void Should_Get_Pail() {
            var parameter = string.Empty;
            var result = await _service.GetAvailablePail(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async void Should_Get_Qc_Pail() {
            var result = await _service.GetQCPail();

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async void Should_Get_Label() {
            var parameter = string.Empty;
            var result = await _service.GetLabel(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async void Should_Get_Materials() {
            var parameter = string.Empty;
            var result = await _service.GetMaterials(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Count);
        }

        [Fact]
        public async void Should_Get_Qc_Label() {
            var parameter = string.Empty;
            var result = await _service.GetQcLabel(parameter, 0);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async void Should_Get_Corrections() {
            var parameter = string.Empty;
            var result = await _service.GetCorrections(parameter, 0, parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Count);
        }

        [Fact]
        public async void Should_Save_Correction() {
            var parameter = new POConsumption { };
            var result = await _service.SaveCorrection(parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Save_Dosage_Materials() {
            var parameter = new POConsumption { };
            var result = await _service.SaveDosageMaterials(parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Change_Status() {
            var parameter = string.Empty;
            var result = await _service.ChangeStatus(parameter, parameter, parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Get_Coefficients() {
            var result = await _service.GetCoefficients();

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Count);
        }

        [Fact]
        public async void Should_Update_Reserve_Quantities() {
            var parameter = Array.Empty<ReserveTank>();
            var result = await _service.UpdateReserveQuantities(parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }
    }
}
