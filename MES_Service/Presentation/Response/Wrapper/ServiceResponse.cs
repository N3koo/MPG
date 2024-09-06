using System.Collections.Generic;

namespace MpgWebService.Presentation.Response.Wrapper {

    public class ServiceResponse<T> {
        public T Data { set; get; }
        public List<ErrorType> Errors { set; get; }

        public void AddError(ErrorType errorType) {
            Errors.Add(errorType);
        }

        public void AddErros(List<ErrorType> erros) {
            Errors.AddRange(erros);
        }

        public static ServiceResponse<T> CombineResponses(params ServiceResponse<T>[] responses) {
            var result = new ServiceResponse<T>();

            foreach (var response in responses) {
                if (response.Errors == null) {
                    continue;
                }

                result.Errors.AddRange(response.Errors);
            }

            return result;
        }

        public static ServiceResponse<T> GetErrors(List<ErrorType> errors) => new() {
            Data = default,
            Errors = errors
        };

        public static ServiceResponse<T> CreateResponse(T data, string message) =>
           data != null ? Ok(data) : NotFound(message);

        public static ServiceResponse<T> Ok(T data) => new() {
            Data = data,
            Errors = null
        };

        public static ServiceResponse<T> NotFound(string message) => new() {
            Data = default,
            Errors = new() { ErrorType.MES(message) }
        };

        public static ServiceResponse<T> CreateErrorMpg(string message) => new() {
            Data = default,
            Errors = new() { ErrorType.MPG(message) }
        };

        public static ServiceResponse<T> CreateErrorMes(string message) => new() {
            Data = default,
            Errors = new() { ErrorType.MES(message) }
        };

        public static ServiceResponse<T> CreateErrorSap(string message) => new() {
            Data = default,
            Errors = new() { ErrorType.SAP(message) }
        };
    }
}