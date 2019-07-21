namespace Server {
	public class GameHandler : Handler {
		int ticks_per_sec;

		public GameHandler(int max_players, int tps) {
			ticks_per_sec = tps;
		}

		protected override int TPS => ticks_per_sec;

		protected override void FixedUpdate(long dt) {
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