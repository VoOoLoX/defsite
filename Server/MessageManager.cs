using System;
using System.Linq;
using Common;

namespace Server;

public class MessageManager {
	public void Receive(Client client, byte[] data, int length) {
		if(data.Length < 1) // Disconnect client???
			return;

		var message_type = MessageType.Unknown;
		if(Enum.IsDefined(typeof(MessageType), data[0]))
			message_type = (MessageType)data[0];

		// Skips first byte (type of message) of the recived data
		var message_data = data.Skip(1).ToArray();

		Parse(message_type, message_data);
	}

	async void Send(Client client, Message msg) =>
		// Log.Info(Encoding.ASCII.GetString(msg.Data.ToArray()));
		await NetworkHandler.Send(client, msg.Data.ToArray());

	void Parse(MessageType type, byte[] data) {
		Message msg;
		switch(type) {
			case MessageType.Unknown:
				break;
			case MessageType.Init:
				break;
			case MessageType.Login:
				msg = new MessageLogin(data);
				Log.Info((msg as MessageLogin).Username + " - " + (msg as MessageLogin).Password);

				var login_text = new MessageText($"{(msg as MessageLogin).Username} logged in.");

				foreach(var client in Server.GetClients())
					Send(client, login_text);
				break;
			case MessageType.Broadcast:
				msg = new MessageBroadcast(data);
				var broadcast_text = new MessageText((msg as MessageBroadcast).Message);

				Log.Info((msg as MessageBroadcast).Message);
				foreach(var client in Server.GetClients())
					Send(client, broadcast_text);
				break;
			case MessageType.Text:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}
}
