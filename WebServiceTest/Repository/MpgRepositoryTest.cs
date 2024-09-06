using DataEntity.Model.Input;
using DataEntity.Model.Output;
using Moq;
using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Clients;
using MpgWebService.Repository.Command;

namespace WebServiceTest.Repository {
    public class MpgRepositoryTest {

        private readonly Mock<MpgClient> _mpg;

        private readonly Mock<MesClient> _mes;

        private readonly MpgRepository repository;

        public MpgRepositoryTest() {
            _mpg = new Mock<MpgClient>();
            _mes = new Mock<MesClient>();

            repository = new MpgRepository(_mpg.Object, _mes.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _mpg.Setup(x => x.ChangeStatus(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string POID, string pail, string status) => {
                    return POID switch {
                        "Ok" => Task.FromResult(ServiceResponse<bool>.Ok(true)),
                        "False" => Task.FromResult(ServiceResponse<bool>.Ok(false)),
                        _ => Task.FromResult(ServiceResponse<bool>.Ok(false)),
                    };
                });

            _mes.Setup(x => x.ChangeStatus(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).
                Returns((string POID, string pail, string status) => {
                    return POID switch {
                        "Ok" => Task.FromResult(ServiceResponse<bool>.Ok(true)),
                        "False" => Task.FromResult(ServiceResponse<bool>.Ok(false)),
                        _ => Task.FromResult(ServiceResponse<bool>.Ok(false))
                    };
                });

            _mes.Setup(x => x.SaveCorrection(It.IsAny<POConsumption>())).Returns(() => {
                var data = new ProductionOrderCorection { };
                return Task.FromResult(ServiceResponse<ProductionOrderCorection>.Ok(data));
            });

            _mpg.Setup(x => x.SaveCorrection(It.IsAny<POConsumption>(), It.IsAny<ProductionOrderCorection>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _mpg.Setup(x => x.SaveDosageMaterials(It.IsAny<POConsumption>())).Returns(() => {
                var data = new List<ProductionOrderConsumption> { };
                return Task.FromResult(ServiceResponse<IList<ProductionOrderConsumption>>.Ok(data));
            });

            _mes.Setup(x => x.SaveDosageMaterials(It.IsAny<IList<ProductionOrderConsumption>>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _mes.Setup(x => x.SetQcStatus(It.IsAny<string>(), It.IsAny<int>())).Returns(() => {
                var data = new QcLabelDto { };
                return Task.FromResult(ServiceResponse<QcLabelDto>.Ok(data));
            });

            _mpg.Setup(x => x.SetQC(It.IsAny<QcLabelDto>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _mes.Setup(x => x.GetCoefficients()).Returns(() => {
                var data = new List<StockVessel> { };
                return Task.FromResult(ServiceResponse<IList<StockVessel>>.Ok(data));
            });

            _mpg.Setup(x => x.GetFirstPail()).Returns(() => {
                var data = new PailQCDto { };
                return Task.FromResult(ServiceResponse<PailQCDto>.Ok(data));
            });

            _mpg.Setup(x => x.GetAvailablePail(It.IsAny<string>())).Returns(() => {
                var data = new PailDto { };
                return Task.FromResult(ServiceResponse<PailDto>.Ok(data));
            });

            _mes.Setup(x => x.GetCorrections(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(() => {
                var data = new List<MaterialDto> { };
                return Task.FromResult(ServiceResponse<IList<MaterialDto>>.Ok(data));
            });

            _mpg.Setup(x => x.GetLabelData(It.IsAny<string>())).Returns(() => {
                var data = new LabelDto { };
                return Task.FromResult(ServiceResponse<LabelDto>.Ok(data));
            });

            _mpg.Setup(x => x.GetMaterials(It.IsAny<string>())).Returns(() => {
                var data = new List<MaterialDto> { };
                return Task.FromResult(ServiceResponse<IList<MaterialDto>>.Ok(data));
            });

            _mes.Setup(x => x.ReserveQuantities(It.IsAny<ReserveTank[]>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });
        }

        [Fact]
        public async void Should_Change_Status() {
            var parameter = string.Empty;
            var result = await repository.ChangeStatus("Ok", parameter, parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Not_Change_Status() {
            var parameter = string.Empty;
            var result = await repository.ChangeStatus("False", parameter, parameter);

            Assert.NotNull(result);
            Assert.False(result.Data);
            Assert.Equal(1, result?.Errors.Count);
        }



    }
}
