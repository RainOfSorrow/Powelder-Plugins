using Microsoft.Xna.Framework;
using SurvivalCore.Economy.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.Localization;

namespace SurvivalCore.Economy.Shop
{
	public class SCommands
	{
		public static void ShopUser(CommandArgs args)
		{
			TSPlayer player = args.Player;
			string text = null;
			string cmd2 = null;
			string text2 = null;
			string text3 = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			if (args.Parameters.Count > 1)
			{
				cmd2 = args.Parameters[1];
			}
			else
			{
				cmd2 = "*";
			}
			text2 = ((args.Parameters.Count <= 2) ? "1" : args.Parameters[2].ToLower());
			if (args.Parameters.Count > 3)
			{
				text3 = args.Parameters[3].ToLower();
			}
			switch (text)
			{
			default:
				args.Player.SendMessage("Sklep: ", Color.Green);
				args.Player.SendInfoMessage("/sklep szukaj <nazwa>");
				args.Player.SendInfoMessage("/sklep kup <index/nazwa> <ilosc>");
				args.Player.SendInfoMessage("/sklep sprzedaj <index/nazwa> <ilosc>");
				break;
			case "search":
			case "szukaj":
			{
				int result3 = 1;
				int.TryParse(text2, out result3);
				List<string> list = new List<string>();
				if (cmd2 == null || cmd2 == "*")
				{
					foreach (Product product5 in Economy.Products)
					{
						list.Add($"[[c/dddd11:{product5.GetIndex()}]] [i:{product5.GetId()}] ([c/11bb11:K] [c/55bb55:{product5.GetBuy()}] ; [c/D42A2A:S] [c/D45A5A:{product5.GetSell()}])");
					}
				}
				else if (cmd2.StartsWith("[i") && cmd2.EndsWith("]"))
				{
					int count = cmd2.IndexOf(':') + 1;
					string text4 = cmd2.Remove(0, count);
					count = text4.IndexOf(']');
					text4 = text4.Remove(count);
					foreach (Product product6 in Economy.Products)
					{
						if (product6.GetId().ToString() == text4)
						{
							list.Add($"[[c/dddd11:{product6.GetIndex()}]] [i:{product6.GetId()}] ([c/11bb11:K] [c/55bb55:{product6.GetBuy()}] ; [c/D42A2A:S] [c/D45A5A:{product6.GetSell()}])");
						}
					}
				}
				else
				{
					foreach (Product product7 in Economy.Products)
					{
						if (product7.GetName().ToLower().Contains(cmd2.ToLower()))
						{
							list.Add($"[[c/dddd11:{product7.GetIndex()}]] [i:{product7.GetId()}] ([c/11bb11:K] [c/55bb55:{product7.GetBuy()}] ; [c/D42A2A:S] [c/D45A5A:{product7.GetSell()}])");
						}
					}
				}
				args.Player.SendMessage(string.Format("Wyniki dla: \"{0}\"", (cmd2 == null) ? "*" : cmd2), Color.Green);
				PaginationTools.SendPage(args.Player, result3, PaginationTools.BuildLinesFromTerms(list, null, " | ", 300), new PaginationTools.Settings
				{
					IncludeHeader = false,
					LineTextColor = new Color(192, 192, 192),
					FooterFormat = string.Format("Wpisz \"/sklep szukaj {0} {1}\", aby zobaczyc nastepna strone", (cmd2 == null) ? "*" : cmd2, result3 + 1),
					FooterTextColor = Color.Green,
					NothingToDisplayString = "[c/00ff00:Nie znaleziono zadnego produktu. Wpisz \"/sklep szukaj *\", aby otrzymac pelna liste produktow.]"
				});
				break;
			}
			case "buy":
			case "kup":
			{
				if (int.TryParse(cmd2, out int index))
				{
					if (index < 0)
					{
						args.Player.SendErrorMessage("Podano wartosc ujemna indexu, mozesz podac tylko dodatnia.");
						return;
					}
					
					Product product3 = new Product(0, null, 0, 0, 0);
					if (Economy.Products.Exists(x => x.Index == index))
					{
						product3 = Economy.Products.Find(x => x.Index == index);
					}
					if (product3.GetIndex() == 0)
					{
						player.SendErrorMessage("Nie znaleziono przedmiotu w sklepie.");
						break;
					}
					if (product3.GetBuy() <= 0)
					{
						player.SendErrorMessage("Tego przedmiotu nie mozna zakupic.");
						break;
					}
					Item item3 = TShock.Utils.GetItemByIdOrName(product3.GetId().ToString())[0];
					int amount = 1;
					if (args.Parameters.Count > 1)
					{
						int.TryParse(text2, out amount);
					}
					if (amount > item3.maxStack)
					{
						amount = item3.maxStack;
					}
					
					if (amount < 0)
					{
						args.Player.SendErrorMessage("Podano wartosc ujemna ilosci, mozesz podac tylko dodatnia.");
						return;
					}
					
					int num3 = amount * product3.GetBuy();
					int money = SurvivalCore.SrvPlayers[player.Index].Money;
					if (money < num3)
					{
						args.Player.SendErrorMessage("Nie stac cie na zakup {0} w ilosci {1}. Brakuje ci {2} {3}", item3.Name, amount, Math.Abs(money - num3), Economy.Config.ValueName);
					}
					else if (args.Player.Dead)
					{
						args.Player.SendErrorMessage("Musisz byc zywy, kiedy chcesz cos zakupic.");
					}
					else if (args.Player.InventorySlotAvailable || (item3.type > 70 && item3.type < 75) || item3.ammo > 0 || item3.type == 58 || item3.type == 184)
					{
						SurvivalCore.SrvPlayers[player.Index].Money -= num3;
						args.Player.GiveItem(item3.type, amount);
						args.Player.SendSuccessMessage($"Pomyslnie zakupiono [i:{item3.type}] w ilosci {amount}. Wydano {num3} {Economy.Config.ValueName}");
						args.Player.SendInfoMessage($"Twoj nowy stan konta: {money - num3:N0} {Economy.Config.ValueName}");
					}
					else
					{
						args.Player.SendErrorMessage("Twoj ekwipunek jest pelny.");
					}
					break;
				}
				Product product4 = new Product(0, null, 0, 0, 0);
				if (Economy.Products.Exists(x => x.Name == cmd2))
				{
					product4 = Economy.Products.Find( x=> x.Name == cmd2);
				}
				if (product4.GetIndex() == 0)
				{
					player.SendErrorMessage("Nie znaleziono przedmiotu w sklepie.");
					break;
				}
				if (product4.GetBuy() <= 0)
				{
					player.SendErrorMessage("Tego przedmiotu nie mozna zakupic.");
					break;
				}
				Item item4 = TShock.Utils.GetItemByIdOrName(product4.GetId().ToString())[0];
				int amount3 = 1;
				if (args.Parameters.Count > 1)
				{
					int.TryParse(text2, out amount3);
				}
				if (amount3 > item4.maxStack)
				{
					amount3 = item4.maxStack;
				}
				int num4 = amount3 * product4.GetBuy();
				int money2 = SurvivalCore.SrvPlayers[player.Index].Money;
				if (money2 < num4)
				{
					args.Player.SendErrorMessage("Nie stac cie na zakup {0} w ilosci {1}. Brakuje ci {2:N0} {3}", item4.Name, amount3, Math.Abs(money2 - num4), Economy.Config.ValueName);
				}
				else if (args.Player.Dead)
				{
					args.Player.SendErrorMessage("Musisz byc zywy, kiedy chcesz cos zakupic.");
				}
				else if (args.Player.InventorySlotAvailable || (item4.type > 70 && item4.type < 75) || item4.ammo > 0 || item4.type == 58 || item4.type == 184)
				{
					SurvivalCore.SrvPlayers[player.Index].Money -= num4;
					args.Player.GiveItem(item4.type, amount3);
					args.Player.SendSuccessMessage($"Pomyslnie zakupiono [i:{item4.type}] w ilosci {amount3}. Wydano {num4:N0} {Economy.Config.ValueName}");
					args.Player.SendInfoMessage($"Twoj nowy stan konta: {money2 - num4:N0} {Economy.Config.ValueName}");
				}
				else
				{
					args.Player.SendErrorMessage("Twoj ekwipunek jest pelny.");
				}
				break;
			}
			case "sell":
			case "sprzedaj":
			{
				if (int.TryParse(cmd2, out int index2))
				{
					if (index2 < 0)
					{
						args.Player.SendErrorMessage("Podano wartosc ujemna, mozesz podac tylko dodatnia.");
						return;
					}
					
					Product product = new Product(0, null, 0, 0, 0);
					if (Economy.Products.Exists((Product x) => x.Index == index2))
					{
						product = Economy.Products.Find((Product x) => x.Index == index2);
					}
					if (product.GetIndex() == 0)
					{
						player.SendErrorMessage("Nie znaleziono przedmiotu w sklepie.");
						break;
					}
					if (product.GetSell() <= 0)
					{
						player.SendErrorMessage("Tego przedmiotu nie mozna sprzedac.");
						break;
					}
					Item item = TShock.Utils.GetItemByIdOrName(product.GetId().ToString())[0];
					int result = 1;
					if (args.Parameters.Count > 1)
					{
						int.TryParse(text2, out result);
					}
					if (result > PowelderAPI.Utils.PlayerItemCount(args.Player, item))
					{
						player.SendErrorMessage("Nie masz tylu {0} w ekwipunku.", item.Name);
						break;
					}
					
					if (result < 0)
					{
						args.Player.SendErrorMessage("Podano wartosc ujemna ilosci, mozesz podac tylko dodatnia.");
						return;
					}
					int num = result * product.GetSell();
					SurvivalCore.SrvPlayers[player.Index].Money += num;
					PowelderAPI.Utils.PlayerRemoveItems(args.Player, item, result);
					args.Player.SendSuccessMessage($"Pomyslnie sprzedano [i:{item.type}] w ilosci {result}. Zarobiono {num:N0} {Economy.Config.ValueName}");
					args.Player.SendInfoMessage($"Twoj nowy stan konta: {SurvivalCore.SrvPlayers[player.Index].Money:N0} {Economy.Config.ValueName}");
					break;
				}
				Product product2 = new Product(0, null, 0, 0, 0);
				if (Economy.Products.Exists((Product x) => x.Name == cmd2))
				{
					product2 = Economy.Products.Find((Product x) => x.Name == cmd2);
				}
				if (product2.GetIndex() == 0)
				{
					player.SendErrorMessage("Nie znaleziono przedmiotu w sklepie.");
					break;
				}
				if (product2.GetSell() <= 0)
				{
					player.SendErrorMessage("Tego przedmiotu nie mozna sprzedac.");
					break;
				}
				Item item2 = TShock.Utils.GetItemByIdOrName(product2.GetId().ToString())[0];
				int result2 = 1;
				if (args.Parameters.Count > 1)
				{
					int.TryParse(text2, out result2);
				}
				if (result2 > PowelderAPI.Utils.PlayerItemCount(args.Player, item2))
				{
					player.SendErrorMessage("Nie masz tylu {0} w ekwipunku.", item2.Name);
					break;
				}
				int num2 = result2 * product2.GetSell();
				SurvivalCore.SrvPlayers[player.Index].Money += num2;
				PowelderAPI.Utils.PlayerRemoveItems(args.Player, item2, result2);
				args.Player.SendSuccessMessage($"Pomyslnie sprzedano [i:{item2.type}] w ilosci {result2}. Zarobiono {num2:N0} {Economy.Config.ValueName}");
				args.Player.SendInfoMessage($"Twoj nowy stan konta: {SurvivalCore.SrvPlayers[player.Index].Money:N0} {Economy.Config.ValueName}");
				break;
			}
			}
		}

		public static void ShopAdmin(CommandArgs args)
		{
			string text = null;
			string text2 = null;
			string s = null;
			string s2 = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			if (args.Parameters.Count > 1)
			{
				text2 = args.Parameters[1].ToLower();
			}
			if (args.Parameters.Count > 2)
			{
				s = args.Parameters[2].ToLower();
			}
			if (args.Parameters.Count > 3)
			{
				s2 = args.Parameters[3].ToLower();
			}
			switch (text)
			{
			default:
				args.Player.SendMessage("Admin Sklep:", Color.Green);
				args.Player.SendInfoMessage("add <ID/nazwa> <kupno> <sprzedaz>  - Dodawanie produktu");
				args.Player.SendInfoMessage("del <index> - Usuwanie produktu");
				args.Player.SendInfoMessage("edit <index> <kupno> <sprzedaz> - Edycja produktu");
				break;
			case "add":
			{
				List<Item> itemByIdOrName = TShock.Utils.GetItemByIdOrName(text2);
				if (itemByIdOrName.Count == 0)
				{
					args.Player.SendErrorMessage("Nie znaleziono ani jednego itemu! :C *Płacz* ;C");
					break;
				}
				if (itemByIdOrName.Count > 1)
				{
					args.Player.SendMultipleMatchError(itemByIdOrName.Select((Item i) => $"{i.Name}({i.netID})"));
					break;
				}
				Item item = itemByIdOrName[0];
				int result4 = 0;
				int result5 = 0;
				if (!int.TryParse(s, out result4))
				{
					args.Player.SendErrorMessage("Wprowadz poprawny Buy.");
					break;
				}
				if (!int.TryParse(s2, out result5))
				{
					args.Player.SendErrorMessage("Wprowadz poprawny Sell.");
					break;
				}
				QueryShop.AddProduct(item.Name, item.type, result4, result5);
				args.Player.SendMessage($"Pomyslnie dodano [i:{item.Name}] ([c/11bb11:K] [c/55bb55:{result4}] ; [c/D42A2A:S] [c/D45A5A:{result5}])", Color.Gray);
				break;
			}
			case "del":
			{
				int result6 = -1;
				if (!int.TryParse(text2, out result6))
				{
					args.Player.SendErrorMessage("Wprowadz poprawny Index.");
				}
				else if (QueryShop.IsExistIndex(result6))
				{
					QueryShop.DelProduct(result6);
					args.Player.SendMessage($"Pomyslnie usunieto.", Color.Gray);
				}
				else
				{
					args.Player.SendErrorMessage("Nie ma takiego produktu o podanym Indeksie.");
				}
				break;
			}
			case "edit":
			{
				int result = 0;
				int result2 = 0;
				int result3 = 0;
				if (!int.TryParse(text2, out result))
				{
					args.Player.SendErrorMessage("Wprowadz poprawny Index.");
				}
				else if (!int.TryParse(s, out result2))
				{
					args.Player.SendErrorMessage("Wprowadz poprawny Buy.");
				}
				else if (!int.TryParse(s2, out result3))
				{
					args.Player.SendErrorMessage("Wprowadz poprawny Sell.");
				}
				else if (QueryShop.IsExistIndex(result))
				{
					QueryShop.UpdateProduct(result, result2, result3);
					args.Player.SendMessage($"Pomyslnie edytowano.", Color.Gray);
				}
				else
				{
					args.Player.SendErrorMessage("Nie ma takiego produktu o podanym Indeksie.");
				}
				break;
			}
			}
		}

	}
}
