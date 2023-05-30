using System.Collections.Generic;

namespace MpgWebService.Presentation.Response {
    public record QcDto {
        public int Priority { get; init; }
        public List<bool> Qc { get; init; }
    }
}
