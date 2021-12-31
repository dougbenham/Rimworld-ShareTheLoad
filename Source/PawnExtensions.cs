using Verse;

namespace Share_The_Load
{
	public static class PawnExtensions
	{
		public static bool IsUs(this Pawn pawn)
		{
			return pawn.Faction != null && pawn.Faction.IsPlayer && pawn.HostFaction == null;
		}
	}
}