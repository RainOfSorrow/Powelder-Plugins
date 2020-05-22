using Microsoft.Xna.Framework;
using SurvivalCore.Economy.Database;
using System;
using System.Threading;
using TShockAPI;
using TShockAPI.DB;

namespace SurvivalCore.Economy.Gambling
{
	public class Casino
	{
		public static void casino(CommandArgs args)
		{
			string text = null;
			string a = null;
			string text2 = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			if (args.Parameters.Count > 1)
			{
				a = args.Parameters[1].ToLower();
			}
			if (args.Parameters.Count > 2)
			{
				text2 = args.Parameters[2].ToLower();
			}
			string text3 = text;
			string text4 = text3;
			if (text4 == null || !(text4 == "slots"))
			{
				args.Player.SendMessage("            [c/595959:«]Kasyno[c/595959:»]", new Color(192, 192, 192));
				args.Player.SendMessage("[c/595959:»]  [c/66ff66:/kasyno slots] [c/595959:-] Gra w sloty.", Color.Gray);
			}
			else if (a == "start")
			{
				if (!SurvivalCore.isStatusBusy[args.Player.Index])
				{
					if (SurvivalCore.srvPlayers[args.Player.Index].Money < 75)
					{
						args.Player.SendErrorMessage("[c/595959:»]  Nie stac cie na los.");
						return;
					}
					SurvivalCore.srvPlayers[args.Player.Index].Money -= 75;
					object parameter = new object[2]
					{
						args.Player,
						args.Player.Account
					};
					Thread thread = new Thread(slotsThread)
					{
						IsBackground = true
					};
					thread.Start(parameter);
					args.Player.SendMessage("[c/595959:»]  Zaczynam losowanie. Mozesz obserwowac proces losowania w miejscu statusu.", Color.Gray);
				}
				else
				{
					args.Player.SendErrorMessage("[c/595959:»]  Status jest zajety.");
				}
			}
			else if (a == "nagrody")
			{
				args.Player.SendMessage("                    [c/595959:«]Slots - Nagrody[c/595959:»]", new Color(192, 192, 192));
				args.Player.SendMessage(" [c/66ff66:☠]  [c/595959:-] 100 € [c/595959:|] [c/66ff66:⛏] [c/595959:-] 150 € [c/595959:|] [c/66ff66:♣] [c/595959:-] 200 € [c/595959:|] [c/66ff66:♫] [c/595959:-] 250 €", Color.Gray);
				args.Player.SendMessage(" [c/66ff66:⚡]  [c/595959:-] 350 € [c/595959:|] [c/66ff66:♖] [c/595959:-] 400 € [c/595959:|] [c/66ff66:☯] [c/595959:-] 450 € [c/595959:|] [c/66ff66:♕] [c/595959:-] 500 €", Color.Gray);
			}
			else
			{
				args.Player.SendMessage("                                                [c/595959:«]Slots - Zasady[c/595959:»]", new Color(192, 192, 192));
				args.Player.SendMessage("Aby zaczac losowanie wpisz [c/66ff66:/kasyno slots start]. Koszt za kazdorazowy los [c/66ff66:75 €]. Wylosowane zostana 3 pola", Color.Gray);
				args.Player.SendMessage("z symbolami. Jezeli wszystkie pola beda takie same, otrzymasz nagrode - [c/66ff66:/kasyno slots nagrody]. Jezeli", Color.Gray);
				args.Player.SendMessage("tylko dwa pola beda takie same, to otrzymasz polowe nagrody.", Color.Gray);
			}
		}

		private static void slotsThread(object packet)
		{
			TSPlayer tSPlayer = (TSPlayer)((Array)packet).GetValue(0);
			UserAccount plr = (UserAccount)((Array)packet).GetValue(1);
			byte b = (byte)tSPlayer.Index;
			SurvivalCore.isStatusBusy[b] = true;
			Slots slots = new Slots();
			for (int i = 1; i != 62; i++)
			{
				if (TShock.Players[b] != tSPlayer || TShock.ShuttingDown)
				{
					QueryPlr.setMoney(plr, DataBase.getSrvPlayer(plr).Money + 75);
					SurvivalCore.isStatusBusy[b] = false;
					return;
				}
				slots.moveSlots();
				tSPlayer.SendData(PacketTypes.Status, slots.getStatus());
				tSPlayer.SendData(PacketTypes.PlayHarp, null, tSPlayer.Index, 1f);
				Thread.Sleep(22 * (int)((double)(float)i * 0.3));
			}
			tSPlayer.SendMessage($"[c/595959:»]  [c/66ff66:Slots - Wygrales][c/595959::] {slots.getResult()} {Economy.config.ValueName}", Color.Gray);
			SurvivalCore.srvPlayers[b].Money += slots.getResult();
			tSPlayer.SendData(PacketTypes.Status, slots.getStatus(true));
			if (slots.special() != null)
			{
				TSPlayer.All.SendMessage(string.Format(slots.special(), tSPlayer.Name), new Color(205, 101, 205));
			}
			Thread.Sleep(4000);
			SurvivalCore.isStatusBusy[b] = false;
		}
	}
}
