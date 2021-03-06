using bnet.protocol.attribute;
using bnet.protocol.server_pool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class RegisterUtilitiesRequest : IProtoBuf
	{
		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		public bool HasState;

		private ServerState _State;

		public List<bnet.protocol.attribute.Attribute> Attribute
		{
			get
			{
				return this._Attribute;
			}
			set
			{
				this._Attribute = value;
			}
		}

		public int AttributeCount
		{
			get
			{
				return this._Attribute.Count;
			}
		}

		public List<bnet.protocol.attribute.Attribute> AttributeList
		{
			get
			{
				return this._Attribute;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint ProgramId
		{
			get;
			set;
		}

		public ServerState State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
				this.HasState = value != null;
			}
		}

		public RegisterUtilitiesRequest()
		{
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void Deserialize(Stream stream)
		{
			RegisterUtilitiesRequest.Deserialize(stream, this);
		}

		public static RegisterUtilitiesRequest Deserialize(Stream stream, RegisterUtilitiesRequest instance)
		{
			return RegisterUtilitiesRequest.Deserialize(stream, instance, (long)-1);
		}

		public static RegisterUtilitiesRequest Deserialize(Stream stream, RegisterUtilitiesRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
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
						instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
					}
					else if (num != 18)
					{
						if (num == 29)
						{
							instance.ProgramId = binaryReader.ReadUInt32();
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
					else if (instance.State != null)
					{
						ServerState.DeserializeLengthDelimited(stream, instance.State);
					}
					else
					{
						instance.State = ServerState.DeserializeLengthDelimited(stream);
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

		public static RegisterUtilitiesRequest DeserializeLengthDelimited(Stream stream)
		{
			RegisterUtilitiesRequest registerUtilitiesRequest = new RegisterUtilitiesRequest();
			RegisterUtilitiesRequest.DeserializeLengthDelimited(stream, registerUtilitiesRequest);
			return registerUtilitiesRequest;
		}

		public static RegisterUtilitiesRequest DeserializeLengthDelimited(Stream stream, RegisterUtilitiesRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return RegisterUtilitiesRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			RegisterUtilitiesRequest registerUtilitiesRequest = obj as RegisterUtilitiesRequest;
			if (registerUtilitiesRequest == null)
			{
				return false;
			}
			if (this.Attribute.Count != registerUtilitiesRequest.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(registerUtilitiesRequest.Attribute[i]))
				{
					return false;
				}
			}
			if (this.HasState != registerUtilitiesRequest.HasState || this.HasState && !this.State.Equals(registerUtilitiesRequest.State))
			{
				return false;
			}
			if (!this.ProgramId.Equals(registerUtilitiesRequest.ProgramId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode ^= attribute.GetHashCode();
			}
			if (this.HasState)
			{
				hashCode ^= this.State.GetHashCode();
			}
			hashCode ^= this.ProgramId.GetHashCode();
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize = attribute.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasState)
			{
				num++;
				uint serializedSize1 = this.State.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			num += 4;
			num++;
			return num;
		}

		public static RegisterUtilitiesRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<RegisterUtilitiesRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			RegisterUtilitiesRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, RegisterUtilitiesRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
			if (instance.HasState)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.State.GetSerializedSize());
				ServerState.Serialize(stream, instance.State);
			}
			stream.WriteByte(29);
			binaryWriter.Write(instance.ProgramId);
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetProgramId(uint val)
		{
			this.ProgramId = val;
		}

		public void SetState(ServerState val)
		{
			this.State = val;
		}
	}
}