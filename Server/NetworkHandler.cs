using System.Linq;
using System.Net.Sockets;
using Defsite;

namespace Server {
	public class NetworkHandler : Handler {
		const int max_message_size = 4096;

		MessageManager message_manager = new MessageManager();

		static bool IsConnected(Client client) {
			try {
				if (client.Socket != null)
					return !(client.Socket.Client.Poll(1, SelectMode.SelectRead) && client.Socket.Client.Available == 0);
				return false;
			}
			catch (SocketException) {
				return false;
			}
		}

		void Disconnect(Client client) {
			if (IsConnected(client)) client.Socket.Client.Disconnect(true);
		}

		async void Recieve(Client client) {
			if (IsConnected(client)) {
				var data = Enumerable.Repeat((byte) 0xff, max_message_size).ToArray();
				var stream = client.Socket.GetStream();
				if (stream.DataAvailable) {
					var message_length = 0;
					while ((message_length = await stream.ReadAsync(data, 0, data.Length)) != 0) message_manager.Recieve(client, data, message_length);
				}
			}
		}

		public static async void Send(Client client, byte[] data) {
			if (IsConnected(client)) {
				var stream = client.Socket.GetStream();
				if (stream.DataAvailable) await stream.WriteAsync(data, 0, data.Length);
			}
		}

		protected override void Update() {
			foreach (var client in Clients.ToArray())
				if (IsConnected(client)) {
					Recieve(client);
				}
				else {
					Disconnect(client);
					Server.RemoveClient(client);
					Clients.Remove(client);
					Log.Info($"Client disconnected: {client.RemoteEndPoint}");
				}
		}
	}
}