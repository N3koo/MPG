using MpgWebService.Repository.Interface;
using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;

namespace MpgWebService.Repository {

    public class SettingsRepository : ISettingsRepository {

        public Task<List<SettingsElement>> GetSettings() {
            List<SettingsElement> list = new();

            foreach (SettingsProperty data in Properties.Settings.Default.Properties) {
                list.Add(new SettingsElement {
                    Name = data.Name,
                    DefaultValue = data.DefaultValue as string
                });
            }

            return Task.FromResult(list);
        }

        public Task<bool> SetSettings(List<SettingsElement> list) {
            list.ForEach(item => {
                Properties.Settings.Default[item.Name] = item.DefaultValue;
            });

            Properties.Settings.Default.Save();

            return Task.FromResult(true);
        }
    }
}
