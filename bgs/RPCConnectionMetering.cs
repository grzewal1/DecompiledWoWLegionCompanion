using bgs.RPCServices;
using bnet.protocol.config;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace bgs
{
	public class RPCConnectionMetering
	{
		private BattleNetLogSource m_log = new BattleNetLogSource("ConnectionMetering");

		private RPCConnectionMetering.MeteringData m_data;

		public RPCConnectionMetering()
		{
		}

		public bool AllowRPCCall(uint serviceID, uint methodID)
		{
			if (this.m_data == null)
			{
				return true;
			}
			RPCConnectionMetering.RuntimeData runtimedData = this.GetRuntimedData(serviceID, methodID);
			if (runtimedData == null)
			{
				return true;
			}
			float realTimeSinceStartup = (float)BattleNet.GetRealTimeSinceStartup();
			if ((double)this.m_data.StartupPeriodEnd > 0 && realTimeSinceStartup < this.m_data.StartupPeriodEnd)
			{
				float startupPeriodEnd = this.m_data.StartupPeriodEnd - realTimeSinceStartup;
				this.m_log.LogDebug("Allow (STARTUP PERIOD {0}) {1} ({2}:{3})", new object[] { startupPeriodEnd, runtimedData.GetServiceAndMethodNames(), serviceID, methodID });
				return true;
			}
			if (runtimedData.AlwaysAllow)
			{
				this.m_log.LogDebug("Allow (ALWAYS ALLOW) {0} ({1}:{2})", new object[] { runtimedData.GetServiceAndMethodNames(), serviceID, methodID });
				return true;
			}
			if (runtimedData.AlwaysDeny)
			{
				this.m_log.LogDebug("Deny (ALWAYS DENY) {0} ({1}:{2})", new object[] { runtimedData.GetServiceAndMethodNames(), serviceID, methodID });
				return false;
			}
			if (runtimedData.FiniteCallsLeft == -1)
			{
				bool flag = runtimedData.CanCall(realTimeSinceStartup);
				this.m_log.LogDebug("{0} (TRACKER) {1} ({2}:{3})", new object[] { (!flag ? "Deny" : "Allow"), runtimedData.GetServiceAndMethodNames(), serviceID, methodID });
				return flag;
			}
			if (runtimedData.FiniteCallsLeft <= 0)
			{
				this.m_log.LogDebug("Deny (FINITE CALLS LEFT 0) {0} ({1}:{2})", new object[] { runtimedData.GetServiceAndMethodNames(), serviceID, methodID });
				return false;
			}
			this.m_log.LogDebug("Allow (FINITE CALLS LEFT {0}) {1} ({2}:{3})", new object[] { runtimedData.FiniteCallsLeft, runtimedData.GetServiceAndMethodNames(), serviceID, methodID });
			RPCConnectionMetering.RuntimeData finiteCallsLeft = runtimedData;
			finiteCallsLeft.FiniteCallsLeft = finiteCallsLeft.FiniteCallsLeft - 1;
			return true;
		}

		private RPCConnectionMetering.RuntimeData GetRuntimedData(uint serviceID, uint methodID)
		{
			uint num = serviceID * 1000 + methodID;
			RPCConnectionMetering.RuntimeData runtimeData = this.m_data.GetRuntimeData(num);
			if (runtimeData == null)
			{
				runtimeData = new RPCConnectionMetering.RuntimeData();
				this.m_data.RuntimeData[num] = runtimeData;
				RPCConnectionMetering.StaticData value = null;
				foreach (KeyValuePair<string, RPCConnectionMetering.StaticData> methodDefault in this.m_data.MethodDefaults)
				{
					if (methodDefault.Value.ServiceId != serviceID || methodDefault.Value.MethodId != methodID)
					{
						continue;
					}
					value = methodDefault.Value;
					break;
				}
				if (value == null)
				{
					foreach (KeyValuePair<string, RPCConnectionMetering.StaticData> serviceDefault in this.m_data.ServiceDefaults)
					{
						if (serviceDefault.Value.ServiceId != serviceID)
						{
							continue;
						}
						value = serviceDefault.Value;
						break;
					}
				}
				if (value == null && this.m_data.GlobalDefault != null)
				{
					value = this.m_data.GlobalDefault;
				}
				if (value == null)
				{
					this.m_log.LogDebug("Always allowing ServiceId={0} MethodId={1}", new object[] { serviceID, methodID });
					runtimeData.AlwaysAllow = true;
					return runtimeData;
				}
				runtimeData.StaticData = value;
				if (value.RateLimitCount == 0)
				{
					runtimeData.AlwaysDeny = true;
				}
				else if (value.RateLimitSeconds != 0)
				{
					runtimeData.Tracker = new RPCConnectionMetering.CallTracker(value.RateLimitCount, value.RateLimitSeconds);
				}
				else
				{
					runtimeData.FiniteCallsLeft = value.RateLimitCount;
				}
			}
			return runtimeData;
		}

		private void InitializeInternalState(RPCMeterConfig config, ServiceCollectionHelper serviceHelper)
		{
			string serviceName;
			string str;
			List<string> strs = new List<string>();
			List<string> strs1 = new List<string>();
			int methodCount = config.MethodCount;
			for (int i = 0; i < methodCount; i++)
			{
				RPCMethodConfig item = config.Method[i];
				RPCConnectionMetering.StaticData staticDatum = new RPCConnectionMetering.StaticData();
				staticDatum.FromProtocol(item);
				if (item.HasServiceName)
				{
					serviceName = item.ServiceName;
					ServiceDescriptor importedServiceByName = serviceHelper.GetImportedServiceByName(serviceName);
					if (importedServiceByName != null)
					{
						staticDatum.ServiceId = importedServiceByName.Id;
						if (!item.HasMethodName)
						{
							if (this.m_data.ServiceDefaults.ContainsKey(serviceName))
							{
								goto Label1;
							}
							this.m_data.ServiceDefaults[serviceName] = staticDatum;
							this.m_log.LogDebug("Adding Service default {0}", new object[] { staticDatum });
						}
						else
						{
							string methodName = item.MethodName;
							str = string.Format("{0}.{1}", serviceName, methodName);
							MethodDescriptor methodDescriptorByName = importedServiceByName.GetMethodDescriptorByName(str);
							if (methodDescriptorByName != null)
							{
								if (this.m_data.MethodDefaults.ContainsKey(str))
								{
									goto Label2;
								}
								staticDatum.MethodId = methodDescriptorByName.Id;
								this.m_data.MethodDefaults[str] = staticDatum;
								this.m_log.LogDebug("Adding Method default {0}", new object[] { staticDatum });
							}
							else
							{
								this.m_log.LogDebug("Configuration specifies an unused method {0}, ignoring.", new object[] { methodName });
								goto Label0;
							}
						}
						strs.Add(serviceName);
					}
					else if (!strs1.Contains(serviceName))
					{
						this.m_log.LogDebug("Ignoring not imported service {0}", new object[] { serviceName });
						strs1.Add(serviceName);
					}
				}
				else if (this.m_data.GlobalDefault != null)
				{
					this.m_log.LogWarning("Static data has two defaults, ignoring additional ones.");
				}
				else
				{
					this.m_data.GlobalDefault = staticDatum;
					this.m_log.LogDebug("Adding global default {0}", new object[] { staticDatum });
				}
			Label0:
			}
			foreach (KeyValuePair<uint, ServiceDescriptor> importedService in serviceHelper.ImportedServices)
			{
				if (strs.Contains(importedService.Value.Name) || this.m_data.GlobalDefault != null)
				{
					continue;
				}
				this.m_log.LogDebug("Configuration for service {0} was not found and will not be metered.", new object[] { importedService.Value.Name });
			}
			return;
		Label1:
			BattleNetLogSource mLog = this.m_log;
			mLog.LogWarning("Default for service {0} already exists, ignoring extras.", new object[] { serviceName });
			goto Label0;
		Label2:
			BattleNetLogSource battleNetLogSource = this.m_log;
			battleNetLogSource.LogWarning("Default for method {0} already exists, ignoring extras.", new object[] { str });
			goto Label0;
		}

		public void ResetStartupPeriod()
		{
			if (this.m_data != null)
			{
				this.m_data.StartupPeriodEnd = (float)BattleNet.GetRealTimeSinceStartup() + this.m_data.StartupPeriodDuration;
			}
		}

		public void SetConnectionMeteringData(byte[] data, ServiceCollectionHelper serviceHelper)
		{
			this.m_data = new RPCConnectionMetering.MeteringData();
			if (data == null || (int)data.Length == 0 || serviceHelper == null)
			{
				this.m_log.LogError("Unable to retrieve Connection Metering data");
				return;
			}
			try
			{
				RPCMeterConfig rPCMeterConfig = RPCMeterConfigParser.ParseConfig(Encoding.ASCII.GetString(data));
				if (rPCMeterConfig == null || !rPCMeterConfig.IsInitialized)
				{
					this.m_data = null;
					throw new Exception("Unable to parse metering config protocol buffer.");
				}
				this.UpdateConfigStats(rPCMeterConfig);
				if (rPCMeterConfig.HasStartupPeriod)
				{
					this.m_data.StartupPeriodDuration = rPCMeterConfig.StartupPeriod;
					this.m_data.StartupPeriodEnd = (float)BattleNet.GetRealTimeSinceStartup() + rPCMeterConfig.StartupPeriod;
					this.m_log.LogDebug("StartupPeriod={0}", new object[] { rPCMeterConfig.StartupPeriod });
					this.m_log.LogDebug("StartupPeriodEnd={0}", new object[] { this.m_data.StartupPeriodEnd });
				}
				this.InitializeInternalState(rPCMeterConfig, serviceHelper);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.m_data = null;
				this.m_log.LogError("EXCEPTION = {0} {1}", new object[] { exception.Message, exception.StackTrace });
			}
			if (this.m_data == null)
			{
				this.m_log.LogError("Unable to parse Connection Metering data");
			}
		}

		private bool UpdateConfigStats(RPCMeterConfig config)
		{
			int methodCount = config.MethodCount;
			for (int i = 0; i < methodCount; i++)
			{
				this.UpdateMethodStats(config.Method[i]);
			}
			RPCConnectionMetering.Stats stats = this.m_data.Stats;
			this.m_log.LogDebug("Config Stats:");
			this.m_log.LogDebug("  MethodCount={0}", new object[] { stats.MethodCount });
			this.m_log.LogDebug("  ServiceNameCount={0}", new object[] { stats.ServiceNameCount });
			this.m_log.LogDebug("  MethodNameCount={0}", new object[] { stats.MethodNameCount });
			this.m_log.LogDebug("  FixedCalledCostCount={0}", new object[] { stats.FixedCalledCostCount });
			this.m_log.LogDebug("  FixedPacketSizeCount={0}", new object[] { stats.FixedPacketSizeCount });
			this.m_log.LogDebug("  VariableMultiplierCount={0}", new object[] { stats.VariableMultiplierCount });
			this.m_log.LogDebug("  MultiplierCount={0}", new object[] { stats.MultiplierCount });
			this.m_log.LogDebug("  RateLimitCountCount={0}", new object[] { stats.RateLimitCountCount });
			this.m_log.LogDebug("  RateLimitSecondsCount={0}", new object[] { stats.RateLimitSecondsCount });
			this.m_log.LogDebug("  MaxPacketSizeCount={0}", new object[] { stats.MaxPacketSizeCount });
			this.m_log.LogDebug("  MaxEncodedSizeCount={0}", new object[] { stats.MaxEncodedSizeCount });
			this.m_log.LogDebug("  TimeoutCount={0}", new object[] { stats.TimeoutCount });
			this.m_log.LogDebug("  AggregatedRateLimitCountCount={0}", new object[] { stats.AggregatedRateLimitCountCount });
			return true;
		}

		private void UpdateMethodStats(RPCMethodConfig method)
		{
			RPCConnectionMetering.Stats stats = this.m_data.Stats;
			stats.MethodCount = stats.MethodCount + 1;
			if (method.HasServiceName)
			{
				RPCConnectionMetering.Stats serviceNameCount = this.m_data.Stats;
				serviceNameCount.ServiceNameCount = serviceNameCount.ServiceNameCount + 1;
			}
			if (method.HasMethodName)
			{
				RPCConnectionMetering.Stats methodNameCount = this.m_data.Stats;
				methodNameCount.MethodNameCount = methodNameCount.MethodNameCount + 1;
			}
			if (method.HasFixedCallCost)
			{
				RPCConnectionMetering.Stats fixedCalledCostCount = this.m_data.Stats;
				fixedCalledCostCount.FixedCalledCostCount = fixedCalledCostCount.FixedCalledCostCount + 1;
			}
			if (method.HasFixedPacketSize)
			{
				RPCConnectionMetering.Stats fixedPacketSizeCount = this.m_data.Stats;
				fixedPacketSizeCount.FixedPacketSizeCount = fixedPacketSizeCount.FixedPacketSizeCount + 1;
			}
			if (method.HasVariableMultiplier)
			{
				RPCConnectionMetering.Stats variableMultiplierCount = this.m_data.Stats;
				variableMultiplierCount.VariableMultiplierCount = variableMultiplierCount.VariableMultiplierCount + 1;
			}
			if (method.HasMultiplier)
			{
				RPCConnectionMetering.Stats multiplierCount = this.m_data.Stats;
				multiplierCount.MultiplierCount = multiplierCount.MultiplierCount + 1;
			}
			if (method.HasRateLimitCount)
			{
				RPCConnectionMetering.Stats rateLimitCountCount = this.m_data.Stats;
				rateLimitCountCount.RateLimitCountCount = rateLimitCountCount.RateLimitCountCount + 1;
				RPCConnectionMetering.Stats aggregatedRateLimitCountCount = this.m_data.Stats;
				aggregatedRateLimitCountCount.AggregatedRateLimitCountCount = aggregatedRateLimitCountCount.AggregatedRateLimitCountCount + method.RateLimitCount;
			}
			if (method.HasRateLimitSeconds)
			{
				RPCConnectionMetering.Stats rateLimitSecondsCount = this.m_data.Stats;
				rateLimitSecondsCount.RateLimitSecondsCount = rateLimitSecondsCount.RateLimitSecondsCount + 1;
			}
			if (method.HasMaxPacketSize)
			{
				RPCConnectionMetering.Stats maxPacketSizeCount = this.m_data.Stats;
				maxPacketSizeCount.MaxPacketSizeCount = maxPacketSizeCount.MaxPacketSizeCount + 1;
			}
			if (method.HasMaxEncodedSize)
			{
				RPCConnectionMetering.Stats maxEncodedSizeCount = this.m_data.Stats;
				maxEncodedSizeCount.MaxEncodedSizeCount = maxEncodedSizeCount.MaxEncodedSizeCount + 1;
			}
			if (method.HasTimeout)
			{
				RPCConnectionMetering.Stats timeoutCount = this.m_data.Stats;
				timeoutCount.TimeoutCount = timeoutCount.TimeoutCount + 1;
			}
		}

		private class CallTracker
		{
			private float[] m_calls;

			private int m_callIndex;

			private float m_numberOfSeconds;

			public CallTracker(uint maxCalls, uint timePeriodInSeconds)
			{
				unsafe
				{
					if (maxCalls == 0 || timePeriodInSeconds == 0)
					{
						return;
					}
					this.m_calls = new float[maxCalls];
					this.m_numberOfSeconds = (float)((float)timePeriodInSeconds);
				}
			}

			public bool CanCall(float now)
			{
				int num;
				if (this.m_calls == null || (int)this.m_calls.Length == 0)
				{
					return false;
				}
				if (this.m_callIndex < (int)this.m_calls.Length)
				{
					float[] mCalls = this.m_calls;
					RPCConnectionMetering.CallTracker callTracker = this;
					int mCallIndex = callTracker.m_callIndex;
					num = mCallIndex;
					callTracker.m_callIndex = mCallIndex + 1;
					mCalls[num] = now;
					return true;
				}
				if (now - this.m_calls[0] <= this.m_numberOfSeconds)
				{
					return false;
				}
				if ((int)this.m_calls.Length == 1)
				{
					this.m_calls[0] = now;
					this.m_callIndex = 1;
					return true;
				}
				int num1 = 0;
				while (num1 + 1 < (int)this.m_calls.Length && now - this.m_calls[num1 + 1] > this.m_numberOfSeconds)
				{
					num1++;
				}
				int length = (int)this.m_calls.Length - (num1 + 1);
				Array.Copy(this.m_calls, num1 + 1, this.m_calls, 0, length);
				this.m_callIndex = length;
				float[] singleArray = this.m_calls;
				RPCConnectionMetering.CallTracker callTracker1 = this;
				int mCallIndex1 = callTracker1.m_callIndex;
				num = mCallIndex1;
				callTracker1.m_callIndex = mCallIndex1 + 1;
				singleArray[num] = now;
				return true;
			}
		}

		private class MeteringData
		{
			private RPCConnectionMetering.Stats m_staticDataStats;

			private RPCConnectionMetering.StaticData m_globalDefault;

			private Dictionary<string, RPCConnectionMetering.StaticData> m_serviceDefaults;

			private Dictionary<string, RPCConnectionMetering.StaticData> m_methodDefaults;

			private Dictionary<uint, RPCConnectionMetering.RuntimeData> m_runtimeData;

			public RPCConnectionMetering.StaticData GlobalDefault
			{
				get
				{
					return this.m_globalDefault;
				}
				set
				{
					this.m_globalDefault = value;
				}
			}

			public Dictionary<string, RPCConnectionMetering.StaticData> MethodDefaults
			{
				get
				{
					return this.m_methodDefaults;
				}
			}

			public Dictionary<uint, RPCConnectionMetering.RuntimeData> RuntimeData
			{
				get
				{
					return this.m_runtimeData;
				}
			}

			public Dictionary<string, RPCConnectionMetering.StaticData> ServiceDefaults
			{
				get
				{
					return this.m_serviceDefaults;
				}
			}

			public float StartupPeriodDuration
			{
				get;
				set;
			}

			public float StartupPeriodEnd
			{
				get;
				set;
			}

			public RPCConnectionMetering.Stats Stats
			{
				get
				{
					return this.m_staticDataStats;
				}
			}

			public MeteringData()
			{
			}

			public RPCConnectionMetering.RuntimeData GetRuntimeData(uint id)
			{
				RPCConnectionMetering.RuntimeData runtimeDatum;
				if (this.m_runtimeData.TryGetValue(id, out runtimeDatum))
				{
					return runtimeDatum;
				}
				return null;
			}
		}

		private class RuntimeData
		{
			private uint m_finiteCallsLeft;

			private RPCConnectionMetering.CallTracker m_callTracker;

			public bool AlwaysAllow
			{
				get;
				set;
			}

			public bool AlwaysDeny
			{
				get;
				set;
			}

			public uint FiniteCallsLeft
			{
				get
				{
					return this.m_finiteCallsLeft;
				}
				set
				{
					this.m_finiteCallsLeft = value;
				}
			}

			public RPCConnectionMetering.StaticData StaticData
			{
				get;
				set;
			}

			public RPCConnectionMetering.CallTracker Tracker
			{
				get
				{
					return this.m_callTracker;
				}
				set
				{
					this.m_callTracker = value;
				}
			}

			public RuntimeData()
			{
			}

			public bool CanCall(float now)
			{
				if (this.m_callTracker == null)
				{
					return true;
				}
				return this.m_callTracker.CanCall(now);
			}

			public string GetServiceAndMethodNames()
			{
				string str = (this.StaticData == null || this.StaticData.ServiceName == null ? "<null>" : this.StaticData.ServiceName);
				return string.Format("{0}.{1}", str, (this.StaticData == null || this.StaticData.MethodName == null ? "<null>" : this.StaticData.MethodName));
			}
		}

		private class StaticData
		{
			private uint m_serviceId;

			private uint m_methodId;

			public uint FixedCallCost
			{
				get;
				set;
			}

			public uint MethodId
			{
				get
				{
					return this.m_methodId;
				}
				set
				{
					this.m_methodId = value;
				}
			}

			public string MethodName
			{
				get;
				set;
			}

			public uint RateLimitCount
			{
				get;
				set;
			}

			public uint RateLimitSeconds
			{
				get;
				set;
			}

			public uint ServiceId
			{
				get
				{
					return this.m_serviceId;
				}
				set
				{
					this.m_serviceId = value;
				}
			}

			public string ServiceName
			{
				get;
				set;
			}

			public StaticData()
			{
			}

			public void FromProtocol(RPCMethodConfig method)
			{
				if (method.HasServiceName)
				{
					this.ServiceName = method.ServiceName;
				}
				if (method.HasMethodName)
				{
					this.MethodName = method.MethodName;
				}
				if (method.HasFixedCallCost)
				{
					this.FixedCallCost = method.FixedCallCost;
				}
				if (method.HasRateLimitCount)
				{
					this.RateLimitCount = method.RateLimitCount;
				}
				if (method.HasRateLimitSeconds)
				{
					this.RateLimitSeconds = method.RateLimitSeconds;
				}
			}

			public override string ToString()
			{
				string str = (!string.IsNullOrEmpty(this.ServiceName) ? this.ServiceName : "<null>");
				string str1 = (!string.IsNullOrEmpty(this.MethodName) ? this.MethodName : "<null>");
				return string.Format("ServiceName={0} MethodName={1} RateLimitCount={2} RateLimitSeconds={3} FixedCallCost={4}", new object[] { str, str1, this.RateLimitCount, this.RateLimitSeconds, this.FixedCallCost });
			}
		}

		private class Stats
		{
			public uint AggregatedRateLimitCountCount
			{
				get;
				set;
			}

			public uint FixedCalledCostCount
			{
				get;
				set;
			}

			public uint FixedPacketSizeCount
			{
				get;
				set;
			}

			public uint MaxEncodedSizeCount
			{
				get;
				set;
			}

			public uint MaxPacketSizeCount
			{
				get;
				set;
			}

			public uint MethodCount
			{
				get;
				set;
			}

			public uint MethodNameCount
			{
				get;
				set;
			}

			public uint MultiplierCount
			{
				get;
				set;
			}

			public uint RateLimitCountCount
			{
				get;
				set;
			}

			public uint RateLimitSecondsCount
			{
				get;
				set;
			}

			public uint ServiceNameCount
			{
				get;
				set;
			}

			public uint TimeoutCount
			{
				get;
				set;
			}

			public uint VariableMultiplierCount
			{
				get;
				set;
			}

			public Stats()
			{
			}
		}
	}
}