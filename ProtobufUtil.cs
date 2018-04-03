using System;
using System.IO;

public class ProtobufUtil
{
	public ProtobufUtil()
	{
	}

	public static T ParseFrom<T>(byte[] bytes, int offset = 0, int length = -1)
	where T : IProtoBuf, new()
	{
		if (length == -1)
		{
			length = (int)bytes.Length;
		}
		MemoryStream memoryStream = new MemoryStream(bytes, offset, length);
		T t = Activator.CreateInstance<T>();
		t.Deserialize(memoryStream);
		return t;
	}

	public static IProtoBuf ParseFromGeneric<T>(byte[] bytes)
	where T : IProtoBuf, new()
	{
		MemoryStream memoryStream = new MemoryStream(bytes);
		T t = Activator.CreateInstance<T>();
		t.Deserialize(memoryStream);
		return (object)t;
	}

	public static byte[] ToByteArray(IProtoBuf protobuf)
	{
		unsafe
		{
			byte[] numArray = new byte[protobuf.GetSerializedSize()];
			protobuf.Serialize(new MemoryStream(numArray));
			return numArray;
		}
	}
}