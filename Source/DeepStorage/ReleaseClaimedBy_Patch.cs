using Verse;
using Verse.AI;

namespace Share_The_Load.DeepStorage
{
	static class ReleaseClaimedBy_Patch
	{
		//public void ReleaseClaimedBy(Pawn claimant, Job job)
		public static void Prefix(Pawn claimant, Job job)
		{
			Log.Message($"{claimant} releasing all jobs of type {job.ToStringSafe()}");
		}
	}
}