using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Share_The_Load.ProjectRimFactory
{
	static class Reserve_Patch
	{
		//public bool Reserve(Pawn claimant, Job job, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null)
		public static bool Prefix(Pawn claimant, Job job, LocalTargetInfo target, ref bool __result)
		{
			if (claimant.IsUs() && target.Cell != LocalTargetInfo.Invalid
			                    && claimant.Map.thingGrid.ThingsAt(target.Cell).Any(t => t.IsPrfStorage())
			                    && job.def == JobDefOf.HaulToCell)
			{
				__result = true;
				return false;
			}

			return true;
		}
	}
}