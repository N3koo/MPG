using SAPServices;

using System.ServiceModel;
using System;

namespace DataEntity.Config {

    /// <summary>
    /// Class that implements the SAP access
    /// </summary>
    public class SapDb {

        /// <summary>
        /// Reference to the main client
        /// </summary>
        private static Z_MPGClient _client;

        /// <summary>
        /// Private constructor
        /// </summary>
        private SapDb() {

        }

        /// <summary>
        /// Used to create a new client to the SAP services
        /// </summary>
        /// <returns>A new client that has access to SAP</returns>
        public static Z_MPGClient GetClient() {
            if (_client == null) {
                BasicHttpBinding binding = new();
                EndpointAddress address = new(new Uri(Properties.Resources.SAP_Server));

                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                binding.OpenTimeout = new TimeSpan(0, 5, 0);
                binding.CloseTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                binding.MaxReceivedMessageSize = 65536 * 1000;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

                _client = new Z_MPGClient(binding, address);
                _client.ClientCredentials.UserName.UserName = Properties.Resources.SAP_User;
                _client.ClientCredentials.UserName.Password = Properties.Resources.SAP_Pass;
                _client.Open();
            }

            return _client;
        }

        /// <summary>
        /// Destructor that closes the connection of the client
        /// </summary>
        ~SapDb() {
            if (_client != null) {
                _client.Close();
            }
        }
    }
}
