using System.Net;
using System.Net.Sockets;

namespace Common {
	public class Client {
		public TcpClient Socket = new TcpClient();


		public Client() {
		}

		public Client(TcpClient client) {
			Socket = client;
		}

		public EndPoint RemoteEndPoint => Socket.Client.RemoteEndPoint;
	}
}