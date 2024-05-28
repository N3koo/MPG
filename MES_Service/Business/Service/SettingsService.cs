using MpgWebService.Presentation.Request.Settings;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Service { 

    public class SettingsService : ISettingsService {

        public readonly ISettingsRepository repository;

        public SettingsService() {
            repository = new SettingsRepository();
        }

        public async Task<IEnumerable<SettingsElement>> GetSettings() =>
            await repository.GetSettings();

        public async Task<bool> SetSettings(List<SettingsElement> settings) =>
            await repository.SetSettings(settings);

    }
}
