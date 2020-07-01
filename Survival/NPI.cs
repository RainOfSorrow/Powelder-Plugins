using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using TShockAPI;
using System.Runtime.InteropServices;
using PowelderAPI;

namespace SurvivalCore
{
	public class Npi
	{

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

					

					if (SurvivalCore.PingMeasure[(byte) who.Index].Length > 2)
					{
						string color = "ffffff";

						if (SurvivalCore.PingMeasure[(byte) who.Index][2] > 120)
							color = "ff0000";
						else if (SurvivalCore.PingMeasure[(byte) who.Index][2] > 65)
							color = "ffff00";
						
						SurvivalCore.Ping[(byte) who.Index] =
							$"[c/{color}:{SurvivalCore.PingMeasure[(byte) who.Index][2]}ms]";
					}
				}

				SurvivalCore.Stoper.Remove((byte)who.Index);
			}
			return false;
		}
		
		
		public static bool SpecialNpcEffect(BinaryReader read, TSPlayer who)
		{
			byte b = read.ReadByte();
			byte b2 = read.ReadByte();
			if (b2 == 1)
			{

				if (!Goals.IsDone(35))
				{
					who.SendErrorMessage("Ten boss w tej chwili jest zablokowany.");
					return true;
				}
				
				if ((SurvivalCore.SrvPlayers[who.Index].BossCooldown - DateTime.Now).TotalSeconds > 0)
				{
					who.SendErrorMessage($"Musisz odczekac jakis czas, aby moc zrespic kolejnego bossa. Mozliwe to bedzie za {PowelderAPI.Utils.ExpireCountDown(SurvivalCore.SrvPlayers[who.Index].BossCooldown)}");
					return true;
				}
				
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

			bool isBoss;
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
					cost = 1300;
					break;
				case -2:
					cost = 2650;
					break;
				case -3:
					cost = 2000;
					break;
				case -4:
					cost = 3100;
					break;
				case -5:
					cost = 4200;
					break;
				case -6:
					cost = 2700;
					break;
				case -7:
					cost = 15000;
					break;
				case -8:
					cost = 3000;
					break;
				case -10:
					cost = 500;
					break;
			}

			
			int bossId = (npcid == 266 || npcid == 13 ? 6969 : npcid);

			if (npcid == 126 || npcid == 125 || npcid == 134 || npcid == 127 || npcid == 128 || npcid == 129 ||
			    npcid == 130 || npcid == 131)
			{
				bossId = 42069;
			}

			if (!Goals.IsDone(bossId))
			{
				tSPlayer.SendErrorMessage("Ten boss w tej chwili jest zablokowany.");
				if (SpawnItemId(npcid) != -1)
				{
					tSPlayer.GiveItem(SpawnItemId(npcid), 1);
				}

				return true;
			}


			if ((SurvivalCore.SrvPlayers[tSPlayer.Index].BossCooldown - DateTime.Now).TotalSeconds > 0)
			{
				tSPlayer.SendErrorMessage(
					$"Musisz odczekac jakis czas, aby moc zrespic kolejnego bossa/inwazje. Mozliwe to bedzie za {PowelderAPI.Utils.ExpireCountDown(SurvivalCore.SrvPlayers[tSPlayer.Index].BossCooldown)}");
				return true;
			}

			if (cost > 0)
			{
				cost = Utils.CostCalc(tSPlayer, cost);
				if (SurvivalCore.SrvPlayers[tSPlayer.Index].Money < cost)
				{
					tSPlayer.SendErrorMessage("Nie stac cie na {0}. Koszt {1} €",
						isBoss ? ("przywolanie " + nPc.FullName) : ("rozpoczecie " + GetInvasion(npcid)), cost);
					if (SpawnItemId(npcid) != -1)
					{
						tSPlayer.GiveItem(SpawnItemId(npcid), 1);
					}

					return true;
				}

				SurvivalCore.SrvPlayers[tSPlayer.Index].Money -= cost;
			}


			SurvivalCore.SrvPlayers[tSPlayer.Index].BossCooldown = DateTime.Now.AddMinutes(30);


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
						if (y > Main.worldSurface - 150.0)
						{
							Random random = new Random();
							if (random.Next(0, 1550) <= 2)
							{
								int num4 = random.Next(35, 85);
								SurvivalCore.SrvPlayers[who.Index].Money += num4;
								who.SendInfoMessage($"Znalazles mieszek, a w nim {num4} €.");
							}
						}

						break;
					case 238:
					{
						if (!Goals.IsDone(262))
						{
							who.SendErrorMessage("Ten boss w tej chwili jest zablokowany.");
							who.SendTileSquare(x, y);
							return true;
						}
						
						if ((SurvivalCore.SrvPlayers[who.Index].BossCooldown - DateTime.Now).TotalSeconds > 0)
						{
							who.SendErrorMessage($"Musisz odczekac jakis czas, aby moc zrespic kolejnego bossa/inwazje. Mozliwe to bedzie za {PowelderAPI.Utils.ExpireCountDown(SurvivalCore.SrvPlayers[who.Index].BossCooldown)}");
							who.SendTileSquare(x, y);
							return true;
						}
						
						break;
					}
				}
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
			if (npc.type == 661)
			{
				if (Main.dayTime)
				{
					who.SendErrorMessage("Wolimy uniknac masowej rzezni.");
					return true;
				}
				
				if (!Goals.IsDone(636))
				{
					who.SendErrorMessage("Ten boss w tej chwili jest zablokowany.");
					return true;
				}
				
				
				if ((SurvivalCore.SrvPlayers[who.Index].BossCooldown - DateTime.Now).TotalSeconds > 0)
				{
					who.GiveItem(SpawnItemId(npcid), 1);
					who.SendErrorMessage($"Musisz odczekac jakis czas, aby moc zrespic kolejnego bossa. Mozliwe to bedzie za {PowelderAPI.Utils.ExpireCountDown(SurvivalCore.SrvPlayers[who.Index].BossCooldown)}");
					return true;
				}

			}
			return false;
		}

		//Classic Tshock's death implementation.
		//Death with no message on chat or if you want to know who died, you can turn on messages.
		//Slightly modified death messages.
		public static void PlayerDeath(object sender, GetDataHandlers.KillMeEventArgs args)
		{
			if (args.Handled)
				return;
			
			args.Player.Dead = true;
			args.Player.RespawnTimer = TShock.Config.RespawnSeconds;

			foreach (NPC npc in Main.npc)
			{
				if (npc.active && (npc.boss || npc.type == 13 || npc.type == 14 || npc.type == 15) &&
					Math.Abs(args.Player.TPlayer.Center.X - npc.Center.X) + Math.Abs(args.Player.TPlayer.Center.Y - npc.Center.Y) < 4000f)
				{
					args.Player.RespawnTimer = TShock.Config.RespawnBossSeconds;
					break;
				}
			}

			// Handle kicks/bans on mediumcore/hardcore deaths.
			if (args.Player.TPlayer.difficulty == 1 || args.Player.TPlayer.difficulty == 2) // Player is not softcore
			{
				bool mediumcore = args.Player.TPlayer.difficulty == 1;
				bool shouldBan = mediumcore ? TShock.Config.BanOnMediumcoreDeath : TShock.Config.BanOnHardcoreDeath;
				bool shouldKick = mediumcore ? TShock.Config.KickOnMediumcoreDeath : TShock.Config.KickOnHardcoreDeath;
				string banReason = mediumcore ? TShock.Config.MediumcoreBanReason : TShock.Config.HardcoreBanReason;
				string kickReason = mediumcore ? TShock.Config.MediumcoreKickReason : TShock.Config.HardcoreKickReason;

				if (shouldBan)
				{
					if (!args.Player.Ban(banReason, false, "TShock"))
					{
						TShock.Log.ConsoleDebug("GetDataHandlers / HandlePlayerKillMeV2 kicked with difficulty {0} {1}", args.Player.Name, args.Player.TPlayer.difficulty);
						args.Player.Kick("You died! Normally, you'd be banned.", true, true);
					}
				}
				else if (shouldKick)
				{
					TShock.Log.ConsoleDebug("GetDataHandlers / HandlePlayerKillMeV2 kicked with difficulty {0} {1}", args.Player.Name, args.Player.TPlayer.difficulty);
					args.Player.Kick(kickReason, true, true, null, false);
				}
			}

			if (args.Player.TPlayer.difficulty == 2 && Main.ServerSideCharacter && args.Player.IsLoggedIn)
			{
				if (TShock.CharacterDB.RemovePlayer(args.Player.Account.ID))
				{
					TShock.Log.ConsoleDebug("GetDataHandlers / HandlePlayerKillMeV2 ssc delete {0} {1}", args.Player.Name, args.Player.TPlayer.difficulty);
					args.Player.SendErrorMessage("You have fallen in hardcore mode, and your items have been lost forever.");
					TShock.CharacterDB.SeedInitialData(args.Player.Account);
				}
			}
			
			if (args.Player.HasPermission("server.gmod"))
			{
				Main.player[args.Player.Index].respawnTimer = 1;
				args.Player.RespawnTimer = 1;
				args.Player.TPlayer.respawnTimer = 1;
			}
			
			args.Player.TPlayer.dead = true;
			Main.player[args.Player.Index].dead = true;
			NetMessage.SendPlayerDeath(args.Player.Index, PlayerDeathReason.LegacyEmpty(), args.Damage, args.Direction, args.Pvp);
			NetMessage.SendPlayerDeath(args.Player.Index, PlayerDeathReason.LegacyEmpty(), args.Damage, args.Direction, args.Pvp, args.Player.Index);
			int num2 = 1173;
			if (SurvivalCore.SrvPlayers[args.Player.Index].Money >= 22500)
			{
				num2 = 3231;
			}
			else if (SurvivalCore.SrvPlayers[args.Player.Index].Money >= 7500)
			{
				num2 = 3229;
			}
			else if (SurvivalCore.SrvPlayers[args.Player.Index].Money >= 2500)
			{
				num2 = 1175;
			}
			if (args.Pvp)
			{
				TSPlayer[] players = TShock.Players;
				foreach (TSPlayer tSPlayer in players)
				{
					if (tSPlayer?.TPlayer.hostile ?? false)
					{
						tSPlayer.SendMessage($"[c/66ff66:{TShock.Players[args.PlayerDeathReason._sourcePlayerIndex].Name}] [i/p{args.PlayerDeathReason._sourceItemPrefix}:{args.PlayerDeathReason._sourceItemType}] [c/595959:→] [c/ff6666:{args.Player.Name}] [i:{num2}]", Color.DarkRed);
					}
				}
				SurvivalCore.SrvPlayers[args.Player.Index].PvpDeaths++;
				SurvivalCore.SrvPlayers[args.PlayerDeathReason._sourcePlayerIndex].PvpKills++;
			}
			
			
			else
			{

				TSPlayer[] players2 = TShock.Players;
				foreach (TSPlayer tSPlayer2 in players2)
				{
					if (tSPlayer2 != null && SurvivalCore.IsDeathMessage[tSPlayer2.Index])
					{
						tSPlayer2.SendMessage(args.PlayerDeathReason.GetDeathText(args.Player.Name).ToString(), Color.DarkRed);
					}
				}
				SurvivalCore.SrvPlayers[args.Player.Index].Deaths++;
			}


			args.Handled = true;
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

		//Sticky Bomb, bomb and Fish bomb are available but there's a cooldown that lasts 7 seconds.
		public static void NewProjectile(object sender, GetDataHandlers.NewProjectileEventArgs e)
		{
			if (e.Handled || e.Player.HasPermission("server.jmod"))
				return;
			
			
			if (e.Type == 28 || e.Type == 37 || e.Type == 519 || e.Type == 773)
			{
				if ((SurvivalCore.SrvPlayers[e.Player.Index].BombCooldown - DateTime.Now).TotalSeconds > 0)
				{
					e.Player.SendErrorMessage($"Zwolnij troche z wyrzucaniem ladunkow. Nastepny rzut bedzie mozliwy za {PowelderAPI.Utils.ExpireCountDown(SurvivalCore.SrvPlayers[e.Player.Index].BombCooldown)}");
					
					if (e.Type == 28)
						e.Player.GiveItem(166, 1);
					else if (e.Type == 37)
						e.Player.GiveItem(235, 1);
					else if (e.Type == 519)
						e.Player.GiveItem(3196, 1);
					else if (e.Type == 773)
						e.Player.GiveItem(4423, 1);
					
					e.Player.RemoveProjectile(e.Identity, e.Owner);
					e.Handled = true;
					return;
				}
				
				SurvivalCore.SrvPlayers[e.Player.Index].BombCooldown = DateTime.Now.AddSeconds(7);
			}
		}
	}
}
