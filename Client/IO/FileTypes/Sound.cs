using System;
using System.IO;
using Defsite;
using OpenTK.Audio.OpenAL;

namespace Client {
	public class Sound {
		short audio_format;
		short bits_per_sample;
		short block_align;
		int byte_rate;
		short channels;

		int data_chunk_size;
		string data_string;
		int file_length;
		string fmt_string;

		int format_chunk_size;

		string riff_string;
		string wave_string;

		public Sound(string path) => Load(path);

		public byte[] Data { get; protected set; }
		public int SampleRate { get; protected set; }
		public ALFormat Format { get; protected set; }

		void Load(string file_path) {
			var path = File.Exists(file_path) ? file_path : string.Empty;
			if (path == string.Empty) throw new Exception($"Invalid image file path: {path}");

			var reader = new BinaryReader(File.OpenRead(path));

			riff_string = new string(reader.ReadChars(4));
			if (riff_string != "RIFF")
				throw new Exception("Invalid file header, can't read string: RIFF");

			file_length = reader.ReadInt32();

			wave_string = new string(reader.ReadChars(4));
			if (wave_string != "WAVE")
				throw new Exception("Invalid file header, can't read string: WAVE");

			fmt_string = new string(reader.ReadChars(4));
			if (fmt_string != "fmt ")
				throw new Exception("Invalid file header, can't read string: fmt");

			format_chunk_size = reader.ReadInt32();
			audio_format = reader.ReadInt16();
			channels = reader.ReadInt16();
			SampleRate = reader.ReadInt32();
			byte_rate = reader.ReadInt32();
			block_align = reader.ReadInt16();
			bits_per_sample = reader.ReadInt16();

			data_string = new string(reader.ReadChars(4));
			if (data_string != "data")
				throw new Exception("Invalid file header, can't read string: data");

			data_chunk_size = reader.ReadInt32();

			Data = reader.ReadBytes(data_chunk_size);

			switch (channels) {
				case 1:
					Format = bits_per_sample == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
					break;
				case 2:
					Format = bits_per_sample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
					break;
				default:
					Log.Panic("The specified sound format is not supported.");
					break;
			}
		}
	}
}