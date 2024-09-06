using Moq;
using MpgWebService.Business.Service;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Report;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;

namespace WebServiceTest.Service {
    public class ReportServiceTest {

        private readonly Mock<IReportRepository> _repository;
        private readonly ReportService _service;

        public ReportServiceTest() {
            _repository = new Mock<IReportRepository>();
            _service = new ReportService(_repository.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _repository.Setup(x => x.GetReport(It.IsAny<Period>())).Returns(() => {
                var data = new List<ReportCommandDto> { };
                return Task.FromResult(ServiceResponse<IList<ReportCommandDto>>.Ok(data));
            });

            _repository.Setup(x => x.GetMaterialsForPail(It.IsAny<string>(), It.IsAny<int>())).Returns(() => {
                var data = new List<ReportMaterialDto> { };
                return Task.FromResult(ServiceResponse<IList<ReportMaterialDto>>.Ok(data));
            });

            _repository.Setup(x => x.GetMaterialsForCommand(It.IsAny<string>())).Returns(() => {
                var data = new List<ReportMaterialDto> { };
                return Task.FromResult(ServiceResponse<IList<ReportMaterialDto>>.Ok(data));
            });
        }

        [Fact]
        public async void Should_Get_Materials_For_Command() {
            var parameter = string.Empty;
            var result = await _service.GetMaterialsForCommand(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Count);
        }

        [Fact]
        public async void Should_Get_Materials_For_Pail() {
            var parameter = string.Empty;
            var result = await _service.GetMaterialsForPail(parameter, 0);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Count);
        }

        [Fact]
        public async void Should_Get_Report() {
            var parameter = new Period { };
            var result = await _service.GetReport(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Count);
        }
    }
}
