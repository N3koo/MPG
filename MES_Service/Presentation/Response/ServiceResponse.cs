using MpgWebService.Business.Data.Exceptions;

namespace MpgWebService.Presentation.Response {

    public enum ServerType {
        Ok,
        Sap,
        Mpg,
        Mes
    }

    public class ServiceResponse {
        public string Message { set; get; }
        public bool Status { set; get; }
        public ServerType Type { set; get; }

        public void CheckErrors() {
            if (Status) {
                return;
            }

            switch (Type) {
                case ServerType.Mes:
                    throw new MesException(Message);
                case ServerType.Mpg:
                    throw new MpgException(Message);
                case ServerType.Sap:
                    throw new SapException(Message);
            }
        }

        public void AddError(string message) {
            Message = $"{Message}\n{message}";
        }

        public static ServiceResponse CreateErrorSap(string message) => new() {
            Message = message,
            Type = ServerType.Sap,
            Status = false
        };

        public static ServiceResponse CreateErrorMpg(string message) => new() {
            Message = message,
            Type = ServerType.Mpg,
            Status = false
        };

        public static ServiceResponse CreateErrorMes(string message) => new() {
            Message = message,
            Type = ServerType.Mes,
            Status = false
        };

        public static ServiceResponse CreateOkResponse(string message) => new() {
            Message = message,
            Type = ServerType.Ok,
            Status = true
        };
    }
}
