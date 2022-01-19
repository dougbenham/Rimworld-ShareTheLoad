using System.Linq;
using Verse;

namespace Share_The_Load.DeepStorage
{
	static class CanReserve_Patch
	{
		//public bool CanReserve(Pawn claimant, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null, bool ignoreOtherReservations = false)
		public static bool Prefix(Pawn claimant, LocalTargetInfo target, int stackCount, ref bool __result)
		{
			if (target.HasThing)
				Log.Message($"{claimant} can reserve {stackCount} of [{target.Label}] {target.Thing.ToStringSafe()} ({target.Thing.stackCount}) @ {target.Cell}?");
			else
				Log.Message($"{claimant} can reserve {stackCount} of [{target.Label}] {target.Cell}?");

			if (claimant.IsUs() &&
			    target.Cell != LocalTargetInfo.Invalid &&
			    !target.HasThing && // only allow location to be reserved infinitely, not specific items
			    claimant.Map.thingGrid.ThingsAt(target.Cell).Any(t => t.GetDeepStorageComp() != null))
			{
				__result = true;
				return false;
			}

			return true;
		}
	}
}