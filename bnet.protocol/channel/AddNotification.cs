using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class AddNotification : IProtoBuf
	{
		public bool HasSelf;

		private bnet.protocol.channel.Member _Self;

		private List<bnet.protocol.channel.Member> _Member = new List<bnet.protocol.channel.Member>();

		public bnet.protocol.channel.ChannelState ChannelState
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

		public List<bnet.protocol.channel.Member> Member
		{
			get
			{
				return this._Member;
			}
			set
			{
				this._Member = value;
			}
		}

		public int MemberCount
		{
			get
			{
				return this._Member.Count;
			}
		}

		public List<bnet.protocol.channel.Member> MemberList
		{
			get
			{
				return this._Member;
			}
		}

		public bnet.protocol.channel.Member Self
		{
			get
			{
				return this._Self;
			}
			set
			{
				this._Self = value;
				this.HasSelf = value != null;
			}
		}

		public AddNotification()
		{
		}

		public void AddMember(bnet.protocol.channel.Member val)
		{
			this._Member.Add(val);
		}

		public void ClearMember()
		{
			this._Member.Clear();
		}

		public void Deserialize(Stream stream)
		{
			AddNotification.Deserialize(stream, this);
		}

		public static AddNotification Deserialize(Stream stream, AddNotification instance)
		{
			return AddNotification.Deserialize(stream, instance, (long)-1);
		}

		public static AddNotification Deserialize(Stream stream, AddNotification instance, long limit)
		{
			if (instance.Member == null)
			{
				instance.Member = new List<bnet.protocol.channel.Member>();
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
						if (instance.Self != null)
						{
							bnet.protocol.channel.Member.DeserializeLengthDelimited(stream, instance.Self);
						}
						else
						{
							instance.Self = bnet.protocol.channel.Member.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 18)
					{
						instance.Member.Add(bnet.protocol.channel.Member.DeserializeLengthDelimited(stream));
					}
					else if (num != 26)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.ChannelState != null)
					{
						bnet.protocol.channel.ChannelState.DeserializeLengthDelimited(stream, instance.ChannelState);
					}
					else
					{
						instance.ChannelState = bnet.protocol.channel.ChannelState.DeserializeLengthDelimited(stream);
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

		public static AddNotification DeserializeLengthDelimited(Stream stream)
		{
			AddNotification addNotification = new AddNotification();
			AddNotification.DeserializeLengthDelimited(stream, addNotification);
			return addNotification;
		}

		public static AddNotification DeserializeLengthDelimited(Stream stream, AddNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AddNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AddNotification addNotification = obj as AddNotification;
			if (addNotification == null)
			{
				return false;
			}
			if (this.HasSelf != addNotification.HasSelf || this.HasSelf && !this.Self.Equals(addNotification.Self))
			{
				return false;
			}
			if (this.Member.Count != addNotification.Member.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Member.Count; i++)
			{
				if (!this.Member[i].Equals(addNotification.Member[i]))
				{
					return false;
				}
			}
			if (!this.ChannelState.Equals(addNotification.ChannelState))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasSelf)
			{
				hashCode ^= this.Self.GetHashCode();
			}
			foreach (bnet.protocol.channel.Member member in this.Member)
			{
				hashCode ^= member.GetHashCode();
			}
			hashCode ^= this.ChannelState.GetHashCode();
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasSelf)
			{
				num++;
				uint serializedSize = this.Self.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.Member.Count > 0)
			{
				foreach (bnet.protocol.channel.Member member in this.Member)
				{
					num++;
					uint serializedSize1 = member.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			uint num1 = this.ChannelState.GetSerializedSize();
			num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			num++;
			return num;
		}

		public static AddNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AddNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AddNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AddNotification instance)
		{
			if (instance.HasSelf)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.Self.GetSerializedSize());
				bnet.protocol.channel.Member.Serialize(stream, instance.Self);
			}
			if (instance.Member.Count > 0)
			{
				foreach (bnet.protocol.channel.Member member in instance.Member)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, member.GetSerializedSize());
					bnet.protocol.channel.Member.Serialize(stream, member);
				}
			}
			if (instance.ChannelState == null)
			{
				throw new ArgumentNullException("ChannelState", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteUInt32(stream, instance.ChannelState.GetSerializedSize());
			bnet.protocol.channel.ChannelState.Serialize(stream, instance.ChannelState);
		}

		public void SetChannelState(bnet.protocol.channel.ChannelState val)
		{
			this.ChannelState = val;
		}

		public void SetMember(List<bnet.protocol.channel.Member> val)
		{
			this.Member = val;
		}

		public void SetSelf(bnet.protocol.channel.Member val)
		{
			this.Self = val;
		}
	}
}