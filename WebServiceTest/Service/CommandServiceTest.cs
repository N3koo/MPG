using DataEntity.Model.Input;
using Moq;
using MpgWebService.Business.Interface.Settings;
using MpgWebService.Business.Service;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;

namespace WebServiceTest.Service {

    public class CommandServiceTest {

        private readonly Mock<ICommandRepository> _repository;
        private readonly Mock<ISettings> _settings;
        private readonly CommandService _service;

        public CommandServiceTest() {
            _settings = new Mock<ISettings>();
            _repository = new Mock<ICommandRepository>();
            _service = new CommandService(_repository.Object, _settings.Object);

            CreateSetup();
        }

        private void CreateSetup() {


            _repository.Setup(x => x.DownloadMaterials()).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _repository.Setup(x => x.UpdateMaterials()).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _repository.Setup(x => x.GetCommand(It.IsAny<string?>())).Returns((string POID) => {
                var data = new ProductionOrder { };
                var resp = $"Nu exista comanda {POID}";

                return POID == null ? Task.FromResult(ServiceResponse<ProductionOrder>.NotFound(resp)) :
                                       Task.FromResult(ServiceResponse<ProductionOrder>.Ok(data));
            });

            _repository.Setup(x => x.GetCommands(It.IsAny<Period?>())).Returns((Period period) => {
                var data = new List<ProductionOrder> { };
                var resp = "Nu exista comenzi in perioada cautata";

                return period == null ? Task.FromResult(ServiceResponse<IList<ProductionOrder>>.NotFound(resp)) :
                                        Task.FromResult(ServiceResponse<IList<ProductionOrder>>.Ok(data));
            });

            _repository.Setup(x => x.BlockCommand(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _repository.Setup(x => x.CheckPriority(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _repository.Setup(x => x.CloseCommand(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _repository.Setup(x => x.GetQC(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<string>.Ok("1;2;3;4"));
            });

            _repository.Setup(x => x.StartCommand(It.IsAny<StartCommand>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _repository.Setup(x => x.PartialProduction(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });
        }

        [Fact]
        public async void Should_Download_Data() {
            var result = await _service.DownloadMaterials();

            _repository.Verify(x => x.DownloadMaterials(), Times.Once());
            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Update_Data() {
            _settings.SetupGet(x => x.Update).Returns(() => DateTime.Now.ToString());
            var result = await _service.DownloadMaterials();

            _repository.Verify(x => x.UpdateMaterials(), Times.Once());
            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Get_Command() {
            var parameter = string.Empty;
            var result = await _service.GetCommand(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async void Should_Not_Get_Command() {
            var result = await _service.GetCommand(null);
            var message = $"Nu exista comanda {null}";

            Assert.NotNull(result);
            Assert.NotNull(result.Errors);
            Assert.Single(result.Errors);
            Assert.Equal(result.Errors.First().Message, message);
        }

        [Fact]
        public async void Should_Get_Commands() {
            var parameter = new Period { };
            var result = await _service.GetCommands(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Count);
        }

        [Fact]
        public async void Should_Not_Get_Commands() {
            var result = await _service.GetCommands(null);

            Assert.NotNull(result);
            Assert.NotNull(result.Errors);
            Assert.Single(result.Errors);
            Assert.Equal("Nu exista comenzi in perioada cautata", result.Errors.First().Message);
        }

        [Fact]
        public async void Should_Block_Command() {
            var parameter = string.Empty;
            var result = await _service.BlockCommand(parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Check_Priority() {
            var parameter = string.Empty;
            var result = await _service.CheckPriority(parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Close_Command() {
            var parameter = string.Empty;
            var result = await _service.CloseCommand(parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Get_QC() {
            var parameter = string.Empty;
            var result = await _service.GetQC(parameter);

            Assert.NotNull(result);
            Assert.Equal("1;2;3;4", result.Data);
        }

        [Fact]
        public async void Should_Start_Command() {
            var parameter = new StartCommand { };
            var result = await _service.StartCommand(parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }

        [Fact]
        public async void Should_Start_Partial_Production() {
            var parameter = string.Empty;
            var result = await _service.StartPartialProduction(parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }
    }
}
