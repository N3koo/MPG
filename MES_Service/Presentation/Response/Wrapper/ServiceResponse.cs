using System.Collections.Generic;

namespace MpgWebService.Presentation.Response.Wrapper {

    public class ServiceResponse {
        public object Data { set; get; }
        public List<ErrorType> Errors { set; get; }

        public void AddError(ErrorType errorType) {
            Errors.Add(errorType);
        }

        public static ServiceResponse CombineResponses(params ServiceResponse[] responses) {
            var result = new ServiceResponse();

            foreach (var response in responses) {
                if (response.Errors == null) {
                    continue;
                }

                result.Errors.AddRange(response.Errors);
            }

            return result;
        }

        public static ServiceResponse CreateResponse(object data, string message) =>
           data != null ? Ok(data) : NotFound(message);

        public static ServiceResponse Ok(object data) => new() {
            Data = data,
            Errors = null
        };

        public static ServiceResponse NotFound(string message) => new() {
            Data = null,
            Errors = new() { ErrorType.MES(message) }
        };

        public static ServiceResponse CreateErrorMpg(string message) => new() {
            Data = null,
            Errors = new() { ErrorType.MPG(message) }
        };

        public static ServiceResponse CreateErrorMes(string message) => new() {
            Data = null,
            Errors = new() { ErrorType.MES(message) }
        };

        public static ServiceResponse CreateErrorSap(string message) => new() {
            Data = null,
            Errors = new() { ErrorType.SAP(message) }
        };
    }
}