using System.Threading.Tasks;

namespace MPG_Interface.Module.Interfaces {

    /// <summary>
    /// Used to implements the observer pattern
    /// </summary>
    public interface IObserver {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Task Update(string poid, string status);
    }
}
