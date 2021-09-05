using System.IO;
using System.Text.Json;
using System.Threading;
using Common;

namespace Server {
	internal static class Program {
		static void Main(string[] args) {
			var file = File.ReadAllBytes("Assets/Settings.json");
			var json = JsonDocument.Parse(file).RootElement;

			var settings = json.GetProperty("settings");

			var server_name = settings.GetProperty("name").GetString();
			var server_ip = settings.GetProperty("ip").GetString();
			var server_port = settings.GetProperty("port").GetInt32();
			var server_tps = settings.GetProperty("tps").GetInt32();
			var server_max_players = settings.GetProperty("max_players").GetInt32();

			var server = new Server(server_ip, server_port, server_name, server_max_players, server_tps);

			Log.Info($"Server: [{server.Name}] [{server.Address}] [{server.Port}]");
			new Thread(() => { server.Run(); }) { Name = "Server" }.Start();

			new Thread(() => {
				while (server.Running)
					server.NetworkHandler.Run();
			}) { Name = "NetworkHandler" }.Start();

			new Thread(() => {
				while (server.Running)
					server.GameHandler.Run();
			}) { Name = "GameHandler" }.Start();

			new Thread(() => {
				while (server.Running)
					server.DatabaseHandler.Run(true);
			}) { Name = "DatabaseHandler" }.Start();
		}
	}
}