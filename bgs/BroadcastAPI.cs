using bnet.protocol.attribute;
using bnet.protocol.notification;
using System;
using System.Collections.Generic;

namespace bgs
{
	public class BroadcastAPI : BattleNetAPI
	{
		private List<BroadcastAPI.BroadcastCallback> m_listenerList = new List<BroadcastAPI.BroadcastCallback>();

		public BroadcastAPI(BattleNetCSharp battlenet) : base(battlenet, "Broadcast")
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
		}

		public void OnBroadcast(Notification notification)
		{
			foreach (BroadcastAPI.BroadcastCallback mListenerList in this.m_listenerList)
			{
				mListenerList(notification.AttributeList);
			}
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
		}

		public void RegisterListener(BroadcastAPI.BroadcastCallback cb)
		{
			if (this.m_listenerList.Contains(cb))
			{
				return;
			}
			this.m_listenerList.Add(cb);
		}

		public delegate void BroadcastCallback(IList<bnet.protocol.attribute.Attribute> AttributeList);
	}
}