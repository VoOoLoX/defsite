using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common;

public class VectorTupleConverter : JsonConverterFactory {
	public override bool CanConvert(Type type_to_convert) {
		var i_tuple = type_to_convert.GetInterface("System.Runtime.CompilerServices.ITuple");
		return i_tuple != null;
	}

	public override JsonConverter CreateConverter(Type type_to_convert, JsonSerializerOptions options) {
		var generic_arguments = type_to_convert.GetGenericArguments();

		var converter_type = generic_arguments.Length switch {
			1 => typeof(VectorConverter<>).MakeGenericType(generic_arguments),
			2 => typeof(VectorConverter<,>).MakeGenericType(generic_arguments),
			3 => typeof(VectorConverter<,,>).MakeGenericType(generic_arguments),
			4 => typeof(VectorConverter<,,,>).MakeGenericType(generic_arguments),
			_ => throw new NotSupportedException(),
		};
		return (JsonConverter)Activator.CreateInstance(converter_type);
	}
}

public class VectorConverter<T1> : JsonConverter<ValueTuple<T1>> {
	public override ValueTuple<T1> Read(ref Utf8JsonReader reader, Type type_to_convert, JsonSerializerOptions options) {
		ValueTuple<T1> result = default;

		if(!reader.Read()) {
			throw new JsonException();
		}

		while(reader.TokenType != JsonTokenType.EndObject) {
			result.Item1 = reader.ValueTextEquals("X") && reader.Read()
				? JsonSerializer.Deserialize<T1>(ref reader, options)
				: throw new JsonException();
			reader.Read();
		}

		return result;
	}

	public override void Write(Utf8JsonWriter writer, ValueTuple<T1> value, JsonSerializerOptions options) {
		writer.WriteStartObject();
		writer.WritePropertyName("X");
		JsonSerializer.Serialize(writer, value.Item1, options);
		writer.WriteEndObject();
	}
}

public class VectorConverter<T1, T2> : JsonConverter<ValueTuple<T1, T2>> {
	public override (T1, T2) Read(ref Utf8JsonReader reader, Type type_to_convert, JsonSerializerOptions options) {
		(T1, T2) result = default;

		if(!reader.Read()) {
			throw new JsonException();
		}

		while(reader.TokenType != JsonTokenType.EndObject) {
			if(reader.ValueTextEquals("X") && reader.Read()) {
				result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
			} else {
				result.Item2 = reader.ValueTextEquals("Y") && reader.Read()
					? JsonSerializer.Deserialize<T2>(ref reader, options)
					: throw new JsonException();
			}
			reader.Read();
		}

		return result;
	}

	public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options) {
		writer.WriteStartObject();
		writer.WritePropertyName("X");
		JsonSerializer.Serialize(writer, value.Item1, options);
		writer.WritePropertyName("Y");
		JsonSerializer.Serialize(writer, value.Item2, options);
		writer.WriteEndObject();
	}
}

public class VectorConverter<T1, T2, T3> : JsonConverter<ValueTuple<T1, T2, T3>> {
	public override (T1, T2, T3) Read(ref Utf8JsonReader reader, Type type_to_convert, JsonSerializerOptions options) {
		(T1, T2, T3) result = default;

		if(!reader.Read()) {
			throw new JsonException();
		}

		while(reader.TokenType != JsonTokenType.EndObject) {
			if(reader.ValueTextEquals("X") && reader.Read()) {
				result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
			} else if(reader.ValueTextEquals("Y") && reader.Read()) {
				result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
			} else {
				result.Item3 = reader.ValueTextEquals("Z") && reader.Read()
					? JsonSerializer.Deserialize<T3>(ref reader, options)
					: throw new JsonException();
			}
			reader.Read();
		}

		return result;
	}

	public override void Write(Utf8JsonWriter writer, (T1, T2, T3) value, JsonSerializerOptions options) {
		writer.WriteStartObject();
		writer.WritePropertyName("X");
		JsonSerializer.Serialize(writer, value.Item1, options);
		writer.WritePropertyName("Y");
		JsonSerializer.Serialize(writer, value.Item2, options);
		writer.WritePropertyName("Z");
		JsonSerializer.Serialize(writer, value.Item3, options);
		writer.WriteEndObject();
	}
}

public class VectorConverter<T1, T2, T3, T4> : JsonConverter<ValueTuple<T1, T2, T3, T4>> {
	public override (T1, T2, T3, T4) Read(ref Utf8JsonReader reader, Type type_to_convert, JsonSerializerOptions options) {
		(T1, T2, T3, T4) result = default;

		if(!reader.Read()) {
			throw new JsonException();
		}

		while(reader.TokenType != JsonTokenType.EndObject) {
			if(reader.ValueTextEquals("X") && reader.Read()) {
				result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
			} else if(reader.ValueTextEquals("Y") && reader.Read()) {
				result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
			} else if(reader.ValueTextEquals("Z") && reader.Read()) {
				result.Item3 = JsonSerializer.Deserialize<T3>(ref reader, options);
			} else {
				result.Item4 = reader.ValueTextEquals("W") && reader.Read()
					? JsonSerializer.Deserialize<T4>(ref reader, options)
					: throw new JsonException();
			}
			reader.Read();
		}

		return result;
	}

	public override void Write(Utf8JsonWriter writer, (T1, T2, T3, T4) value, JsonSerializerOptions options) {
		writer.WriteStartObject();
		writer.WritePropertyName("X");
		JsonSerializer.Serialize(writer, value.Item1, options);
		writer.WritePropertyName("Y");
		JsonSerializer.Serialize(writer, value.Item2, options);
		writer.WritePropertyName("Z");
		JsonSerializer.Serialize(writer, value.Item3, options);
		writer.WritePropertyName("W");
		JsonSerializer.Serialize(writer, value.Item4, options);
		writer.WriteEndObject();
	}
}