using Microsoft.AspNetCore.Mvc;
using Moq;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Data.Extension;
using MpgWebService.Presentation.Controllers;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Command;
using MpgWebService.Presentation.Response.Wrapper;
using WebServiceTest.Data;

namespace WebServiceTest.Controllers {

    public class CommandTest {

        private readonly Mock<ICommandService> _serivce;
        private readonly CommandController _controller;

        public CommandTest() {
            _serivce = new Mock<ICommandService>();
            _controller = new CommandController(_serivce.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _serivce.Setup(x => x.GetCommands(It.IsAny<Period>())).Returns(() => {
                var data = new ProductionOrderDto() { };
                var list = new List<ProductionOrderDto>() { data };

                return Task.FromResult(ServiceResponse<IList<ProductionOrderDto>>.Ok(list));
            });

            _serivce.Setup(x => x.GetCommand(It.IsAny<string>())).Returns(() => {
                var data = DataCreator.CreateOrder().AsDto();

                return Task.FromResult(ServiceResponse<ProductionOrderDto>.Ok(data));
            });

            _serivce.Setup(x => x.StartCommand(It.IsAny<StartCommand>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _serivce.Setup(x => x.GetQC(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<string>.Ok("1;2;3;4"));
            });

            _serivce.Setup(x => x.BlockCommand(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _serivce.Setup(x => x.CheckPriority(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _serivce.Setup(x => x.CloseCommand(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _serivce.Setup(x => x.StartPartialProduction(It.IsAny<string>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });

            _serivce.Setup(x => x.DownloadMaterials()).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });
        }

        [Fact]
        public async void Should_Get_Commands() {
            var period = DataCreator.CreatePeriod();
            var result = await _controller.GetCommands(period) as OkObjectResult;
            var data = result?.Value as ServiceResponse<IList<ProductionOrderDto>>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.NotNull(data.Data);
            Assert.Equal(1, data.Data.Count);
        }

        [Fact]
        public async void Should_Get_A_Command() {
            var result = await _controller.GetCommand(String.Empty) as OkObjectResult;
            var data = result?.Value as ServiceResponse<ProductionOrderDto>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.NotNull(data.Data);
        }

        [Fact]
        public async void Should_Start_Command() {
            var start = DataCreator.CreateStartCommand();
            var result = await _controller.StartCommand(start) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }

        [Fact]
        public async void Should_Get_QC() {
            var start = String.Empty;
            var result = await _controller.GetQC(start) as OkObjectResult;
            var data = result?.Value as ServiceResponse<string>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal("1;2;3;4", data.Data);
        }

        [Fact]
        public async void Should_Check_Priority() {
            var start = String.Empty;
            var result = await _controller.CheckPriority(start) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }

        [Fact]
        public async void Should_Block_Command() {
            var start = String.Empty;
            var result = await _controller.BlockCommand(start) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }

        [Fact]
        public async void Should_Close_Command() {
            var start = String.Empty;
            var result = await _controller.CloseCommand(start) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }

        [Fact]
        public async void Should_Start_Partial_Production() {
            var start = String.Empty;
            var result = await _controller.StartPartialProduction(start) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }

        [Fact]
        public async void Should_Download_Materials() {
            var result = await _controller.DownloadMaterials() as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }
    }
}
