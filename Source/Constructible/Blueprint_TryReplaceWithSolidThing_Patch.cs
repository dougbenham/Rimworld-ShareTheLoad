using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Share_The_Load.Constructible
{
	[HarmonyPatch(typeof(Blueprint), nameof(Blueprint.TryReplaceWithSolidThing))]
	static class Blueprint_TryReplaceWithSolidThing_Patch
	{
		class InterruptedHaul
		{
			public Pawn Pawn { get; set; }
			public Thing Thing { get; set; }
			public int Count { get; set; }
		}
		
		static void Prefix(Blueprint __instance, out List<InterruptedHaul> __state)
		{
			__state = new List<InterruptedHaul>();

			foreach (var reservation in __instance.Map.reservationManager.ReservationsReadOnly)
			{
				// Find reservations that are hauling to this blueprint and track them
				if (reservation.Job.def == JobDefOf.HaulToContainer &&
					reservation.Job.targetB == __instance)
				{
					__state.Add(new InterruptedHaul() {Pawn = reservation.Claimant, Thing = reservation.Job.targetA.Thing, Count = reservation.Job.targetA.Thing.stackCount});
				}
			}
		}

		static void Postfix(List<InterruptedHaul> __state, ref Thing createdThing)
		{
			// If the blueprint did actually turn into a frame
			if (createdThing != null)
			{
				foreach (var l in __state)
				{
					// reassign each pawn to haul to the frame instead of the blueprint
					l.Pawn.jobs.StopAll();

					var job = JobMaker.MakeJob(JobDefOf.HaulToContainer, new LocalTargetInfo(l.Thing), new LocalTargetInfo(createdThing));
					job.count = l.Count;
					job.haulMode = HaulMode.ToContainer;
					l.Pawn.jobs.StartJob(job, JobCondition.InterruptForced);
				}
			}
		}
	}
}