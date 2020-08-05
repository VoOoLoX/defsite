using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Defsite;

namespace Server
{
    public class Server
    {
        static object remove_client_lock = new object();
        static object update_clients_lock = new object();
        static object get_clients_lock = new object();
        static object add_clients_lock = new object();

        static List<Client> clients = new List<Client>();

        public DatabaseHandler DatabaseHandler;
        public GameHandler GameHandler;
        IPAddress ip;
        string name;
        public NetworkHandler NetworkHandler;
        int port = 0;

        public bool Running = true;

        public Server(string ip, int port, string name, int max_players, int tps)
        {
            this.ip = IPAddress.Parse(ip);
            this.port = port;
            this.name = name;
            DatabaseHandler = new DatabaseHandler();
            GameHandler = new GameHandler(max_players, tps);
            NetworkHandler = new NetworkHandler();
        }

        public IPAddress IP => ip;
        public string Name => name;
        public int Port => port;

        public static List<Client> GetClients()
        {
            lock (get_clients_lock)
            {
                return clients;
            }
        }

        public static void UpdateClients(List<Client> c)
        {
            lock (update_clients_lock)
            {
                clients = c;
            }
        }

        public static void RemoveClient(Client client)
        {
            lock (remove_client_lock)
            {
                clients.Remove(client);
            }
        }

        public void Run()
        {
            var server = new TcpListener(ip, port);

            server.Start();
            Log.Info("Server started");

            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
                if (server.Pending())
                {
                    var client_socket = server.AcceptTcpClient();
                    lock (add_clients_lock)
                    {
                        clients.Add(new Client(client_socket));
                    }

                    Log.Info($"Added client: {client_socket.Client.RemoteEndPoint}");
                }

            Log.Info($"Stopping server: {name}");
            Running = false;
            server.Stop();
        }
    }
}