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
				args.Player.SendMessage("                [c/595959:«]Sklep[c/595959:»]", new Color(192, 192, 192));
				args.Player.SendMessage("[c/595959:»]  [c/66FF66:/sklep] szukaj <nazwa>", Color.Gray);
				args.Player.SendMessage("[c/595959:»]  [c/66FF66:/sklep] kup <index/nazwa> <ilosc>", Color.Gray);
				args.Player.SendMessage("[c/595959:»]  [c/66FF66:/sklep] sprzedaj <index/nazwa> <ilosc>", Color.Gray);
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
						list.Add($"[[c/dddd11:{product5.getIndex()}]] [i:{product5.getID()}] ([c/11bb11:K] [c/55bb55:{product5.getBuy()}] ; [c/D42A2A:S] [c/D45A5A:{product5.getSell()}])");
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
						if (product6.getID().ToString() == text4)
						{
							list.Add($"[[c/dddd11:{product6.getIndex()}]] [i:{product6.getID()}] ([c/11bb11:K] [c/55bb55:{product6.getBuy()}] ; [c/D42A2A:S] [c/D45A5A:{product6.getSell()}])");
						}
					}
				}
				else
				{
					foreach (Product product7 in Economy.Products)
					{
						if (product7.getName().ToLower().Contains(cmd2.ToLower()))
						{
							list.Add($"[[c/dddd11:{product7.getIndex()}]] [i:{product7.getID()}] ([c/11bb11:K] [c/55bb55:{product7.getBuy()}] ; [c/D42A2A:S] [c/D45A5A:{product7.getSell()}])");
						}
					}
				}
				args.Player.SendMessage(string.Format("[c/66ff66:Wyniki dla][c/595959::] \"{0}\"", (cmd2 == null) ? "*" : cmd2), Color.Gray);
				PaginationTools.SendPage(args.Player, result3, PaginationTools.BuildLinesFromTerms(list, null, " | ", 300), new PaginationTools.Settings
				{
					IncludeHeader = false,
					LineTextColor = new Color(192, 192, 192),
					FooterFormat = string.Format("[c/595959:»]  [c/808080:Wpisz] [c/66FF66:/sklep szukaj {0} {1}][c/808080:, aby zobaczyc nastepna strone.]", (cmd2 == null) ? "*" : cmd2, result3 + 1),
					NothingToDisplayString = "[c/808080:Nie znaleziono zadnego produktu. Wpisz ][c/66ff66:/sklep szukaj *][c/808080:, aby otrzymac pelna liste produktow.]"
				});
				break;
			}
			case "buy":
			case "kup":
			{
				if (int.TryParse(cmd2, out int Index))
				{
					Product product3 = new Product(0, null, 0, 0, 0);
					if (Economy.Products.Exists((Product x) => x.Index == Index))
					{
						product3 = Economy.Products.Find((Product x) => x.Index == Index);
					}
					if (product3.getIndex() == 0)
					{
						player.SendErrorMessage("[c/595959:»]  Nie znaleziono przedmiotu w sklepie.");
						break;
					}
					if (product3.getBuy() <= 0)
					{
						player.SendErrorMessage("[c/595959:»]  Tego przedmiotu nie mozna zakupic.");
						break;
					}
					Item item3 = TShock.Utils.GetItemByIdOrName(product3.getID().ToString())[0];
					int amount = 1;
					if (args.Parameters.Count > 1)
					{
						int.TryParse(text2, out amount);
					}
					if (amount > item3.maxStack)
					{
						amount = item3.maxStack;
					}
					int num3 = amount * product3.getBuy();
					int money = SurvivalCore.srvPlayers[player.Index].Money;
					if (money < num3)
					{
						args.Player.SendErrorMessage("[c/595959:»]  Nie stac cie na zakup {0} w ilosci {1}. [c/595959:(]Brakuje ci {2} {3}[c/595959:)]", item3.Name, amount, Math.Abs(money - num3), Economy.config.ValueName);
					}
					else if (args.Player.Dead)
					{
						args.Player.SendErrorMessage("[c/595959:»]  Musisz byc zywy, kiedy chcesz cos zakupic.");
					}
					else if (args.Player.InventorySlotAvailable || (item3.type > 70 && item3.type < 75) || item3.ammo > 0 || item3.type == 58 || item3.type == 184)
					{
						SurvivalCore.srvPlayers[player.Index].Money -= num3;
						args.Player.GiveItem(item3.type, amount);
						args.Player.SendMessage($"[c/595959:»]  Pomyslnie zakupiono [c/66ff66:{item3.Name}] w ilosci [c/66ff66:{amount}]. [c/595959:(]Wydano [c/ff6666:{num3}] {Economy.config.ValueName}[c/595959:)]", Color.Gray);
						args.Player.SendMessage($"[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {money - num3:N0} {Economy.config.ValueName}", Color.Gray);
					}
					else
					{
						args.Player.SendErrorMessage("[c/595959:»]  Twoj ekwipunek jest pelny.");
					}
					break;
				}
				Product product4 = new Product(0, null, 0, 0, 0);
				if (Economy.Products.Exists((Product x) => x.Name == cmd2))
				{
					product4 = Economy.Products.Find((Product x) => x.Name == cmd2);
				}
				if (product4.getIndex() == 0)
				{
					player.SendErrorMessage("[c/595959:»]  Nie znaleziono przedmiotu w sklepie.");
					break;
				}
				if (product4.getBuy() <= 0)
				{
					player.SendErrorMessage("[c/595959:»]  Tego przedmiotu nie mozna zakupic.");
					break;
				}
				Item item4 = TShock.Utils.GetItemByIdOrName(product4.getID().ToString())[0];
				int amount3 = 1;
				if (args.Parameters.Count > 1)
				{
					int.TryParse(text2, out amount3);
				}
				if (amount3 > item4.maxStack)
				{
					amount3 = item4.maxStack;
				}
				int num4 = amount3 * product4.getBuy();
				int money2 = SurvivalCore.srvPlayers[player.Index].Money;
				if (money2 < num4)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie stac cie na zakup {0} w ilosci {1}. [c/595959:(]Brakuje ci {2:N0} {3}[c/595959:)]", item4.Name, amount3, Math.Abs(money2 - num4), Economy.config.ValueName);
				}
				else if (args.Player.Dead)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Musisz byc zywy, kiedy chcesz cos zakupic.");
				}
				else if (args.Player.InventorySlotAvailable || (item4.type > 70 && item4.type < 75) || item4.ammo > 0 || item4.type == 58 || item4.type == 184)
				{
					SurvivalCore.srvPlayers[player.Index].Money -= num4;
					args.Player.GiveItem(item4.type, amount3);
					args.Player.SendMessage($"[c/595959:»]  Pomyslnie zakupiono [c/66ff66:{item4.Name}] w ilosci [c/66ff66:{amount3}]. [c/595959:(]Wydano [c/66ff66:{num4:N0}] {Economy.config.ValueName}[c/595959:)]", Color.Gray);
					args.Player.SendMessage($"[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {money2 - num4:N0} {Economy.config.ValueName}", Color.Gray);
				}
				else
				{
					args.Player.SendErrorMessage("[c/595959:»]  Twoj ekwipunek jest pelny.");
				}
				break;
			}
			case "sell":
			case "sprzedaj":
			{
				if (int.TryParse(cmd2, out int Index2))
				{
					Product product = new Product(0, null, 0, 0, 0);
					if (Economy.Products.Exists((Product x) => x.Index == Index2))
					{
						product = Economy.Products.Find((Product x) => x.Index == Index2);
					}
					if (product.getIndex() == 0)
					{
						player.SendErrorMessage("[c/595959:»]  Nie znaleziono przedmiotu w sklepie.");
						break;
					}
					if (product.getSell() <= 0)
					{
						player.SendErrorMessage("[c/595959:»]  Tego przedmiotu nie mozna sprzedac.");
						break;
					}
					Item item = TShock.Utils.GetItemByIdOrName(product.getID().ToString())[0];
					int result = 1;
					if (args.Parameters.Count > 1)
					{
						int.TryParse(text2, out result);
					}
					if (result > PlayerItemAmount(args.Player, item))
					{
						player.SendErrorMessage("[c/595959:»]  Nie masz tylu {0} w ekwipunku.", item.Name);
						break;
					}
					int num = result * product.getSell();
					SurvivalCore.srvPlayers[player.Index].Money += num;
					PlayerRemovingItems(args.Player, item, result);
					args.Player.SendMessage($"[c/595959:»]  Pomyslnie sprzedano [c/66ff66:{item.Name}] w ilosci [c/66ff66:{result}]. [c/595959:(]Zarobiono [c/66ff66:{num:N0}] {Economy.config.ValueName}[c/595959:)]", Color.Gray);
					args.Player.SendMessage($"[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {SurvivalCore.srvPlayers[player.Index].Money:N0} {Economy.config.ValueName}", Color.Gray);
					break;
				}
				Product product2 = new Product(0, null, 0, 0, 0);
				if (Economy.Products.Exists((Product x) => x.Name == cmd2))
				{
					product2 = Economy.Products.Find((Product x) => x.Name == cmd2);
				}
				if (product2.getIndex() == 0)
				{
					player.SendErrorMessage("[c/595959:»]  Nie znaleziono przedmiotu w sklepie.");
					break;
				}
				if (product2.getSell() <= 0)
				{
					player.SendErrorMessage("[c/595959:»]  Tego przedmiotu nie mozna sprzedac.");
					break;
				}
				Item item2 = TShock.Utils.GetItemByIdOrName(product2.getID().ToString())[0];
				int result2 = 1;
				if (args.Parameters.Count > 1)
				{
					int.TryParse(text2, out result2);
				}
				if (result2 > PlayerItemAmount(args.Player, item2))
				{
					player.SendErrorMessage("[c/595959:»]  Nie masz tylu {0} w ekwipunku.", item2.Name);
					break;
				}
				int num2 = result2 * product2.getSell();
				SurvivalCore.srvPlayers[player.Index].Money += num2;
				PlayerRemovingItems(args.Player, item2, result2);
				args.Player.SendMessage($"[c/595959:»]  Pomyslnie sprzedano [c/66ff66:{item2.Name}] w ilosci [c/66ff66:{result2}]. [c/595959:(]Zarobiono [c/66ff66:{num2:N0}] {Economy.config.ValueName}[c/595959:)]", Color.Gray);
				args.Player.SendMessage($"[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {SurvivalCore.srvPlayers[player.Index].Money:N0} {Economy.config.ValueName}", Color.Gray);
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
				args.Player.SendMessage("[c/66ff66:Sklep][c/595959::]", Color.Gray);
				args.Player.SendMessage("[c/595959:»]  add <ID/nazwa> <kupno> <sprzedaz>  - Dodawanie produktu", Color.Gray);
				args.Player.SendMessage("[c/595959:»]  del <index> - Usuwanie produktu", Color.Gray);
				args.Player.SendMessage("[c/595959:»]  edit <index> <kupno> <sprzedaz> - Edycja produktu", Color.Gray);
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
					args.Player.SendErrorMessage("[c/595959:»]  Wprowadz poprawny Buy.");
					break;
				}
				if (!int.TryParse(s2, out result5))
				{
					args.Player.SendErrorMessage("[c/595959:»]  Wprowadz poprawny Sell.");
					break;
				}
				QueryShop.addProduct(item.Name, item.type, result4, result5);
				args.Player.SendMessage($"[c/595959:»]  Pomyslnie dodano [i:{item.Name}] ([c/11bb11:K] [c/55bb55:{result4}] ; [c/D42A2A:S] [c/D45A5A:{result5}])", Color.Gray);
				break;
			}
			case "del":
			{
				int result6 = -1;
				if (!int.TryParse(text2, out result6))
				{
					args.Player.SendErrorMessage("[c/595959:»]  Wprowadz poprawny Index.");
				}
				else if (QueryShop.isExistIndex(result6))
				{
					QueryShop.delProduct(result6);
					args.Player.SendMessage($"[c/595959:»]  Pomyslnie usunieto.", Color.Gray);
				}
				else
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie ma takiego produktu o podanym Indeksie.");
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
					args.Player.SendErrorMessage("[c/595959:»]  Wprowadz poprawny Index.");
				}
				else if (!int.TryParse(s, out result2))
				{
					args.Player.SendErrorMessage("[c/595959:»]  Wprowadz poprawny Buy.");
				}
				else if (!int.TryParse(s2, out result3))
				{
					args.Player.SendErrorMessage("[c/595959:»]  Wprowadz poprawny Sell.");
				}
				else if (QueryShop.isExistIndex(result))
				{
					QueryShop.updateProduct(result, result2, result3);
					args.Player.SendMessage($"[c/595959:»]  Pomyslnie edytowano.", Color.Gray);
				}
				else
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie ma takiego produktu o podanym Indeksie.");
				}
				break;
			}
			}
		}

		private static int PlayerItemAmount(TSPlayer plr, Item item)
		{
			int num = 0;
			Item[] inventory = plr.TPlayer.inventory;
			foreach (Item item2 in inventory)
			{
				if (item2.Name == item.Name)
				{
					num += item2.stack;
				}
			}
			return num;
		}

		private static void PlayerRemovingItems(TSPlayer plr, Item item, int amount)
		{
			int num = 0;
			while (true)
			{
				if (num >= plr.TPlayer.inventory.Count())
				{
					return;
				}
				if (plr.TPlayer.inventory[num].Name == item.Name)
				{
					if (plr.TPlayer.inventory[num].stack >= amount)
					{
						break;
					}
					amount -= plr.TPlayer.inventory[num].stack;
					plr.TPlayer.inventory[num] = new Item();
					NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[num].Name), plr.Index, num, (int)plr.TPlayer.inventory[num].prefix);
					NetMessage.SendData(5, plr.Index, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[num].Name), plr.Index, num, (int)plr.TPlayer.inventory[num].prefix);
				}
				num++;
			}
			plr.TPlayer.inventory[num].stack -= amount;
			NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[num].Name), plr.Index, num, (int)plr.TPlayer.inventory[num].prefix);
			NetMessage.SendData(5, plr.Index, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[num].Name), plr.Index, num, (int)plr.TPlayer.inventory[num].prefix);
		}
	}
}
