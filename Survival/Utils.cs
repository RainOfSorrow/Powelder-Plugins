using System;
using System.Linq;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace SurvivalCore
{
	public class Utils
	{
		public static bool IsAvalible(int type)
		{
			if (!NPC.downedMoonlord)
			{
			}
			if (!NPC.downedAncientCultist)
			{
			}
			if (!NPC.downedChristmasIceQueen)
			{
			}
			if (!NPC.downedChristmasSantank)
			{
			}
			if (!NPC.downedChristmasTree)
			{
			}
			if (!NPC.downedHalloweenKing)
			{
			}
			if (!NPC.downedHalloweenTree)
			{
			}
			if (!NPC.downedFishron)
			{
			}
			if (!NPC.downedGolemBoss)
			{
			}
			if (!NPC.downedPlantBoss)
			{
			}
			if (!NPC.downedPirates)
			{
			}
			if (!NPC.downedFrost)
			{
			}
			if (!NPC.downedMechBossAny)
			{
			}
			if (!NPC.downedMechBoss3)
			{
			}
			if (!NPC.downedMechBoss2)
			{
			}
			if (!NPC.downedMechBoss1)
			{
			}
			if (!Main.hardMode)
			{
			}
			if (!NPC.downedGoblins)
			{
			}
			if (!NPC.downedQueenBee)
			{
			}
			if (!NPC.downedBoss3)
			{
			}
			if (!NPC.downedBoss2)
			{
			}
			if (!NPC.downedBoss1)
			{
				if (type == 1299)
				{
					return false;
				}
			}
			if (!NPC.downedSlimeKing)
			{
				if (type == 240 || type == 2585 || type == 2610)
				{
					return false;
				}
			}
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
			else if (who.HasPermission("isSpecial"))
			{
				cost = (int)(cost * 0.5);
			}
			return cost;
		}
	}
}
