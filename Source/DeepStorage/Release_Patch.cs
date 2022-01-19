using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Share_The_Load.DeepStorage
{
	static class Release_Patch
	{
		//public void Release(LocalTargetInfo target, Pawn claimant, Job job)
		public static void Prefix(LocalTargetInfo target, Pawn claimant, Job job)
		{
			if (target.HasThing)
				Log.Message($"{claimant} released [{target.Label}] {target.Thing.ToStringSafe()} ({target.Thing.stackCount}) @ {target.Cell} during job {job.ToStringSafe()}");
			else
				Log.Message($"{claimant} released [{target.Label}] {target.Cell} during job {job.ToStringSafe()}");
		}
	}
}