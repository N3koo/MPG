using System.Collections.Generic;

namespace MES_Service.DTO {
    public record QcDto {
        public int Priority { get; init; }
        public List<bool> Qc { get; init; }
    }
}
