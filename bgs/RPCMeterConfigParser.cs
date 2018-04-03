using bnet.protocol.config;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace bgs
{
	public class RPCMeterConfigParser
	{
		public RPCMeterConfigParser()
		{
		}

		public static RPCMeterConfig ParseConfig(string str)
		{
			RPCMeterConfig rPCMeterConfig = new RPCMeterConfig();
			Tokenizer tokenizer = new Tokenizer(str);
			while (true)
			{
			Label2:
				string str1 = tokenizer.NextString();
				if (str1 == null)
				{
					break;
				}
				if (str1 == null)
				{
					goto Label0;
				}
				else if (str1 == "method")
				{
					rPCMeterConfig.AddMethod(RPCMeterConfigParser.ParseMethod(tokenizer));
				}
				else if (str1 == "income_per_second:")
				{
					rPCMeterConfig.IncomePerSecond = tokenizer.NextUInt32();
				}
				else if (str1 == "initial_balance:")
				{
					rPCMeterConfig.InitialBalance = tokenizer.NextUInt32();
				}
				else if (str1 == "cap_balance:")
				{
					rPCMeterConfig.CapBalance = tokenizer.NextUInt32();
				}
				else
				{
					if (str1 != "startup_period:")
					{
						goto Label0;
					}
					rPCMeterConfig.StartupPeriod = tokenizer.NextFloat();
				}
			}
			return rPCMeterConfig;
		Label0:
			tokenizer.SkipUnknownToken();
			goto Label2;
		}

		public static RPCMethodConfig ParseMethod(Tokenizer tokenizer)
		{
			int num;
			RPCMethodConfig rPCMethodConfig = new RPCMethodConfig();
			tokenizer.NextOpenBracket();
			while (true)
			{
			Label2:
				string str = tokenizer.NextString();
				if (str == null)
				{
					throw new Exception("Parsing ended with unfinished RPCMethodConfig");
				}
				if (str == "}")
				{
					break;
				}
				if (str != null)
				{
					if (RPCMeterConfigParser.<>f__switch$map8 == null)
					{
						Dictionary<string, int> strs = new Dictionary<string, int>(11)
						{
							{ "service_name:", 0 },
							{ "method_name:", 1 },
							{ "fixed_call_cost:", 2 },
							{ "fixed_packet_size:", 3 },
							{ "variable_multiplier:", 4 },
							{ "multiplier:", 5 },
							{ "rate_limit_count:", 6 },
							{ "rate_limit_seconds:", 7 },
							{ "max_packet_size:", 8 },
							{ "max_encoded_size:", 9 },
							{ "timeout:", 10 }
						};
						RPCMeterConfigParser.<>f__switch$map8 = strs;
					}
					if (RPCMeterConfigParser.<>f__switch$map8.TryGetValue(str, out num))
					{
						switch (num)
						{
							case 0:
							{
								rPCMethodConfig.ServiceName = tokenizer.NextQuotedString();
								break;
							}
							case 1:
							{
								rPCMethodConfig.MethodName = tokenizer.NextQuotedString();
								break;
							}
							case 2:
							{
								rPCMethodConfig.FixedCallCost = tokenizer.NextUInt32();
								break;
							}
							case 3:
							{
								rPCMethodConfig.FixedPacketSize = tokenizer.NextUInt32();
								break;
							}
							case 4:
							{
								rPCMethodConfig.VariableMultiplier = (float)((float)tokenizer.NextUInt32());
								break;
							}
							case 5:
							{
								rPCMethodConfig.Multiplier = tokenizer.NextFloat();
								break;
							}
							case 6:
							{
								rPCMethodConfig.RateLimitCount = tokenizer.NextUInt32();
								break;
							}
							case 7:
							{
								rPCMethodConfig.RateLimitSeconds = tokenizer.NextUInt32();
								break;
							}
							case 8:
							{
								rPCMethodConfig.MaxPacketSize = tokenizer.NextUInt32();
								break;
							}
							case 9:
							{
								rPCMethodConfig.MaxEncodedSize = tokenizer.NextUInt32();
								break;
							}
							case 10:
							{
								rPCMethodConfig.Timeout = tokenizer.NextFloat();
								break;
							}
							default:
							{
								goto Label1;
							}
						}
					}
					else
					{
						goto Label1;
					}
				}
				else
				{
					goto Label1;
				}
			}
			return rPCMethodConfig;
		Label1:
			tokenizer.SkipUnknownToken();
			goto Label2;
		}
	}
}