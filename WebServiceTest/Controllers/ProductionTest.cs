using Microsoft.AspNetCore.Mvc;
using Moq;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Controllers;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Response.Wrapper;

namespace WebServiceTest.Controllers {
    public class ProductionTest {

        private readonly Mock<IProductionService> _service;
        private readonly ProductionController _controller;

        public ProductionTest() {
            _service = new Mock<IProductionService>();
            _controller = new ProductionController(_service.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _service.Setup(x => x.GetProductionStatus(It.IsAny<Period>())).Returns(() => {
                var data = new List<ProductionDto>();

                return Task.FromResult(ServiceResponse<IList<ProductionDto>>.Ok(data));
            });
        }

        [Fact]
        public async void Should_Get_Production_Status() {
            var period = new Period { };
            var result = await _controller.GetResult(period) as OkObjectResult;
            var data = result?.Value as ServiceResponse<IList<ProductionDto>>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal(0, data.Data.Count);
        }
    }
}
