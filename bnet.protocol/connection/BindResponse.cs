using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.connection
{
	public class BindResponse : IProtoBuf
	{
		private List<uint> _ImportedServiceId = new List<uint>();

		public List<uint> ImportedServiceId
		{
			get
			{
				return this._ImportedServiceId;
			}
			set
			{
				this._ImportedServiceId = value;
			}
		}

		public int ImportedServiceIdCount
		{
			get
			{
				return this._ImportedServiceId.Count;
			}
		}

		public List<uint> ImportedServiceIdList
		{
			get
			{
				return this._ImportedServiceId;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public BindResponse()
		{
		}

		public void AddImportedServiceId(uint val)
		{
			this._ImportedServiceId.Add(val);
		}

		public void ClearImportedServiceId()
		{
			this._ImportedServiceId.Clear();
		}

		public void Deserialize(Stream stream)
		{
			BindResponse.Deserialize(stream, this);
		}

		public static BindResponse Deserialize(Stream stream, BindResponse instance)
		{
			return BindResponse.Deserialize(stream, instance, (long)-1);
		}

		public static BindResponse Deserialize(Stream stream, BindResponse instance, long limit)
		{
			if (instance.ImportedServiceId == null)
			{
				instance.ImportedServiceId = new List<uint>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						if (limit >= (long)0)
						{
							throw new EndOfStreamException();
						}
						break;
					}
					else if (num == 10)
					{
						long position = (long)ProtocolParser.ReadUInt32(stream);
						position += stream.Position;
						while (stream.Position < position)
						{
							instance.ImportedServiceId.Add(ProtocolParser.ReadUInt32(stream));
						}
						if (stream.Position != position)
						{
							throw new ProtocolBufferException("Read too many bytes in packed data");
						}
					}
					else
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
				}
				else
				{
					if (stream.Position != limit)
					{
						throw new ProtocolBufferException("Read past max limit");
					}
					break;
				}
			}
			return instance;
		}

		public static BindResponse DeserializeLengthDelimited(Stream stream)
		{
			BindResponse bindResponse = new BindResponse();
			BindResponse.DeserializeLengthDelimited(stream, bindResponse);
			return bindResponse;
		}

		public static BindResponse DeserializeLengthDelimited(Stream stream, BindResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return BindResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			BindResponse bindResponse = obj as BindResponse;
			if (bindResponse == null)
			{
				return false;
			}
			if (this.ImportedServiceId.Count != bindResponse.ImportedServiceId.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ImportedServiceId.Count; i++)
			{
				if (!this.ImportedServiceId[i].Equals(bindResponse.ImportedServiceId[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (uint importedServiceId in this.ImportedServiceId)
			{
				hashCode ^= importedServiceId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.ImportedServiceId.Count > 0)
			{
				num++;
				uint num1 = num;
				foreach (uint importedServiceId in this.ImportedServiceId)
				{
					num += ProtocolParser.SizeOfUInt32(importedServiceId);
				}
				num += ProtocolParser.SizeOfUInt32(num - num1);
			}
			return num;
		}

		public static BindResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<BindResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			BindResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, BindResponse instance)
		{
			if (instance.ImportedServiceId.Count > 0)
			{
				stream.WriteByte(10);
				uint num = 0;
				foreach (uint importedServiceId in instance.ImportedServiceId)
				{
					num += ProtocolParser.SizeOfUInt32(importedServiceId);
				}
				ProtocolParser.WriteUInt32(stream, num);
				foreach (uint importedServiceId1 in instance.ImportedServiceId)
				{
					ProtocolParser.WriteUInt32(stream, importedServiceId1);
				}
			}
		}

		public void SetImportedServiceId(List<uint> val)
		{
			this.ImportedServiceId = val;
		}
	}
}