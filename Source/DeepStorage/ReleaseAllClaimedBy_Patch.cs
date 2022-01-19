using Verse;

namespace Share_The_Load.DeepStorage
{
	public class ReleaseAllClaimedBy_Patch
	{
		//void ReleaseAllClaimedBy(Pawn claimant)
		public static void Prefix(Pawn claimant)
		{
			Log.Message($"Releasing all reservations by {claimant.ToStringSafe()}");
		}
	}
}