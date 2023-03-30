using System;

namespace MpgWebService.Business.Data.Exceptions {

    [Serializable]
    public class SapException : Exception {
        public SapException() {

        }

        public SapException(string message) : base(message) {

        }

        public SapException(string message, Exception inner) : base(message, inner) {

        }
    }
}
