using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Share_The_Load.Constructible
{
	[HarmonyPatch(typeof(ReservationManager), nameof(ReservationManager.Reserve))]
	static class Reserve_Patch
	{
		//public bool Reserve(Pawn claimant, Job job, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null)
		public static bool Prefix(Pawn claimant, Job job, LocalTargetInfo target, int stackCount, ref bool __result)
		{
			if (target.HasThing)
				Log.Message($"{claimant} reserved {stackCount} of {target.Thing.ToStringSafe()} ({target.Thing.stackCount}) @ {target.Cell} during {job.def.defName} ({job.count})");
			else
				Log.Message($"{claimant} reserved {stackCount} of {target.Cell} during {job.def.defName} ({job.count})");

			if (claimant.IsUs()
			    && target.Thing is IConstructible c && !(c is Blueprint_Install)
			    && job.def == JobDefOf.HaulToContainer
			    && c.MaterialsNeeded().Count > 0)
			{
				int count = job.count;
				Thing building = target.Thing;
				Thing deliverThing = job.targetA.Thing;
				ThingDef resource = deliverThing.def;
				int neededCount = c.MaterialsNeeded().FirstOrDefault(tc => tc.thingDef == resource)?.count ?? 0;

				Log.Message($"{claimant} reserving {building} resource = {resource}({count})");
				Log.Message($"	out of: {c.MaterialsNeeded().ToStringSafeEnumerable()}");


				int availableCount = deliverThing.stackCount + job.targetQueueA?.Sum(tar => tar.Thing.stackCount) ?? 0;
				count = Mathf.Min(new int[] { count, claimant.carryTracker.MaxStackSpaceEver(resource), availableCount, neededCount });
				Log.Message($"{c} was expecting {resource}(" + ExpectingComp.ExpectedCount(building, resource) + ")");
				ExpectingComp.Add(claimant, job, building, resource, count);
				Log.Message($"{c} now expecting {resource}(" + ExpectingComp.ExpectedCount(building, resource) + ")");
				__result = true;
				return false;
			}
			return true;
		}
	}
}