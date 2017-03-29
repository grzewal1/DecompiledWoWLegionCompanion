using bgs.RPCServices;
using bgs.types;
using bnet.protocol;
using bnet.protocol.challenge;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace bgs
{
	public class ChallengeAPI : BattleNetAPI
	{
		private const uint PWD_FOURCC = 5265220;

		private ServiceDescriptor m_challengeService = new bgs.RPCServices.ChallengeService();

		private ServiceDescriptor m_challengeNotifyService = new ChallengeNotify();

		private List<ChallengeInfo> m_challengeUpdateList = new List<ChallengeInfo>();

		private Dictionary<uint, ChallengeInfo> m_challengePendingList = new Dictionary<uint, ChallengeInfo>();

		private Dictionary<uint, ulong> s_pendingAnswers = new Dictionary<uint, ulong>();

		private ExternalChallenge m_nextExternalChallenge;

		public ServiceDescriptor ChallengeNotifyService
		{
			get
			{
				return this.m_challengeNotifyService;
			}
		}

		public ServiceDescriptor ChallengeService
		{
			get
			{
				return this.m_challengeService;
			}
		}

		public ChallengeAPI(BattleNetCSharp battlenet) : base(battlenet, "Challenge")
		{
		}

		private void AbortChallenge(ulong id)
		{
			ChallengeCancelledRequest challengeCancelledRequest = new ChallengeCancelledRequest();
			challengeCancelledRequest.SetId((uint)id);
			this.m_rpcConnection.QueueRequest(this.ChallengeService.Id, 3, challengeCancelledRequest, new RPCContextDelegate(this.AbortChallengeCallback), 0);
			uint key = 0;
			bool flag = false;
			foreach (KeyValuePair<uint, ChallengeInfo> mChallengePendingList in this.m_challengePendingList)
			{
				if (mChallengePendingList.Value.challengeId != id)
				{
					continue;
				}
				key = mChallengePendingList.Key;
				flag = true;
				break;
			}
			if (flag)
			{
				this.m_challengePendingList.Remove(key);
			}
		}

		private void AbortChallengeCallback(RPCContext context)
		{
		}

		public void AnswerChallenge(ulong challengeID, string answer)
		{
			ChallengeAnsweredRequest challengeAnsweredRequest = new ChallengeAnsweredRequest();
			challengeAnsweredRequest.SetAnswer("pass");
			challengeAnsweredRequest.SetData(new byte[] { typeof(<PrivateImplementationDetails>).GetField("$$field-12").FieldHandle });
			if (!challengeAnsweredRequest.IsInitialized)
			{
				return;
			}
			RPCContext rPCContext = this.m_rpcConnection.QueueRequest(this.ChallengeService.Id, 2, challengeAnsweredRequest, new RPCContextDelegate(this.ChallengeAnsweredCallback), 0);
			this.s_pendingAnswers.Add(rPCContext.Header.Token, challengeID);
		}

		public void CancelChallenge(ulong challengeID)
		{
			this.AbortChallenge(challengeID);
		}

		private void ChallengeAnsweredCallback(RPCContext context)
		{
			ChallengeAnsweredResponse challengeAnsweredResponse = ChallengeAnsweredResponse.ParseFrom(context.Payload);
			if (!challengeAnsweredResponse.IsInitialized)
			{
				return;
			}
			ulong num = (ulong)0;
			if (!this.s_pendingAnswers.TryGetValue(context.Header.Token, out num))
			{
				return;
			}
			if (challengeAnsweredResponse.HasDoRetry && challengeAnsweredResponse.DoRetry)
			{
				ChallengeInfo challengeInfo = new ChallengeInfo()
				{
					challengeId = num,
					isRetry = true
				};
				this.m_challengeUpdateList.Add(challengeInfo);
			}
			this.s_pendingAnswers.Remove(context.Header.Token);
		}

		private void ChallengedPickedCallback(RPCContext context)
		{
			ChallengeInfo challengeInfo;
			if (!this.m_challengePendingList.TryGetValue(context.Header.Token, out challengeInfo))
			{
				base.ApiLog.LogWarning("Battle.net Challenge API C#: Received unexpected ChallengedPicked.");
				return;
			}
			BattleNetErrors status = (BattleNetErrors)context.Header.Status;
			if (status == BattleNetErrors.ERROR_OK)
			{
				this.m_challengeUpdateList.Add(challengeInfo);
				this.m_challengePendingList.Remove(context.Header.Token);
				return;
			}
			this.m_challengePendingList.Remove(context.Header.Token);
			base.ApiLog.LogWarning(string.Concat("Battle.net Challenge API C#: Failed ChallengedPicked. ", status));
		}

		private void ChallengeResultCallback(RPCContext context)
		{
		}

		private void ChallengeUserCallback(RPCContext context)
		{
			ChallengeUserRequest challengeUserRequest = ChallengeUserRequest.ParseFrom(context.Payload);
			if (!challengeUserRequest.IsInitialized)
			{
				return;
			}
			ulong id = (ulong)challengeUserRequest.Id;
			bool flag = false;
			int num = 0;
			while (num < challengeUserRequest.ChallengesCount)
			{
				if (challengeUserRequest.Challenges[num].Type != 5265220)
				{
					num++;
				}
				else
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.AbortChallenge(id);
				return;
			}
			ChallengePickedRequest challengePickedRequest = new ChallengePickedRequest();
			challengePickedRequest.SetChallenge(5265220);
			challengePickedRequest.SetId((uint)id);
			challengePickedRequest.SetNewChallengeProtocol(true);
			RPCContext rPCContext = this.m_rpcConnection.QueueRequest(this.ChallengeService.Id, 1, challengePickedRequest, new RPCContextDelegate(this.ChallengedPickedCallback), 0);
			ChallengeInfo challengeInfo = new ChallengeInfo()
			{
				challengeId = id,
				isRetry = false
			};
			this.m_challengePendingList.Add(rPCContext.Header.Token, challengeInfo);
		}

		public void ClearChallenges()
		{
			this.m_challengeUpdateList.Clear();
		}

		public void GetChallenges([Out] ChallengeInfo[] challenges)
		{
			this.m_challengeUpdateList.CopyTo(challenges);
		}

		public ExternalChallenge GetNextExternalChallenge()
		{
			ExternalChallenge mNextExternalChallenge = this.m_nextExternalChallenge;
			if (this.m_nextExternalChallenge != null)
			{
				this.m_nextExternalChallenge = this.m_nextExternalChallenge.Next;
			}
			return mNextExternalChallenge;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
			rpcConnection.RegisterServiceMethodListener(this.m_challengeNotifyService.Id, 1, new RPCContextDelegate(this.ChallengeUserCallback));
			rpcConnection.RegisterServiceMethodListener(this.m_challengeNotifyService.Id, 2, new RPCContextDelegate(this.ChallengeResultCallback));
			rpcConnection.RegisterServiceMethodListener(this.m_challengeNotifyService.Id, 3, new RPCContextDelegate(this.OnExternalChallengeCallback));
			rpcConnection.RegisterServiceMethodListener(this.m_challengeNotifyService.Id, 4, new RPCContextDelegate(this.OnExternalChallengeResultCallback));
		}

		public int NumChallenges()
		{
			return this.m_challengeUpdateList.Count;
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
		}

		private void OnExternalChallengeCallback(RPCContext context)
		{
			ChallengeExternalRequest challengeExternalRequest = ChallengeExternalRequest.ParseFrom(context.Payload);
			if (!challengeExternalRequest.IsInitialized || !challengeExternalRequest.HasPayload)
			{
				base.ApiLog.LogWarning("Bad ChallengeExternalRequest received IsInitialized={0} HasRequestToken={1} HasPayload={2} HasPayloadType={3}", new object[] { challengeExternalRequest.IsInitialized, challengeExternalRequest.HasRequestToken, challengeExternalRequest.HasPayload, challengeExternalRequest.HasPayloadType });
				return;
			}
			if (challengeExternalRequest.PayloadType != "web_auth_url")
			{
				base.ApiLog.LogWarning("Received a PayloadType we don't know how to handle PayloadType={0}", new object[] { challengeExternalRequest.PayloadType });
				return;
			}
			ExternalChallenge externalChallenge = new ExternalChallenge()
			{
				PayLoadType = challengeExternalRequest.PayloadType,
				URL = Encoding.ASCII.GetString(challengeExternalRequest.Payload)
			};
			base.ApiLog.LogDebug("Received external challenge PayLoadType={0} URL={1}", new object[] { externalChallenge.PayLoadType, externalChallenge.URL });
			if (this.m_nextExternalChallenge != null)
			{
				this.m_nextExternalChallenge.Next = externalChallenge;
			}
			else
			{
				this.m_nextExternalChallenge = externalChallenge;
			}
		}

		private void OnExternalChallengeResultCallback(RPCContext context)
		{
		}
	}
}