using System;

namespace MpgWebService.Business.Data.Exceptions {

    [Serializable]
    public class MpgException : Exception {
        public MpgException() {

        }

        public MpgException(string message) : base(message) {

        }

        public MpgException(string message, Exception inner) : base(message, inner) {

        }
    }
}
