using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TerrariaApi.Server;
using TShockAPI;

namespace PowelderAPI.CommandRegions
{
	public class CommandRegions
	{
		public static List<CRegion> CRegions = new List<CRegion>();

		public static CrPlayer[] Players = new CrPlayer[256];

		public static void CommandInitialize()
		{
			Commands.ChatCommands.Add(new Command("server.admin", AdminCommand, "cregion", "creg"));
		}

		public static void RegionThread()
		{
			string name;
			while (true)
			{
				Thread.Sleep(1000);
				foreach (TSPlayer player in TShock.Players.Where((TSPlayer p) => p?.Active ?? false))
				{
					try
					{
						name = CRegion.GetCurrentRegion(player);
						Players[player.Index].CurrentRegion = name;
						if (name == null)
						{
							Players[player.Index].Seconds = -1;
						}
						else
						{
							if (Players[player.Index].Seconds == -1)
							{
								Players[player.Index].Seconds = 5;
								Players[player.Index].OldRegion = name;
							}
							if (Players[player.Index].CurrentRegion == Players[player.Index].OldRegion)
							{
								if (Players[player.Index].Seconds == 0)
								{
									Players[player.Index].Seconds = -1;
									CRegions.Find((CRegion c) => c.Name == name).Execute(player);
									player.SendData(PacketTypes.CreateCombatTextExtended, name + "!", (int)new Color(169, 55, 255).PackedValue, player.TPlayer.Center.X, player.TPlayer.Center.Y);
								}
								else
								{
									player.SendData(PacketTypes.CreateCombatTextExtended, Players[player.Index].Seconds.ToString(), (int)new Color(169, 55, 255).PackedValue, player.TPlayer.Center.X, player.TPlayer.Center.Y);
									Players[player.Index].Seconds--;
								}
							}
							else
							{
								Players[player.Index].Seconds = 5;
								Players[player.Index].OldRegion = name;
							}
						}
					}
					catch (NullReferenceException)
					{
					}
				}
			}
		}

		public static void OnJoin(JoinEventArgs args)
		{
			Players[args.Who] = new CrPlayer(args.Who);
		}

		public static void OnLeave(LeaveEventArgs args)
		{
			Players[args.Who] = null;
		}

		public static void OnGetData(GetDataEventArgs args)
		{
			if (!args.Handled && !args.Handled && args.MsgID == PacketTypes.Tile)
			{
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
				{
					TSPlayer tSPlayer = TShock.Players[args.Msg.whoAmI];
					byte type = binaryReader.ReadByte();
					short x = binaryReader.ReadInt16();
					short y = binaryReader.ReadInt16();
					if ((type == 0 || type == 1) && tSPlayer.HasPermission("server.admin"))
					{
						if (Players[tSPlayer.Index].Set[0])
						{
							Players[tSPlayer.Index].Pos1.X = x;
							Players[tSPlayer.Index].Pos1.Y = y;
							Players[tSPlayer.Index].Set[0] = false;
							Players[tSPlayer.Index].Set[1] = true;
							tSPlayer.SendMessage($"[c/595959:»]  Ustawiono punkt pierwszy. ({x}, {y})", Color.Gray);
							tSPlayer.SendMessage("[c/595959:»]  Teraz ustaw drugi punkt w prawym dolnym rogu.", Color.Gray);
							tSPlayer.SendTileSquare(x, y);
							args.Handled = true;
						}
						else if (Players[tSPlayer.Index].Set[1])
						{
							Players[tSPlayer.Index].Pos2.X = x;
							Players[tSPlayer.Index].Pos2.Y = y;
							Players[tSPlayer.Index].Set[1] = false;
							tSPlayer.SendMessage($"[c/595959:»]  Ustawiono punkt drugi. ({x}, {y})", Color.Gray);
							tSPlayer.SendTileSquare(x, y);
							args.Handled = true;
						}
						else if (Players[tSPlayer.Index].Destroy)
						{
							CRegion region = CRegion.GetRegion(x, y);
							if (region == null)
							{
								tSPlayer.SendErrorMessage("[c/595959:»]  Nie znaleziono regionu.");
							}
							else
							{
								CRegions.Remove(region);
								CrDatabase.DestroyRegion(region.Name);
								tSPlayer.SendMessage("[c/595959:»]  Pomyslnie usunieto region [c/66ff66:" + region.Name + "].", Color.Gray);
							}
							Players[tSPlayer.Index].Destroy = false;
							tSPlayer.SendTileSquare(x, y);
							args.Handled = true;
						}
						else if (Players[tSPlayer.Index].Modify)
						{
							CRegion region = CRegion.GetRegion(x, y);
							if (region == null)
							{
								tSPlayer.SendErrorMessage("[c/595959:»]  Nie znaleziono regionu.");
							}
							else
							{
								CRegions.Find((CRegion c) => c == region).Action = Players[tSPlayer.Index].Cmd;
								CrDatabase.ModifyRegion(region.Name, Players[tSPlayer.Index].Cmd);
								tSPlayer.SendMessage("[c/595959:»]  Pomyslnie zmodyfikowano region [c/66ff66:" + region.Name + "].", Color.Gray);
							}
							Players[tSPlayer.Index].Modify = false;
							tSPlayer.SendTileSquare(x, y);
							args.Handled = true;
						}
					}
				}
			}
		}

		public static void AdminCommand(CommandArgs args)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			if (args.Parameters.Count > 1)
			{
				text2 = args.Parameters[1];
			}
			if (args.Parameters.Count > 2)
			{
				text3 = args.Parameters[2];
			}
			if (args.Parameters.Count > 3)
			{
				text4 = args.Parameters[3];
			}
			switch (text)
			{
			default:
				args.Player.SendMessage("            [c/595959:«]CommandRegions[c/595959:»]", new Color(192, 192, 192));
				args.Player.SendMessage("[c/595959:»]  [c/66FF66:/cregion] set", Color.Gray);
				args.Player.SendMessage("[c/595959:»]  [c/66FF66:/cregion] define <name> <action>", Color.Gray);
				args.Player.SendMessage("[c/595959:»]  [c/66FF66:/cregion] destroy", Color.Gray);
				args.Player.SendMessage("[c/595959:»]  [c/66FF66:/cregion] modify <new action>", Color.Gray);
				break;
			case "set":
				args.Player.SendMessage("[c/595959:»]  Ustaw pierwszy punkt w lewym gornym rogu.", Color.Gray);
				Players[args.Player.Index].Set[0] = true;
				break;
			case "define":
				if (Players[args.Player.Index].Pos1 == Point.Zero || Players[args.Player.Index].Pos2 == Point.Zero)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie masz ustawionych punktow.");
					args.Player.SendMessage("[c/595959:»]  Wpisz [c/66ff66:/cregion set], aby zaczac ustawiac.", Color.Gray);
					break;
				}
				if (text2 == null)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie podano nazwy.");
					break;
				}
				if (text3 == null)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie podano komendy.");
					break;
				}
				args.Player.SendMessage("[c/595959:»]  Pomyslnie ustawiono region [c/66ff66:" + text2 + "].", Color.Gray);
				CRegions.Add(new CRegion(Players[args.Player.Index], text2, text3));
				Players[args.Player.Index].Pos1 = Point.Zero;
				Players[args.Player.Index].Pos2 = Point.Zero;
				break;
			case "destroy":
				args.Player.SendMessage("[c/595959:»]  Zmodyfikuj obiekt wewnatrz regionu, aby go usunac.", Color.Gray);
				Players[args.Player.Index].Destroy = true;
				break;
			case "modify":
				if (text2 == null)
				{
					args.Player.SendErrorMessage("[c/595959:»]  Nie podano komendy.");
					break;
				}
				args.Player.SendMessage("[c/595959:»]  Zmodyfikuj obiekt wewnatrz regionu, aby go zmodyfikowac.", Color.Gray);
				Players[args.Player.Index].Cmd = text2;
				Players[args.Player.Index].Modify = true;
				break;
			}
		}
	}
}
