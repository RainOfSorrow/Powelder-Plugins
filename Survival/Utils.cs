using System;
using System.Linq;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace SurvivalCore
{
	public class Utils
	{
		public static bool BossCondition(int type)
		{

			return true;
		}

		public static int CostCalc(TSPlayer who, int cost)
		{
			if (who.HasPermission("isGracz+"))
			{
				cost = (int)(cost * 0.8);
			}
			else if (who.HasPermission("isGracz++"))
			{
				cost = (int)(cost * 0.6);
			}
			return cost;
		}
	}
}
