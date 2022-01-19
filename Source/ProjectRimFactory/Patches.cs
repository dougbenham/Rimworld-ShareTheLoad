using System;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Share_The_Load.ProjectRimFactory
{
	[StaticConstructorOnStartup]
	public static class Patches
	{
		private static readonly Type _massStorageUnitBuildingType = AccessTools.TypeByName("ProjectRimFactory.Storage.Building_MassStorageUnit");
		private static readonly Type _storageUnitIoBaseType = AccessTools.TypeByName("ProjectRimFactory.Storage.Building_StorageUnitIOBase");

		static Patches()
		{
			Verse.Log.Message("[Share The Load] Checking for ProjectRimFactory!");
			if (_massStorageUnitBuildingType == null || _storageUnitIoBaseType == null)
			{
				return;
			}

			Verse.Log.Message("[Share The Load] Patching for ProjectRimFactory!");

			Harmony harmony = new Harmony("Uuugggg.rimworld.Share_The_Load-RF.main");
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), "CanReserve"), new HarmonyMethod(typeof(CanReserve_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), "Reserve"), new HarmonyMethod(typeof(Reserve_Patch), "Prefix"));
			//harmony.Patch(AccessTools.Method(typeof(ReservationManager), "Release"), new HarmonyMethod(typeof(Release_Patch), "Prefix"));
			//harmony.Patch(AccessTools.Method(typeof(ReservationManager), "ReleaseClaimedBy"), new HarmonyMethod(typeof(ReleaseClaimedBy_Patch), "Prefix"));
		}

		public static bool IsPrfStorage(this Thing building)
		{
			return _massStorageUnitBuildingType.IsInstanceOfType(building) ||
			       _storageUnitIoBaseType.IsInstanceOfType(building);
		}
	}
}