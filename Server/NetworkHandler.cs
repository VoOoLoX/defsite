using System.Net.Sockets;
using System.Threading.Tasks;
using Common;

namespace Server {
	public class NetworkHandler : Handler {
		const int max_message_size = 4096;

		MessageManager message_manager = new();

		static bool IsConnected(Client client) {
			try {
				if (client.Socket?.Client != null && client.Socket.Client.Connected)
					return !(client.Socket.Client.Poll(0, SelectMode.SelectRead) && client.Socket.Client.Available == 0);
				return false;
			} catch (SocketException) {
				return false;
			}
		}

		void Disconnect(Client client) {
			if (IsConnected(client)) client.Socket.Client.Disconnect(true);
		}

		async void Receive(Client client) {
			if (IsConnected(client)) {
				var data = new byte[max_message_size];
				var stream = client.Socket.GetStream();
				if (stream.DataAvailable) {
					var message_length = 0;
					while ((message_length = await stream.ReadAsync(data, 0, data.Length)) != 0)
						message_manager.Receive(client, data, message_length);
				}
			}
		}

		public static async Task Send(Client client, byte[] data) {
			if (IsConnected(client)) {
				var stream = client.Socket.GetStream();
				await stream.WriteAsync(data, 0, data.Length);
			}
		}

		protected override void Update() {
			foreach (var client in Clients.ToArray())
				if (IsConnected(client)) {
					Receive(client);
				} else {
					Disconnect(client);
					Server.RemoveClient(client);
					Clients.Remove(client);
					Log.Info($"Client disconnected: {client.RemoteEndPoint}");
				}
		}

		protected override void FixedUpdate(object state) {
			// Log.Info($"Tick");
		}
	}
}