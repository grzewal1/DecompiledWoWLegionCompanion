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
		T t;
		if (length == -1)
		{
			length = (int)bytes.Length;
		}
		MemoryStream memoryStream = new MemoryStream(bytes, offset, length);
		T t1 = default(T);
		if (t1 == null)
		{
			t = Activator.CreateInstance<T>();
		}
		else
		{
			t1 = default(T);
			t = t1;
		}
		T t2 = t;
		t2.Deserialize(memoryStream);
		return t2;
	}

	public static IProtoBuf ParseFromGeneric<T>(byte[] bytes)
	where T : IProtoBuf, new()
	{
		T t;
		MemoryStream memoryStream = new MemoryStream(bytes);
		T t1 = default(T);
		if (t1 == null)
		{
			t = Activator.CreateInstance<T>();
		}
		else
		{
			t1 = default(T);
			t = t1;
		}
		T t2 = t;
		t2.Deserialize(memoryStream);
		return (object)t2;
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