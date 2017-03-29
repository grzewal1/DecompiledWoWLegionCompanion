using System;
using System.IO;

namespace bnet.protocol.game_master
{
	public class FindGameResponse : IProtoBuf
	{
		public bool HasRequestId;

		private ulong _RequestId;

		public bool HasFactoryId;

		private ulong _FactoryId;

		public bool HasQueued;

		private bool _Queued;

		public ulong FactoryId
		{
			get
			{
				return this._FactoryId;
			}
			set
			{
				this._FactoryId = value;
				this.HasFactoryId = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool Queued
		{
			get
			{
				return this._Queued;
			}
			set
			{
				this._Queued = value;
				this.HasQueued = true;
			}
		}

		public ulong RequestId
		{
			get
			{
				return this._RequestId;
			}
			set
			{
				this._RequestId = value;
				this.HasRequestId = true;
			}
		}

		public FindGameResponse()
		{
		}

		public void Deserialize(Stream stream)
		{
			FindGameResponse.Deserialize(stream, this);
		}

		public static FindGameResponse Deserialize(Stream stream, FindGameResponse instance)
		{
			return FindGameResponse.Deserialize(stream, instance, (long)-1);
		}

		public static FindGameResponse Deserialize(Stream stream, FindGameResponse instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.Queued = false;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 9)
						{
							instance.RequestId = binaryReader.ReadUInt64();
						}
						else if (num1 == 17)
						{
							instance.FactoryId = binaryReader.ReadUInt64();
						}
						else if (num1 == 24)
						{
							instance.Queued = ProtocolParser.ReadBool(stream);
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
						if (limit >= (long)0)
						{
							throw new EndOfStreamException();
						}
						break;
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

		public static FindGameResponse DeserializeLengthDelimited(Stream stream)
		{
			FindGameResponse findGameResponse = new FindGameResponse();
			FindGameResponse.DeserializeLengthDelimited(stream, findGameResponse);
			return findGameResponse;
		}

		public static FindGameResponse DeserializeLengthDelimited(Stream stream, FindGameResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return FindGameResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			FindGameResponse findGameResponse = obj as FindGameResponse;
			if (findGameResponse == null)
			{
				return false;
			}
			if (this.HasRequestId != findGameResponse.HasRequestId || this.HasRequestId && !this.RequestId.Equals(findGameResponse.RequestId))
			{
				return false;
			}
			if (this.HasFactoryId != findGameResponse.HasFactoryId || this.HasFactoryId && !this.FactoryId.Equals(findGameResponse.FactoryId))
			{
				return false;
			}
			if (this.HasQueued == findGameResponse.HasQueued && (!this.HasQueued || this.Queued.Equals(findGameResponse.Queued)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasRequestId)
			{
				hashCode = hashCode ^ this.RequestId.GetHashCode();
			}
			if (this.HasFactoryId)
			{
				hashCode = hashCode ^ this.FactoryId.GetHashCode();
			}
			if (this.HasQueued)
			{
				hashCode = hashCode ^ this.Queued.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasRequestId)
			{
				num++;
				num = num + 8;
			}
			if (this.HasFactoryId)
			{
				num++;
				num = num + 8;
			}
			if (this.HasQueued)
			{
				num++;
				num++;
			}
			return num;
		}

		public static FindGameResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<FindGameResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			FindGameResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, FindGameResponse instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasRequestId)
			{
				stream.WriteByte(9);
				binaryWriter.Write(instance.RequestId);
			}
			if (instance.HasFactoryId)
			{
				stream.WriteByte(17);
				binaryWriter.Write(instance.FactoryId);
			}
			if (instance.HasQueued)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.Queued);
			}
		}

		public void SetFactoryId(ulong val)
		{
			this.FactoryId = val;
		}

		public void SetQueued(bool val)
		{
			this.Queued = val;
		}

		public void SetRequestId(ulong val)
		{
			this.RequestId = val;
		}
	}
}