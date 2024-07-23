namespace MpgWebService.Presentation.Response.Wrapper {

    public enum ServerType {
        Sap,
        Mpg,
        Mes
    }

    public class ErrorType {
        public string Message { set; get; }
        public ServerType Type { set; get; }

        public static ErrorType MES(string message) => new() {
            Message = message,
            Type = ServerType.Mes
        };

        public static ErrorType MPG(string message) => new() {
            Message = message,
            Type = ServerType.Mpg
        };

        public static ErrorType SAP(string message) => new() {
            Message = message,
            Type = ServerType.Sap
        };
    }
}