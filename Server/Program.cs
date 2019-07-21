using System.Linq;
using System.Threading;
using Defsite;

namespace Server {
	internal class Program {
		static void Main(string[] args) {
			var settings = new Config("Assets/Settings.toml");

			var servers = (from server in settings["servers"].Children from prop in server.Children select new Server(prop["ip"], prop["port"], prop["name"], prop["max_players"], prop["tps"])).ToList();

			foreach (var server in servers) {
				Log.Info($"Server: [{server.Name}] [{server.IP}] [{server.Port}]");
				new Thread(() => { server.Run(); }) {Name = "Server"}.Start();

				new Thread(() => {
					while (server.Running)
						server.NetworkHandler.Run();
				}) {Name = "NetworkHandler"}.Start();

				new Thread(() => {
					while (server.Running)
						server.GameHandler.Run(true);
				}) {Name = "GameHandler"}.Start();

				new Thread(() => {
					while (server.Running)
						server.DatabaseHandler.Run(true);
				}) {Name = "DatabaseHandler"}.Start();
			}
		}
	}
}