﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using Verse;
using Harmony;

namespace Share_The_Load
{
	[HarmonyPatch(typeof(ItemAvailability), "ThingsAvailableAnywhere")]
	public static class DeliverAsMuchAsPossible
	{
		//public bool ThingsAvailableAnywhere(ThingDefCountClass need, Pawn pawn)
		public static bool Prefix(ThingDefCountClass need, Pawn pawn, ref bool __result)
		{
			if (Settings.Get().deliverAsMuchAsYouCan)
			{
				List<Thing> list = pawn.Map.listerThings.ThingsOfDef(need.thingDef);
				__result = list.Any(t => !t.IsForbidden(pawn));
				return false;
			}
			return true;
		}
	}

	//This is just to change a break into a continue
	//Honestly is a vanilla bug
	//Would only deliver resource #2 once there's enough resource #1 
	//Though resource #1 doesn't care if there's enough #2
	//[HarmonyPatch(typeof(WorkGiver_ConstructDeliverResources), "ResourceDeliverJobFor")]
	public static class BreakToContinue_Patch
	{
		//protected Job ResourceDeliverJobFor(Pawn pawn, IConstructible c, bool canRemoveExistingFloorUnderNearbyNeeders = true)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
		{
			Label forCheck = new Label();
			Label continueLabel = il.DefineLabel();
			bool setLabel = false;

			List<CodeInstruction> instList = instructions.ToList();
			for(int i = 0; i < instList.Count(); i++)
			{
				CodeInstruction inst = instList[i];

				//i=0;Br To Check label
				if (inst.opcode == OpCodes.Br && instList[i - 1].opcode == OpCodes.Stloc_S && instList[i - 2].opcode == OpCodes.Ldc_I4_0)
				{
					forCheck = (Label)inst.operand;

					List<CodeInstruction> instructionsList = instructions.ToList();
					for (int k = instructionsList.Count() - 1; k >= 0; k--)
					{
						if (instructionsList[k].labels.Contains(forCheck))
						{
							instructionsList[k - 4].labels.Add(continueLabel);
							setLabel = true;
							break;
						}
					}
				}
				else if (inst.opcode == OpCodes.Br && instList[i - 2].opcode == OpCodes.Ldfld)// operand == need, but inside a compilergenerated mess
				{
					if (setLabel)
						inst.operand = continueLabel;
				}
				yield return inst;
			}
		}
	}
}
