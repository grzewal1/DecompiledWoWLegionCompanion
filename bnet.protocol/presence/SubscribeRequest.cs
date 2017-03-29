using bnet.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.presence
{
	public class SubscribeRequest : IProtoBuf
	{
		public bool HasAgentId;

		private bnet.protocol.EntityId _AgentId;

		private List<uint> _ProgramId = new List<uint>();

		public bnet.protocol.EntityId AgentId
		{
			get
			{
				return this._AgentId;
			}
			set
			{
				this._AgentId = value;
				this.HasAgentId = value != null;
			}
		}

		public bnet.protocol.EntityId EntityId
		{
			get;
			set;
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ulong ObjectId
		{
			get;
			set;
		}

		public List<uint> ProgramId
		{
			get
			{
				return this._ProgramId;
			}
			set
			{
				this._ProgramId = value;
			}
		}

		public int ProgramIdCount
		{
			get
			{
				return this._ProgramId.Count;
			}
		}

		public List<uint> ProgramIdList
		{
			get
			{
				return this._ProgramId;
			}
		}

		public SubscribeRequest()
		{
		}

		public void AddProgramId(uint val)
		{
			this._ProgramId.Add(val);
		}

		public void ClearProgramId()
		{
			this._ProgramId.Clear();
		}

		public void Deserialize(Stream stream)
		{
			SubscribeRequest.Deserialize(stream, this);
		}

		public static SubscribeRequest Deserialize(Stream stream, SubscribeRequest instance)
		{
			return SubscribeRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SubscribeRequest Deserialize(Stream stream, SubscribeRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.ProgramId == null)
			{
				instance.ProgramId = new List<uint>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 10)
						{
							if (instance.AgentId != null)
							{
								bnet.protocol.EntityId.DeserializeLengthDelimited(stream, instance.AgentId);
							}
							else
							{
								instance.AgentId = bnet.protocol.EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 18)
						{
							if (instance.EntityId != null)
							{
								bnet.protocol.EntityId.DeserializeLengthDelimited(stream, instance.EntityId);
							}
							else
							{
								instance.EntityId = bnet.protocol.EntityId.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 24)
						{
							instance.ObjectId = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 37)
						{
							instance.ProgramId.Add(binaryReader.ReadUInt32());
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

		public static SubscribeRequest DeserializeLengthDelimited(Stream stream)
		{
			SubscribeRequest subscribeRequest = new SubscribeRequest();
			SubscribeRequest.DeserializeLengthDelimited(stream, subscribeRequest);
			return subscribeRequest;
		}

		public static SubscribeRequest DeserializeLengthDelimited(Stream stream, SubscribeRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return SubscribeRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SubscribeRequest subscribeRequest = obj as SubscribeRequest;
			if (subscribeRequest == null)
			{
				return false;
			}
			if (this.HasAgentId != subscribeRequest.HasAgentId || this.HasAgentId && !this.AgentId.Equals(subscribeRequest.AgentId))
			{
				return false;
			}
			if (!this.EntityId.Equals(subscribeRequest.EntityId))
			{
				return false;
			}
			if (!this.ObjectId.Equals(subscribeRequest.ObjectId))
			{
				return false;
			}
			if (this.ProgramId.Count != subscribeRequest.ProgramId.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ProgramId.Count; i++)
			{
				if (!this.ProgramId[i].Equals(subscribeRequest.ProgramId[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAgentId)
			{
				hashCode = hashCode ^ this.AgentId.GetHashCode();
			}
			hashCode = hashCode ^ this.EntityId.GetHashCode();
			hashCode = hashCode ^ this.ObjectId.GetHashCode();
			foreach (uint programId in this.ProgramId)
			{
				hashCode = hashCode ^ programId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAgentId)
			{
				num++;
				uint serializedSize = this.AgentId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			uint serializedSize1 = this.EntityId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			num = num + ProtocolParser.SizeOfUInt64(this.ObjectId);
			if (this.ProgramId.Count > 0)
			{
				foreach (uint programId in this.ProgramId)
				{
					num++;
					num = num + 4;
				}
			}
			num = num + 2;
			return num;
		}

		public static SubscribeRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SubscribeRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SubscribeRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SubscribeRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasAgentId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.AgentId.GetSerializedSize());
				bnet.protocol.EntityId.Serialize(stream, instance.AgentId);
			}
			if (instance.EntityId == null)
			{
				throw new ArgumentNullException("EntityId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.EntityId.GetSerializedSize());
			bnet.protocol.EntityId.Serialize(stream, instance.EntityId);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			if (instance.ProgramId.Count > 0)
			{
				foreach (uint programId in instance.ProgramId)
				{
					stream.WriteByte(37);
					binaryWriter.Write(programId);
				}
			}
		}

		public void SetAgentId(bnet.protocol.EntityId val)
		{
			this.AgentId = val;
		}

		public void SetEntityId(bnet.protocol.EntityId val)
		{
			this.EntityId = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}

		public void SetProgramId(List<uint> val)
		{
			this.ProgramId = val;
		}
	}
}