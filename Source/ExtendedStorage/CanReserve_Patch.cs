using System.Linq;
using Verse;

namespace Share_The_Load.ExtendedStorage
{
	//[HarmonyPatch(typeof(ReservationManager), "CanReserve")]
	static class CanReserve_Patch
	{
		//public bool CanReserve(Pawn claimant, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null, bool ignoreOtherReservations = false)
		public static bool Prefix(Pawn claimant, LocalTargetInfo target, ref bool __result)
		{
			if (claimant.IsUs() && target.Cell != LocalTargetInfo.Invalid)
			{
				if(claimant.Map.thingGrid.ThingsAt(target.Cell)
						.FirstOrDefault(t => t.GetType() == Patches.typeBuilding_ExtendedStorage)
					is Thing storage)
				{
					Log.Message($"{claimant} can reserve? {target.Cell} is {storage}");

					int canDo = storage.ApparentMaxStorage() - storage.StoredThingTotal();
					int expected = ExpectingComp.ExpectedCount(q => q.claimed == storage);

					if (canDo > expected)
					{
						Log.Message($"{claimant} can reserve {storage}");
						__result = true;
						return false;
					}
				}
			}
			return true;
		}
	}
}