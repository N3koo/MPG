using Moq;
using MpgWebService.Business.Service;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;

namespace WebServiceTest.Service {
    public class ProductionServiceTest {

        public readonly Mock<IProductionRepository> _repository;
        public readonly ProductionService _service;

        public ProductionServiceTest() {
            _repository = new Mock<IProductionRepository>();
            _service = new ProductionService(_repository.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _repository.Setup(x => x.CheckProductionStatus(It.IsAny<Period>())).Returns(() => {
                var data = new List<ProductionDto> { };
                return Task.FromResult(ServiceResponse<IList<ProductionDto>>.Ok(data));
            });
        }

        [Fact]
        public async void Should_Get_Production() {
            var parameter = new Period { };
            var result = await _service.GetProductionStatus(parameter);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Count);
        }
    }
}
