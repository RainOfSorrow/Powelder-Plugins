using System.Collections.Generic;
using TShockAPI;

namespace SurvivalCore
{
	internal class Recipe
	{
		public int cost;

		public SimpleItem result;

		public List<SimpleItem> ingrediens = new List<SimpleItem>();

		public Recipe(int Cost, SimpleItem Result, params SimpleItem[] Ingrediens)
		{
			cost = Cost;
			result = Result;
			ingrediens.AddRange(Ingrediens);
		}

		public string getIngrediensString()
		{
			string text = null;
			bool flag = true;
			foreach (SimpleItem ingredien in ingrediens)
			{
				if (flag)
				{
					text = text + "[i/s" + ingredien.amount + ":" + ingredien.id + "]";
					flag = false;
				}
				else
				{
					text = text + ", [i/s" + ingredien.amount + ":" + ingredien.id + "]";
				}
			}
			if (cost != 0)
			{
				text += $", [c/ffff66:{cost} €]";
			}
			return text;
		}

		public bool isPlayerHaveIngrediens(TSPlayer plr, int amount = 1)
		{
			foreach (SimpleItem ingredien in ingrediens)
			{
				if (PowelderAPI.Utils.PlayerItemAmount(plr, TShock.Utils.GetItemById(ingredien.id)) < ingredien.amount * amount)
				{
					return false;
				}
			}
			return true;
		}

		public void Do(TSPlayer plr, int amount = 1)
		{
			for (int i = 0; i < amount; i++)
			{
				foreach (SimpleItem ingredien in ingrediens)
				{
					PowelderAPI.Utils.PlayerRemovingItems(plr, TShock.Utils.GetItemById(ingredien.id), ingredien.amount);
				}
				PowelderAPI.Utils.GiveItemWithoutSpawn(plr, TShock.Utils.GetItemById(result.id), result.amount);
			}
		}
	}

	internal class SimpleItem
	{
		public string name;

		public int id;

		public int amount;

		public SimpleItem(int i, int a)
		{
			name = TShock.Utils.GetItemById(i).Name;
			id = i;
			amount = a;
		}
	}
}
