namespace MpgWebService.Data.Wrappers {

    public class Response<T> {

        public T Data { get; set; }
        public bool Succeded { set; get; }
        public string[] Errors { set; get; }
        public string Message { set; get; }

        public Response() {

        }

        public Response(T data) {
            Succeded = true;
            Message = string.Empty;
            Errors = null;
            Data = data;
        }

    }
}
