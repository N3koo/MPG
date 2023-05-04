using MpgWebService.Presentation.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {
    public interface ISettingsService {
        Task<IEnumerable<SettingsElement>> GetSettings();
        Task<bool> SetSettings(List<SettingsElement> settings);
    }
}
