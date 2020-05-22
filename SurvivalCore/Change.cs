using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace SurvivalCore
{
	internal class Change
	{
		public static ChangeConfig config;

		public static Dictionary<int, Recipe> recipes = new Dictionary<int, Recipe>();

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
				foreach (int key in recipes.Keys)
				{
					list.Add($" [[c/dddd11:{key}]] [i/s{recipes[key].result.amount}:{recipes[key].result.id}] ← {recipes[key].getIngrediensString()}");
				}
				PaginationTools.SendPage(args.Player, result, PaginationTools.BuildLinesFromTerms(list, null, ""), new PaginationTools.Settings
				{
					HeaderTextColor = new Color(128, 128, 128),
					FooterTextColor = new Color(128, 128, 128),
					HeaderFormat = "Lista dostepnych receptur[c/595959::]",
					MaxLinesPerPage = 4,
					LineTextColor = new Color(192, 192, 192),
					FooterFormat = $"Wpisz [c/66FF66:/wytworz list {result + 1}], aby zobaczyc nastepna strone.",
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
				args.Player.SendMessage("[c/595959:»]  [c/66ff66:Uzycie][c/595959::] /wytworz <index> <ilosc>", Color.Gray);
				args.Player.SendMessage("[c/595959:»]  [c/66ff66:Uzycie][c/595959::] /wytworz list <strona>", Color.Gray);
				return;
			}
			if (!int.TryParse(text, out int result3))
			{
				args.Player.SendErrorMessage("[c/595959:»]  Podano niepoprawny index.");
				return;
			}
			if (!recipes.ContainsKey(result3))
			{
				args.Player.SendErrorMessage("[c/595959:»]  Nie ma receptury o takim indexie.");
				return;
			}
			if (TShock.Utils.GetItemById(recipes[result3].result.id).maxStack < result2)
			{
				result2 = TShock.Utils.GetItemById(recipes[result3].result.id).maxStack;
			}
			if (SurvivalCore.srvPlayers[args.Player.Index].Money < recipes[result3].cost)
			{
				args.Player.SendErrorMessage($"[c/595959:»]  Nie stac cie na wytworzenie [i:{recipes[result3].result.id}] w ilosci {result2}. [c/595959:(]Koszt {recipes[result3].cost * result2} €[c/595959:)]");
				return;
			}
			if (!recipes[result3].isPlayerHaveIngrediens(args.Player))
			{
				args.Player.SendErrorMessage("[c/595959:»]  Brakuje wymaganych materialow do stworzenia przedmiotu.");
				return;
			}
			if (!args.Player.InventorySlotAvailable)
			{
				args.Player.SendErrorMessage("[c/595959:»]  Twoj ekwipunek jest pelny.");
				return;
			}
			recipes[result3].Do(args.Player, result2);
			SurvivalCore.srvPlayers[args.Player.Index].Money -= recipes[result3].cost * result2;
			args.Player.SendMessage($"[c/595959:»]  Pomyslnie wytworzono [i:{recipes[result3].result.id}] w ilosci [c/66ff66:{recipes[result3].result.amount * result2}].", Color.Gray);
		}
	}

	internal class ChangeConfig
	{
		public Recipe[] recipes = new Recipe[5]
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
