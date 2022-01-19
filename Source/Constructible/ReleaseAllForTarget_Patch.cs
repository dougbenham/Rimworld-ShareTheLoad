using HarmonyLib;
using Verse;
using Verse.AI;

namespace Share_The_Load.Constructible
{
	[HarmonyPatch(typeof(ReservationManager), nameof(ReservationManager.ReleaseAllForTarget))]
	public class ReleaseAllForTarget_Patch
	{
		//void ReleaseAllForTarget(Thing t)
		public static void Prefix(Thing t)
		{
			Log.Message($"Releasing all reservations for {t.ToStringSafe()}");

			ExpectingComp.Remove(q => q.claimed == t);
		}
	}
}