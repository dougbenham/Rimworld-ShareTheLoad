using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Share_The_Load.Constructible
{
	[HarmonyPatch(typeof(WorkGiver_ConstructDeliverResources), "ResourceDeliverJobFor")]
	public static class MaterialsNeeded_Patch
	{
		//protected Job ResourceDeliverJobFor(Pawn pawn, IConstructible c, bool canRemoveExistingFloorUnderNearbyNeeders = true)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo MaterialsNeededInfo = AccessTools.Method(typeof(IConstructible), "MaterialsNeeded");

			MethodInfo FilterForExpectedInfo = AccessTools.Method(typeof(MaterialsNeeded_Patch), nameof(MaterialsNeeded_Patch.FilterForExpected));

			foreach (CodeInstruction i in instructions)
			{
				yield return i;
				if(i.Calls(MaterialsNeededInfo))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_2);//constructible thing
					yield return new CodeInstruction(OpCodes.Call, FilterForExpectedInfo);
				}
			}
		}

		public static List<ThingDefCountClass> FilterForExpected(List<ThingDefCountClass> materialsNeeded, Thing c)
		{
			List<ThingDefCountClass> needs = new List<ThingDefCountClass>(materialsNeeded.Count);
			foreach(var t in materialsNeeded)
			{
				needs.Add(new ThingDefCountClass(t.thingDef, t.count));
			}
			
			for (int i = 0; i < needs.Count; i++)
			{
				ThingDefCountClass thingNeeds = needs[i];
				thingNeeds.count -= ExpectingComp.ExpectedCount(c, thingNeeds.thingDef);
				if(thingNeeds.count <= 0)
				{
					needs.Remove(thingNeeds);
					i--;
				}
			}
			return needs;
		}
	}
}