using Verse;

namespace Share_The_Load.DeepStorage
{
	public class ReleaseAllForTarget_Patch
	{
		//void ReleaseAllForTarget(Thing t)
		public static void Prefix(Thing t)
		{
			Log.Message($"Releasing all reservations for {t.ToStringSafe()}");
		}
	}
}