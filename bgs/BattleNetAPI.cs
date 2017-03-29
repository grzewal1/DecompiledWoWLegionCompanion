using bnet.protocol;
using System;

namespace bgs
{
	public class BattleNetAPI
	{
		private BattleNetAPI.LogDelegate[] m_logDelegates;

		protected BattleNetCSharp m_battleNet;

		protected RPCConnection m_rpcConnection;

		public BattleNetLogSource m_logSource;

		public BattleNetLogSource ApiLog
		{
			get
			{
				return this.m_logSource;
			}
		}

		protected BattleNetAPI(BattleNetCSharp battlenet, string logSourceName)
		{
			this.m_battleNet = battlenet;
			this.m_logSource = new BattleNetLogSource(logSourceName);
			this.m_logDelegates = new BattleNetAPI.LogDelegate[] { new BattleNetAPI.LogDelegate(this.m_logSource.LogDebug), new BattleNetAPI.LogDelegate(this.m_logSource.LogError) };
		}

		public bool CheckRPCCallback(string name, RPCContext context)
		{
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			BattleNetAPI.LogDelegate mLogDelegates = this.m_logDelegates[(status != BattleNetErrors.ERROR_OK ? 1 : 0)];
			object[] objArray = new object[] { status, null, null };
			objArray[1] = (!string.IsNullOrEmpty(name) ? name : "<null>");
			objArray[2] = (context.Request == null ? "<null>" : context.Request.ToString());
			mLogDelegates("Callback invoked, status = {0}, name = {1}, request = {2}", objArray);
			return status == BattleNetErrors.ERROR_OK;
		}

		public virtual void Initialize()
		{
		}

		public virtual void InitRPCListeners(RPCConnection rpcConnection)
		{
			this.m_rpcConnection = rpcConnection;
		}

		public virtual void OnConnected(BattleNetErrors error)
		{
		}

		public virtual void OnDisconnected()
		{
		}

		public virtual void OnGameAccountSelected()
		{
		}

		public virtual void OnLogon()
		{
		}

		public virtual void Process()
		{
		}

		private delegate void LogDelegate(string format, params object[] args);
	}
}