﻿using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;
using RimWorld;

namespace Share_The_Load
{
	[HarmonyPatch(typeof(GenConstruct), "HandleBlockingThingJob")]
	class HandleAllBlockingThings
	{
		//public static Job HandleBlockingThingJob(Thing constructible, Pawn worker, bool forced = false)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo FirstBlockingThingInfo = AccessTools.Method(typeof(GenConstruct), "FirstBlockingThing");

			MethodInfo FirstReservableBlockingThingInfo = AccessTools.Method(typeof(HandleAllBlockingThings), nameof(FirstReservableBlockingThing));

			foreach (CodeInstruction i in instructions)
			{
				if (i.Calls(FirstBlockingThingInfo))
					yield return new CodeInstruction(OpCodes.Call, FirstReservableBlockingThingInfo);
				else
					yield return i;
			}
		}

		public static Thing FirstReservableBlockingThing(Thing constructible, Pawn pawnToIgnore)
		{
			Thing thing = constructible is Blueprint b ? GenConstruct.MiniToInstallOrBuildingToReinstall(b) : null;

			foreach(var pos in constructible.OccupiedRect())
				foreach(Thing t in pos.GetThingList(constructible.Map))
					if (GenConstruct.BlocksConstruction(constructible, t) && t != pawnToIgnore && t != thing && pawnToIgnore.CanReserve(t))
						return t;

			return null;
		}
	}

	[HarmonyPatch(typeof(GenConstruct), "BlocksConstruction")]
	static class PawnBlockConstruction
	{
		static bool Prefix(ref bool __result, Thing t)
		{
			if (t is Pawn)
			{
				__result = false;
				return false;
			}
			return true;
		}
	}
}
