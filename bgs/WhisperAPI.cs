using bgs.types;
using bnet.protocol;
using bnet.protocol.attribute;
using bnet.protocol.notification;
using System;
using System.Collections.Generic;

namespace bgs
{
	public class WhisperAPI : BattleNetAPI
	{
		private List<BnetWhisper> m_whispers = new List<BnetWhisper>();

		public WhisperAPI(BattleNetCSharp battlenet) : base(battlenet, "Whisper")
		{
		}

		public void ClearWhispers()
		{
			this.m_whispers.Clear();
		}

		public void GetWhisperInfo(ref WhisperInfo info)
		{
			info.whisperSize = this.m_whispers.Count;
		}

		public void GetWhispers([Out] BnetWhisper[] whispers)
		{
			this.m_whispers.CopyTo(whispers, 0);
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
		}

		public void OnWhisper(Notification notification)
		{
			if (!notification.HasSenderId)
			{
				return;
			}
			if (notification.AttributeCount <= 0)
			{
				return;
			}
			BnetWhisper bnetWhisper = new BnetWhisper();
			bnetWhisper.SetSpeakerId(BnetGameAccountId.CreateFromProtocol(notification.SenderId));
			bnetWhisper.SetReceiverId(BnetGameAccountId.CreateFromProtocol(notification.TargetId));
			for (int i = 0; i < notification.AttributeCount; i++)
			{
				bnet.protocol.attribute.Attribute item = notification.Attribute[i];
				if (item.Name == "whisper")
				{
					bnetWhisper.SetMessage(item.Value.StringValue);
				}
			}
			if (string.IsNullOrEmpty(bnetWhisper.GetMessage()))
			{
				return;
			}
			bnetWhisper.SetTimestampMilliseconds(TimeUtils.GetElapsedTimeSinceEpoch(null).TotalMilliseconds);
			this.m_whispers.Add(bnetWhisper);
		}

		public void SendWhisper(BnetGameAccountId gameAccount, string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}
			Notification notification = new Notification();
			notification.SetType("WHISPER");
			bnet.protocol.EntityId entityId = new bnet.protocol.EntityId();
			entityId.SetLow(gameAccount.GetLo());
			entityId.SetHigh(gameAccount.GetHi());
			notification.SetTargetId(entityId);
			bnet.protocol.attribute.Attribute attribute = new bnet.protocol.attribute.Attribute();
			attribute.SetName("whisper");
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetStringValue(message);
			attribute.SetValue(variant);
			notification.AddAttribute(attribute);
			this.m_rpcConnection.QueueRequest(this.m_battleNet.NotificationService.Id, 1, notification, new RPCContextDelegate(this.WhisperSentCallback), 0);
			BnetGameAccountId bnetGameAccountId = BnetGameAccountId.CreateFromEntityId(BattleNet.GetMyGameAccountId());
			BnetWhisper bnetWhisper = new BnetWhisper();
			bnetWhisper.SetSpeakerId(bnetGameAccountId);
			bnetWhisper.SetReceiverId(gameAccount);
			bnetWhisper.SetMessage(message);
			bnetWhisper.SetTimestampMilliseconds(TimeUtils.GetElapsedTimeSinceEpoch(null).TotalMilliseconds);
			this.m_whispers.Add(bnetWhisper);
		}

		private void WhisperSentCallback(RPCContext context)
		{
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status != BattleNetErrors.ERROR_OK)
			{
				base.ApiLog.LogWarning(string.Concat("Battle.net Whisper API C#: Failed to SendWhisper. ", status));
				this.m_battleNet.EnqueueErrorInfo(BnetFeature.Whisper, BnetFeatureEvent.Whisper_OnSend, status, context.Context);
			}
		}
	}
}