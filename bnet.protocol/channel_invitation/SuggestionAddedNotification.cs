using bnet.protocol.invitation;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel_invitation
{
	public class SuggestionAddedNotification : IProtoBuf
	{
		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bnet.protocol.invitation.Suggestion Suggestion
		{
			get;
			set;
		}

		public SuggestionAddedNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			SuggestionAddedNotification.Deserialize(stream, this);
		}

		public static SuggestionAddedNotification Deserialize(Stream stream, SuggestionAddedNotification instance)
		{
			return SuggestionAddedNotification.Deserialize(stream, instance, (long)-1);
		}

		public static SuggestionAddedNotification Deserialize(Stream stream, SuggestionAddedNotification instance, long limit)
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
					else if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.Suggestion != null)
					{
						bnet.protocol.invitation.Suggestion.DeserializeLengthDelimited(stream, instance.Suggestion);
					}
					else
					{
						instance.Suggestion = bnet.protocol.invitation.Suggestion.DeserializeLengthDelimited(stream);
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

		public static SuggestionAddedNotification DeserializeLengthDelimited(Stream stream)
		{
			SuggestionAddedNotification suggestionAddedNotification = new SuggestionAddedNotification();
			SuggestionAddedNotification.DeserializeLengthDelimited(stream, suggestionAddedNotification);
			return suggestionAddedNotification;
		}

		public static SuggestionAddedNotification DeserializeLengthDelimited(Stream stream, SuggestionAddedNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return SuggestionAddedNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SuggestionAddedNotification suggestionAddedNotification = obj as SuggestionAddedNotification;
			if (suggestionAddedNotification == null)
			{
				return false;
			}
			if (!this.Suggestion.Equals(suggestionAddedNotification.Suggestion))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			return hashCode ^ this.Suggestion.GetHashCode();
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Suggestion.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			return num + 1;
		}

		public static SuggestionAddedNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SuggestionAddedNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SuggestionAddedNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SuggestionAddedNotification instance)
		{
			if (instance.Suggestion == null)
			{
				throw new ArgumentNullException("Suggestion", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Suggestion.GetSerializedSize());
			bnet.protocol.invitation.Suggestion.Serialize(stream, instance.Suggestion);
		}

		public void SetSuggestion(bnet.protocol.invitation.Suggestion val)
		{
			this.Suggestion = val;
		}
	}
}