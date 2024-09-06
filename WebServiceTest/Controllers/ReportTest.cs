using Microsoft.AspNetCore.Mvc;
using Moq;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Controllers;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Report;
using MpgWebService.Presentation.Response.Wrapper;

namespace WebServiceTest.Controllers {
    public class ReportTest {

        private readonly Mock<IReportService> _service;

        private readonly ReportController _controller;

        public ReportTest() {
            _service = new Mock<IReportService>();
            _controller = new ReportController(_service.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _service.Setup(x => x.GetReport(It.IsAny<Period>())).Returns(() => {
                var data = new List<ReportCommandDto>();

                return Task.FromResult(ServiceResponse<IList<ReportCommandDto>>.Ok(data));
            });

            _service.Setup(x => x.GetMaterialsForCommand(It.IsAny<string>())).Returns(() => {
                var data = new List<ReportMaterialDto>();

                return Task.FromResult(ServiceResponse<IList<ReportMaterialDto>>.Ok(data));
            });

            _service.Setup(x => x.GetMaterialsForPail(It.IsAny<string>(), It.IsAny<int>())).Returns(() => {
                var data = new List<ReportMaterialDto>();

                return Task.FromResult(ServiceResponse<IList<ReportMaterialDto>>.Ok(data));
            });
        }

        [Fact]
        public async void Should_Get_Report() {
            var parameter = new Period { };
            var result = await _controller.GetReport(parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<IList<ReportCommandDto>>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal(0, data.Data.Count);
        }

        [Fact]
        public async void Should_Get_Command_Materials() {
            var parameter = String.Empty;
            var result = await _controller.GetCommandMaterials(parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<IList<ReportMaterialDto>>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal(0, data.Data.Count);
        }

        [Fact]
        public async void Should_Get_Pail_Materials() {
            var parameter = String.Empty;
            var result = await _controller.GetPailMaterials(parameter, 0) as OkObjectResult;
            var data = result?.Value as ServiceResponse<IList<ReportMaterialDto>>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal(0, data.Data.Count);
        }
    }
}
