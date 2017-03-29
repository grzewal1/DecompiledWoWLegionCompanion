using bgs;
using System;

namespace bgs.Shared.Util
{
	public interface IRegistry
	{
		BattleNetErrors DeleteData(string path, string name);

		BattleNetErrors RetrieveInt(string path, string name, out int i);

		BattleNetErrors RetrieveString(string path, string name, out string s, bool encrypt = false);

		BattleNetErrors RetrieveVector(string path, string name, out byte[] vec, bool encrypt = true);
	}
}