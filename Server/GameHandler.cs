namespace Server {
	public class GameHandler : Handler {
		public GameHandler(int max_players, int tps) {
			TPS = tps;
		}

		protected override int TPS { get; }

		protected override void FixedUpdate(object state) {
			//foreach (var client in Clients.ToArray()) {
			//	if (client.IsConnected())
			//		client.Recieve();
			//	else {
			//		client.Disconnect();
			//		Clients.Remove(client);
			//		Console.WriteLine($"Client disconnected: {client.RemoteEndPoint}");
			//	}
			//}
			//Console.WriteLine("GameHandler update");
		}
	}
}