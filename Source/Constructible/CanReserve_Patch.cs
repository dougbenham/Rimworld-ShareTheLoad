using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Share_The_Load.Constructible
{
	[HarmonyPatch(typeof(ReservationManager), nameof(ReservationManager.CanReserve))]
	static class CanReserve_Patch
	{
		//public bool CanReserve(Pawn claimant, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null, bool ignoreOtherReservations = false)
		public static bool Prefix(Pawn claimant, LocalTargetInfo target, ref bool __result)
		{
			if (claimant.IsUs() && target.Thing is IConstructible c && !(c is Blueprint_Install))
			{
				Log.Message($"{claimant} can reserve? {c} needs {c.MaterialsNeeded().ToStringSafeEnumerable()}");

				// Even though you'd like to check MaterialsNeeded_Patch.FilterForExpected(c.MaterialsNeeded(), c)
				// We don't know if the job is deliver or finish, so can't decide at this point

				if (c.MaterialsNeeded().Count > 0)
				{
					Log.Message($"{claimant} can reserve {target.Thing}");
					__result = true;
					return false;
				}
			}
			return true;
		}
	}
}