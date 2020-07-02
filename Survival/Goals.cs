using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using OTAPI;
using PowelderAPI;
using Terraria;
using Terraria.GameContent.Bestiary;
using TShockAPI;
using TShockAPI.DB;

namespace SurvivalCore
{
    public class Goals
    {
        private static List<Goal> _goalsList = new List<Goal>();
        

        public static void Setup(IDbConnection db)
        {
            IQueryBuilder provider;
            IQueryBuilder queryBuilder = new SqliteQueryCreator();
            provider = queryBuilder;
            SqlTableCreator sqlTableCreator = new SqlTableCreator(db, provider);
            SqlTable table = new SqlTable("Goals",
                new SqlColumn("Index", MySqlDbType.Int32) { Unique = true, AutoIncrement = true},
                new SqlColumn("Name", MySqlDbType.Text),
                new SqlColumn("Id", MySqlDbType.Int16),
                new SqlColumn("ToComplete", MySqlDbType.Int32),
                new SqlColumn("Progress", MySqlDbType.Int32),
                new SqlColumn("Queue", MySqlDbType.Int16));
            sqlTableCreator.EnsureTableStructure(table);
        }

        public static void ProgressLoad()
        {
            _goalsList = new List<Goal>();

            using (QueryResult queryResult = PowelderApi.Db.QueryReader("SELECT * FROM Goals ORDER BY `Index` ASC"))
            {
                while (queryResult.Read())
                {
                    _goalsList.Add(new Goal(
                        queryResult.Get<int>("Index"),
                        queryResult.Get<string>("Name"),
                        queryResult.Get<int>("Id"),
                        queryResult.Get<int>("ToComplete"),
                        queryResult.Get<int>("Progress")
                    ));
                }
            }

        }

        public static void ProgressUpdate()
        {
            foreach (var goal in _goalsList)
            {
                PowelderApi.Db.Query("UPDATE Goals SET Progress=@1 WHERE `Index`=@0", goal.Index, goal.Progress);
            }
        }

        public static Goal GetCurrentGoal()
        {
            foreach (var goal in _goalsList)
            {
                if (goal.ToComplete != goal.Progress)
                    return goal;
            }
            return new Goal(-1, "Brak", -1, 1, 0);
        }
        
        public static bool IsDone(int npcId)
        {

            if (_goalsList.Exists(x => x.Id == npcId))
            {
                int index = _goalsList.FindIndex(x => x.Id == npcId);

                return _goalsList[index].ToComplete == _goalsList[index].Progress;
            }

            return true;
        }

        public static void ProgressCommand(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                Goal goal1 = GetCurrentGoal();
                args.Player.SendInfoMessage($"Aktualny stan postepu to {goal1.Name}: {goal1.Progress}/{goal1.ToComplete} ({Math.Round((float)goal1.Progress / (float)goal1.ToComplete, 4) * 100}%)");
                args.Player.SendInfoMessage("Aby pomoc w uzyskaniu celu uzyj komendy /postep <kwota>");
                return;;
            }

            int money = 0;
            if (!int.TryParse(args.Parameters[0], out money))
            {
                args.Player.SendErrorMessage("W kwocie musisz podac sama liczbe.");
                return;
            }

            if (money < 0)
            {
                args.Player.SendErrorMessage("Podano kwote w liczbie ujemnej.");
                return;
            }

            if (SurvivalCore.SrvPlayers[args.Player.Index].Money < money)
            {
                args.Player.SendErrorMessage("Chciales wplacic wiecej niz posiadasz.");
                return;
            }

            Goal goal = GetCurrentGoal();

            if (goal.ToComplete - (goal.Progress + money) < 0)
                money = goal.ToComplete - goal.Progress;

            goal.Progress += money;
            SurvivalCore.SrvPlayers[args.Player.Index].Money -= money;
            args.Player.SendSuccessMessage($"Pomyslnie wplacono {money}. Nowy stan prostepu to {goal.Name}: {goal.Progress}/{goal.ToComplete} ({Math.Round((float)goal.Progress / (float)goal.ToComplete, 4) * 100}% )");

            if (goal.ToComplete == goal.Progress)
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                foreach (var plr in TShock.Players.Where(x => x != null))
                {
                    plr.SendWarningMessage($"Aktualny cel zostal osiagniety! Nowy cel to {GetCurrentGoal().Name}.");
                    int proj = Projectile.NewProjectile(plr.TPlayer.position.X, plr.TPlayer.position.Y - 32, 0f, -8f, rand.Next(168, 170), 0, (float)0);
                    Main.projectile[proj].Kill();
                }
            }

        }

        
    }
    
    
    public class Goal
    {
        public int Index;
        public string Name;
        public int Id;
        public int ToComplete;
        public int Progress;

        public Goal(int index, string name, int id, int toComplete, int progress)
        {
            Index = index;
            Name = name;
            Id = id;
            ToComplete = toComplete;
            Progress = progress;
        }
    }
}