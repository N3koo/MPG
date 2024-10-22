using System.Collections.Generic;

namespace MPG_Interface.Module.Data.Input
{
    public class ServiceResponse<T>
    {
        public T Data { set; get; }
        public List<Error> Errors { set; get; }

        public class Error
        {
            public string Message { set; get; }
            public int Type { set; get; }
        }
    }
}
