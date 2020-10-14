using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Server {
	public abstract class Message {
		int reader_index;
		public abstract MessageType Type { get; }
		public List<byte> Data { get; } = new List<byte>();

		public Message(bool outgoing = false) => ResetIndex(outgoing);

		protected void ResetIndex(bool outgoing = false) {
			if (!outgoing)
				Data.Add((byte)Type);
			reader_index = 0;
		}

		public void WriteBytes<T>(T data) {
			switch (data) {
				case bool bool_value:
					var b_value = BitConverter.GetBytes(bool_value);
					Data.AddRange(b_value);
					break;
				case char char_value:
					var c_value = BitConverter.GetBytes(char_value);
					Data.AddRange(c_value);
					break;

				case int int_value:
					var i_value = BitConverter.GetBytes(int_value);
					Data.AddRange(i_value);
					break;
				case short short_value:
					var sh_value = BitConverter.GetBytes(short_value);
					Data.AddRange(sh_value);
					break;
				case long long_value:
					var ln_value = BitConverter.GetBytes(long_value);
					Data.AddRange(ln_value);
					break;

				case float float_value:
					var f_value = BitConverter.GetBytes(float_value);
					Data.AddRange(f_value);
					break;
				case double double_value:
					var d_value = BitConverter.GetBytes(double_value);
					Data.AddRange(d_value);
					break;

				case string string_value:
					var s_value = Encoding.ASCII.GetBytes(string_value);
					Data.AddRange(s_value);
					break;
			}
		}

		public T ReadBytes<T>(byte[] data) {
			if (typeof(T) == typeof(bool)) {
				var b_value = BitConverter.ToBoolean(data.Skip(reader_index).ToArray());
				reader_index += sizeof(bool);
				return (T)Convert.ChangeType(b_value, typeof(T));
			}

			if (typeof(T) == typeof(char)) {
				var c_value = BitConverter.ToChar(data.Skip(reader_index).ToArray());
				reader_index += sizeof(char);
				return (T)Convert.ChangeType(c_value, typeof(T));
			}

			if (typeof(T) == typeof(int)) {
				var i_value = BitConverter.ToInt32(data.Skip(reader_index).ToArray());
				reader_index += sizeof(int);
				return (T)Convert.ChangeType(i_value, typeof(T));
			}

			if (typeof(T) == typeof(short)) {
				var sh_value = BitConverter.ToInt16(data.Skip(reader_index).ToArray());
				reader_index += sizeof(short);
				return (T)Convert.ChangeType(sh_value, typeof(T));
			}

			if (typeof(T) == typeof(long)) {
				var ln_value = BitConverter.ToInt64(data.Skip(reader_index).ToArray());
				reader_index += sizeof(long);
				return (T)Convert.ChangeType(ln_value, typeof(T));
			}

			if (typeof(T) == typeof(double)) {
				var d_value = BitConverter.ToDouble(data.Skip(reader_index).ToArray());
				reader_index += sizeof(double);
				return (T)Convert.ChangeType(d_value, typeof(T));
			}

			if (typeof(T) == typeof(string)) {
				var r = new BinaryReader(new MemoryStream(data.Skip(reader_index).ToArray()));
				var s_value = r.ReadString();
				//String length + string + null char
				//echo -e 'B\x07VoOoLoX\0\x06CooLpW\0' | nc localhost 7331
				reader_index += s_value.Length;
				return (T)Convert.ChangeType(s_value, typeof(T));
			}

			throw new Exception($"Can't interpret given bytes as a type: {typeof(T).Name}");
		}
	}

	//Messages received from client
	public class MessageUnknown : Message {
		public override MessageType Type => MessageType.Unknown;
	}

	public class MessageLogin : Message {
		public MessageLogin(byte[] data) : base() {
			Username = ReadBytes<string>(data);
			Password = ReadBytes<string>(data);
		}

		public override MessageType Type => MessageType.Login;

		public string Username { get; private set; }
		public string Password { get; private set; }
	}

	public class MessageBroadcast : Message {
		public MessageBroadcast(byte[] data) : base() {
			Message = ReadBytes<string>(data);
		}

		public override MessageType Type => MessageType.Broadcast;

		public string Message { get; private set; }
	}

	//Messages sent to client
	public class MessageText : Message {
		public MessageText(string text) : base(true) {
			WriteBytes(text);
		}

		public override MessageType Type => MessageType.Text;
	}
}