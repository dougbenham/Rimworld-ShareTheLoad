using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Share_The_Load.ExtendedStorage
{
	//[HarmonyPatch(typeof(ReservationManager), "Release")]
	static class Release_Patch
	{
		//public void Release(LocalTargetInfo target, Pawn claimant, Job job)
		public static void Prefix(LocalTargetInfo target, Pawn claimant, Job job)
		{
			if (claimant.IsUs() &&
			    target.Cell != LocalTargetInfo.Invalid &&
			    claimant.Map.thingGrid.ThingsAt(target.Cell).FirstOrDefault(t => t.GetType() == Patches.typeBuilding_ExtendedStorage) is Thing thing &&
			    job.def == JobDefOf.HaulToCell)
				ExpectingComp.Remove(q => q.claimant == claimant && q.job == job && q.claimed == thing);
		}
	}
}