using System.Net;
using System;

namespace MpgWebService.Data.Wrappers {
    
    public interface IResponse {
    }

    public class Response<T> : IResponse {

        public T Data { get; set; }
        public HttpStatusCode Status { set; get; }
        public string[] Errors { set; get; }
        public string Message { set; get; }

        public static Response<T> CreateResponse(T data) =>
            data == null ? NotFound() : Ok(data);

        public static Response<T> Success(string message) => new() {
            Status = HttpStatusCode.OK,
            Message = message,
            Errors = null,
            Data = default
        };

        private static Response<T> NotFound() => new() {
            Status = HttpStatusCode.NotFound,
            Message = string.Empty,
            Errors = null,
            Data = default
        };
 
        private static Response<T> Ok(T data) => new() {
            Data = data,
            Status = HttpStatusCode.OK,
            Errors = null,
            Message = string.Empty
        };

        public static Response<T> Error(Exception ex) => new() {
            Data = default,
            Status = HttpStatusCode.InternalServerError,
            Errors = new []{ ex.Message },
            Message = string.Empty
        };
    }
}
