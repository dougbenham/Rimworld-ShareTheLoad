﻿using UnityEngine;
using Verse;

namespace Share_The_Load
{
	class Settings : ModSettings
	{
		public bool deliverAsMuchAsYouCan = false;
		public bool makeWayJobs = false;

		public static Settings Get()
		{
			return LoadedModManager.GetMod<Share_The_Load.Mod>().GetSettings<Settings>();
		}

		public void DoWindowContents(Rect wrect)
		{
			var options = new Listing_Standard();
			options.Begin(wrect);
			
			options.CheckboxLabeled("TD.SettingDeliverAny".Translate(), ref deliverAsMuchAsYouCan);
			options.CheckboxLabeled("TD.SettingBlockingJobs".Translate(), ref makeWayJobs, "TD.SettingBlockingJobsDesc".Translate());
			options.Gap();

			options.End();
		}
		
		public override void ExposeData()
		{
			Scribe_Values.Look(ref deliverAsMuchAsYouCan, "deliverAsMuchAsYouCan", false);
			Scribe_Values.Look(ref makeWayJobs, "makeWayJobs", true);
		}
	}
}