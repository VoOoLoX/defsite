using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Common;

namespace Server;

public class Server {
	static object remove_client_lock = new();
	static object update_clients_lock = new();
	static object get_clients_lock = new();
	static object add_clients_lock = new();

	static List<Client> clients = new();

	public DatabaseHandler DatabaseHandler;
	public GameHandler GameHandler;
	public NetworkHandler NetworkHandler;

	public bool Running = true;

	public Server(string ip, int port, string name, int max_players, int tps) {
		Address = IPAddress.Parse(ip);
		Port = port;
		Name = name;
		DatabaseHandler = new DatabaseHandler();
		GameHandler = new GameHandler(max_players, tps);
		NetworkHandler = new NetworkHandler();
	}

	public IPAddress Address { get; }

	public string Name { get; }

	public int Port { get; }

	public static List<Client> GetClients() {
		lock(get_clients_lock) {
			return clients;
		}
	}

	public static void UpdateClients(List<Client> c) {
		lock(update_clients_lock) {
			clients = c;
		}
	}

	public static void RemoveClient(Client client) {
		lock(remove_client_lock) {
			clients.Remove(client);
		}
	}

	public void Run() {
		var server = new TcpListener(Address, Port);

		server.Start();
		Log.Info("Server started");

		while(!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)) {
			if(server.Pending()) {
				var client_socket = server.AcceptTcpClient();
				lock(add_clients_lock) {
					clients.Add(new Client(client_socket));
				}

				Log.Info($"Added client: {client_socket.Client.RemoteEndPoint}");
			}
		}

		Log.Info($"Stopping server: {Name}");
		Running = false;
		server.Stop();
	}
}
