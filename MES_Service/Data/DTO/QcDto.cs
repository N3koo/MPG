using System.Collections.Generic;

namespace MpgWebService.DTO {
    public record QcDto {
        public int Priority { get; init; }
        public List<bool> Qc { get; init; }
    }
}
