using MpgWebService.Presentation.Request.Settings;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace MpgWebService.Repository {

    public class SettingsRepository : ISettingsRepository {

        public Task<ServiceResponse<IList<SettingsElement>>> GetSettings() {
            List<SettingsElement> list = new();

            foreach (SettingsProperty data in Properties.Settings.Default.Properties) {
                list.Add(new() {
                    Name = data.Name,
                    DefaultValue = data.DefaultValue as string
                });
            }

            var result = ServiceResponse<IList<SettingsElement>>.Ok(list);
            return Task.FromResult(result);
        }

        public Task<ServiceResponse<bool>> SetSettings(List<SettingsElement> list) {
            list.ForEach(item => {
                Properties.Settings.Default[item.Name] = item.DefaultValue;
            });

            Properties.Settings.Default.Save();

            var response = ServiceResponse<bool>.Ok(true);
            return Task.FromResult(response);
        }
    }
}
