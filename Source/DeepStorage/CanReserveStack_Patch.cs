using Verse;

namespace Share_The_Load.DeepStorage
{
	static class CanReserveStack_Patch
	{
		// int CanReserveStack(Pawn claimant, LocalTargetInfo target, int maxPawns = 1, ReservationLayerDef layer = null, bool ignoreOtherReservations = false)
		public static bool Prefix(Pawn claimant, LocalTargetInfo target, ref int __result)
		{
			if (target.HasThing)
				Log.Message($"{claimant} can reserve stack of [{target.Label}] {target.Thing.ToStringSafe()} ({target.Thing.stackCount}) @ {target.Cell}?");
			else
				Log.Message($"{claimant} can reserve stack of [{target.Label}] {target.Cell}?");
			
			return true;
		}
	}
}