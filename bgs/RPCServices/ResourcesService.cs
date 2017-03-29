using bgs;
using System;

namespace bgs.RPCServices
{
	public class ResourcesService : ServiceDescriptor
	{
		public const uint GET_CONTENT_HANDLE = 1;

		public ResourcesService() : base("bnet.protocol.resources.Resources")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.resources.Resources.GetContentHandle", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ContentHandle>)) };
			}
		}
	}
}