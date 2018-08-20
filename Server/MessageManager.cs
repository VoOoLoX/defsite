using Defsite;
using System;
using System.Linq;
using System.Text;

namespace Server {
	public class MessageManager {
		public void Recieve(Client client, byte[] data, int length) {
			if (data.Length < 1)
				return;

			var message_type = MessageType.Unknown;
			if (Enum.IsDefined(typeof(MessageType), data[0]))
				message_type = (MessageType)data[0];

			var message_data = data.Skip(1).ToArray();

			Parse(message_type, message_data);

			//Console.WriteLine($"[{message_id.ToString()}] {Encoding.ASCII.GetString(message_data, 0, length - 1)}");
			//NetworkHandler.Send(client, data);
		}

		void Send(Client client, Message msg) {

		}

		void Parse(MessageType type, byte[] data) {
			Message msg;
			switch (type) {
				case MessageType.Login:
					msg = new MessageLogin(data);
					Console.WriteLine((msg as MessageLogin).Username + " - " + (msg as MessageLogin).Password);
					break;
			}
		}
	}
}
