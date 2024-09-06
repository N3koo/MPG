using Moq;
using MpgWebService.Business.Service;
using MpgWebService.Presentation.Request.Settings;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;

namespace WebServiceTest.Service {
    public class SettingsServiceTest {

        private readonly Mock<ISettingsRepository> _repository;

        private readonly SettingsService _service;

        public SettingsServiceTest() {
            _repository = new Mock<ISettingsRepository>();
            _service = new SettingsService(_repository.Object);

            CreateSetup();
        }

        private void CreateSetup() {
            _repository.Setup(x => x.GetSettings()).Returns(() => {
                var data = new List<SettingsElement> { };
                return Task.FromResult(ServiceResponse<IList<SettingsElement>>.Ok(data));
            });

            _repository.Setup(x => x.SetSettings(It.IsAny<List<SettingsElement>>())).Returns(() => {
                return Task.FromResult(ServiceResponse<bool>.Ok(true));
            });
        }

        [Fact]
        public async void Should_Get_Settings() {
            var result = await _service.GetSettings();

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Count);
        }

        [Fact]
        public async void ShoulD_Set_Settings() {
            var parameter = new List<SettingsElement> { };
            var result = await _service.SetSettings(parameter);

            Assert.NotNull(result);
            Assert.True(result.Data);
        }
    }
}
