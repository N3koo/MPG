using System;

namespace MpgWebService.Business.Data.Exceptions {
    [Serializable]
    public class MesException : Exception {
        public MesException() {

        }

        public MesException(string message) : base(message) {

        }

        public MesException(string message, Exception inner) : base(message, inner) {

        }
    }
}
