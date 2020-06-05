using Microsoft.Xna.Framework;
using System;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace PowelderAPI.CommandRegions
{
	public class CRegion
	{
		public string Name;

		public int PosX;

		public int PosY;

		public int Height;

		public int Width;

		public string Action;

		public CRegion(string n, int x, int y, int h, int w, string action)
		{
			Name = n;
			PosX = x;
			PosY = y;
			Height = h;
			Width = w;
			this.Action = action;
		}

		public CRegion(CrPlayer cRPlayer, string n, string action)
		{
			Name = n;
			this.Action = action;
			PosX = Math.Min(cRPlayer.Pos1.X, cRPlayer.Pos2.X);
			PosY = Math.Min(cRPlayer.Pos1.Y, cRPlayer.Pos2.Y);
			Width = Math.Abs(cRPlayer.Pos1.X - cRPlayer.Pos2.X);
			Height = Math.Abs(cRPlayer.Pos1.Y - cRPlayer.Pos2.Y);
			CrDatabase.AddRegion(PosX, PosY, Height, Width, n, action);
		}

		public static string GetCurrentRegion(TSPlayer plr)
		{
			foreach (CRegion cRegion in CommandRegions.CRegions)
			{
				if (plr.TileX >= cRegion.PosX && plr.TileX <= cRegion.PosX + cRegion.Width && plr.TileY >= cRegion.PosY && plr.TileY <= cRegion.PosY + cRegion.Height)
				{
					return cRegion.Name;
				}
			}
			return null;
		}

		public void Execute(TSPlayer plr)
		{
			if (Action.StartsWith(".warp"))
			{
				if (Action.Length <= 6)
				{
					plr.SendErrorMessage("[c/595959:»]  Wystapil problem z portalem, napisz do kogos z kadry. Kod 0");
					return;
				}
				string text = Action.Remove(0, 6);
				Warp warp = TShock.Warps.Find(text);
				if (warp != null)
				{
					if (plr.Teleport(warp.Position.X * 16, warp.Position.Y * 16, 1))
					{
						plr.SendMessage("Przeteleportowano do [c/66FF66:" + text + "].", new Color(128, 128, 128));
					}
				}
				else
				{
					plr.SendErrorMessage("Nie znaleziono takiego warpa.");
				}
			}
			else if (Action.StartsWith(".server"))
			{
				if (Action.Length <= 8)
				{
					plr.SendErrorMessage("Wystapil problem z portalem, napisz do kogos z kadry. Kod 0");
					return;
				}
				string text = Action.Remove(0, 8);
				
				
				
			}
			else if (Action.StartsWith("/"))
			{
				Commands.HandleCommand(plr, Action);
			}
			else
			{
				plr.SendErrorMessage("Wystapil problem z portalem, napisz do kogos z kadry. Kod 1");
			}
		}

		public static CRegion GetRegion(int x, int y)
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
