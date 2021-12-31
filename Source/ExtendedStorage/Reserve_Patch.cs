using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Share_The_Load.ExtendedStorage
{
	//[HarmonyPatch(typeof(ReservationManager), "Reserve")]
	static class Reserve_Patch
	{
		//public bool Reserve(Pawn claimant, Job job, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null)
		public static bool Prefix(Pawn claimant, Job job, LocalTargetInfo target, ref bool __result)
		{
			if (claimant.IsUs() && target.Cell != LocalTargetInfo.Invalid
			                    && claimant.Map.thingGrid.ThingsAt(target.Cell)
				                    .FirstOrDefault(t => t.GetType() == ExtendedStoragePatches.typeBuilding_ExtendedStorage) is Thing storage
			                    && job.def == JobDefOf.HaulToCell)
			{
				int canDo = storage.ApparentMaxStorage() - storage.StoredThingTotal();
				if (canDo > 0)
				{
					int count = job.count;
					Thing deliverThing = job.targetA.Thing;
					ThingDef resource = deliverThing.def;

					Log.Message($"{claimant} reservingES {storage} resource = {resource}({count})");
					Log.Message($"	out of: {canDo}");


					int availableCount = deliverThing.stackCount;// + job.targetQueueA?.Sum(tar => tar.Thing.stackCount) ?? 0;
					//HaulToCell doesn't queue up its reservations, and so we don't know if there are more to get
					count = Mathf.Min(new int[] { count, claimant.carryTracker.MaxStackSpaceEver(resource), availableCount, canDo });

					Log.Message($"{storage} was expecting {resource}(" + ExpectingComp.ExpectedCount(storage, resource) + ")");
					ExpectingComp.Add(claimant, job, storage, resource, count);
					Log.Message($"{storage} now expecting {resource}(" + ExpectingComp.ExpectedCount(storage, resource) + ")");

					__result = true;
					return false;
				}
			}
			return true;
		}
	}
}