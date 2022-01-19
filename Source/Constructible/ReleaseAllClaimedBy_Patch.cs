using HarmonyLib;
using Verse;
using Verse.AI;

namespace Share_The_Load.Constructible
{
	[HarmonyPatch(typeof(ReservationManager), nameof(ReservationManager.ReleaseAllClaimedBy))]
	static class ReleaseAllClaimedBy_Patch
	{
		//public void ReleaseAllClaimedBy(Pawn claimant)
		public static void Prefix(Pawn claimant)
		{
			Log.Message($"Releasing all reservations for {claimant.ToStringSafe()}");

			ExpectingComp.Remove(q => q.claimant == claimant);
		}
	}
}
