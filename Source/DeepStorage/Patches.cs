using System.Reflection;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace Share_The_Load.DeepStorage
{
	[StaticConstructorOnStartup]
	public static class Patches
	{
		private static readonly MethodInfo _tryGetDeepStorageCompMethod;
		private static readonly MethodInfo _capacityToStoreThingAtMethod;

		static Patches()
		{
			Verse.Log.Message("Share The Load: Checking for DeepStorage!");
			var compDeepStorage = AccessTools.TypeByName("LWM.DeepStorage.CompDeepStorage");
			if (compDeepStorage == null)
				return;

			var tryGetCompMethod = AccessTools.Method(AccessTools.TypeByName("Verse.ThingCompUtility"), "TryGetComp");
			if (tryGetCompMethod == null)
			{
				Verse.Log.Error("ShareTheLoad: Couldn't find Verse.ThingCompUtility.TryGetComp(..)");
				return;
			}

			_tryGetDeepStorageCompMethod = tryGetCompMethod.MakeGenericMethod(compDeepStorage);

			_capacityToStoreThingAtMethod = AccessTools.Method(compDeepStorage, "CapacityToStoreThingAt");
			if (_capacityToStoreThingAtMethod == null)
			{
				Verse.Log.Error("ShareTheLoad: Couldn't find LWM.DeepStorage.CompDeepStorage.CapacityToStoreThingAt(..)");
				return;
			}

			Verse.Log.Message("Share The Load: Patching for DeepStorage!");

			Harmony harmony = new Harmony("Uuugggg.rimworld.Share_The_Load-DS.main");
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), nameof(ReservationManager.CanReserve)), new HarmonyMethod(typeof(CanReserve_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), nameof(ReservationManager.Reserve)), new HarmonyMethod(typeof(Reserve_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), nameof(ReservationManager.Release)), new HarmonyMethod(typeof(Release_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), nameof(ReservationManager.ReleaseClaimedBy)), new HarmonyMethod(typeof(ReleaseClaimedBy_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), nameof(ReservationManager.ReleaseAllClaimedBy)), new HarmonyMethod(typeof(ReleaseAllClaimedBy_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), nameof(ReservationManager.ReleaseAllForTarget)), new HarmonyMethod(typeof(ReleaseAllForTarget_Patch), "Prefix"));
			harmony.Patch(AccessTools.Method(typeof(ReservationManager), nameof(ReservationManager.CanReserveStack)), new HarmonyMethod(typeof(CanReserveStack_Patch), "Prefix"));
		}

		public static object GetDeepStorageComp(this Thing building)
		{
			return _tryGetDeepStorageCompMethod.Invoke(null, new object[] {building});
		}

		public static int CapacityToStoreThingAt(this object deepStorageComp, Thing thing, Map map, IntVec3 cell)
		{
			return (int) _capacityToStoreThingAtMethod.Invoke(deepStorageComp, new object[] {thing, map, cell});
		}
	}
}