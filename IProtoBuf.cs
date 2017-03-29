using System;
using System.IO;

public interface IProtoBuf
{
	void Deserialize(Stream stream);

	uint GetSerializedSize();

	void Serialize(Stream stream);
}