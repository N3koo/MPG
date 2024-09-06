using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request.Settings;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Service {

    public class SettingsService : ISettingsService {

        public readonly ISettingsRepository repository;

        public SettingsService(ISettingsRepository repository) {
            this.repository = repository;
        }

        public async Task<ServiceResponse<IList<SettingsElement>>> GetSettings() =>
            await repository.GetSettings();

        public async Task<ServiceResponse<bool>> SetSettings(List<SettingsElement> settings) =>
            await repository.SetSettings(settings);

    }
}
