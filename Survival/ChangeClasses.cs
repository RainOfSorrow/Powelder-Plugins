using System.Collections.Generic;
using TShockAPI;

namespace SurvivalCore
{
	internal class Recipe
	{
		public int Cost;

		public SimpleItem Result;

		public List<SimpleItem> Ingredients = new List<SimpleItem>();

		public Recipe(int cost, SimpleItem result, params SimpleItem[] ingrediens)
		{
			this.Cost = cost;
			this.Result = result;
			this.Ingredients.AddRange(ingrediens);
		}

		public string GetIngredientsString()
		{
			string text = null;
			bool flag = true;
			foreach (SimpleItem ingredient in Ingredients)
			{
				if (flag)
				{
					text = text + "[i/s" + ingredient.Amount + ":" + ingredient.Id + "]";
					flag = false;
				}
				else
				{
					text = text + ", [i/s" + ingredient.Amount + ":" + ingredient.Id + "]";
				}
			}
			if (Cost != 0)
			{
				text += $", [c/ffff66:{Cost} €]";
			}
			return text;
		}

		public bool IsPlayerHaveIngredients(TSPlayer plr, int amount = 1)
		{
			foreach (SimpleItem ingredient in Ingredients)
			{
				if (PowelderAPI.Utils.PlayerItemCount(plr, TShock.Utils.GetItemById(ingredient.Id)) < ingredient.Amount * amount)
				{
					return false;
				}
			}
			return true;
		}

		public int Do(TSPlayer plr, int amount = 1)
		{
			if (!plr.InventorySlotAvailable)
			{
				plr.SendErrorMessage("Zabraklo miejsca w ekwipunku.");
				return 0;
			}
			
			for (int i = 0; i < amount; i++)
			{

				foreach (SimpleItem ingredient in Ingredients)
				{
					PowelderAPI.Utils.PlayerRemoveItems(plr, TShock.Utils.GetItemById(ingredient.Id), ingredient.Amount);
				}
				plr.GiveItem(Result.Id, Result.Amount);
			}

			return amount;
		}
	}

	internal struct SimpleItem
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
