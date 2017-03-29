using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.game_master
{
	public class ChangeGameRequest : IProtoBuf
	{
		public bool HasOpen;

		private bool _Open;

		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		public bool HasReplace;

		private bool _Replace;

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

		public bnet.protocol.game_master.GameHandle GameHandle
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

		public bool Open
		{
			get
			{
				return this._Open;
			}
			set
			{
				this._Open = value;
				this.HasOpen = true;
			}
		}

		public bool Replace
		{
			get
			{
				return this._Replace;
			}
			set
			{
				this._Replace = value;
				this.HasReplace = true;
			}
		}

		public ChangeGameRequest()
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
			ChangeGameRequest.Deserialize(stream, this);
		}

		public static ChangeGameRequest Deserialize(Stream stream, ChangeGameRequest instance)
		{
			return ChangeGameRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ChangeGameRequest Deserialize(Stream stream, ChangeGameRequest instance, long limit)
		{
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
			}
			instance.Replace = false;
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
							if (instance.GameHandle != null)
							{
								bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream, instance.GameHandle);
							}
							else
							{
								instance.GameHandle = bnet.protocol.game_master.GameHandle.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 16)
						{
							instance.Open = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 26)
						{
							instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 32)
						{
							instance.Replace = ProtocolParser.ReadBool(stream);
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

		public static ChangeGameRequest DeserializeLengthDelimited(Stream stream)
		{
			ChangeGameRequest changeGameRequest = new ChangeGameRequest();
			ChangeGameRequest.DeserializeLengthDelimited(stream, changeGameRequest);
			return changeGameRequest;
		}

		public static ChangeGameRequest DeserializeLengthDelimited(Stream stream, ChangeGameRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ChangeGameRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChangeGameRequest changeGameRequest = obj as ChangeGameRequest;
			if (changeGameRequest == null)
			{
				return false;
			}
			if (!this.GameHandle.Equals(changeGameRequest.GameHandle))
			{
				return false;
			}
			if (this.HasOpen != changeGameRequest.HasOpen || this.HasOpen && !this.Open.Equals(changeGameRequest.Open))
			{
				return false;
			}
			if (this.Attribute.Count != changeGameRequest.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(changeGameRequest.Attribute[i]))
				{
					return false;
				}
			}
			if (this.HasReplace == changeGameRequest.HasReplace && (!this.HasReplace || this.Replace.Equals(changeGameRequest.Replace)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.GameHandle.GetHashCode();
			if (this.HasOpen)
			{
				hashCode = hashCode ^ this.Open.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode = hashCode ^ attribute.GetHashCode();
			}
			if (this.HasReplace)
			{
				hashCode = hashCode ^ this.Replace.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.GameHandle.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.HasOpen)
			{
				num++;
				num++;
			}
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize1 = attribute.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.HasReplace)
			{
				num++;
				num++;
			}
			num++;
			return num;
		}

		public static ChangeGameRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChangeGameRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChangeGameRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChangeGameRequest instance)
		{
			if (instance.GameHandle == null)
			{
				throw new ArgumentNullException("GameHandle", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.GameHandle.GetSerializedSize());
			bnet.protocol.game_master.GameHandle.Serialize(stream, instance.GameHandle);
			if (instance.HasOpen)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.Open);
			}
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
			if (instance.HasReplace)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.Replace);
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetGameHandle(bnet.protocol.game_master.GameHandle val)
		{
			this.GameHandle = val;
		}

		public void SetOpen(bool val)
		{
			this.Open = val;
		}

		public void SetReplace(bool val)
		{
			this.Replace = val;
		}
	}
}