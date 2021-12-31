﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;
using HarmonyLib;
using UnityEngine;//For the Mathf.min of 3 things

namespace Share_The_Load
{
	[HarmonyPatch(typeof(ReservationManager), "CanReserve")]
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

	[HarmonyPatch(typeof(ReservationManager), "Reserve")]
	static class Reserve_Patch
	{
		//public bool Reserve(Pawn claimant, Job job, LocalTargetInfo target, int maxPawns = 1, int stackCount = -1, ReservationLayerDef layer = null)
		public static bool Prefix(Pawn claimant, Job job, LocalTargetInfo target, ref bool __result)
		{
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

	[HarmonyPatch(typeof(ReservationManager), "Release")]
	static class Release_Patch
	{
		//public void Release(LocalTargetInfo target, Pawn claimant, Job job)
		public static void Prefix(LocalTargetInfo target, Pawn claimant, Job job)
		{
			if (claimant.IsUs()
				&& target.Thing is IConstructible c && !(c is Blueprint_Install)
				&& job.def == JobDefOf.HaulToContainer)
				ExpectingComp.Remove(q => q.claimant == claimant && q.job == job && q.claimed == target.Thing);
		}
	}

	[HarmonyPatch(typeof(ReservationManager), "ReleaseClaimedBy")]
	static class ReleaseClaimedBy_Patch
	{
		//public void ReleaseClaimedBy(Pawn claimant, Job job)
		public static void Prefix(Pawn claimant, Job job)
		{
			if (job.def == JobDefOf.HaulToContainer)
				ExpectingComp.Remove(q => q.claimant == claimant && q.job == job);
		}
	}

	[HarmonyPatch(typeof(ReservationManager), "ReleaseAllClaimedBy")]
	static class ReleaseAllClaimedBy_Patch
	{
		//public void ReleaseAllClaimedBy(Pawn claimant)
		public static void Prefix(Pawn claimant)
		{
			ExpectingComp.Remove(q => q.claimant == claimant);
		}
	}
}
