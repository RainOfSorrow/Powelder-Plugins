using Microsoft.Xna.Framework;

namespace PowelderAPI.CommandRegions
{
	public class CRPlayer
	{
		public bool[] set;

		public bool destroy;

		public bool modify;

		public string cmd;

		public string currentRegion;

		public string oldRegion;

		public short seconds;

		public Point pos1;

		public Point pos2;

		public CRPlayer(int Index)
		{
			pos1 = Point.Zero;
			pos2 = Point.Zero;
			set = new bool[2];
			currentRegion = null;
			oldRegion = null;
			seconds = -1;
		}
	}
}
