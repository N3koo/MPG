using MpgWebService.Presentation.Request.Settings;
using MpgWebService.Presentation.Response.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface ISettingsRepository {

        Task<ServiceResponse<IList<SettingsElement>>> GetSettings();

        Task<ServiceResponse<bool>> SetSettings(List<SettingsElement> list);

    }
}
