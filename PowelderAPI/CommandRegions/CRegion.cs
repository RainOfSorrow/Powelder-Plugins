using Microsoft.Xna.Framework;
using System;
using TShockAPI;
using TShockAPI.DB;

namespace PowelderAPI.CommandRegions
{
	public class CRegion
	{
		public string name;

		public int PosX;

		public int PosY;

		public int Height;

		public int Width;

		public string action;

		public CRegion(string n, int x, int y, int h, int w, string action)
		{
			name = n;
			PosX = x;
			PosY = y;
			Height = h;
			Width = w;
			this.action = action;
		}

		public CRegion(CRPlayer cRPlayer, string n, string action)
		{
			name = n;
			this.action = action;
			PosX = Math.Min(cRPlayer.pos1.X, cRPlayer.pos2.X);
			PosY = Math.Min(cRPlayer.pos1.Y, cRPlayer.pos2.Y);
			Width = Math.Abs(cRPlayer.pos1.X - cRPlayer.pos2.X);
			Height = Math.Abs(cRPlayer.pos1.Y - cRPlayer.pos2.Y);
			CRDatabase.addRegion(PosX, PosY, Height, Width, n, action);
		}

		public static string getCurrentRegion(TSPlayer plr)
		{
			foreach (CRegion cRegion in CommandRegions.CRegions)
			{
				if (plr.TileX >= cRegion.PosX && plr.TileX <= cRegion.PosX + cRegion.Width && plr.TileY >= cRegion.PosY && plr.TileY <= cRegion.PosY + cRegion.Height)
				{
					return cRegion.name;
				}
			}
			return null;
		}

		public void Execute(TSPlayer plr)
		{
			if (action.StartsWith(".warp"))
			{
				if (action.Length <= 6)
				{
					plr.SendErrorMessage("[c/595959:»]  Wystapil problem z portalem, napisz do kogos z kadry. Kod 0");
					return;
				}
				string text = action.Remove(0, 6);
				Warp warp = TShock.Warps.Find(text);
				if (warp != null)
				{
					if (plr.Teleport(warp.Position.X * 16, warp.Position.Y * 16, 1))
					{
						plr.SendMessage("[c/595959:»]  Przeteleportowano do [c/66FF66:" + text + "].", new Color(128, 128, 128));
					}
				}
				else
				{
					plr.SendErrorMessage("[c/595959:»]  Nie znaleziono takiego warpa.");
				}
			}
			else if (action.StartsWith("/"))
			{
				Commands.HandleCommand(plr, action);
			}
			else
			{
				plr.SendErrorMessage("[c/595959:»]  Wystapil problem z portalem, napisz do kogos z kadry. Kod 1");
			}
		}

		public static CRegion getRegion(int x, int y)
		{
			foreach (CRegion cRegion in CommandRegions.CRegions)
			{
				if (x >= cRegion.PosX && x <= cRegion.PosX + cRegion.Width && y >= cRegion.PosY && y <= cRegion.PosY + cRegion.Height)
				{
					return cRegion;
				}
			}
			return null;
		}
	}
}
