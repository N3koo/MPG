using Microsoft.AspNetCore.Mvc;
using Moq;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Controllers;
using MpgWebService.Presentation.Request.Settings;
using MpgWebService.Presentation.Response.Wrapper;

namespace WebServiceTest.Controllers {
    public class SettingsTest {

        private readonly Mock<ISettingsService> _service;

        private readonly SettingsController _controller;

        public SettingsTest() {
            _service = new Mock<ISettingsService>();
            _controller = new SettingsController(_service.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _service.Setup(x => x.GetSettings()).Returns(() => {
                var data = new List<SettingsElement>();

                return Task.FromResult(ServiceResponse<IList<SettingsElement>>.Ok(data));
            });

            _service.Setup(x => x.SetSettings(It.IsAny<List<SettingsElement>>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });
        }

        [Fact]
        public async void Should_Get_Settings() {
            var result = await _controller.GetSettings() as OkObjectResult;
            var data = result?.Value as ServiceResponse<IList<SettingsElement>>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal(0, data.Data.Count);
        }

        [Fact]
        public async void Should_Set_Settings() {
            var parameter = new List<SettingsElement>();
            var result = await _controller.SetSettings(parameter) as OkObjectResult;
            var data = result?.Value as ServiceResponse<bool>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.True(data.Data);
        }

    }
}
