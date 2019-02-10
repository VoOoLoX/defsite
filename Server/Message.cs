using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Server {
	public abstract class Message {
		public abstract MessageType Type { get; }
		public virtual List<byte> Data => new List<byte>() { (byte)Type };
		protected void ResetIndex() => reader_index = 0;

		int reader_index = 0;

		public void WriteBytes<T>(T data) {
			switch (data) {
				case bool bool_value:
					var b_value = BitConverter.GetBytes(bool_value);
					Data.Concat(b_value.ToList());
					break;
				case char char_value:
					var c_value = BitConverter.GetBytes(char_value);
					Data.Concat(c_value.ToList());
					break;

				case int int_value:
					var i_value = BitConverter.GetBytes(int_value);
					Data.Concat(i_value.ToList());
					break;
				case short short_value:
					var sh_value = BitConverter.GetBytes(short_value);
					Data.Concat(sh_value.ToList());
					break;
				case long long_value:
					var ln_value = BitConverter.GetBytes(long_value);
					Data.Concat(ln_value.ToList());
					break;

				case float float_value:
					var f_value = BitConverter.GetBytes(float_value);
					Data.Concat(f_value.ToList());
					break;
				case double double_value:
					var d_value = BitConverter.GetBytes(double_value);
					Data.Concat(d_value.ToList());
					break;

				case string string_value:
					var s_value = Encoding.Unicode.GetBytes(string_value);
					Data.Concat(s_value.ToList());
					break;
			}
		}

		public T ReadBytes<T>(byte[] data) {
			if (typeof(T) == typeof(bool)) {
				var b_value = BitConverter.ToBoolean(data.Skip(reader_index).ToArray());
				reader_index += sizeof(bool);
				return (T)Convert.ChangeType(b_value, typeof(T));
			} else if (typeof(T) == typeof(char)) {
				var c_value = BitConverter.ToChar(data.Skip(reader_index).ToArray());
				reader_index += sizeof(char);
				return (T)Convert.ChangeType(c_value, typeof(T));
			} else if (typeof(T) == typeof(int)) {
				var i_value = BitConverter.ToInt32(data.Skip(reader_index).ToArray());
				reader_index += sizeof(int);
				return (T)Convert.ChangeType(i_value, typeof(T));
			} else if (typeof(T) == typeof(short)) {
				var sh_value = BitConverter.ToInt16(data.Skip(reader_index).ToArray());
				reader_index += sizeof(short);
				return (T)Convert.ChangeType(sh_value, typeof(T));
			} else if (typeof(T) == typeof(long)) {
				var ln_value = BitConverter.ToInt64(data.Skip(reader_index).ToArray());
				reader_index += sizeof(long);
				return (T)Convert.ChangeType(ln_value, typeof(T));
			} else if (typeof(T) == typeof(double)) {
				var d_value = BitConverter.ToDouble(data.Skip(reader_index).ToArray());
				reader_index += sizeof(double);
				return (T)Convert.ChangeType(d_value, typeof(T));
			} else if (typeof(T) == typeof(string)) {
				var r = new BinaryReader(new MemoryStream(data.Skip(reader_index).ToArray()));
				var s_value = r.ReadString();
				//String length + string + null char
				//echo -e 'B\x07VoOoLoX\0\x06CooLpW\0' | nc localhost 7331
				reader_index += s_value.Length + 2;
				return (T)Convert.ChangeType(s_value, typeof(T));
			} else {
				throw new Exception($"Cannon't interpret given bytes as a type: {typeof(T).Name}");
			}
		}
	}

	//Messages recieved from client
	public class MessageUnknown : Message {
		public override MessageType Type => MessageType.Unknown;
	}

	public class MessageLogin : Message {
		public override MessageType Type => MessageType.Login;

		public string Username { get; private set; }
		public string Password { get; private set; }

		public MessageLogin(byte[] data) {
			ResetIndex();
			Username = ReadBytes<string>(data);
			Password = ReadBytes<string>(data);
		}
	}
}
