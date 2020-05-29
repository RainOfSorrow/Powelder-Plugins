using System.Collections.Generic;
using TShockAPI;

namespace SurvivalCore
{
	internal class Recipe
	{
		public int Cost;

		public SimpleItem Result;

		public List<SimpleItem> Ingrediens = new List<SimpleItem>();

		public Recipe(int cost, SimpleItem result, params SimpleItem[] ingrediens)
		{
			this.Cost = cost;
			this.Result = result;
			this.Ingrediens.AddRange(ingrediens);
		}

		public string GetIngrediensString()
		{
			string text = null;
			bool flag = true;
			foreach (SimpleItem ingredien in Ingrediens)
			{
				if (flag)
				{
					text = text + "[i/s" + ingredien.Amount + ":" + ingredien.Id + "]";
					flag = false;
				}
				else
				{
					text = text + ", [i/s" + ingredien.Amount + ":" + ingredien.Id + "]";
				}
			}
			if (Cost != 0)
			{
				text += $", [c/ffff66:{Cost} €]";
			}
			return text;
		}

		public bool IsPlayerHaveIngrediens(TSPlayer plr, int amount = 1)
		{
			foreach (SimpleItem ingredien in Ingrediens)
			{
				if (PowelderAPI.Utils.PlayerItemCount(plr, TShock.Utils.GetItemById(ingredien.Id)) < ingredien.Amount * amount)
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
				foreach (SimpleItem ingredien in Ingrediens)
				{
					PowelderAPI.Utils.PlayerRemoveItems(plr, TShock.Utils.GetItemById(ingredien.Id), ingredien.Amount);
				}
				PowelderAPI.Utils.GiveItemWithoutSpawn(plr, TShock.Utils.GetItemById(Result.Id), Result.Amount);
			}
		}
	}

	internal class SimpleItem
	{
		public string Name;

		public int Id;

		public int Amount;

		public SimpleItem(int i, int a)
		{
			Name = TShock.Utils.GetItemById(i).Name;
			Id = i;
			Amount = a;
		}
	}
}
