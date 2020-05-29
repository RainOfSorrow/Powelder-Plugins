using Microsoft.Xna.Framework;

namespace PowelderAPI.CommandRegions
{
	public class CrPlayer
	{
		public bool[] Set;

		public bool Destroy;

		public bool Modify;

		public string Cmd;

		public string CurrentRegion;

		public string OldRegion;

		public short Seconds;

		public Point Pos1;

		public Point Pos2;

		public CrPlayer(int index)
		{
			Pos1 = Point.Zero;
			Pos2 = Point.Zero;
			Set = new bool[2];
			CurrentRegion = null;
			OldRegion = null;
			Seconds = -1;
		}
	}
}
