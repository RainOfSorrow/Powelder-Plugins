using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace PowelderAPI
{
	public class Utils
	{
		public static int PlayerItemCount(TSPlayer plr, Item item)
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

		public static void PlayerRemoveItems(TSPlayer plr, Item item, int amount)
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

		public static void GiveItemWithoutSpawn(TSPlayer plr, Item item, int amount)
		{
			if (item.maxStack > 1)
			{
				for (int i = 0; i < plr.TPlayer.inventory.Count(); i++)
				{
					if (plr.TPlayer.inventory[i].stack != plr.TPlayer.inventory[i].maxStack && plr.TPlayer.inventory[i].type == item.type)
					{
						int num = plr.TPlayer.inventory[i].maxStack - plr.TPlayer.inventory[i].stack;
						if (amount <= num)
						{
							plr.TPlayer.inventory[i].stack += amount;
							NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[i].Name), plr.Index, i, (int)plr.TPlayer.inventory[i].prefix);
							NetMessage.SendData(5, plr.Index, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[i].Name), plr.Index, i, (int)plr.TPlayer.inventory[i].prefix);
							return;
						}
						amount -= num;
						plr.TPlayer.inventory[i].stack += num;
						NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[i].Name), plr.Index, i, (int)plr.TPlayer.inventory[i].prefix);
						NetMessage.SendData(5, plr.Index, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[i].Name), plr.Index, i, (int)plr.TPlayer.inventory[i].prefix);
					}
				}
			}
			int num2 = 0;
			while (true)
			{
				if (num2 >= plr.TPlayer.inventory.Count())
				{
					return;
				}
				if (plr.TPlayer.inventory[num2].type == 0)
				{
					if (amount < item.maxStack)
					{
						break;
					}
					item.stack = item.maxStack;
					plr.TPlayer.inventory[num2] = item;
					amount -= item.maxStack;
					NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[num2].Name), plr.Index, num2, (int)plr.TPlayer.inventory[num2].prefix);
					NetMessage.SendData(5, plr.Index, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[num2].Name), plr.Index, num2, (int)plr.TPlayer.inventory[num2].prefix);
				}
				num2++;
			}
			item.stack = amount;
			plr.TPlayer.inventory[num2] = item;
			NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[num2].Name), plr.Index, num2, (int)plr.TPlayer.inventory[num2].prefix);
			NetMessage.SendData(5, plr.Index, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[num2].Name), plr.Index, num2, (int)plr.TPlayer.inventory[num2].prefix);
		}

		public static void SetItemInventory(TSPlayer plr, Item item, short slot)
		{
			plr.TPlayer.inventory[slot] = item;
			
			NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[slot].Name), plr.Index, slot, (int)plr.TPlayer.inventory[slot].prefix);
			NetMessage.SendData(5, plr.Index, -1, NetworkText.FromLiteral(plr.TPlayer.inventory[slot].Name), plr.Index, slot, (int)plr.TPlayer.inventory[slot].prefix);
		}

		public static bool IsNpcOnWorld(int type)
		{
			NPC[] npc = Main.npc;
			foreach (NPC nPc in npc)
			{
				if (nPc.type == type)
				{
					return true;
				}
			}
			return false;
		}

		public static string GetGroupColor(string group)
		{
			string result = "cccccc";
			if (group == "Iwobos")
			{
				result = "27AD34";
			}
			else if (group == "Xedlefix")
			{
				result = "18B690";
			}
			else if (group == "GlobalMod")
			{
				result = "ff3333";
			}
			else if (group == "Moderator")
			{
				result = "D3B62B";
			}
			else if (group == "JuniorMod")
			{
				result = "3498DB";
			}
			else if (group == "Special")
			{
				result = "fd7c24";
			}
			else if (group == "Gracz++")
			{
				result = "FFD700";
			}
			else if (group == "Gracz+")
			{
				result = "AEC1C2";
			}
			else if (group == "Nowy")
			{
				result = "999999";
			}
			return result;
		}

		public static int GetGroupItem(string itemm)
		{
			int result = 1;
			if (itemm == "Iwobos")
			{
				result = 3925;
			}
			else if (itemm == "Xedlefix")
			{
				result = 3570;
			}
			else if (itemm == "GlobalMod")
			{
				result = 1996;
			}
			else if (itemm == "Moderator")
			{
				result = 1336;
			}
			else if (itemm == "JuniorMod")
			{
				result = 2219;
			}
			else if (itemm == "Special")
			{
				result = 122;
			}
			else if (itemm == "Gracz++")
			{
				result = 3521;
			}
			else if (itemm == "Gracz+")
			{
				result = 3515;
			}
			else if (itemm == "Nowy")
			{
				result = 8;
			}
			return result;
		}

		public static string GetShortPrefix(string group)
		{
			switch (group)
			{
				case "Xedlefix":
					return "[c/595959:;][c/" + Utils.GetGroupColor(group) + ":Zalozyciel][c/595959:;] ";
				case "Iwobos":
					return "[c/595959:;][c/" + Utils.GetGroupColor(group) + ":Zalozyciel][c/595959:;] ";
				case "GlobalMod":
					return "[c/595959:;][c/" + Utils.GetGroupColor(group) + ":GlobalMod][c/595959:;] ";
				case "Moderator":
					return "[c/595959:;][c/" + Utils.GetGroupColor(group) + ":Mod][c/595959:;] ";
				case "JuniorMod":
					return "[c/595959:;][c/" + Utils.GetGroupColor(group) + ":JuniorMod][c/595959:;] ";
				case "Special":
					return "[c/595959:;][c/" + Utils.GetGroupColor(group) + ":Special][c/595959:;] ";
				default:
					return null;
			}
		}
	}

}