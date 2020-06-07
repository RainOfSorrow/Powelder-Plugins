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
		public static void Admin(CommandArgs args)
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
				player.SendMessage("Ekonomia:", Color.Green);
				player.SendInfoMessage("/eco set <nick> <ilosc> - Ustawianie stanu konta");
				player.SendInfoMessage("/eco add <nick> <ilosc> - Dodawanie do stanu konta");
				player.SendInfoMessage("/eco take <nick> <ilosc> - Zabieranie ze stanu konta");
				break;
			case "set":
			{
				if (args.Parameters.Count < 2)
				{
					args.Player.SendErrorMessage("Nie podano gracza.");
					break;
				}
				List<TSPlayer> list2 = TSPlayer.FindByNameOrID(args.Parameters[1]);
				int result2;
				if (list2.Count < 1)
				{
					args.Player.SendErrorMessage("Nie mozna bylo znalezc gracza {0}.", args.Parameters[1]);
				}
				else if (list2.Count > 1)
				{
					args.Player.SendErrorMessage("Znaleziono wiecej niz jednego gracza.", args.Parameters[1]);
				}
				else if (args.Parameters.Count < 3)
				{
					args.Player.SendErrorMessage("Nie podano ilosci pieniedzy.");
				}
				else if (int.TryParse(args.Parameters[2], out result2))
				{
					if (result2 <= 0)
					{
						args.Player.SendErrorMessage("Podana wartosc nie moze byc rowna 0 lub mniejsza.");
					}
					else
					{
						Operations.SetMoney(args.Player, list2[0], result2);
					}
				}
				else
				{
					args.Player.SendErrorMessage("Podano zla wartosc.");
				}
				break;
			}
			case "add":
			{
				if (args.Parameters.Count < 2)
				{
					args.Player.SendErrorMessage("Nie podano gracza.");
					break;
				}
				List<TSPlayer> list3 = TSPlayer.FindByNameOrID(args.Parameters[1]);
				int result3;
				if (list3.Count < 1)
				{
					args.Player.SendErrorMessage("Nie mozna bylo znalezc gracza {0}.", args.Parameters[1]);
				}
				else if (list3.Count > 1)
				{
					args.Player.SendErrorMessage("Znaleziono wiecej niz jednego gracza.", args.Parameters[1]);
				}
				else if (args.Parameters.Count < 3)
				{
					args.Player.SendErrorMessage("Nie podano ilosci pieniedzy.");
				}
				else if (int.TryParse(args.Parameters[2], out result3))
				{
					if (result3 <= 0)
					{
						args.Player.SendErrorMessage("Podana wartosc nie moze byc rowna 0 lub mniejsza.");
					}
					else
					{
						Operations.AddMoney(args.Player, list3[0], result3);
					}
				}
				else
				{
					args.Player.SendErrorMessage("Podano zla wartosc.");
				}
				break;
			}
			case "take":
			{
				if (args.Parameters.Count < 2)
				{
					args.Player.SendErrorMessage("Nie podano gracza.");
					break;
				}
				List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[1]);
				int result;
				if (list.Count < 1)
				{
					args.Player.SendErrorMessage("Nie mozna bylo znalezc gracza {0}.", args.Parameters[1]);
				}
				else if (list.Count > 1)
				{
					args.Player.SendErrorMessage("Znaleziono wiecej niz jednego gracza.", args.Parameters[1]);
				}
				else if (args.Parameters.Count < 3)
				{
					args.Player.SendErrorMessage("Nie podano ilosci pieniedzy.");
				}
				else if (int.TryParse(args.Parameters[2], out result))
				{
					if (result <= 0)
					{
						args.Player.SendErrorMessage("Podana wartosc nie moze byc rowna 0 lub mniejsza.");
					}
					else
					{
						Operations.TakeMoney(args.Player, list[0], result);
					}
				}
				else
				{
					args.Player.SendErrorMessage("Podano zla wartosc.");
				}
				break;
			}
			}
		}

		public static void Currency(CommandArgs args)
		{
			if (args.Parameters.Count > 0)
			{
				List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
				if (list.Count < 1)
				{
					args.Player.SendErrorMessage("Nie mozna bylo znalezc gracza {0}.", args.Parameters[0]);
				}
				else if (list.Count > 1)
				{
					args.Player.SendErrorMessage("Znaleziono wiecej niz jednego gracza.", args.Parameters[0]);
				}
				else
				{
					args.Player.SendInfoMessage(string.Format("Stan konta {0}: {1} {2}", list[0].Name, SurvivalCore.SrvPlayers[list[0].Index].Money.ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
				}
			}
			else
			{
				args.Player.SendInfoMessage(string.Format("Stan twojego konta: {0} {1}", SurvivalCore.SrvPlayers[args.Player.Index].Money.ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
			}
		}

		public static void Transfer(CommandArgs args)
		{
			if (args.Parameters.Count <= 0)
			{
				args.Player.SendErrorMessage("Uzycie: /przelej <nick> <ilosc>");
				return;
			}
			List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[0]);
			int result;
			if (list.Count < 1)
			{
				args.Player.SendErrorMessage("Nie mozna bylo znalezc gracza {0}.", args.Parameters[0]);
			}
			else if (list.Count > 1)
			{
				args.Player.SendErrorMessage("Znaleziono wiecej niz jednego gracza.", args.Parameters[0]);
			}
			else if (list[0] == args.Player)
			{
				args.Player.SendErrorMessage("Nie mozna przelac pieniedzy samemu sobie.");
			}
			else if (int.TryParse(args.Parameters[1], out result))
			{
				if (result <= 0)
				{
					args.Player.SendErrorMessage("Podana wartosc nie moze byc rowna 0 lub mniejsza.");
				}
				else
				{
					Operations.SendMoney(args.Player, list[0], result);
				}
			}
			else
			{
				args.Player.SendErrorMessage("Podano zla wartosc.");
			}
		}

		public static void CollectMoney(CommandArgs args)
		{
			TSPlayer player = args.Player;

			if (QueryDly.LoadNext(player) == null)
			{
				QueryDly.CreateRecord(player, DateTime.Now.AddDays(1.0));
				SurvivalCore.SrvPlayers[args.Player.Index].Money += Economy.Config.DailyAmount;
				player.SendSuccessMessage($"Pomyslne odebrano swoje pierwsze kieszonkowe wynoszace: {Economy.Config.DailyAmount}] {Economy.Config.ValueName}.");
				player.SendInfoMessage(string.Format("Twoj nowy stan konta: {0} {1}", SurvivalCore.SrvPlayers[args.Player.Index].Money.ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
				return;
			}
			else if ((DateTime.Now - DateTime.ParseExact(QueryDly.LoadNext(player), Economy.DFormat, null)).TotalMilliseconds > 0)
			{
				SurvivalCore.SrvPlayers[args.Player.Index].Money += Economy.Config.DailyAmount;
				player.SendSuccessMessage($"Pomyslne odebrano swoje kieszonkowe wynoszace {Economy.Config.DailyAmount} {Economy.Config.ValueName}.");
				player.SendInfoMessage(string.Format("Twoj nowy stan konta: {0} {1}", SurvivalCore.SrvPlayers[args.Player.Index].Money.ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
				QueryDly.UpdateNext(player, DateTime.Now.AddDays(1));
			}
			else
			{
				player.SendErrorMessage("Kieszonkowe mozna odbierac co 24h.");
				player.SendErrorMessage($"Nastepny odbior bedzie mozliwy za {ExpireCountDown(QueryDly.LoadNext(player))}.");
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
