using DataEntity.Model.Input;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Mpg;

namespace WebServiceTest.Data {
    public class DataCreator {

        public static ProductionOrder CreateOrder() => new() {
            POID = ""
        };

        public static Period CreatePeriod(DateTime start = default, DateTime end = default) => new() {
            StartDate = start,
            EndDate = end,
        };

        public static StartCommand CreateStartCommand() => new() {
        };

        public static PailQCDto CreateQcDtO() => new() {

        };

        public static PailDto CreatePailDto() => new() {

        };

        public static IList<MaterialDto> CreateListOfMaterials() => new List<MaterialDto>() {
        };

        public static LabelDto CreateLabel() => new() {

        };

        public static QcLabelDto GetQcLabel() => new() {
        };
    }
}
