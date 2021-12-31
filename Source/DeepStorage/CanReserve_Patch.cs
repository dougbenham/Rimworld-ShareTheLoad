using System.Linq;
using RimWorld;
using Verse;

namespace Share_The_Load.DeepStorage
{
	static class CanReserve_Patch
	{
		//public bool CanReserve(Pawn claimant, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null, bool ignoreOtherReservations = false)
		public static bool Prefix(Pawn claimant, LocalTargetInfo target, ref bool __result)
		{
			if (claimant.IsUs() && target.Cell != LocalTargetInfo.Invalid)
			{
				if (claimant.Map.thingGrid.ThingsAt(target.Cell).FirstOrDefault(t => t.GetDeepStorageComp() != null) is Thing storage)
				{
					Log.Message($"{claimant} can reserve {storage}");
					__result = true;
					return false;
				}
			}
			return true;
		}
	}
}