using bgs.RPCServices;
using bgs.types;
using bnet.protocol;
using bnet.protocol.attribute;
using bnet.protocol.presence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace bgs
{
	public class PresenceAPI : BattleNetAPI
	{
		private const float CREDIT_LIMIT = -100f;

		private const float COST_PER_REQUEST = 1f;

		private const float PAYDOWN_RATE_PER_MS = 0.00333333341f;

		private const string variablePrefix = "$0x";

		private float m_presenceSubscriptionBalance;

		private long m_lastPresenceSubscriptionSent;

		private Stopwatch m_stopWatch;

		private HashSet<bnet.protocol.EntityId> m_queuedSubscriptions = new HashSet<bnet.protocol.EntityId>();

		private Map<bnet.protocol.EntityId, PresenceAPI.PresenceRefCountObject> m_presenceSubscriptions = new Map<bnet.protocol.EntityId, PresenceAPI.PresenceRefCountObject>();

		private List<PresenceUpdate> m_presenceUpdates = new List<PresenceUpdate>();

		private PresenceAPI.EntityIdToFieldsMap m_presenceCache = new PresenceAPI.EntityIdToFieldsMap();

		private PresenceAPI.RichPresenceToStringsMap m_richPresenceStringTables = new PresenceAPI.RichPresenceToStringsMap();

		private HashSet<PresenceUpdate> m_pendingRichPresenceUpdates = new HashSet<PresenceUpdate>();

		private int m_numOutstandingRichPresenceStringFetches;

		private ServiceDescriptor m_presenceService = new bgs.RPCServices.PresenceService();

		private static char[] hexChars;

		public ServiceDescriptor PresenceService
		{
			get
			{
				return this.m_presenceService;
			}
		}

		static PresenceAPI()
		{
			PresenceAPI.hexChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F' };
		}

		public PresenceAPI(BattleNetCSharp battlenet) : base(battlenet, "Presence")
		{
			this.m_stopWatch = new Stopwatch();
		}

		public void ClearPresence()
		{
			this.m_presenceUpdates.Clear();
		}

		private void DecrementOutstandingRichPresenceStringFetches()
		{
			if (this.m_numOutstandingRichPresenceStringFetches <= 0)
			{
				base.ApiLog.LogWarning("Number of outstanding rich presence string fetches tracked incorrectly - decemented to negative");
				return;
			}
			PresenceAPI mNumOutstandingRichPresenceStringFetches = this;
			mNumOutstandingRichPresenceStringFetches.m_numOutstandingRichPresenceStringFetches = mNumOutstandingRichPresenceStringFetches.m_numOutstandingRichPresenceStringFetches - 1;
			this.TryToResolveRichPresence();
		}

		private void DownloadCompletedCallback(byte[] data, object userContext)
		{
			// 
			// Current member / type: System.Void bgs.PresenceAPI::DownloadCompletedCallback(System.Byte[],System.Object)
			// File path: C:\Users\RenameME-4\Desktop\wow_app\wow_v1.2.0_com.blizzard.wowcompanion\assets\bin\Data\Managed\Assembly-CSharp.dll
			// 
			// Product version: 2017.1.116.2
			// Exception in: System.Void DownloadCompletedCallback(System.Byte[],System.Object)
			// 
			// La référence d'objet n'est pas définie à une instance d'un objet.
			//    à ..() dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 81
			//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 24
			//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
			//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 498
			//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 120
			//    à ..Visit(IEnumerable ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 374
			//    à ..( ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 24
			//    à ..Visit(ICodeNode ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
			//    à ..(DecompilationContext ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:ligne 19
			//    à ..(MethodBody ,  , ILanguage ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			//    à ..(MethodBody , ILanguage ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			//    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			//    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			//    à ..(ILanguage , MethodDefinition ,  ) dans C:\Builds\556\Behemoth\ReleaseBranch Production Build NT\Sources\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private bool FetchRichPresenceResource(bnet.protocol.attribute.Variant presenceValue)
		{
			if (presenceValue == null)
			{
				return false;
			}
			RichPresence richPresence = RichPresence.ParseFrom(presenceValue.MessageValue);
			if (richPresence == null || !richPresence.IsInitialized)
			{
				base.ApiLog.LogError("Rich presence field from battle.net does not contain valid RichPresence message");
				return false;
			}
			if (this.m_richPresenceStringTables.ContainsKey(richPresence))
			{
				return false;
			}
			FourCC fourCC = new FourCC(richPresence.ProgramId);
			FourCC fourCC1 = new FourCC(richPresence.StreamId);
			FourCC fourCC2 = new FourCC(BattleNet.Client().GetLocaleName());
			this.IncrementOutstandingRichPresenceStringFetches();
			ResourcesAPI resources = this.m_battleNet.Resources;
			resources.LookupResource(fourCC, fourCC1, fourCC2, new ResourcesAPI.ResourceLookupCallback(this.ResouceLookupCallback), richPresence);
			return true;
		}

		public void GetPresence([Out] PresenceUpdate[] updates)
		{
			this.m_presenceUpdates.CopyTo(updates);
		}

		public void HandlePresenceUpdates(ChannelState channelState, ChannelAPI.ChannelReferenceObject channelRef)
		{
			bgs.types.EntityId high = new bgs.types.EntityId();
			high.hi = channelRef.m_channelData.m_channelId.High;
			high.lo = channelRef.m_channelData.m_channelId.Low;
			FieldKey fieldKey = new FieldKey();
			fieldKey.SetProgram(BnetProgramId.BNET.GetValue());
			fieldKey.SetGroup(1);
			fieldKey.SetField(3);
			FieldKey fieldKey1 = fieldKey;
			List<PresenceUpdate> presenceUpdates = new List<PresenceUpdate>();
			foreach (FieldOperation fieldOperationList in channelState.FieldOperationList)
			{
				if (fieldOperationList.Operation != FieldOperation.Types.OperationType.CLEAR)
				{
					this.m_presenceCache.SetCache(high, fieldOperationList.Field.Key, fieldOperationList.Field.Value);
				}
				else
				{
					this.m_presenceCache.SetCache(high, fieldOperationList.Field.Key, null);
				}
				PresenceUpdate boolValue = new PresenceUpdate()
				{
					entityId = high,
					programId = fieldOperationList.Field.Key.Program,
					groupId = fieldOperationList.Field.Key.Group,
					fieldId = fieldOperationList.Field.Key.Field,
					index = fieldOperationList.Field.Key.Index,
					boolVal = false,
					intVal = (long)0,
					stringVal = string.Empty,
					valCleared = false,
					blobVal = new byte[0]
				};
				if (fieldOperationList.Operation == FieldOperation.Types.OperationType.CLEAR)
				{
					boolValue.valCleared = true;
					bool program = fieldKey1.Program == fieldOperationList.Field.Key.Program;
					bool group = fieldKey1.Group == fieldOperationList.Field.Key.Group;
					bool field = fieldKey1.Field == fieldOperationList.Field.Key.Field;
					if (program && group && field)
					{
						BnetEntityId bnetEntityId = BnetEntityId.CreateFromEntityId(boolValue.entityId);
						this.m_battleNet.Friends.RemoveFriendsActiveGameAccount(bnetEntityId, fieldOperationList.Field.Key.Index);
					}
				}
				else if (fieldOperationList.Field.Value.HasBoolValue)
				{
					boolValue.boolVal = fieldOperationList.Field.Value.BoolValue;
				}
				else if (fieldOperationList.Field.Value.HasIntValue)
				{
					boolValue.intVal = fieldOperationList.Field.Value.IntValue;
				}
				else if (fieldOperationList.Field.Value.HasStringValue)
				{
					boolValue.stringVal = fieldOperationList.Field.Value.StringValue;
				}
				else if (fieldOperationList.Field.Value.HasFourccValue)
				{
					boolValue.stringVal = (new BnetProgramId(fieldOperationList.Field.Value.FourccValue)).ToString();
				}
				else if (fieldOperationList.Field.Value.HasEntityidValue)
				{
					boolValue.entityIdVal.hi = fieldOperationList.Field.Value.EntityidValue.High;
					boolValue.entityIdVal.lo = fieldOperationList.Field.Value.EntityidValue.Low;
					bool flag = fieldKey1.Program == fieldOperationList.Field.Key.Program;
					bool group1 = fieldKey1.Group == fieldOperationList.Field.Key.Group;
					bool field1 = fieldKey1.Field == fieldOperationList.Field.Key.Field;
					if (flag && group1 && field1)
					{
						BnetEntityId bnetEntityId1 = BnetEntityId.CreateFromEntityId(boolValue.entityId);
						this.m_battleNet.Friends.AddFriendsActiveGameAccount(bnetEntityId1, fieldOperationList.Field.Value.EntityidValue, fieldOperationList.Field.Key.Index);
					}
				}
				else if (fieldOperationList.Field.Value.HasBlobValue)
				{
					boolValue.blobVal = fieldOperationList.Field.Value.BlobValue;
				}
				else if (!fieldOperationList.Field.Value.HasMessageValue)
				{
					continue;
				}
				else if (fieldOperationList.Field.Key.Field != 8)
				{
					continue;
				}
				else
				{
					this.FetchRichPresenceResource(fieldOperationList.Field.Value);
					this.HandleRichPresenceUpdate(boolValue, fieldOperationList.Field.Key);
					continue;
				}
				presenceUpdates.Add(boolValue);
			}
			presenceUpdates.Reverse();
			this.m_presenceUpdates.AddRange(presenceUpdates);
		}

		private void HandleRichPresenceUpdate(PresenceUpdate rpUpdate, FieldKey fieldKey)
		{
			FieldKey fieldKey1 = new FieldKey();
			fieldKey1.SetProgram(BnetProgramId.BNET.GetValue());
			fieldKey1.SetGroup(2);
			fieldKey1.SetField(8);
			fieldKey1.SetIndex((ulong)0);
			if (!fieldKey1.Equals(fieldKey))
			{
				return;
			}
			this.m_pendingRichPresenceUpdates.Add(rpUpdate);
			this.TryToResolveRichPresence();
		}

		private void HandleSubscriptionRequests()
		{
			if (this.m_queuedSubscriptions.Count > 0)
			{
				long elapsedMilliseconds = this.m_stopWatch.ElapsedMilliseconds;
				this.m_presenceSubscriptionBalance = Math.Min(0f, this.m_presenceSubscriptionBalance + (float)(elapsedMilliseconds - this.m_lastPresenceSubscriptionSent) * 0.00333333341f);
				this.m_lastPresenceSubscriptionSent = elapsedMilliseconds;
				List<bnet.protocol.EntityId> entityIds = new List<bnet.protocol.EntityId>();
				foreach (bnet.protocol.EntityId mQueuedSubscription in this.m_queuedSubscriptions)
				{
					if (this.m_presenceSubscriptionBalance - 1f >= -100f)
					{
						PresenceAPI.PresenceRefCountObject item = this.m_presenceSubscriptions[mQueuedSubscription];
						SubscribeRequest subscribeRequest = new SubscribeRequest();
						subscribeRequest.SetObjectId(ChannelAPI.GetNextObjectId());
						subscribeRequest.SetEntityId(mQueuedSubscription);
						item.objectId = subscribeRequest.ObjectId;
						this.m_battleNet.Channel.AddActiveChannel(subscribeRequest.ObjectId, new ChannelAPI.ChannelReferenceObject(mQueuedSubscription, ChannelAPI.ChannelType.PRESENCE_CHANNEL));
						this.m_rpcConnection.QueueRequest(this.m_presenceService.Id, 1, subscribeRequest, new RPCContextDelegate(this.PresenceSubscribeCallback), 0);
						PresenceAPI mPresenceSubscriptionBalance = this;
						mPresenceSubscriptionBalance.m_presenceSubscriptionBalance = mPresenceSubscriptionBalance.m_presenceSubscriptionBalance - 1f;
						entityIds.Add(mQueuedSubscription);
					}
					else
					{
						break;
					}
				}
				foreach (bnet.protocol.EntityId entityId in entityIds)
				{
					this.m_queuedSubscriptions.Remove(entityId);
				}
			}
		}

		private void IncrementOutstandingRichPresenceStringFetches()
		{
			PresenceAPI mNumOutstandingRichPresenceStringFetches = this;
			mNumOutstandingRichPresenceStringFetches.m_numOutstandingRichPresenceStringFetches = mNumOutstandingRichPresenceStringFetches.m_numOutstandingRichPresenceStringFetches + 1;
		}

		public override void Initialize()
		{
			base.Initialize();
			this.m_stopWatch.Start();
			this.m_lastPresenceSubscriptionSent = (long)0;
			this.m_presenceSubscriptionBalance = 0f;
		}

		public override void InitRPCListeners(RPCConnection rpcConnection)
		{
			base.InitRPCListeners(rpcConnection);
		}

		private int LastIndexOfOccurenceFromIndex(string str, char[] testChars, int startIndex)
		{
			int num = -1;
			char[] charArray = str.ToCharArray();
			int num1 = startIndex;
			while (num1 < (int)charArray.Length)
			{
				char chr = charArray[num1];
				bool flag = false;
				char[] chrArray = PresenceAPI.hexChars;
				int num2 = 0;
				while (num2 < (int)chrArray.Length)
				{
					if (chr != chrArray[num2])
					{
						num2++;
					}
					else
					{
						num = num1;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					num1++;
				}
				else
				{
					break;
				}
			}
			return num;
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
			this.m_presenceSubscriptions.Clear();
			this.m_presenceUpdates.Clear();
			this.m_queuedSubscriptions.Clear();
			this.m_stopWatch.Stop();
			this.m_lastPresenceSubscriptionSent = (long)0;
			this.m_presenceSubscriptionBalance = 0f;
		}

		public int PresenceSize()
		{
			return this.m_presenceUpdates.Count;
		}

		public void PresenceSubscribe(bnet.protocol.EntityId entityId)
		{
		}

		private void PresenceSubscribeCallback(RPCContext context)
		{
			base.CheckRPCCallback("PresenceSubscribeCallback", context);
		}

		public void PresenceUnsubscribe(bnet.protocol.EntityId entityId)
		{
			if (this.m_presenceSubscriptions.ContainsKey(entityId))
			{
				PresenceAPI.PresenceRefCountObject item = this.m_presenceSubscriptions[entityId];
				item.refCount = item.refCount - 1;
				if (this.m_presenceSubscriptions[entityId].refCount <= 0)
				{
					if (this.m_queuedSubscriptions.Contains(entityId))
					{
						this.m_queuedSubscriptions.Remove(entityId);
						return;
					}
					PresenceAPI.PresenceUnsubscribeContext presenceUnsubscribeContext = new PresenceAPI.PresenceUnsubscribeContext(this.m_battleNet, this.m_presenceSubscriptions[entityId].objectId);
					UnsubscribeRequest unsubscribeRequest = new UnsubscribeRequest();
					unsubscribeRequest.SetEntityId(entityId);
					this.m_rpcConnection.QueueRequest(this.m_presenceService.Id, 2, unsubscribeRequest, new RPCContextDelegate(presenceUnsubscribeContext.PresenceUnsubscribeCallback), 0);
					this.m_presenceSubscriptions.Remove(entityId);
				}
			}
		}

		private void PresenceUpdateCallback(RPCContext context)
		{
			base.CheckRPCCallback("PresenceUpdateCallback", context);
		}

		public override void Process()
		{
			base.Process();
			this.HandleSubscriptionRequests();
		}

		public void PublishField(UpdateRequest updateRequest)
		{
			this.m_rpcConnection.QueueRequest(this.m_presenceService.Id, 3, updateRequest, new RPCContextDelegate(this.PresenceUpdateCallback), 0);
		}

		public void PublishRichPresence([In] RichPresenceUpdate[] updates)
		{
			UpdateRequest updateRequest = new UpdateRequest()
			{
				EntityId = this.m_battleNet.GameAccountId
			};
			FieldOperation fieldOperation = new FieldOperation();
			Field field = new Field();
			FieldKey fieldKey = new FieldKey();
			fieldKey.SetProgram(BnetProgramId.BNET.GetValue());
			fieldKey.SetGroup(2);
			fieldKey.SetField(8);
			RichPresenceUpdate[] richPresenceUpdateArray = updates;
			for (int i = 0; i < (int)richPresenceUpdateArray.Length; i++)
			{
				RichPresenceUpdate richPresenceUpdate = richPresenceUpdateArray[i];
				fieldKey.SetIndex(richPresenceUpdate.presenceFieldIndex);
				RichPresence richPresence = new RichPresence();
				richPresence.SetIndex(richPresenceUpdate.index);
				richPresence.SetProgramId(richPresenceUpdate.programId);
				richPresence.SetStreamId(richPresenceUpdate.streamId);
				bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
				variant.SetMessageValue(ProtobufUtil.ToByteArray(richPresence));
				field.SetKey(fieldKey);
				field.SetValue(variant);
				fieldOperation.SetField(field);
				updateRequest.SetEntityId(this.m_battleNet.GameAccountId);
				updateRequest.AddFieldOperation(fieldOperation);
			}
			this.PublishField(updateRequest);
		}

		public void RequestPresenceFields(bool isGameAccountEntityId, [In] bgs.types.EntityId entityId, [In] PresenceFieldKey[] fieldList)
		{
			QueryRequest queryRequest = new QueryRequest();
			bnet.protocol.EntityId entityId1 = new bnet.protocol.EntityId();
			entityId1.SetHigh(entityId.hi);
			entityId1.SetLow(entityId.lo);
			queryRequest.SetEntityId(entityId1);
			PresenceFieldKey[] presenceFieldKeyArray = fieldList;
			for (int i = 0; i < (int)presenceFieldKeyArray.Length; i++)
			{
				PresenceFieldKey presenceFieldKey = presenceFieldKeyArray[i];
				FieldKey fieldKey = new FieldKey();
				fieldKey.SetProgram(presenceFieldKey.programId);
				fieldKey.SetGroup(presenceFieldKey.groupId);
				fieldKey.SetField(presenceFieldKey.fieldId);
				fieldKey.SetIndex(presenceFieldKey.index);
				queryRequest.AddKey(fieldKey);
			}
			this.m_rpcConnection.QueueRequest(this.m_presenceService.Id, 4, queryRequest, (RPCContext context) => this.RequestPresenceFieldsCallback(new bgs.types.EntityId(entityId), context), 0);
		}

		private void RequestPresenceFieldsCallback(bgs.types.EntityId entityId, RPCContext context)
		{
			if (base.CheckRPCCallback("RequestPresenceFieldsCallback", context))
			{
				foreach (Field fieldList in QueryResponse.ParseFrom(context.Payload).FieldList)
				{
					this.m_presenceCache.SetCache(entityId, fieldList.Key, fieldList.Value);
					PresenceUpdate boolValue = new PresenceUpdate()
					{
						entityId = entityId,
						programId = fieldList.Key.Program,
						groupId = fieldList.Key.Group,
						fieldId = fieldList.Key.Field,
						index = fieldList.Key.Index,
						boolVal = false,
						intVal = (long)0,
						stringVal = string.Empty,
						valCleared = false,
						blobVal = new byte[0]
					};
					if (fieldList.Value.HasBoolValue)
					{
						boolValue.boolVal = fieldList.Value.BoolValue;
					}
					else if (fieldList.Value.HasIntValue)
					{
						boolValue.intVal = fieldList.Value.IntValue;
					}
					else if (!fieldList.Value.HasFloatValue)
					{
						if (fieldList.Value.HasStringValue)
						{
							boolValue.stringVal = fieldList.Value.StringValue;
						}
						else if (fieldList.Value.HasBlobValue)
						{
							boolValue.blobVal = fieldList.Value.BlobValue;
						}
						else if (fieldList.Value.HasMessageValue)
						{
							if (fieldList.Key.Field != 8)
							{
								boolValue.blobVal = fieldList.Value.MessageValue;
							}
							else
							{
								this.FetchRichPresenceResource(fieldList.Value);
								this.HandleRichPresenceUpdate(boolValue, fieldList.Key);
							}
						}
						else if (fieldList.Value.HasFourccValue)
						{
							boolValue.stringVal = (new BnetProgramId(fieldList.Value.FourccValue)).ToString();
						}
						else if (!fieldList.Value.HasUintValue)
						{
							if (!fieldList.Value.HasEntityidValue)
							{
								boolValue.valCleared = true;
							}
							else
							{
								boolValue.entityIdVal.hi = fieldList.Value.EntityidValue.High;
								boolValue.entityIdVal.lo = fieldList.Value.EntityidValue.Low;
							}
						}
					}
					this.m_presenceUpdates.Add(boolValue);
				}
			}
		}

		private void ResolveRichPresence()
		{
			string str;
			if (this.m_pendingRichPresenceUpdates.Count == 0)
			{
				return;
			}
			List<PresenceUpdate> presenceUpdates = new List<PresenceUpdate>();
			foreach (PresenceUpdate mPendingRichPresenceUpdate in this.m_pendingRichPresenceUpdates)
			{
				if (!this.ResolveRichPresenceStrings(out str, mPendingRichPresenceUpdate.entityId, (ulong)0, 0))
				{
					continue;
				}
				presenceUpdates.Add(mPendingRichPresenceUpdate);
				PresenceUpdate presenceUpdate = mPendingRichPresenceUpdate;
				presenceUpdate.fieldId = 1000;
				presenceUpdate.stringVal = str;
				this.m_presenceUpdates.Add(presenceUpdate);
			}
			foreach (PresenceUpdate presenceUpdate1 in presenceUpdates)
			{
				this.m_pendingRichPresenceUpdates.Remove(presenceUpdate1);
			}
		}

		private bool ResolveRichPresenceStrings(out string richPresenceString, bgs.types.EntityId entityId, ulong index, int recurseDepth)
		{
			richPresenceString = string.Empty;
			FieldKey fieldKey = new FieldKey();
			fieldKey.SetProgram(BnetProgramId.BNET.GetValue());
			fieldKey.SetGroup(2);
			fieldKey.SetField(8);
			fieldKey.SetIndex(index);
			bnet.protocol.attribute.Variant cache = this.m_presenceCache.GetCache(entityId, fieldKey);
			if (cache == null)
			{
				base.ApiLog.LogError("Expected field missing from presence cache when resolving rich presence string");
				return false;
			}
			RichPresence richPresence = RichPresence.ParseFrom(cache.MessageValue);
			if (richPresence == null || !richPresence.IsInitialized)
			{
				base.ApiLog.LogError("Rich presence field did not contain valid RichPresence message when resolving");
				return false;
			}
			if (!this.m_richPresenceStringTables.ContainsKey(richPresence))
			{
				return false;
			}
			PresenceAPI.IndexToStringMap item = this.m_richPresenceStringTables[richPresence];
			if (!item.ContainsKey((ulong)richPresence.Index))
			{
				base.ApiLog.LogWarning("Rich presence string table data is missing");
				return false;
			}
			richPresenceString = item[(ulong)richPresence.Index];
			if (recurseDepth >= 1 || this.SubstituteVariables(out richPresenceString, richPresenceString, entityId, recurseDepth + 1))
			{
				return true;
			}
			base.ApiLog.LogWarning("Failed to substitute rich presence variables in: {0}", new object[] { richPresenceString });
			return false;
		}

		private void ResouceLookupCallback(bgs.ContentHandle contentHandle, object userContext)
		{
			if (contentHandle == null)
			{
				base.ApiLog.LogWarning("BN resource look up failed unable to proceed");
				this.DecrementOutstandingRichPresenceStringFetches();
				return;
			}
			base.ApiLog.LogDebug("Lookup done Region={0} Usage={1} SHA256={2}", new object[] { contentHandle.Region, contentHandle.Usage, contentHandle.Sha256Digest });
			this.m_battleNet.LocalStorage.GetFile(contentHandle, new LocalStorageAPI.DownloadCompletedCallback(this.DownloadCompletedCallback), userContext);
		}

		public void SetPresenceBlob(uint field, byte[] val)
		{
			UpdateRequest updateRequest = new UpdateRequest()
			{
				EntityId = this.m_battleNet.GameAccountId
			};
			FieldOperation fieldOperation = new FieldOperation();
			Field field1 = new Field();
			FieldKey fieldKey = new FieldKey();
			fieldKey.SetProgram(BnetProgramId.WOW.GetValue());
			fieldKey.SetGroup(2);
			fieldKey.SetField(field);
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			if (val == null)
			{
				val = new byte[0];
			}
			variant.SetBlobValue(val);
			field1.SetKey(fieldKey);
			field1.SetValue(variant);
			fieldOperation.SetField(field1);
			updateRequest.SetEntityId(this.m_battleNet.GameAccountId);
			updateRequest.AddFieldOperation(fieldOperation);
			this.PublishField(updateRequest);
		}

		public void SetPresenceBool(uint field, bool val)
		{
			UpdateRequest updateRequest = new UpdateRequest();
			FieldOperation fieldOperation = new FieldOperation();
			Field field1 = new Field();
			FieldKey fieldKey = new FieldKey();
			fieldKey.SetProgram(BnetProgramId.WOW.GetValue());
			fieldKey.SetGroup(2);
			fieldKey.SetField(field);
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetBoolValue(val);
			field1.SetKey(fieldKey);
			field1.SetValue(variant);
			fieldOperation.SetField(field1);
			updateRequest.SetEntityId(this.m_battleNet.GameAccountId);
			updateRequest.AddFieldOperation(fieldOperation);
			this.PublishField(updateRequest);
		}

		public void SetPresenceInt(uint field, long val)
		{
			UpdateRequest updateRequest = new UpdateRequest()
			{
				EntityId = this.m_battleNet.GameAccountId
			};
			FieldOperation fieldOperation = new FieldOperation();
			Field field1 = new Field();
			FieldKey fieldKey = new FieldKey();
			fieldKey.SetProgram(BnetProgramId.WOW.GetValue());
			fieldKey.SetGroup(2);
			fieldKey.SetField(field);
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetIntValue(val);
			field1.SetKey(fieldKey);
			field1.SetValue(variant);
			fieldOperation.SetField(field1);
			updateRequest.SetEntityId(this.m_battleNet.GameAccountId);
			updateRequest.AddFieldOperation(fieldOperation);
			this.PublishField(updateRequest);
		}

		public void SetPresenceString(uint field, string val)
		{
			UpdateRequest updateRequest = new UpdateRequest()
			{
				EntityId = this.m_battleNet.GameAccountId
			};
			FieldOperation fieldOperation = new FieldOperation();
			Field field1 = new Field();
			FieldKey fieldKey = new FieldKey();
			fieldKey.SetProgram(BnetProgramId.WOW.GetValue());
			fieldKey.SetGroup(2);
			fieldKey.SetField(field);
			bnet.protocol.attribute.Variant variant = new bnet.protocol.attribute.Variant();
			variant.SetStringValue(val);
			field1.SetKey(fieldKey);
			field1.SetValue(variant);
			fieldOperation.SetField(field1);
			updateRequest.SetEntityId(this.m_battleNet.GameAccountId);
			updateRequest.AddFieldOperation(fieldOperation);
			this.PublishField(updateRequest);
		}

		private bool SubstituteVariables(out string substitutedString, string originalStr, bgs.types.EntityId entityId, int recurseDepth)
		{
			int num;
			ulong num1;
			string str;
			bool flag;
			substitutedString = originalStr;
			int num2 = 0;
		Label1:
			while (num2 < substitutedString.Length)
			{
				num2 = substitutedString.IndexOf("$0x", num2);
				if (num2 != -1)
				{
					int length = num2 + "$0x".Length;
					int num3 = this.LastIndexOfOccurenceFromIndex(substitutedString, PresenceAPI.hexChars, length);
					int num4 = num3 + 1 - length;
					num = num3 + 1 - num2;
					string str1 = substitutedString.Substring(length, num4);
					num1 = (ulong)0;
					try
					{
						num1 = Convert.ToUInt64(str1, 16);
						goto Label0;
					}
					catch (Exception exception)
					{
						base.ApiLog.LogWarning("Failed to convert {0} to ulong when substiting rich presence variables", new object[] { str1 });
						flag = false;
					}
					return flag;
				}
				else
				{
					break;
				}
			}
			return true;
		Label0:
			if (!this.ResolveRichPresenceStrings(out str, entityId, num1, recurseDepth))
			{
				base.ApiLog.LogWarning("Failed resolve rich presence string for \"{0}\" when substiting variables in \"{1}\"", new object[] { num1, originalStr });
				return false;
			}
			string str2 = substitutedString.Substring(num2, num);
			substitutedString = substitutedString.Replace(str2, str);
			goto Label1;
		}

		private void TryToResolveRichPresence()
		{
			if (this.m_numOutstandingRichPresenceStringFetches == 0)
			{
				this.ResolveRichPresence();
				if (this.m_pendingRichPresenceUpdates.Count != 0)
				{
					base.ApiLog.LogWarning("Failed to resolve rich presence strings");
					this.m_pendingRichPresenceUpdates.Clear();
				}
			}
		}

		private class EntityIdToFieldsMap : Map<bgs.types.EntityId, PresenceAPI.FieldKeyToPresenceMap>
		{
			public EntityIdToFieldsMap()
			{
			}

			public bnet.protocol.attribute.Variant GetCache(bgs.types.EntityId entity, FieldKey key)
			{
				if (key == null)
				{
					return null;
				}
				if (!base.ContainsKey(entity))
				{
					return null;
				}
				PresenceAPI.FieldKeyToPresenceMap item = base[entity];
				if (!item.ContainsKey(key))
				{
					return null;
				}
				return item[key];
			}

			public void SetCache(bgs.types.EntityId entity, FieldKey key, bnet.protocol.attribute.Variant value)
			{
				if (key == null)
				{
					return;
				}
				if (!base.ContainsKey(entity))
				{
					base[entity] = new PresenceAPI.FieldKeyToPresenceMap();
				}
				base[entity][key] = value;
			}
		}

		private class FieldKeyToPresenceMap : Map<FieldKey, bnet.protocol.attribute.Variant>
		{
			public FieldKeyToPresenceMap()
			{
			}
		}

		private class IndexToStringMap : Map<ulong, string>
		{
			public IndexToStringMap()
			{
			}
		}

		public class PresenceRefCountObject
		{
			public ulong objectId;

			public int refCount;

			public PresenceRefCountObject()
			{
			}
		}

		public class PresenceUnsubscribeContext
		{
			private ulong m_objectId;

			private BattleNetCSharp m_battleNet;

			public PresenceUnsubscribeContext(BattleNetCSharp battleNet, ulong objectId)
			{
				this.m_battleNet = battleNet;
				this.m_objectId = objectId;
			}

			public void PresenceUnsubscribeCallback(RPCContext context)
			{
				if (this.m_battleNet.Presence.CheckRPCCallback("PresenceUnsubscribeCallback", context))
				{
					this.m_battleNet.Channel.RemoveActiveChannel(this.m_objectId);
				}
			}
		}

		private class RichPresenceToStringsMap : Map<RichPresence, PresenceAPI.IndexToStringMap>
		{
			public RichPresenceToStringsMap()
			{
			}
		}
	}
}