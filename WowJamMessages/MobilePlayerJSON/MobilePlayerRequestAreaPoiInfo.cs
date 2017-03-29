using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4813, Name="MobilePlayerRequestAreaPoiInfo", Version=38820897)]
	public class MobilePlayerRequestAreaPoiInfo
	{
		public MobilePlayerRequestAreaPoiInfo()
		{
		}
	}
}