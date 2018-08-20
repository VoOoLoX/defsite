using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Defsite {
	public class Client {
		public TcpClient Socket = new TcpClient();

		public EndPoint RemoteEndPoint => Socket.Client.RemoteEndPoint;


		public Client() {
		}

		public Client(TcpClient client) {
			Socket = client;

		}
	}
}
