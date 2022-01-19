using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Share_The_Load.Constructible
{
	//[HarmonyPatch(typeof(Blueprint), nameof(Blueprint.TryReplaceWithSolidThing))]
	static class Blueprint_TryReplaceWithSolidThing_Patch
	{
		class InterruptedHaul
		{
			public Pawn Pawn { get; set; }
			public Thing Thing { get; set; }
			public int Count { get; set; }
		}
		
		static void Prefix(Blueprint __instance, Pawn workerPawn, out List<InterruptedHaul> __state)
		{
			__state = new List<InterruptedHaul>();

			foreach (var reservation in __instance.Map.reservationManager.ReservationsReadOnly)
			{
				// Find reservations that are hauling to this blueprint and track them
				if (reservation.Job.def == JobDefOf.HaulToContainer &&
					reservation.Claimant.IsCarryingThing(reservation.Job.targetA.Thing) &&
					reservation.Job.targetB == __instance)
				{
					__state.Add(new InterruptedHaul() {Pawn = reservation.Claimant, Thing = reservation.Job.targetA.Thing, Count = reservation.Job.targetA.Thing.stackCount});
					//Verse.Log.Message($"{reservation.Claimant.ThingID} was carrying {reservation.Job.targetA.Thing.Label} to {__instance.Label}, is the pawn who solidified? {reservation.Claimant == workerPawn}");
				}
			}
		}

		static void Postfix(Blueprint __instance, List<InterruptedHaul> __state, ref Thing createdThing)
		{
			var b = createdThing as Frame;
			//Verse.Log.Message($"Frame came out? {b != null} | Materials needed: {b?.MaterialsNeeded().Count}");

			// If the blueprint did actually turn into a frame
			if (createdThing is Frame f && f.MaterialsNeeded().Count > 0)
			{
				foreach (var l in __state)
				{
					//Verse.Log.Message($"After solidification, {l.Pawn.ThingID} is carrying {l.Pawn.carryTracker.CarriedThing?.Label} and has job [{l.Pawn.CurJob}]");

					//l.Pawn.CurJob.targetQueueB = l.Pawn.CurJob.targetQueueB ?? new List<LocalTargetInfo>();
					//l.Pawn.CurJob.targetQueueB.Add(new LocalTargetInfo(createdThing));
					// reassign each pawn to haul to the frame instead of the blueprint
					/*l.Pawn.jobs.StopAll();*/

					//var targetA = new LocalTargetInfo(l.Thing);
					//var canReserve = l.Pawn.CanReserve(targetA, stackCount: l.Count);

					//Verse.Log.Message($"After stopping, {l.Pawn.ThingID} is carrying {l.Pawn.carryTracker.CarriedThing?.Label} and has job [{l.Pawn.CurJob}] | TargetA spawned? {l.Thing.Spawned} | Can reserve? {canReserve}");

					/*if (canReserve)
					{
						var job = JobMaker.MakeJob(JobDefOf.HaulToContainer, targetA, new LocalTargetInfo(createdThing));
						job.count = l.Count;
						job.haulMode = HaulMode.ToContainer;
						l.Pawn.jobs.StartJob(job, JobCondition.InterruptForced);
					}*/
				}
			}
		}
	}
}