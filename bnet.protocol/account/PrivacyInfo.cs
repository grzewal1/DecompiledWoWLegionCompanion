using System;
using System.IO;

namespace bnet.protocol.account
{
	public class PrivacyInfo : IProtoBuf
	{
		public bool HasIsUsingRid;

		private bool _IsUsingRid;

		public bool HasIsRealIdVisibleForViewFriends;

		private bool _IsRealIdVisibleForViewFriends;

		public bool HasIsHiddenFromFriendFinder;

		private bool _IsHiddenFromFriendFinder;

		public bool HasGameInfoPrivacy;

		private PrivacyInfo.Types.GameInfoPrivacy _GameInfoPrivacy;

		public PrivacyInfo.Types.GameInfoPrivacy GameInfoPrivacy
		{
			get
			{
				return this._GameInfoPrivacy;
			}
			set
			{
				this._GameInfoPrivacy = value;
				this.HasGameInfoPrivacy = true;
			}
		}

		public bool IsHiddenFromFriendFinder
		{
			get
			{
				return this._IsHiddenFromFriendFinder;
			}
			set
			{
				this._IsHiddenFromFriendFinder = value;
				this.HasIsHiddenFromFriendFinder = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool IsRealIdVisibleForViewFriends
		{
			get
			{
				return this._IsRealIdVisibleForViewFriends;
			}
			set
			{
				this._IsRealIdVisibleForViewFriends = value;
				this.HasIsRealIdVisibleForViewFriends = true;
			}
		}

		public bool IsUsingRid
		{
			get
			{
				return this._IsUsingRid;
			}
			set
			{
				this._IsUsingRid = value;
				this.HasIsUsingRid = true;
			}
		}

		public PrivacyInfo()
		{
		}

		public void Deserialize(Stream stream)
		{
			PrivacyInfo.Deserialize(stream, this);
		}

		public static PrivacyInfo Deserialize(Stream stream, PrivacyInfo instance)
		{
			return PrivacyInfo.Deserialize(stream, instance, (long)-1);
		}

		public static PrivacyInfo Deserialize(Stream stream, PrivacyInfo instance, long limit)
		{
			instance.GameInfoPrivacy = PrivacyInfo.Types.GameInfoPrivacy.PRIVACY_FRIENDS;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 24)
						{
							instance.IsUsingRid = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 32)
						{
							instance.IsRealIdVisibleForViewFriends = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 40)
						{
							instance.IsHiddenFromFriendFinder = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 48)
						{
							instance.GameInfoPrivacy = (PrivacyInfo.Types.GameInfoPrivacy)((int)ProtocolParser.ReadUInt64(stream));
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

		public static PrivacyInfo DeserializeLengthDelimited(Stream stream)
		{
			PrivacyInfo privacyInfo = new PrivacyInfo();
			PrivacyInfo.DeserializeLengthDelimited(stream, privacyInfo);
			return privacyInfo;
		}

		public static PrivacyInfo DeserializeLengthDelimited(Stream stream, PrivacyInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return PrivacyInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			PrivacyInfo privacyInfo = obj as PrivacyInfo;
			if (privacyInfo == null)
			{
				return false;
			}
			if (this.HasIsUsingRid != privacyInfo.HasIsUsingRid || this.HasIsUsingRid && !this.IsUsingRid.Equals(privacyInfo.IsUsingRid))
			{
				return false;
			}
			if (this.HasIsRealIdVisibleForViewFriends != privacyInfo.HasIsRealIdVisibleForViewFriends || this.HasIsRealIdVisibleForViewFriends && !this.IsRealIdVisibleForViewFriends.Equals(privacyInfo.IsRealIdVisibleForViewFriends))
			{
				return false;
			}
			if (this.HasIsHiddenFromFriendFinder != privacyInfo.HasIsHiddenFromFriendFinder || this.HasIsHiddenFromFriendFinder && !this.IsHiddenFromFriendFinder.Equals(privacyInfo.IsHiddenFromFriendFinder))
			{
				return false;
			}
			if (this.HasGameInfoPrivacy == privacyInfo.HasGameInfoPrivacy && (!this.HasGameInfoPrivacy || this.GameInfoPrivacy.Equals(privacyInfo.GameInfoPrivacy)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasIsUsingRid)
			{
				hashCode ^= this.IsUsingRid.GetHashCode();
			}
			if (this.HasIsRealIdVisibleForViewFriends)
			{
				hashCode ^= this.IsRealIdVisibleForViewFriends.GetHashCode();
			}
			if (this.HasIsHiddenFromFriendFinder)
			{
				hashCode ^= this.IsHiddenFromFriendFinder.GetHashCode();
			}
			if (this.HasGameInfoPrivacy)
			{
				hashCode ^= this.GameInfoPrivacy.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasIsUsingRid)
			{
				num++;
				num++;
			}
			if (this.HasIsRealIdVisibleForViewFriends)
			{
				num++;
				num++;
			}
			if (this.HasIsHiddenFromFriendFinder)
			{
				num++;
				num++;
			}
			if (this.HasGameInfoPrivacy)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64((ulong)this.GameInfoPrivacy);
			}
			return num;
		}

		public static PrivacyInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<PrivacyInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			PrivacyInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, PrivacyInfo instance)
		{
			if (instance.HasIsUsingRid)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.IsUsingRid);
			}
			if (instance.HasIsRealIdVisibleForViewFriends)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.IsRealIdVisibleForViewFriends);
			}
			if (instance.HasIsHiddenFromFriendFinder)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.IsHiddenFromFriendFinder);
			}
			if (instance.HasGameInfoPrivacy)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.GameInfoPrivacy);
			}
		}

		public void SetGameInfoPrivacy(PrivacyInfo.Types.GameInfoPrivacy val)
		{
			this.GameInfoPrivacy = val;
		}

		public void SetIsHiddenFromFriendFinder(bool val)
		{
			this.IsHiddenFromFriendFinder = val;
		}

		public void SetIsRealIdVisibleForViewFriends(bool val)
		{
			this.IsRealIdVisibleForViewFriends = val;
		}

		public void SetIsUsingRid(bool val)
		{
			this.IsUsingRid = val;
		}

		public static class Types
		{
			public enum GameInfoPrivacy
			{
				PRIVACY_ME,
				PRIVACY_FRIENDS,
				PRIVACY_EVERYONE
			}
		}
	}
}