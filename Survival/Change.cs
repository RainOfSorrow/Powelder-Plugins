using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace SurvivalCore
{
	internal class Change
	{
		public static ChangeConfig Config;

		public static Dictionary<int, Recipe> Recipes = new Dictionary<int, Recipe>();

		public static void Command(CommandArgs args)
		{
			string text = null;
			string s = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			if (args.Parameters.Count > 1)
			{
				s = args.Parameters[1].ToLower();
			}
			if (text == "list")
			{
				int result = 1;
				int.TryParse(s, out result);
				if (result == 0)
				{
					result = 1;
				}
				List<string> list = new List<string>();
				foreach (int key in Recipes.Keys)
				{
					list.Add($" [[c/dddd11:{key}]] [i/s{Recipes[key].Result.Amount}:{Recipes[key].Result.Id}] ← {Recipes[key].GetIngrediensString()}");
				}
				PaginationTools.SendPage(args.Player, result, PaginationTools.BuildLinesFromTerms(list, null, ""), new PaginationTools.Settings
				{
					HeaderTextColor = new Color(0, 255, 0),
					FooterTextColor = new Color(128, 128, 128),
					HeaderFormat = "Lista dostepnych receptur:",
					MaxLinesPerPage = 4,
					LineTextColor = new Color(192, 192, 192),
					FooterFormat = $"Wpisz /wytworz list {result + 1}], aby zobaczyc nastepna strone.",
					NothingToDisplayString = "Error 404."
				});
				return;
			}
			int result2 = 1;
			int.TryParse(s, out result2);
			if (result2 == 0)
			{
				result2 = 1;
			}
			if (text == null)
			{
				args.Player.SendMessage("Uzycie: /wytworz <index> <ilosc>", Color.Gray);
				args.Player.SendMessage("Uzycie: /wytworz list <strona>", Color.Gray);
				return;
			}
			if (!int.TryParse(text, out int result3))
			{
				args.Player.SendErrorMessage("Podano niepoprawny index.");
				return;
			}
			if (!Recipes.ContainsKey(result3))
			{
				args.Player.SendErrorMessage("Nie ma receptury o takim indexie.");
				return;
			}
			if (TShock.Utils.GetItemById(Recipes[result3].Result.Id).maxStack < result2)
			{
				result2 = TShock.Utils.GetItemById(Recipes[result3].Result.Id).maxStack;
			}
			if (SurvivalCore.SrvPlayers[args.Player.Index].Money < Recipes[result3].Cost)
			{
				args.Player.SendErrorMessage($"Nie stac cie na wytworzenie [i:{Recipes[result3].Result.Id}] w ilosci {result2}. Koszt {Recipes[result3].Cost * result2} €.");
				return;
			}
			if (!Recipes[result3].IsPlayerHaveIngrediens(args.Player))
			{
				args.Player.SendErrorMessage("Brakuje wymaganych materialow do stworzenia przedmiotu.");
				return;
			}
			if (!args.Player.InventorySlotAvailable)
			{
				args.Player.SendErrorMessage("Twoj ekwipunek jest pelny.");
				return;
			}
			int done = Recipes[result3].Do(args.Player, result2);
			SurvivalCore.SrvPlayers[args.Player.Index].Money -= Recipes[result3].Cost * done;
			args.Player.SendSuccessMessage($"Pomyslnie wytworzono [i:{Recipes[result3].Result.Id}] w ilosci [c/66ff66:{Recipes[result3].Result.Amount * done}].", Color.Gray);
		}
	}

	internal class ChangeConfig
	{
		public Recipe[] Recipes = new Recipe[5]
		{
			new Recipe(4500, new SimpleItem(1156, 1), new SimpleItem(1533, 1)),
			new Recipe(4500, new SimpleItem(1572, 1), new SimpleItem(1537, 1)),
			new Recipe(4500, new SimpleItem(1260, 1), new SimpleItem(1536, 1)),
			new Recipe(4500, new SimpleItem(1569, 1), new SimpleItem(1535, 1)),
			new Recipe(4500, new SimpleItem(1571, 1), new SimpleItem(1534, 1))
		};

		public void Write(string file)
		{
			File.WriteAllText(file, JsonConvert.SerializeObject((object)this, (Formatting)1));
		}

		public static ChangeConfig Read(string file)
		{
			return File.Exists(file) ? JsonConvert.DeserializeObject<ChangeConfig>(File.ReadAllText(file)) : new ChangeConfig();
		}
	}
}
