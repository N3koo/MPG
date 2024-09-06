using MpgWebService.Presentation.Request.Settings;
using MpgWebService.Presentation.Response.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {

    public interface ISettingsService {

        Task<ServiceResponse<IList<SettingsElement>>> GetSettings();

        Task<ServiceResponse<bool>> SetSettings(List<SettingsElement> settings);

    }
}
