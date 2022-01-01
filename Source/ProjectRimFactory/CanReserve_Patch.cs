using System.Linq;
using Verse;

namespace Share_The_Load.ProjectRimFactory
{
	static class CanReserve_Patch
	{
		//public bool CanReserve(Pawn claimant, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null, bool ignoreOtherReservations = false)
		public static bool Prefix(Pawn claimant, LocalTargetInfo target, ref bool __result)
		{
			if (claimant.IsUs() && target.Cell != LocalTargetInfo.Invalid)
			{
				if (claimant.Map.thingGrid.ThingsAt(target.Cell).Any(t => t.IsPrfStorage()))
				{
					__result = true;
					return false;
				}
			}
			return true;
		}
	}
}