using Microsoft.Xna.Framework;
using SurvivalCore.Economy.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using TShockAPI;

namespace SurvivalCore.Economy
{
	public class ECommands
	{
		public static void admin(CommandArgs args)
		{
			TSPlayer player = args.Player;
			string text = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			switch (text)
			{
			default:
				player.SendMessage("[c/66ff66:Ekonomia][c/595959::]", Color.Gray);
				player.SendMessage("[c/595959:»]  set <nick> <ilość> - Ustawianie stanu konta", Color.Gray);
				player.SendMessage("[c/595959:»]  add <nick> <ilość> - Dodawanie do stanu konta", Color.Gray);
				player.SendMessage("[c/595959:»]  take <nick> <ilość> - Zabieranie ze stanu konta", Color.Gray);
				break;
			case "set":
			{
				if (args.Parameters.Count < 2)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie podano gracza.");
					break;
				}
				List<TSPlayer> list2 = TSPlayer.FindByNameOrID(args.Parameters[1]);
				int result2;
				if (list2.Count < 1)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie mozna bylo znalezc gracza {0}.", args.Parameters[1]);
				}
				else if (list2.Count > 1)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Znaleziono wiecej niz jednego gracza.", args.Parameters[1]);
				}
				else if (args.Parameters.Count < 3)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie podano ilosci pieniedzy.");
				}
				else if (int.TryParse(args.Parameters[2], out result2))
				{
					if (result2 <= 0)
					{
						args.Player.SendErrorMessage("[c/595959:»]  Podana wartosc nie moze byc rowna 0 lub mniejsza.");
					}
					else
					{
						Operations.setMoney(args.Player, list2[0], result2);
					}
				}
				else
				{
					args.Player.SendErrorMessage("[c/595959:»]  Podano zla wartosc.");
				}
				break;
			}
			case "add":
			{
				if (args.Parameters.Count < 2)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie podano gracza.");
					break;
				}
				List<TSPlayer> list3 = TSPlayer.FindByNameOrID(args.Parameters[1]);
				int result3;
				if (list3.Count < 1)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie mozna bylo znalezc gracza {0}.", args.Parameters[1]);
				}
				else if (list3.Count > 1)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Znaleziono wiecej niz jednego gracza.", args.Parameters[1]);
				}
				else if (args.Parameters.Count < 3)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie podano ilosci pieniedzy.");
				}
				else if (int.TryParse(args.Parameters[2], out result3))
				{
					if (result3 <= 0)
					{
						args.Player.SendErrorMessage("[c/595959:»]  Podana wartosc nie moze byc rowna 0 lub mniejsza.");
					}
					else
					{
						Operations.addMoney(args.Player, list3[0], result3);
					}
				}
				else
				{
					args.Player.SendErrorMessage("[c/595959:»]  Podano zla wartosc.");
				}
				break;
			}
			case "take":
			{
				if (args.Parameters.Count < 2)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie podano gracza.");
					break;
				}
				List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[1]);
				int result;
				if (list.Count < 1)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie mozna bylo znalezc gracza {0}.", args.Parameters[1]);
				}
				else if (list.Count > 1)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Znaleziono wiecej niz jednego gracza.", args.Parameters[1]);
				}
				else if (args.Parameters.Count < 3)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie podano ilosci pieniedzy.");
				}
				else if (int.TryParse(args.Parameters[2], out result))
				{
					if (result <= 0)
					{
						args.Player.SendErrorMessage("[c/595959:»]  Podana wartosc nie moze byc rowna 0 lub mniejsza.");
					}
					else
					{
						Operations.takeMoney(args.Player, list[0], result);
					}
				}
				else
				{
					args.Player.SendErrorMessage("[c/595959:»]  Podano zla wartosc.");
				}
				break;
			}
			}
		}

		public static void currency(CommandArgs args)
		{
			if (args.Parameters.Count > 0)
			{
				List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
				if (list.Count < 1)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie mozna bylo znalezc gracza {0}.", args.Parameters[0]);
				}
				else if (list.Count > 1)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Znaleziono wiecej niz jednego gracza.", args.Parameters[0]);
				}
				else
				{
					args.Player.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Stan konta {0}][c/595959::] {1} {2}", list[0].Name, SurvivalCore.srvPlayers[list[0].Index].Money.ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
				}
			}
			else
			{
				args.Player.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Stan twojego konta][c/595959::] {0} {1}", SurvivalCore.srvPlayers[args.Player.Index].Money.ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
			}
		}

		public static void transfer(CommandArgs args)
		{
			if (args.Parameters.Count <= 0)
			{
				args.Player.SendMessage("[c/595959:»]  [c/66ff66:Uzycie][c/595959::] /przelej <nick> <ilosc>", Color.Gray);
				return;
			}
			List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
			int result;
			if (list.Count < 1)
			{
				args.Player.SendErrorMessage("[c/595959:»]  Nie mozna bylo znalezc gracza {0}.", args.Parameters[0]);
			}
			else if (list.Count > 1)
			{
				args.Player.SendErrorMessage("[c/595959:»]  Znaleziono wiecej niz jednego gracza.", args.Parameters[0]);
			}
			else if (list[0] == args.Player)
			{
				args.Player.SendErrorMessage("[c/595959:»]  Nie mozna przelac pieniedzy samemu sobie.");
			}
			else if (int.TryParse(args.Parameters[1], out result))
			{
				if (result <= 0)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Podana wartosc nie moze byc rowna 0 lub mniejsza.");
				}
				else
				{
					Operations.sendMoney(args.Player, list[0], result);
				}
			}
			else
			{
				args.Player.SendErrorMessage("[c/595959:»]  Podano zla wartosc.");
			}
		}

		public static void collectMoney(CommandArgs args)
		{
			TSPlayer player = args.Player;

			if (QueryDly.loadNext(player) == null)
			{
				QueryDly.createRecord(player, DateTime.Now.AddDays(1.0));
				SurvivalCore.srvPlayers[args.Player.Index].Money += Economy.config.DailyAmount;
				player.SendMessage($"[c/595959:»]  Pomyslne odebrano swoje pierwsze kieszonkowe wynoszace [c/66ff66:{Economy.config.DailyAmount}] {Economy.config.ValueName}.", Color.Gray);
				player.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {0} {1}", SurvivalCore.srvPlayers[args.Player.Index].Money.ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
				return;
			}
			else if ((DateTime.Now - DateTime.ParseExact(QueryDly.loadNext(player), Economy.DFormat, null)).TotalMilliseconds > 0)
			{
				SurvivalCore.srvPlayers[args.Player.Index].Money += Economy.config.DailyAmount;
				player.SendMessage($"[c/595959:»]  Pomyslne odebrano swoje kieszonkowe wynoszace [c/66ff66:{Economy.config.DailyAmount}] {Economy.config.ValueName}.", Color.Gray);
				player.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {0} {1}", SurvivalCore.srvPlayers[args.Player.Index].Money.ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
				QueryDly.updateNext(player, DateTime.Now.AddDays(1));
			}
			else
			{
				player.SendErrorMessage("[c/595959:»]  Kieszonkowe mozna odbierac co 24h.");
				player.SendMessage($"[c/595959:»]  Nastepny odbior bedzie mozliwy za [c/66ff66:{ExpireCountDown(QueryDly.loadNext(player))}].", Color.Gray);
			}
		}

		private static string ExpireCountDown(string time)
		{
			DateTime result = DateTime.Now;
			DateTime.TryParseExact(time, Economy.DFormat, null, DateTimeStyles.None, out result);
			TimeSpan timeSpan = result - DateTime.Now;
			return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h {timeSpan.Minutes}min {timeSpan.Seconds}sec";
		}
	}
}
