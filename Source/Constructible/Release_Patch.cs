using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Share_The_Load.Constructible
{
	[HarmonyPatch(typeof(ReservationManager), nameof(ReservationManager.Release))]
	static class Release_Patch
	{
		//public void Release(LocalTargetInfo target, Pawn claimant, Job job)
		public static void Prefix(LocalTargetInfo target, Pawn claimant, Job job)
		{
			if (claimant.IsUs()
			    && target.Thing is IConstructible c && !(c is Blueprint_Install)
			    && job.def == JobDefOf.HaulToContainer)
				ExpectingComp.Remove(q => q.claimant == claimant && q.job == job && q.claimed == target.Thing);
		}
	}
}