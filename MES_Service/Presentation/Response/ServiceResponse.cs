using MpgWebService.Business.Data.Exceptions;
using System;
using System.Threading.Tasks;

namespace MpgWebService.Presentation.Response {

    public enum ServerType {
        Ok,
        Sap,
        Mpg,
        Mes
    }

    public class ServiceResponse {
        public object Data { set; get; }
        public string Message { set; get; }
        public bool Error { set; get; }
        public ServerType Type { set; get; }

        public void CheckErrors() {
            if (Error) {
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

        public static ServiceResponse CreateResponse(object data, string message) =>
            data == null ? NotFound(message) : Ok(data);

        public static ServiceResponse Ok(string message) => new() {
            Data = null,
            Message = message,
            Error = false,
            Type = ServerType.Ok
        };

        public static ServiceResponse Ok(object data) => new() {
            Data = data,
            Message = "OK",
            Error = false,
            Type = ServerType.Ok
        };

        public static ServiceResponse NotFound(string message) => new() {
            Data = null,
            Message = message,
            Error = false,
            Type = ServerType.Ok
        };

        public static ServiceResponse InternalError(Exception ex) => new() {
            Data = null,
            Message = ex.Message,
            Error = true,
            Type = ServerType.Ok
        };

        public static ServiceResponse CreateErrorSap(string message) => new() {
            Message = message,
            Type = ServerType.Sap,
            Error = false
        };

        public static ServiceResponse CreateErrorMpg(string message) => new() {
            Message = message,
            Type = ServerType.Mpg,
            Error = false
        };

        public static ServiceResponse CreateErrorMes(string message) => new() {
            Message = message,
            Type = ServerType.Mes,
            Error = false
        };
    }
}
