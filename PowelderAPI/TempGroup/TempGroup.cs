using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace PowelderAPI.TempGroup
{
    public static class TempGroup
    {
        public static Dictionary<string, TempGroupPlayer> Players = new Dictionary<string, TempGroupPlayer>();

        public static void TempGroupCommand(CommandArgs args)
        {
            string param1 = null;
            string param2 = null;
            string param3 = null;
            string param4 = null;

            if (args.Parameters.Count > 0)
                param1 = args.Parameters[0];
            if (args.Parameters.Count > 1)
                param2 = args.Parameters[1];
            if (args.Parameters.Count > 2)
                param3 = args.Parameters[2];
            if (args.Parameters.Count > 3)
                param4 = args.Parameters[3];


            if (param1 == "set")
            {
                if (param2 == null)
                {
                    args.Player.SendErrorMessage("[c/595959:»]  Nie podano nicku.");
                    return;
                }

                List<TSPlayer> plrs = TSPlayer.FindByNameOrID(param2);

                if (plrs.Count == 0)
                {
                    args.Player.SendErrorMessage("[c/595959:»]  Nie znaleziono gracza");
                    return;
                }


                int seconds = 0;
                if (!TShock.Utils.TryParseTime(param4, out seconds))
                {
                    args.Player.SendErrorMessage("[c/595959:»]  Niepoprawny format czasu.");
                    return;
                }

                Group actualGroup = TShock.Groups.GetGroupByName(param3);

                if (actualGroup.Name.ToLower() == "gosc")
                {
                    args.Player.SendErrorMessage("[c/595959:»]  Problem z nazwa rangi. Sprawdz czy podales poprawna.");
                    return;
                }

                Group primaryGroup = plrs[0].Group;

                plrs[0].Group = actualGroup;
                plrs[0].Account.Group = actualGroup.Name;
                TShock.UserAccounts.SetUserGroup(plrs[0].Account, actualGroup.Name);


                TempGroupDbManage.AddTempGroup(plrs[0].Name, primaryGroup.Name, actualGroup.Name, DateTime.Now.AddSeconds(seconds));
                Players.Add(plrs[0].Name, new TempGroupPlayer(primaryGroup.Name, actualGroup.Name, DateTime.Now.AddSeconds(seconds)));

                args.Player.SendMessage($"[c/595959:»]  Pomyslnie przyznano range [c/ffff00:{actualGroup.Name}] dla [c/66ff66:{plrs[0].Name}].", Color.Gray);
                plrs[0].SendMessage($"[c/595959:»] Dziekujemy za zakup rangi [c/ffff00:{actualGroup.Name}]! [c/66ff66::)]", Color.Gray);
                plrs[0].SendMessage($"[c/595959:»] Twoja zakupiona ranga bedzie aktualna przez 30 dni dzialania serwera.", Color.Gray);
            }
            else if (param1 == "rem")
            {
                if (param2 == null)
                {
                    args.Player.SendErrorMessage("[c/595959:»]  Nie podano nicku.");
                    return;
                }

                List<TSPlayer> plrs = TSPlayer.FindByNameOrID(param2);

                if (plrs.Count == 0)
                {
                    UserAccount user = TShock.UserAccounts.GetUserAccountByName(param2);
                    if (user != null)
                    {
                        TempGroupPlayer tgPlayer = TempGroupDbManage.GetTempGroup(param2);

                        if (tgPlayer.ActualGroup == null)
                        {
                            args.Player.SendErrorMessage("[c/595959:»]  Podany gracz nie ma tymczasowej rangi.");
                            return;
                        }

                        TempGroupDbManage.RemoveTempGroup(param2);
                        user.Group = tgPlayer.PrimaryGroup;
                        TShock.UserAccounts.SetUserGroup(user, user.Group);

                    }
                    else
                    {
                        args.Player.SendErrorMessage("[c/595959:»]  Nie znaleziono gracza.");
                        return;
                    }
                }
                else
                {
                    param2 = plrs[0].Name;
                    TempGroupPlayer tgPlayer = TempGroupDbManage.GetTempGroup(plrs[0].Name);

                    if (tgPlayer.ActualGroup == null)
                    {
                        args.Player.SendErrorMessage("[c/595959:»]  Podany gracz nie ma tymczasowej rangi.");
                        return;
                    }

                    plrs[0].Account.Group = tgPlayer.PrimaryGroup;
                    plrs[0].Group = TShock.Groups.GetGroupByName(tgPlayer.PrimaryGroup);


                    TempGroupDbManage.RemoveTempGroup(param2);
                    TShock.UserAccounts.SetUserGroup(plrs[0].Account, plrs[0].Account.Group);

                    Players.Remove(plrs[0].Name);
                }

                args.Player.SendMessage($"[c/595959:»]  Pomyslnie usunieto tymczasowa range dla [c/66ff66:{param2}].", Color.Gray);
            }
            else if (param1 == "len")
            {
                int seconds = 0;
                if (!TShock.Utils.TryParseTime(param2, out seconds))
                {
                    args.Player.SendErrorMessage("[c/595959:»]  Niepoprawny format czasu.");
                    return;
                }

                TempGroupDbManage.LengthenTempGroups(seconds);

                args.Player.SendMessage($"[c/595959:»]  Pomyslnie wydluzono wszystkie rangi.", Color.Gray);
            }
            else
            {

                args.Player.SendMessage("                 [c/595959:«]TempGroup[c/595959:»]", Color.LightGray);
                args.Player.SendMessage("[c/595959:»]  set <nick> <ranga> <czas>", Color.Gray);
                args.Player.SendMessage("[c/595959:»]  rem <nick>", Color.Gray);
                args.Player.SendMessage("[c/595959:»]  len <czas>", Color.Gray);
                return;
            }


        }

        public static void OnUpdate(EventArgs args)
        {
            foreach (string player in _ = Players.Keys.ToArray())
            {
                if ((DateTime.Now - Players[player].ExpireDate).TotalMilliseconds > 0)
                {
                    TSPlayer plr = TSPlayer.FindByNameOrID(player)[0];

                    if (plr == null || !plr.Active)
                        return;

                    plr.Group = TShock.Groups.GetGroupByName(Players[player].PrimaryGroup);
                    plr.Account.Group = Players[player].PrimaryGroup;
                    TShock.UserAccounts.SetUserGroup(plr.Account, Players[player].PrimaryGroup);

                    TempGroupDbManage.RemoveTempGroup(plr.Name);

                    plr.SendMessage($"[c/595959:»]  Waznosc twojej rangi [c/ffff66:{Players[player].ActualGroup}] wygasla.", Color.Gray);

                    Players.Remove(player);
                }
            }
        }

        public static void OnJoin(JoinEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.Who];

            if (plr == null)
                return;

            TempGroupPlayer tgplr = TempGroupDbManage.GetTempGroup(plr.Name);

            if (tgplr.ActualGroup == null)
                return;

            if ((DateTime.Now - tgplr.ExpireDate).TotalMilliseconds > 0)
            {
                plr.Group = TShock.Groups.GetGroupByName(tgplr.PrimaryGroup);
                plr.Account.Group = tgplr.PrimaryGroup;
                TShock.UserAccounts.SetUserGroup(plr.Account, tgplr.PrimaryGroup);

                TempGroupDbManage.RemoveTempGroup(plr.Name);

                plr.SendMessage($"[c/595959:»]  Waznosc twojej rangi [c/ffff66:{tgplr.ActualGroup}] wygasla.", Color.Gray);
                return;
            }

            Players.Add(plr.Name, tgplr);
        }

        public static void OnLeave(LeaveEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.Who];

            if (plr == null)
                return;

            if (!Players.ContainsKey(plr.Name))
                return;

            Players.Remove(plr.Name);
        }

        public static void PostLogin(TShockAPI.Hooks.PlayerPostLoginEventArgs args)
        {
            if (args.Player == null)
                return;

            TempGroupPlayer tgplr = TempGroupDbManage.GetTempGroup(args.Player.Name);

            if (tgplr.ActualGroup == null)
                return;

            if ((DateTime.Now - tgplr.ExpireDate).TotalMilliseconds > 0)
            {
                args.Player.Group = TShock.Groups.GetGroupByName(tgplr.PrimaryGroup);
                args.Player.Account.Group = tgplr.PrimaryGroup;
                TShock.UserAccounts.SetUserGroup(args.Player.Account, tgplr.PrimaryGroup);

                TempGroupDbManage.RemoveTempGroup(args.Player.Name);

                args.Player.SendMessage($"[c/595959:»]  Waznosc twojej rangi [c/ffff66:{tgplr.ActualGroup}] wygasla.", Color.Gray);
                return;
            }

            if (Players.ContainsKey(args.Player.Name))
                return;

            Players.Add(args.Player.Name, tgplr);
        }
    }
    public struct TempGroupPlayer
    {
        public string PrimaryGroup;
        public string ActualGroup;
        public DateTime ExpireDate;

        public TempGroupPlayer(string pGroup, string aGroup, DateTime eDate)
        {
            PrimaryGroup = pGroup;
            ActualGroup = aGroup;
            ExpireDate = eDate;
        }
    }
}
