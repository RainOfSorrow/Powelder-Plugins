using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using TShockAPI;
using System.Runtime.InteropServices;

namespace SurvivalCore
{
	public class Npi
	{
		public static bool PasswordSend(BinaryReader read, TSPlayer who)
		{
			string text = read.ReadString();
			return false;
		}

		public static bool ItemOwner(BinaryReader read, TSPlayer who)
		{
			short num = read.ReadInt16();
			read.ReadByte();
			if (num == 400)
			{
				try
				{
					SurvivalCore.Stoper[(byte)who.Index].Stop();
				}
				catch (KeyNotFoundException)
				{
					return false;
				}

				if (!SurvivalCore.PingMeasure.ContainsKey((byte)who.Index))
					SurvivalCore.PingMeasure.Add((byte)who.Index, new int[5]);

				SurvivalCore.PingMeasure[(byte)who.Index][SurvivalCore.TickPing] = (int)SurvivalCore.Stoper[(byte)who.Index].Elapsed.TotalMilliseconds;

				if (!Array.Exists(SurvivalCore.PingMeasure[(byte)who.Index], x => x == 0))
				{
					Array.Sort(SurvivalCore.PingMeasure[(byte)who.Index]);

					SurvivalCore.Ping[(byte)who.Index] = SurvivalCore.PingMeasure[(byte)who.Index][2];
				}

				SurvivalCore.Stoper.Remove((byte)who.Index);
			}
			return false;
		}

		public static bool PlayerInfo(BinaryReader read, TSPlayer who)
		{
			read.ReadByte();
			read.ReadByte();
			read.ReadByte();
			read.ReadString();
			return false;
		}

		public static bool TogglePvP(BinaryReader read, TSPlayer who)
		{
			read.ReadByte();
			bool flag = read.ReadBoolean();
			who.TPlayer.hostile = flag;
			who.SendData(PacketTypes.TogglePvp, "", who.Index);
			if (flag)
			{
				TSPlayer[] players = TShock.Players;
				foreach (TSPlayer tSPlayer in players)
				{
					if (tSPlayer != null)
					{
						tSPlayer.SendData(PacketTypes.TogglePvp, "", who.Index);
						if (tSPlayer.TPlayer.hostile)
						{
							tSPlayer.SendMessage("[c/595959:»]  [c/66ff66:" + who.Name + "] dolacza do trybu PVP.", Color.Gray);
						}
					}
				}
			}
			else
			{
				TSPlayer[] players2 = TShock.Players;
				foreach (TSPlayer tSPlayer2 in players2)
				{
					if (tSPlayer2 != null)
					{
						tSPlayer2.SendData(PacketTypes.TogglePvp, "", who.Index);
						if (tSPlayer2.TPlayer.hostile)
						{
							tSPlayer2.SendMessage("[c/595959:»]  [c/66ff66:" + who.Name + "] wychodzi z trybu PVP.", Color.Gray);
						}
					}
				}
				who.SendMessage("[c/595959:»]  Wychodzisz z trybu PVP.", Color.Gray);
			}
			return true;
		}

		public static bool PlayerTeam(BinaryReader read, TSPlayer who)
		{
			read.ReadByte();
			byte b = read.ReadByte();
			string text;
			switch (b)
			{
			default:
				text = "ffffff";
				who.SendMessage("[c/595959:»]  Przestajesz byc w jakiejkolwiek druzynie.", Color.Gray);
				break;
			case 1:
				text = "C33434";
				who.SendMessage("[c/595959:»]  Dolaczasz do [c/" + text + ":Czerwonej] druzyny.", Color.Gray);
				break;
			case 2:
				text = "34C34C";
				who.SendMessage("[c/595959:»]  Dolaczasz do [c/" + text + ":Zielonej] druzyny.", Color.Gray);
				break;
			case 3:
				text = "3485C3";
				who.SendMessage("[c/595959:»]  Dolaczasz do [c/" + text + ":Niebieskiej] druzyny.", Color.Gray);
				break;
			case 4:
				text = "D9C659";
				who.SendMessage("[c/595959:»]  Dolaczasz do [c/" + text + ":Zoltej] druzyny.", Color.Gray);
				break;
			case 5:
				text = "C959D9";
				who.SendMessage("[c/595959:»]  Dolaczasz do [c/" + text + ":Rozowej] druzyny.", Color.Gray);
				break;
			}
			string text2;
			switch (who.Team)
			{
			default:
				text2 = "ffffff";
				break;
			case 1:
				text2 = "C33434";
				break;
			case 2:
				text2 = "34C34C";
				break;
			case 3:
				text2 = "3485C3";
				break;
			case 4:
				text2 = "D9C659";
				break;
			case 5:
				text2 = "C959D9";
				break;
			}
			if (b != 0)
			{
				TSPlayer[] players = TShock.Players;
				foreach (TSPlayer tSPlayer in players)
				{
					if (tSPlayer != null && tSPlayer.Team == b)
					{
						tSPlayer.SendMessage("[c/595959:»]  [c/" + text + ":" + who.Name + "] dolaczyl do waszej druzyny.", Color.Gray);
					}
				}
			}
			TSPlayer[] players2 = TShock.Players;
			foreach (TSPlayer tSPlayer2 in players2)
			{
				if (tSPlayer2 != null && tSPlayer2.Team == who.Team && tSPlayer2 != who && tSPlayer2.Team != 0)
				{
					tSPlayer2.SendMessage("[c/595959:»]  [c/" + text2 + ":" + who.Name + "] odszedl z waszej druzyny.", Color.Gray);
				}
			}
			Main.player[who.Index].team = b;
			NetMessage.SendData(45, -1, -1, NetworkText.Empty, who.Index);
			NetMessage.SendData(45, -1, who.Index, NetworkText.Empty, who.Index);
			return true;
		}

		public static bool UpdatePlayer(BinaryReader read, TSPlayer who)
		{
			byte b = read.ReadByte();
			byte b2 = read.ReadByte();
			byte b3 = read.ReadByte();
			byte b4 = read.ReadByte();
			return false;
		}

		public static bool PlayMusic(BinaryReader read, TSPlayer who)
		{
			byte b = read.ReadByte();
			float num = read.ReadSingle();
			return false;
		}

		public static bool ItemDrop(BinaryReader read, TSPlayer who)
		{
			short num = read.ReadInt16();
			read.ReadSingle();
			read.ReadSingle();
			read.ReadSingle();
			read.ReadSingle();
			read.ReadInt16();
			read.ReadByte();
			read.ReadByte();
			short num2 = read.ReadInt16();
			if (num2 == 267 && num == 400)
			{
				PowelderAPI.Utils.GiveItemWithoutSpawn(who, TShock.Utils.GetItemById(num2), 1);
				return true;
			}
			return false;
		}

		public static bool SpecialNpcEffect(BinaryReader read, TSPlayer who)
		{
			byte b = read.ReadByte();
			byte b2 = read.ReadByte();
			if (b2 == 1)
			{
				int cost = 1100;
				cost = Utils.CostCalc(who, cost);
				if (SurvivalCore.SrvPlayers[b].Money < cost)
				{
					who.SendErrorMessage("[c/595959:»]  Nie stac cie na przywolanie Skeletrona. [c/595959:(]Koszt {0} €[c/595959:)]", cost);
					return true;
				}
				SurvivalCore.SrvPlayers[b].Money -= cost;
				TSPlayer.All.SendInfoMessage("[c/595959:»]  {0} przywolal Skeletrona.", who.Name);
			}
			return false;
		}

		public static bool BossOrInvasionStart(BinaryReader read)
		{
			TSPlayer tSPlayer = TShock.Players[read.ReadInt16()];
			int npcid = read.ReadInt16();
			if ((DateTime.UtcNow - tSPlayer.LastThreat).TotalMilliseconds < 5000.0)
			{
				return true;
			}
			bool isBoss = false;
			int cost = 0;
			NPC nPc = new NPC();
			nPc.SetDefaults(npcid);
			isBoss = nPc.boss;
			if (!isBoss)
			{
				switch (npcid)
				{
				case 4:
				case 13:
				case 50:
				case 75:
				case 125:
				case 126:
				case 127:
				case 128:
				case 129:
				case 130:
				case 131:
				case 134:
				case 222:
				case 245:
				case 266:
				case 370:
				case 398:
				case 422:
				case 439:
				case 493:
				case 507:
				case 517:
				case 657:
					isBoss = true;
					break;
				}
			}
			switch (npcid)
			{
			case -1:
				cost = 500;
				break;
			case -2:
				cost = 2200;
				break;
			case -3:
				cost = 2000;
				break;
			case -4:
				cost = 4500;
				break;
			case -5:
				cost = 4500;
				break;
			case -6:
				cost = 2500;
				break;
			case -7:
				cost = 5500;
				break;
			case -8:
				cost = 6000;
				break;
			case -10:
				cost = 500;
				break;;
			case 4:
				cost = 300;
				break;
			case 13:
				cost = 400;
				break;
			case 50:
				cost = 200;
				break;
			case 75:
				cost = 15000;
				break;
			case 125:
			case 126:
			case 127:
			case 128:
			case 129:
			case 130:
			case 131:
			case 134:
				cost = 2000;
				break;
			case 222:
				cost = 800;
				break;
			case 245:
				cost = 4200;
				break;
			case 266:
				cost = 300;
				break;
			case 370:
				cost = 4200;
				break;
			case 422:
			case 439:
			case 493:
			case 507:
			case 517:
				cost = 5000;
				break;
			case 398:
				cost = 6000;
				break;
			case 657: //Queen slime
				cost = 300;
				break;
			}

			if (cost > 0)
			{
				cost = Utils.CostCalc(tSPlayer, cost);
				if (SurvivalCore.SrvPlayers[tSPlayer.Index].Money < cost)
				{
					tSPlayer.SendErrorMessage("[c/595959:»]  Nie stac cie na {0}. [c/595959:(]Koszt {1} €[c/595959:)]",
						isBoss ? ("przywolanie " + nPc.FullName) : ("rozpoczecie " + GetInvasion(npcid)), cost);
					if (SpawnItemId(npcid) != -1)
					{
						PowelderAPI.Utils.GiveItemWithoutSpawn(tSPlayer, TShock.Utils.GetItemById(SpawnItemId(npcid)),
							1);
					}

					return true;
				}

				SurvivalCore.SrvPlayers[tSPlayer.Index].Money -= cost;
				TSPlayer.All.SendInfoMessage("[c/595959:»]  {0} {1}.", tSPlayer.Name,
					isBoss ? ("przywolal " + nPc.FullName) : ("rozpoczal " + GetInvasion(npcid)));
			}

			return false;
		}

		public static bool Tile(BinaryReader read, TSPlayer who)
		{
			byte b = read.ReadByte();
			short x = read.ReadInt16();
			short y = read.ReadInt16();
			ushort num3 = read.ReadUInt16();
			if (who == null)
			{
				return false;
			}
			if (TShock.Regions.CanBuild(x, y, who))
			{
				return false;
			}
			if (b == 0)
			{
				switch (Main.tile[x, y].type)
				{
				case 0:
				case 1:
				case 40:
				case 59:
				case 123:
					if ((double)y > Main.worldSurface - 150.0)
					{
						Random random = new Random();
						if (random.Next(0, 1550) <= 2)
						{
							int num4 = random.Next(25, 75);
							SurvivalCore.SrvPlayers[who.Index].Money += num4;
							who.SendMessage($"[c/595959:»]  Znalazles mieszek, a w nim [c/66ff66:{num4}] €.", Color.Gray);
						}
					}
					break;
				case 238:
				{
					int cost = 3300;
					cost = Utils.CostCalc(who, cost);
					if (SurvivalCore.SrvPlayers[who.Index].Money < cost)
					{
						who.SendErrorMessage("[c/595959:»]  Nie stac cie na przywolanie Plantery. [c/595959:(]Koszt {0} €[c/595959:)]", cost);
						TSPlayer.All.SendTileSquare(x, y);
						return true;
					}
					SurvivalCore.SrvPlayers[who.Index].Money -= cost;
					TSPlayer.All.SendInfoMessage("[c/595959:»]  {0} przywolal Plantere.", who.Name);
					break;
				}
				}
			}
			return false;
		}

		public static bool ItemFrame(BinaryReader read, TSPlayer who)
		{
			read.ReadInt16();
			read.ReadInt16();
			int num = read.ReadInt16();
			if (num == 267)
			{
				PowelderAPI.Utils.GiveItemWithoutSpawn(who, TShock.Utils.GetItemById(267), 1);
				who.SendErrorMessage("[c/595959:»]  Nie mozna wlozyc Guide Voodoo Doll do item frame.");
				return true;
			}
			return false;
		}

		public static bool StrikeNpc(BinaryReader read, TSPlayer who)
		{
			short npcid = read.ReadInt16();
			short damage = read.ReadInt16();
			float knockback = read.ReadSingle();
			read.ReadByte();
			bool crit = read.ReadBoolean();
			NPC npc = Main.npc[npcid];
			double endDamage = Main.CalculateDamageNPCsTake(damage, npc.defense);
			if (crit)
				endDamage *= 2.0;

			bool isDead = false || npc.life - endDamage <= 0;
			if (npc.type == 66 && isDead)
			{
				Main.npc[npcid].active = false;
				PowelderAPI.Utils.GiveItemWithoutSpawn(who, TShock.Utils.GetItemById(267), 1);
			}
			else if (npc.type == 661)
			{
				if (Main.dayTime)
				{
					who.SendErrorMessage("[c/595959:»]  Wolimy uniknąć masowej rzeźni.");
					return true;
				}
				
				int cost = 4300;
				cost = Utils.CostCalc(who, cost);
				if (SurvivalCore.SrvPlayers[who.Index].Money < cost)
				{
					who.SendErrorMessage("[c/595959:»]  Nie stac cie na przywolanie Empress of Light. [c/595959:(]Koszt {0} €[c/595959:)]", cost);
					return true;
				}
				SurvivalCore.SrvPlayers[who.Index].Money -= cost;
				TSPlayer.All.SendInfoMessage("[c/595959:»]  {0} przywolal Empress of Light.", who.Name);
			}
			return false;
		}

		public static bool PlayerDeath(BinaryReader read, TSPlayer who)
		{
			byte b = read.ReadByte();
			PlayerDeathReason playerDeathReason = PlayerDeathReason.FromReader(read);
			short num = read.ReadInt16();
			byte direction = (byte)(read.ReadByte() - 1);
			bool flag = ((BitsByte)read.ReadByte())[0];
			if (num > 20000)
			{
				who.Kick("Ogarnij sie i nie cwaniacz /shrug \n\n- Xedlefix", true);
				TShock.Log.ConsoleError("Death Exploit Attempt: Damage {0}", num);
				return false;
			}
			if (b >= byte.MaxValue)
			{
				return true;
			}
			if (OnKillMe(b, direction, num, flag))
			{
				return true;
			}
			if (playerDeathReason.GetDeathText(TShock.Players[b].Name).ToString().Length > 500)
			{
				who.Kick("Ogarnij sie i nie cwaniacz /shrug \n\n- Xedlefix", true);
				return true;
			}
			who.Dead = true;
			who.RespawnTimer = TShock.Config.RespawnSeconds;
			Main.player[who.Index].respawnTimer = TShock.Config.RespawnSeconds;
			//if (who.TpCoords != Point.Zero)
			//{
			//	who.SendInfoMessage("[c/595959:»]  W wyniku smierci twoja teleportacja zostala anulowana.");
			//	who.TpCoords = Point.Zero;
			//	who.warpWait = 0;
			//}
			NPC[] npc = Main.npc;
			foreach (NPC nPc in npc)
			{
				if (nPc.active && (nPc.boss || nPc.type == 13 || nPc.type == 14 || nPc.type == 15) && Math.Abs(who.TPlayer.Center.X - nPc.Center.X) + Math.Abs(who.TPlayer.Center.Y - nPc.Center.Y) < 4000f)
				{
					who.RespawnTimer = TShock.Config.RespawnBossSeconds;
					Main.player[who.Index].respawnTimer = TShock.Config.RespawnSeconds;
					break;
				}
			}
			if (who.TPlayer.difficulty == 2 && (TShock.Config.KickOnHardcoreDeath || TShock.Config.BanOnHardcoreDeath))
			{
				if (TShock.Config.BanOnHardcoreDeath)
				{
					if (who.Ban(TShock.Config.HardcoreBanReason, false, "hardcore-death"))
					{
						who.Ban("Death results in a ban, but you are immune to bans.", true);
					}
				}
				else
				{
					who.Kick(TShock.Config.HardcoreKickReason, true);
				}
			}
			if (who.TPlayer.difficulty == 2 && Main.ServerSideCharacter && who.IsLoggedIn && TShock.CharacterDB.RemovePlayer(who.Account.ID))
			{
				TShock.CharacterDB.SeedInitialData(who.Account);
			}
			if (who.HasPermission("server.admin"))
			{
				Main.player[who.Index].respawnTimer = 1;
				who.RespawnTimer = 1;
				who.TPlayer.respawnTimer = 1;
			}
			who.TPlayer.dead = true;
			Main.player[who.Index].dead = true;
			NetMessage.SendPlayerDeath(who.Index, PlayerDeathReason.LegacyEmpty(), num, direction, flag);
			NetMessage.SendPlayerDeath(who.Index, PlayerDeathReason.LegacyEmpty(), num, direction, flag, who.Index);
			int num2 = 1173;
			if (SurvivalCore.SrvPlayers[who.Index].Money >= 22500)
			{
				num2 = 3231;
			}
			else if (SurvivalCore.SrvPlayers[who.Index].Money >= 7500)
			{
				num2 = 3229;
			}
			else if (SurvivalCore.SrvPlayers[who.Index].Money >= 2500)
			{
				num2 = 1175;
			}
			if (flag)
			{
				TSPlayer[] players = TShock.Players;
				foreach (TSPlayer tSPlayer in players)
				{
					if (tSPlayer?.TPlayer.hostile ?? false)
					{
						tSPlayer.SendMessage($"[c/66ff66:{TShock.Players[playerDeathReason._sourcePlayerIndex].Name}] [i/p{playerDeathReason._sourceItemPrefix}:{playerDeathReason._sourceItemType}] [c/595959:→] [c/ff6666:{who.Name}] [i:{num2}]", Color.SlateGray);
					}
				}
				SurvivalCore.SrvPlayers[who.Index].PvpDeaths++;
				SurvivalCore.SrvPlayers[playerDeathReason._sourcePlayerIndex].PvpKills++;
			}
			else
			{
				TSPlayer[] players2 = TShock.Players;
				foreach (TSPlayer tSPlayer2 in players2)
				{
					if (tSPlayer2 != null && SurvivalCore.IsDeathMessage[tSPlayer2.Index])
					{
						tSPlayer2.SendMessage(string.Format("[i:{0}] {1} ", num2, playerDeathReason.GetDeathText("[c/ff6666:" + who.Name + "]")), Color.Gray);
					}
				}
				SurvivalCore.SrvPlayers[who.Index].Deaths++;
			}
			return true;
		}

		protected static string GetInvasion(int type)
		{
			switch (type)
			{
			case -1:
				return "Goblin Invasion";
			case -2:
				return "Snow Legion";
			case -3:
				return "Pirate Invasion";
			case -4:
				return "Pumpkin Moon";
			case -5:
				return "Frost Moon";
			case -6:
				return "Solar Eclipse";
			case -7:
				return "Martian Invasion";
			case -8:
				return "Moon Lord";
			case -10:
				return "Blood Moon";
			default:
				return "null";
			}
		}

		protected static int SpawnItemId(int type)
		{
			switch (type)
			{
			case -1:
				return 361;
			case -2:
				return 602;
			case -3:
				return 1315;
			case -4:
				return 1844;
			case -5:
				return 1958;
			case -6:
				return 2767;
			case -8:
				return 3601;
			case -10:
				return 4271;
			case 4:
				return 43;
			case 13:
				return 70;
			case 50:
				return 560;
			case 125:
			case 126:
				return 544;
			case 127:
			case 128:
			case 129:
			case 130:
			case 131:
				return 557;
			case 134:
				return 556;
			case 222:
				return 1133;
			case 245:
				return 1293;
			case 266:
				return 1331;
			case 370:
				return 2673;
			case 398:
				return 3601;
			case 657:
				return 4988;
			default:
				return 0;
			}
		}

		protected static bool OnKillMe(byte plr, byte direction, short damage, bool pvp)
		{
			if (GetDataHandlers.KillMe == null)
			{
				return false;
			}
			GetDataHandlers.KillMeEventArgs killMeEventArgs = new GetDataHandlers.KillMeEventArgs
			{
				PlayerId = plr,
				Direction = direction,
				Damage = damage,
				Pvp = pvp
			};
			GetDataHandlers.KillMe.Invoke(null, killMeEventArgs);
			return killMeEventArgs.Handled;
		}
	}
}
