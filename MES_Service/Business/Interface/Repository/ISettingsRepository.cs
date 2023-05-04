using MpgWebService.Presentation.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {
    public interface ISettingsRepository {
        Task<List<SettingsElement>> GetSettings();
        Task<bool> SetSettings(List<SettingsElement> list);
    }
}
