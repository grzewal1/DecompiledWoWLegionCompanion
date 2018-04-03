using System;
using System.IO;
using System.Text;

namespace bnet.protocol.authentication
{
	public class LogonRequest : IProtoBuf
	{
		public bool HasProgram;

		private string _Program;

		public bool HasPlatform;

		private string _Platform;

		public bool HasLocale;

		private string _Locale;

		public bool HasEmail;

		private string _Email;

		public bool HasVersion;

		private string _Version;

		public bool HasApplicationVersion;

		private int _ApplicationVersion;

		public bool HasPublicComputer;

		private bool _PublicComputer;

		public bool HasSsoId;

		private byte[] _SsoId;

		public bool HasDisconnectOnCookieFail;

		private bool _DisconnectOnCookieFail;

		public bool HasAllowLogonQueueNotifications;

		private bool _AllowLogonQueueNotifications;

		public bool HasWebClientVerification;

		private bool _WebClientVerification;

		public bool HasCachedWebCredentials;

		private byte[] _CachedWebCredentials;

		public bool HasUserAgent;

		private string _UserAgent;

		public bool AllowLogonQueueNotifications
		{
			get
			{
				return this._AllowLogonQueueNotifications;
			}
			set
			{
				this._AllowLogonQueueNotifications = value;
				this.HasAllowLogonQueueNotifications = true;
			}
		}

		public int ApplicationVersion
		{
			get
			{
				return this._ApplicationVersion;
			}
			set
			{
				this._ApplicationVersion = value;
				this.HasApplicationVersion = true;
			}
		}

		public byte[] CachedWebCredentials
		{
			get
			{
				return this._CachedWebCredentials;
			}
			set
			{
				this._CachedWebCredentials = value;
				this.HasCachedWebCredentials = value != null;
			}
		}

		public bool DisconnectOnCookieFail
		{
			get
			{
				return this._DisconnectOnCookieFail;
			}
			set
			{
				this._DisconnectOnCookieFail = value;
				this.HasDisconnectOnCookieFail = true;
			}
		}

		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				this._Email = value;
				this.HasEmail = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public string Locale
		{
			get
			{
				return this._Locale;
			}
			set
			{
				this._Locale = value;
				this.HasLocale = value != null;
			}
		}

		public string Platform
		{
			get
			{
				return this._Platform;
			}
			set
			{
				this._Platform = value;
				this.HasPlatform = value != null;
			}
		}

		public string Program
		{
			get
			{
				return this._Program;
			}
			set
			{
				this._Program = value;
				this.HasProgram = value != null;
			}
		}

		public bool PublicComputer
		{
			get
			{
				return this._PublicComputer;
			}
			set
			{
				this._PublicComputer = value;
				this.HasPublicComputer = true;
			}
		}

		public byte[] SsoId
		{
			get
			{
				return this._SsoId;
			}
			set
			{
				this._SsoId = value;
				this.HasSsoId = value != null;
			}
		}

		public string UserAgent
		{
			get
			{
				return this._UserAgent;
			}
			set
			{
				this._UserAgent = value;
				this.HasUserAgent = value != null;
			}
		}

		public string Version
		{
			get
			{
				return this._Version;
			}
			set
			{
				this._Version = value;
				this.HasVersion = value != null;
			}
		}

		public bool WebClientVerification
		{
			get
			{
				return this._WebClientVerification;
			}
			set
			{
				this._WebClientVerification = value;
				this.HasWebClientVerification = true;
			}
		}

		public LogonRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			LogonRequest.Deserialize(stream, this);
		}

		public static LogonRequest Deserialize(Stream stream, LogonRequest instance)
		{
			return LogonRequest.Deserialize(stream, instance, (long)-1);
		}

		public static LogonRequest Deserialize(Stream stream, LogonRequest instance, long limit)
		{
			instance.DisconnectOnCookieFail = false;
			instance.AllowLogonQueueNotifications = false;
			instance.WebClientVerification = false;
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
						instance.Program = ProtocolParser.ReadString(stream);
					}
					else if (num == 18)
					{
						instance.Platform = ProtocolParser.ReadString(stream);
					}
					else if (num == 26)
					{
						instance.Locale = ProtocolParser.ReadString(stream);
					}
					else if (num == 34)
					{
						instance.Email = ProtocolParser.ReadString(stream);
					}
					else if (num == 42)
					{
						instance.Version = ProtocolParser.ReadString(stream);
					}
					else if (num == 48)
					{
						instance.ApplicationVersion = (int)ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 56)
					{
						instance.PublicComputer = ProtocolParser.ReadBool(stream);
					}
					else if (num == 66)
					{
						instance.SsoId = ProtocolParser.ReadBytes(stream);
					}
					else if (num == 72)
					{
						instance.DisconnectOnCookieFail = ProtocolParser.ReadBool(stream);
					}
					else if (num == 80)
					{
						instance.AllowLogonQueueNotifications = ProtocolParser.ReadBool(stream);
					}
					else if (num == 88)
					{
						instance.WebClientVerification = ProtocolParser.ReadBool(stream);
					}
					else if (num == 98)
					{
						instance.CachedWebCredentials = ProtocolParser.ReadBytes(stream);
					}
					else if (num == 114)
					{
						instance.UserAgent = ProtocolParser.ReadString(stream);
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

		public static LogonRequest DeserializeLengthDelimited(Stream stream)
		{
			LogonRequest logonRequest = new LogonRequest();
			LogonRequest.DeserializeLengthDelimited(stream, logonRequest);
			return logonRequest;
		}

		public static LogonRequest DeserializeLengthDelimited(Stream stream, LogonRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return LogonRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			LogonRequest logonRequest = obj as LogonRequest;
			if (logonRequest == null)
			{
				return false;
			}
			if (this.HasProgram != logonRequest.HasProgram || this.HasProgram && !this.Program.Equals(logonRequest.Program))
			{
				return false;
			}
			if (this.HasPlatform != logonRequest.HasPlatform || this.HasPlatform && !this.Platform.Equals(logonRequest.Platform))
			{
				return false;
			}
			if (this.HasLocale != logonRequest.HasLocale || this.HasLocale && !this.Locale.Equals(logonRequest.Locale))
			{
				return false;
			}
			if (this.HasEmail != logonRequest.HasEmail || this.HasEmail && !this.Email.Equals(logonRequest.Email))
			{
				return false;
			}
			if (this.HasVersion != logonRequest.HasVersion || this.HasVersion && !this.Version.Equals(logonRequest.Version))
			{
				return false;
			}
			if (this.HasApplicationVersion != logonRequest.HasApplicationVersion || this.HasApplicationVersion && !this.ApplicationVersion.Equals(logonRequest.ApplicationVersion))
			{
				return false;
			}
			if (this.HasPublicComputer != logonRequest.HasPublicComputer || this.HasPublicComputer && !this.PublicComputer.Equals(logonRequest.PublicComputer))
			{
				return false;
			}
			if (this.HasSsoId != logonRequest.HasSsoId || this.HasSsoId && !this.SsoId.Equals(logonRequest.SsoId))
			{
				return false;
			}
			if (this.HasDisconnectOnCookieFail != logonRequest.HasDisconnectOnCookieFail || this.HasDisconnectOnCookieFail && !this.DisconnectOnCookieFail.Equals(logonRequest.DisconnectOnCookieFail))
			{
				return false;
			}
			if (this.HasAllowLogonQueueNotifications != logonRequest.HasAllowLogonQueueNotifications || this.HasAllowLogonQueueNotifications && !this.AllowLogonQueueNotifications.Equals(logonRequest.AllowLogonQueueNotifications))
			{
				return false;
			}
			if (this.HasWebClientVerification != logonRequest.HasWebClientVerification || this.HasWebClientVerification && !this.WebClientVerification.Equals(logonRequest.WebClientVerification))
			{
				return false;
			}
			if (this.HasCachedWebCredentials != logonRequest.HasCachedWebCredentials || this.HasCachedWebCredentials && !this.CachedWebCredentials.Equals(logonRequest.CachedWebCredentials))
			{
				return false;
			}
			if (this.HasUserAgent == logonRequest.HasUserAgent && (!this.HasUserAgent || this.UserAgent.Equals(logonRequest.UserAgent)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasProgram)
			{
				hashCode ^= this.Program.GetHashCode();
			}
			if (this.HasPlatform)
			{
				hashCode ^= this.Platform.GetHashCode();
			}
			if (this.HasLocale)
			{
				hashCode ^= this.Locale.GetHashCode();
			}
			if (this.HasEmail)
			{
				hashCode ^= this.Email.GetHashCode();
			}
			if (this.HasVersion)
			{
				hashCode ^= this.Version.GetHashCode();
			}
			if (this.HasApplicationVersion)
			{
				hashCode ^= this.ApplicationVersion.GetHashCode();
			}
			if (this.HasPublicComputer)
			{
				hashCode ^= this.PublicComputer.GetHashCode();
			}
			if (this.HasSsoId)
			{
				hashCode ^= this.SsoId.GetHashCode();
			}
			if (this.HasDisconnectOnCookieFail)
			{
				hashCode ^= this.DisconnectOnCookieFail.GetHashCode();
			}
			if (this.HasAllowLogonQueueNotifications)
			{
				hashCode ^= this.AllowLogonQueueNotifications.GetHashCode();
			}
			if (this.HasWebClientVerification)
			{
				hashCode ^= this.WebClientVerification.GetHashCode();
			}
			if (this.HasCachedWebCredentials)
			{
				hashCode ^= this.CachedWebCredentials.GetHashCode();
			}
			if (this.HasUserAgent)
			{
				hashCode ^= this.UserAgent.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasProgram)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Program);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasPlatform)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.Platform);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			if (this.HasLocale)
			{
				num++;
				uint num1 = (uint)Encoding.UTF8.GetByteCount(this.Locale);
				num = num + ProtocolParser.SizeOfUInt32(num1) + num1;
			}
			if (this.HasEmail)
			{
				num++;
				uint byteCount2 = (uint)Encoding.UTF8.GetByteCount(this.Email);
				num = num + ProtocolParser.SizeOfUInt32(byteCount2) + byteCount2;
			}
			if (this.HasVersion)
			{
				num++;
				uint num2 = (uint)Encoding.UTF8.GetByteCount(this.Version);
				num = num + ProtocolParser.SizeOfUInt32(num2) + num2;
			}
			if (this.HasApplicationVersion)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64((ulong)this.ApplicationVersion);
			}
			if (this.HasPublicComputer)
			{
				num++;
				num++;
			}
			if (this.HasSsoId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.SsoId.Length) + (int)this.SsoId.Length;
			}
			if (this.HasDisconnectOnCookieFail)
			{
				num++;
				num++;
			}
			if (this.HasAllowLogonQueueNotifications)
			{
				num++;
				num++;
			}
			if (this.HasWebClientVerification)
			{
				num++;
				num++;
			}
			if (this.HasCachedWebCredentials)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32((int)this.CachedWebCredentials.Length) + (int)this.CachedWebCredentials.Length;
			}
			if (this.HasUserAgent)
			{
				num++;
				uint byteCount3 = (uint)Encoding.UTF8.GetByteCount(this.UserAgent);
				num = num + ProtocolParser.SizeOfUInt32(byteCount3) + byteCount3;
			}
			return num;
		}

		public static LogonRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<LogonRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			LogonRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, LogonRequest instance)
		{
			if (instance.HasProgram)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Program));
			}
			if (instance.HasPlatform)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Platform));
			}
			if (instance.HasLocale)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Locale));
			}
			if (instance.HasEmail)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Email));
			}
			if (instance.HasVersion)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Version));
			}
			if (instance.HasApplicationVersion)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.ApplicationVersion);
			}
			if (instance.HasPublicComputer)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteBool(stream, instance.PublicComputer);
			}
			if (instance.HasSsoId)
			{
				stream.WriteByte(66);
				ProtocolParser.WriteBytes(stream, instance.SsoId);
			}
			if (instance.HasDisconnectOnCookieFail)
			{
				stream.WriteByte(72);
				ProtocolParser.WriteBool(stream, instance.DisconnectOnCookieFail);
			}
			if (instance.HasAllowLogonQueueNotifications)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteBool(stream, instance.AllowLogonQueueNotifications);
			}
			if (instance.HasWebClientVerification)
			{
				stream.WriteByte(88);
				ProtocolParser.WriteBool(stream, instance.WebClientVerification);
			}
			if (instance.HasCachedWebCredentials)
			{
				stream.WriteByte(98);
				ProtocolParser.WriteBytes(stream, instance.CachedWebCredentials);
			}
			if (instance.HasUserAgent)
			{
				stream.WriteByte(114);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.UserAgent));
			}
		}

		public void SetAllowLogonQueueNotifications(bool val)
		{
			this.AllowLogonQueueNotifications = val;
		}

		public void SetApplicationVersion(int val)
		{
			this.ApplicationVersion = val;
		}

		public void SetCachedWebCredentials(byte[] val)
		{
			this.CachedWebCredentials = val;
		}

		public void SetDisconnectOnCookieFail(bool val)
		{
			this.DisconnectOnCookieFail = val;
		}

		public void SetEmail(string val)
		{
			this.Email = val;
		}

		public void SetLocale(string val)
		{
			this.Locale = val;
		}

		public void SetPlatform(string val)
		{
			this.Platform = val;
		}

		public void SetProgram(string val)
		{
			this.Program = val;
		}

		public void SetPublicComputer(bool val)
		{
			this.PublicComputer = val;
		}

		public void SetSsoId(byte[] val)
		{
			this.SsoId = val;
		}

		public void SetUserAgent(string val)
		{
			this.UserAgent = val;
		}

		public void SetVersion(string val)
		{
			this.Version = val;
		}

		public void SetWebClientVerification(bool val)
		{
			this.WebClientVerification = val;
		}
	}
}