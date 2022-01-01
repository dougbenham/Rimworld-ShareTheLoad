using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Share_The_Load.ExtendedStorage
{
	[StaticConstructorOnStartup]
	public static class Patches
	{
		public static Type typeBuilding_ExtendedStorage;

		static Patches()
		{
			Verse.Log.Message("Share The Load: Checking for Extended Storage!");
			typeBuilding_ExtendedStorage = AccessTools.TypeByName("ExtendedStorage.Building_ExtendedStorage");
			if (typeBuilding_ExtendedStorage == null)
				return;

			ApparentMaxStorageInfo = AccessTools.Property(typeBuilding_ExtendedStorage, "ApparentMaxStorage").GetGetMethod();
			StoredThingTotalInfo = AccessTools.Property(typeBuilding_ExtendedStorage, "StoredThingTotal").GetGetMethod();

			if (ApparentMaxStorageInfo == null || StoredThingTotalInfo == null)
			{
				Verse.Log.Warning("ShareTheLoad couldn't work with ExtendedStorage, whooops!");
				return;
			}

			Verse.Log.Message("Share The Load: Patching for Extended Storage!");

			Harmony harmony = new Harmony("Uuugggg.rimworld.Share_The_Load-ES.main");
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), "CanReserve"), new HarmonyMethod(typeof(CanReserve_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), "Reserve"), new HarmonyMethod(typeof(Reserve_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), "Release"), new HarmonyMethod(typeof(Release_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), "ReleaseClaimedBy"), new HarmonyMethod(typeof(ReleaseClaimedBy_Patch), "Prefix"));
		}

		public static MethodInfo ApparentMaxStorageInfo;
		public static int ApparentMaxStorage(this Thing building) //Building_ExtendedStorage
		{
			return (int) ApparentMaxStorageInfo.Invoke(building, null);
		}

		public static MethodInfo StoredThingTotalInfo;
		public static int StoredThingTotal(this Thing building) //Building_ExtendedStorage
		{
			return (int) StoredThingTotalInfo.Invoke(building, null);
		}
	}

	//Redundant with normal Constructible
	//[HarmonyPatch(typeof(ReservationManager), "ReleaseAllClaimedBy")]
	//static class ReleaseAllClaimedBy_Patch
	//{
	//	//public void ReleaseAllClaimedBy(Pawn claimant)
	//	public static void Prefix(Pawn claimant)
	//	{
	//		ExpectingComp.Remove(q => q.claimant == claimant);
	//	}
	//}
}