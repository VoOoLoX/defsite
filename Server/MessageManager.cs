using System;
using System.Linq;
using Defsite;

namespace Server {
	public class MessageManager {
		public void Recieve(Client client, byte[] data, int length) {
			if (data.Length < 1)
				return;

			var message_type = MessageType.Unknown;
			if (Enum.IsDefined(typeof(MessageType), data[0]))
				message_type = (MessageType) data[0];

			var message_data = data.Skip(1).ToArray();

//			Log.Info($"[{message_type}] {Encoding.ASCII.GetString(message_data, 0, length - 1)}");
			Parse(message_type, message_data);

			// NetworkHandler.Send(client, data);
		}

		async void Send(Client client, Message msg) {
//			Log.Info(Encoding.ASCII.GetString(msg.Data.ToArray()));
			await NetworkHandler.Send(client, msg.Data.ToArray());
		}

		void Parse(MessageType type, byte[] data) {
			Message msg;
			switch (type) {
				case MessageType.Login:
					msg = new MessageLogin(data);
					Log.Info((msg as MessageLogin).Username + " - " + (msg as MessageLogin).Password);
					break;
				case MessageType.Broadcast:
					msg = new MessageBroadcast(data);
					var text = new MessageText((msg as MessageBroadcast).Message);

//					Log.Info(Encoding.ASCII.GetString(msg, 0, length - 1));
					foreach (var c in Server.GetClients())
						Send(c, text);
					break;
				case MessageType.Unknown:
					break;
				case MessageType.Init:
					break;
				case MessageType.Text:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}