using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Share_The_Load.Constructible
{
	[HarmonyPatch(typeof(ReservationManager), nameof(ReservationManager.ReleaseClaimedBy))]
	static class ReleaseClaimedBy_Patch
	{
		//public void ReleaseClaimedBy(Pawn claimant, Job job)
		public static void Prefix(Pawn claimant, Job job)
		{
			if (job.def == JobDefOf.HaulToContainer)
				ExpectingComp.Remove(q => q.claimant == claimant && q.job == job);
		}
	}
}