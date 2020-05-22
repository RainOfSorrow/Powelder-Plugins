using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace SurvivalCore.Economy.Shop
{
	public class Reforge
	{
		public static void ReforgeCost(CommandArgs args)
		{
			args.Player.SendMessage(CalcReforge(args.Player.SelectedItem, 0).ToString(), Color.Khaki);
		}

		private static int CalcReforge(Item item, byte prefix)
		{
			int storeValue = item.GetStoreValue();
			float num = 0f;
			byte b = prefix;
			byte b2 = b;
			if (b2 != 1)
			{
				num = 0f;
			}
			else
			{
				num = 0.25f;
			}
			return storeValue;
		}
	}
}
