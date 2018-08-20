using Defsite;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server {
	public class Server {
		static object remove_client_lock = new object();
		static object update_clients_lock = new object();
		static object get_clients_lock = new object();

		static List<Client> clients = new List<Client>();
		IPAddress ip = default(IPAddress);
		int port = 0;
		string name = string.Empty;

		public bool Running = true;
		public IPAddress IP => ip;
		public string Name => name;
		public int Port => port;

		public DatabaseHandler DatabaseHandler;
		public GameHandler GameHandler;
		public NetworkHandler NetworkHandler;

		public Server(string ip, int port, string name, int max_players, int tps) {
			this.ip = IPAddress.Parse(ip);
			this.port = port;
			this.name = name;
			DatabaseHandler = new DatabaseHandler();
			GameHandler = new GameHandler(max_players, tps);
			NetworkHandler = new NetworkHandler();
		}

		public static List<Client> GetClients() {
			lock (get_clients_lock) {
				return clients;
			}
		}

		public static void UpdateClients(List<Client> c) {
			lock (update_clients_lock) {
				clients = c;
			}
		}

		public static void RemoveClient(Client client) {
			lock (remove_client_lock) {
				clients.Remove(client);
			}
		}

		public void Run() {
			var server = new TcpListener(ip, port);
			server.Start();

			while ((!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))) {
				if (server.Pending()) {
					var client_socket = server.AcceptTcpClient();
					clients.Add(new Client(client_socket));
					Console.WriteLine($"Added client: {client_socket.Client.RemoteEndPoint}");
				}
			}

			Console.WriteLine($"Stopping server: {name}");
			Running = false;
			server.Stop();
		}
	}
}
