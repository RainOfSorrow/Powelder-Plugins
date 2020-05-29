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
			return (item.Name + prefix.ToString()).Length;
		}
	}
}
