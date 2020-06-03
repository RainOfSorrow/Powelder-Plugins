using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace PowelderAPI
{
    public static class PowelderCommands
    {
	    public static void BroadcastXedlefix(CommandArgs args)
		{
			string str = string.Join(" ", args.Parameters);
			TSPlayer.All.SendMessage("[i:3570] [c/595959:⮘] [c/18B690:Serwer] [c/595959:⮚] [c/18B690:Xedlefix][c/595959::] " + str, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}

		public static void BroadcastIwobos(CommandArgs args)
		{
			string str = string.Join(" ", args.Parameters);
			TSPlayer.All.SendMessage("[i:3570] [c/595959:⮘] [c/27AD34:Serwer] [c/595959:⮚] [c/27AD34:Iwobos][c/595959::] " + str, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}


		public static void Broadcast(CommandArgs args)
		{
			string str = string.Join(" ", args.Parameters);
			Console.WriteLine("OGLOSZENIE " + args.Player.Name + ": " + str);
			
			foreach (TSPlayer player in TShock.Players)
			{
				if (player.Active && player != null && (player.Account != null && player.ConnectionAlive) && player.RealPlayer)
				{
					player.SendData((PacketTypes) 119, "Ogloszenie!", (int) new Color(255, 68, 68).PackedValue, player.TPlayer.Center.X, player.TPlayer.Center.Y, 0.0f, 0);
					player.SendMessage("[i:1526] [c/595959:⮘] [c/ff2222:Ogloszenie] [c/595959:⮚] " + str, new Color((int) byte.MaxValue, 68, 68));
				}
			}
		}
    }
}