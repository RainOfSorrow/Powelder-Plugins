using Microsoft.Xna.Framework;
using SurvivalCore.Economy.Database;
using System;
using System.Collections.Generic;
using System.Threading;
using TShockAPI;
using TShockAPI.DB;

namespace SurvivalCore.Economy.Gambling
{
	public class Casino
	{
		public static void CasinoCommand(CommandArgs args)
		{
			string text = null;
			string arg1 = null;
			string arg2 = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			if (args.Parameters.Count > 1)
			{
				arg1 = args.Parameters[1].ToLower();
			}
			if (args.Parameters.Count > 2)
			{
				arg2 = args.Parameters[2].ToLower();
			}
			
			
			string text3 = text;
			string text4 = text3;
			if (text4 == null || !(text4 == "slots"))
			{
				args.Player.SendMessage("Kasyno:", Color.Green);
				args.Player.SendMessage("/kasyno slots] [c/595959:-] Gra w sloty.", Color.Gray);
			}
			else if (arg1 == "start")
			{
				if (!SurvivalCore.IsStatusBusy[args.Player.Index])
				{
					if (SurvivalCore.SrvPlayers[args.Player.Index].Money < 85)
					{
						args.Player.SendErrorMessage("Nie stac cie na los.");
						return;
					}
					SurvivalCore.SrvPlayers[args.Player.Index].Money -= 85;
					object parameter = new object[2]
					{
						args.Player,
						args.Player.Account
					};
					Thread thread = new Thread(SlotsThread)
					{
						IsBackground = true
					};
					thread.Start(parameter);
					args.Player.SendMessage("[i:3312] [c/595959:;] [c/9f339f:Kasyno] [c/595959:;] Zaczynam losowanie. Mozesz obserwowac proces losowania w miejscu statusu.", new Color(205, 101, 205));
				}
				else
				{
					args.Player.SendErrorMessage("Status jest zajety.");
				}
			}
			else if (arg1 == "nagrody")
			{
				args.Player.SendMessage("Slots - Nagrody:", Color.Green);
				args.Player.SendMessage(" [i:20]  - 100 €  [i:19] - 150 €\n" +
				                             " [i:57] - 250 €  [i:175] - 400 €", Color.Yellow);
				args.Player.SendMessage(" [i:382]  - 600 €  [i:1225] - 850 €\n" +
				                             " [i:1006] - 1150 €  [i:3467] - 1400 €", Color.Yellow);
			}
			else
			{
				args.Player.SendMessage("Slots - Zasady:", Color.Green);
				args.Player.SendMessage("Aby zaczac losowanie wpisz /kasyno slots start. Koszt za kazdorazowy los 85 €. Wylosowane zostana 3 pola", Color.Yellow);
				args.Player.SendMessage("ze sztabkami. Jezeli wszystkie pola beda takie same, otrzymasz nagrode - /kasyno slots nagrody. Jezeli", Color.Yellow);
				args.Player.SendMessage("tylko dwa pola beda takie same, to otrzymasz polowe nagrody.", Color.Yellow);
			}
		}

		private static void SlotsThread(object packet)
		{
			try
			{
				TSPlayer tSPlayer = (TSPlayer) ((Array) packet).GetValue(0);
				UserAccount plr = (UserAccount) ((Array) packet).GetValue(1);
				byte b = (byte) tSPlayer.Index;
				SurvivalCore.IsStatusBusy[b] = true;
				Slots slots = new Slots();
				for (int i = 1; i != 62; i++)
				{
					if (TShock.Players[b] != tSPlayer || TShock.ShuttingDown)
					{
						QueryPlr.SetMoney(plr, DataBase.GetSrvPlayer(plr).Money + 85);
						SurvivalCore.IsStatusBusy[b] = false;
						return;
					}

					slots.MoveSlots();
					tSPlayer.SendData(PacketTypes.Status, slots.GetStatus(), 0 , 3);
					Thread.Sleep(22 * (int) (i * 0.3));
				}

				tSPlayer.SendMessage(
					$"[i:3312] [c/595959:;] [c/9f339f:Kasyno] [c/595959:;] Wygrales {slots.GetResult()} {Economy.Config.ValueName}",
					new Color(205, 101, 205));
				SurvivalCore.SrvPlayers[b].Money += slots.GetResult();
				tSPlayer.SendData(PacketTypes.Status, slots.GetStatus(true), 0, 3);
				if (slots.Special() != null)
				{
					TSPlayer.All.SendMessage(string.Format(slots.Special(), tSPlayer.Name), new Color(205, 101, 205));
				}

				Thread.Sleep(4000);
				SurvivalCore.IsStatusBusy[b] = false;

			}
			catch (Exception)
			{
				//Ignore
			}
		}
	}
}
