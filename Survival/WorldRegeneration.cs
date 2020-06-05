using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace SurvivalCore
{
	internal class WorldRegeneration
	{
		public static void DoTrees()
		{
			for (int i = 0; (double)i < (double)Main.maxTilesX * 0.001; i++)
			{
				int num = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
				int num2 = WorldGen.genRand.Next(25, 50);
				for (int j = num - num2; j < num + num2; j++)
				{
					for (int k = 20; (double)k < Main.worldSurface; k++)
					{
						if (!IsProtected(num, num2))
						{
							WorldGen.GrowEpicTree(j, k);
						}
					}
				}
			}
			WorldGen.AddTrees();
			TSPlayer.All.SendMessage("[i:27] [c/595959:⮘] [c/996633:Drzewa] [c/595959:⮚] Wygenerowano nowe drzewa.", new Color(210, 166, 121));
		}

		public static void DoOres(string name, float oreAmts)
		{
			ushort num = 0;
			if (name == "cobalt")
			{
				num = 107;
			}
			else if (name == "mythril")
			{
				num = 108;
			}
			else if (name == "copper")
			{
				num = 7;
			}
			else if (name == "iron")
			{
				num = 6;
			}
			else if (name == "silver")
			{
				num = 9;
			}
			else if (name == "gold")
			{
				num = 8;
			}
			else if (name == "demonite")
			{
				num = 22;
			}
			else if (name == "sapphire")
			{
				num = 63;
			}
			else if (name == "ruby")
			{
				num = 64;
			}
			else if (name == "emerald")
			{
				num = 65;
			}
			else if (name == "topaz")
			{
				num = 66;
			}
			else if (name == "amethyst")
			{
				num = 67;
			}
			else if (name == "diamond")
			{
				num = 68;
			}
			else if (name == "adamantite")
			{
				num = 111;
			}
			else if (name == "hellstone")
			{
				num = 58;
			}
			else if (name == "meteorite")
			{
				num = 37;
			}
			else if (name == "tin")
			{
				num = 166;
			}
			else if (name == "lead")
			{
				num = 167;
			}
			else if (name == "tungsten")
			{
				num = 168;
			}
			else if (name == "platinum")
			{
				num = 169;
			}
			else if (name == "crimtane")
			{
				num = 204;
			}
			else if (name == "palladium")
			{
				num = 221;
			}
			else if (name == "orichalcum")
			{
				num = 222;
			}
			else if (name == "titanium")
			{
				num = 223;
			}
			else if (name == "chlorophyte")
			{
				num = 211;
			}
			else if (name == "dirt")
			{
				num = 0;
			}
			else if (name == "stone")
			{
				num = 1;
			}
			else if (name == "sand")
			{
				num = 53;
			}
			else
			{
				if (!(name == "silt"))
				{
					return;
				}
				num = 123;
			}
			int num2 = 0;
			while ((float)num2 < oreAmts)
			{
				int num3 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
				double worldSurface = Main.worldSurface;
				int minValue;
				int minValue2;
				int maxValue;
				int maxValue2;
				if (num == 111 || num == 22 || num == 204 || num == 211 || num == 223 || (num >= 63 && num <= 68))
				{
					worldSurface = (Main.rockLayer + Main.rockLayer + (double)Main.maxTilesY) / 3.0;
					minValue = 2;
					minValue2 = 2;
					maxValue = 3;
					maxValue2 = 3;
				}
				else if (num == 58)
				{
					worldSurface = Main.maxTilesY - 200;
					minValue = 4;
					minValue2 = 4;
					maxValue = 9;
					maxValue2 = 9;
				}
				else
				{
					worldSurface = Main.rockLayer;
					minValue = 5;
					minValue2 = 9;
					maxValue = 5;
					maxValue2 = 9;
				}
				int num4 = WorldGen.genRand.Next((int)worldSurface, Main.maxTilesY - 150);
				if (!IsProtected(num3, num4))
				{
					WorldGen.OreRunner(num3, num4, WorldGen.genRand.Next(minValue2, maxValue2), WorldGen.genRand.Next(minValue, maxValue), num);
					num2++;
				}
			}
		}

		private static bool IsProtected(int x, int y)
		{
			IEnumerable<Region> enumerable = TShock.Regions.InAreaRegion(x, y);
			foreach (Region item in enumerable)
			{
				if (item.DisableBuild)
				{
					return true;
				}
			}
			return false;
		}
	}
}
