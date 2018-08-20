using System;
using System.Collections.Generic;
using System.Threading;

namespace Server {
	class Program {
		static void Main(string[] args) {
			var servers = new List<Server>();
			var settings = new Config("Assets/Settings.cfg");

			foreach (var obj in settings.GetScope("servers").Objects) {
				foreach (var p in obj.Properties) Console.WriteLine($"Loaded property: {p}");
				var server = new Server(obj.GetString("ip"), obj.GetInt("port"), obj.GetString("name"), obj.GetInt("max_players"), obj.GetInt("tps"));
				servers.Add(server);
			}

			foreach (var server in servers) {
				Console.WriteLine($"Server: [{server.Name}] [{server.IP}] [{server.Port}]");
				new Thread(() => {
					server.Run();
				}) { Name = "Server" }.Start();

				new Thread(() => {
					while (server.Running)
						server.NetworkHandler.Run();
				}) { Name = "NetworkHandler" }.Start();

				new Thread(() => {
					while (server.Running)
						server.GameHandler.Run(true);
				}) { Name = "GameHandler" }.Start();

				new Thread(() => {
					while (server.Running)
						server.DatabaseHandler.Run(true);
				}) { Name = "DatabaseHandler" }.Start();
			}
		}
	}
}
