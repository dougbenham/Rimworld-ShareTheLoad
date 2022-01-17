using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Share_The_Load.DeepStorage
{
	static class Reserve_Patch
	{
		//public bool Reserve(Pawn claimant, Job job, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null)
		public static bool Prefix(Pawn claimant, Job job, LocalTargetInfo target, int stackCount, ref bool __result)
		{
			/*if (target.HasThing)
				Verse.Log.Message($"{claimant} reserved {stackCount} of [{target.Label}] {target.Thing.ToStringSafe()} ({target.Thing.stackCount}) @ {target.Cell} during job {job.ToStringSafe()}");
			else
				Verse.Log.Message($"{claimant} reserved {stackCount} of [{target.Label}] {target.Cell} during job {job.ToStringSafe()}");*/

			if (claimant.IsUs()
			    && target.Cell != LocalTargetInfo.Invalid
			    && !target.HasThing // only allow location to be reserved infinitely, not specific items
			    && claimant.Map.thingGrid.ThingsAt(target.Cell).Any(t => t.GetDeepStorageComp() != null)
			    && job.def == JobDefOf.HaulToCell)
			{
				__result = true;
				return false;
			}

			return true;
		}
	}
}