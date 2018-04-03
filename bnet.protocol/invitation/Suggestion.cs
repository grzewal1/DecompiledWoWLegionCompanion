using bnet.protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.invitation
{
	public class Suggestion : IProtoBuf
	{
		public bool HasChannelId;

		private EntityId _ChannelId;

		public bool HasSuggesterName;

		private string _SuggesterName;

		public bool HasSuggesteeName;

		private string _SuggesteeName;

		public bool HasSuggesterAccountId;

		private EntityId _SuggesterAccountId;

		public EntityId ChannelId
		{
			get
			{
				return this._ChannelId;
			}
			set
			{
				this._ChannelId = value;
				this.HasChannelId = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public EntityId SuggesteeId
		{
			get;
			set;
		}

		public string SuggesteeName
		{
			get
			{
				return this._SuggesteeName;
			}
			set
			{
				this._SuggesteeName = value;
				this.HasSuggesteeName = value != null;
			}
		}

		public EntityId SuggesterAccountId
		{
			get
			{
				return this._SuggesterAccountId;
			}
			set
			{
				this._SuggesterAccountId = value;
				this.HasSuggesterAccountId = value != null;
			}
		}

		public EntityId SuggesterId
		{
			get;
			set;
		}

		public string SuggesterName
		{
			get
			{
				return this._SuggesterName;
			}
			set
			{
				this._SuggesterName = value;
				this.HasSuggesterName = value != null;
			}
		}

		public Suggestion()
		{
		}

		public void Deserialize(Stream stream)
		{
			Suggestion.Deserialize(stream, this);
		}

		public static Suggestion Deserialize(Stream stream, Suggestion instance)
		{
			return Suggestion.Deserialize(stream, instance, (long)-1);
		}

		public static Suggestion Deserialize(Stream stream, Suggestion instance, long limit)
		{
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
						if (instance.ChannelId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.ChannelId);
						}
						else
						{
							instance.ChannelId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 18)
					{
						if (instance.SuggesterId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.SuggesterId);
						}
						else
						{
							instance.SuggesterId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 26)
					{
						if (instance.SuggesteeId != null)
						{
							EntityId.DeserializeLengthDelimited(stream, instance.SuggesteeId);
						}
						else
						{
							instance.SuggesteeId = EntityId.DeserializeLengthDelimited(stream);
						}
					}
					else if (num == 34)
					{
						instance.SuggesterName = ProtocolParser.ReadString(stream);
					}
					else if (num == 42)
					{
						instance.SuggesteeName = ProtocolParser.ReadString(stream);
					}
					else if (num != 50)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.SuggesterAccountId != null)
					{
						EntityId.DeserializeLengthDelimited(stream, instance.SuggesterAccountId);
					}
					else
					{
						instance.SuggesterAccountId = EntityId.DeserializeLengthDelimited(stream);
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

		public static Suggestion DeserializeLengthDelimited(Stream stream)
		{
			Suggestion suggestion = new Suggestion();
			Suggestion.DeserializeLengthDelimited(stream, suggestion);
			return suggestion;
		}

		public static Suggestion DeserializeLengthDelimited(Stream stream, Suggestion instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Suggestion.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Suggestion suggestion = obj as Suggestion;
			if (suggestion == null)
			{
				return false;
			}
			if (this.HasChannelId != suggestion.HasChannelId || this.HasChannelId && !this.ChannelId.Equals(suggestion.ChannelId))
			{
				return false;
			}
			if (!this.SuggesterId.Equals(suggestion.SuggesterId))
			{
				return false;
			}
			if (!this.SuggesteeId.Equals(suggestion.SuggesteeId))
			{
				return false;
			}
			if (this.HasSuggesterName != suggestion.HasSuggesterName || this.HasSuggesterName && !this.SuggesterName.Equals(suggestion.SuggesterName))
			{
				return false;
			}
			if (this.HasSuggesteeName != suggestion.HasSuggesteeName || this.HasSuggesteeName && !this.SuggesteeName.Equals(suggestion.SuggesteeName))
			{
				return false;
			}
			if (this.HasSuggesterAccountId == suggestion.HasSuggesterAccountId && (!this.HasSuggesterAccountId || this.SuggesterAccountId.Equals(suggestion.SuggesterAccountId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasChannelId)
			{
				hashCode ^= this.ChannelId.GetHashCode();
			}
			hashCode ^= this.SuggesterId.GetHashCode();
			hashCode ^= this.SuggesteeId.GetHashCode();
			if (this.HasSuggesterName)
			{
				hashCode ^= this.SuggesterName.GetHashCode();
			}
			if (this.HasSuggesteeName)
			{
				hashCode ^= this.SuggesteeName.GetHashCode();
			}
			if (this.HasSuggesterAccountId)
			{
				hashCode ^= this.SuggesterAccountId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasChannelId)
			{
				num++;
				uint serializedSize = this.ChannelId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			uint serializedSize1 = this.SuggesterId.GetSerializedSize();
			num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			uint num1 = this.SuggesteeId.GetSerializedSize();
			num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			if (this.HasSuggesterName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.SuggesterName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasSuggesteeName)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.SuggesteeName);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasSuggesterAccountId)
			{
				num++;
				uint serializedSize2 = this.SuggesterAccountId.GetSerializedSize();
				num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
			}
			num += 2;
			return num;
		}

		public static Suggestion ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Suggestion>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Suggestion.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Suggestion instance)
		{
			if (instance.HasChannelId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.ChannelId.GetSerializedSize());
				EntityId.Serialize(stream, instance.ChannelId);
			}
			if (instance.SuggesterId == null)
			{
				throw new ArgumentNullException("SuggesterId", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteUInt32(stream, instance.SuggesterId.GetSerializedSize());
			EntityId.Serialize(stream, instance.SuggesterId);
			if (instance.SuggesteeId == null)
			{
				throw new ArgumentNullException("SuggesteeId", "Required by proto specification.");
			}
			stream.WriteByte(26);
			ProtocolParser.WriteUInt32(stream, instance.SuggesteeId.GetSerializedSize());
			EntityId.Serialize(stream, instance.SuggesteeId);
			if (instance.HasSuggesterName)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.SuggesterName));
			}
			if (instance.HasSuggesteeName)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.SuggesteeName));
			}
			if (instance.HasSuggesterAccountId)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteUInt32(stream, instance.SuggesterAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.SuggesterAccountId);
			}
		}

		public void SetChannelId(EntityId val)
		{
			this.ChannelId = val;
		}

		public void SetSuggesteeId(EntityId val)
		{
			this.SuggesteeId = val;
		}

		public void SetSuggesteeName(string val)
		{
			this.SuggesteeName = val;
		}

		public void SetSuggesterAccountId(EntityId val)
		{
			this.SuggesterAccountId = val;
		}

		public void SetSuggesterId(EntityId val)
		{
			this.SuggesterId = val;
		}

		public void SetSuggesterName(string val)
		{
			this.SuggesterName = val;
		}
	}
}