using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Share_The_Load.Constructible
{
	[HarmonyPatch(typeof(WorkGiver_ConstructDeliverResources), "FindNearbyNeeders")]
	public static class FindNearbyNeeders_Patch
	{
		//private HashSet<Thing> FindNearbyNeeders(Pawn pawn, ThingDefCountClass need, IConstructible c, int resTotalAvailable, bool canRemoveExistingFloorUnderNearbyNeeders, out int neededTotal, out Job jobToMakeNeederAvailable)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo AmountNeededByOfInfo = AccessTools.Method(typeof(GenConstruct), "AmountNeededByOf");
			FieldInfo CountThingDefInfo = AccessTools.Field(typeof(ThingDefCountClass), "thingDef");

			MethodInfo AdjustForExpectedInfo = AccessTools.Method(typeof(FindNearbyNeeders_Patch), nameof(AdjustForExpected));

			foreach (CodeInstruction i in instructions)
			{
				yield return i;
				if(i.Calls(AmountNeededByOfInfo))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_2);
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					yield return new CodeInstruction(OpCodes.Ldfld, CountThingDefInfo);
					yield return new CodeInstruction(OpCodes.Call, AdjustForExpectedInfo);
				}
			}
		}

		public static int AdjustForExpected(int needed, Thing c, ThingDef resource)
		{
			return needed - ExpectingComp.ExpectedCount(c, resource);
		}
	}
}
