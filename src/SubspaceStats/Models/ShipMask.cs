namespace SubspaceStats.Models
{
	[Flags]
	public enum ShipMask
	{
		None = 0,
		Warbird = 1,
		Javelin = 2,
		Spider = 4,
		Leviathan = 8,
		Terrier = 16,
		Weasel = 32,
		Lancaster = 64,
		Shark = 128,
		All = 255,
	}
}
