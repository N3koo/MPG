using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MPG_Interface.Module.Data.Output {
    public class StartCommand {
        public string POID { set; get; }
        public int? Priority { set; get; }
        public bool[] QC { set; get; }

        public static List<StartCommand> ListQc { set; get; } = new();

        private static bool[] CreateQC(string qc, int size) {
            var local = new bool[size];

            string[] splits = qc.Split(";");
            foreach (string item in splits) {
                int index = int.Parse(item, CultureInfo.InvariantCulture) - 1;
                local[index] = true;
            }

            return local;
        }

        public static StartCommand GetCommand(string POID) {
            return ListQc.First(p => p.POID == POID);
        }

        public static StartCommand CreateCommand(string POID, string qc, int size) {
            var result = ListQc.FirstOrDefault(p => p.POID == POID);
            if (result != null) {
                return result;
            }

            result = new StartCommand {
                POID = POID,
                QC = CreateQC(qc, size),
            };

            ListQc.Add(result);
            return result;
        }

        public static void SetPriority(string POID, int Priority) {
            ListQc.First(p => p.POID == POID).Priority = Priority;
        }
    }
}
