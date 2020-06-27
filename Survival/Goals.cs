using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
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
            
            using (QueryResult queryResult = PowelderApi.Db.QueryReader("SELECT * FROM Goals"))
            {
                while (queryResult.Read())
                {
                    _goalsList.Add(new Goal(
                        queryResult.Get<int>("Index"),
                        queryResult.Get<string>("Name"),
                        queryResult.Get<int>("Id"),
                        queryResult.Get<int>("ToComplete"),
                        queryResult.Get<int>("Progress"),
                        queryResult.Get<int>("Queue")
                    ));
                }
            }
        }

        public static void ProgressUpdate()
        {
            foreach (var goal in _goalsList)
            {
                PowelderApi.Db.Query("UPDATE Goals SET Progress=@1 WHERE Index=@0", goal.Index, goal.Progress);
            }
        }

        public static bool IsDone(int npcId)
        {

            if (_goalsList.Exists(x => x.Id == npcId))
            {
                int index = _goalsList.FindIndex(x => x.Id == npcId);

                return _goalsList[index].ToComplete == _goalsList[index].Progress;
            }

            return false;
        }

        public static void ProgressCommand(CommandArgs args)
        {
           
        }

        

        public struct Goal
        {
            public int Index;
            public string Name;
            public int Id;
            public int ToComplete;
            public int Progress;
            public int Queue;

            public Goal(int index, string name, int id, int toComplete, int progress, int queue)
            {
                Index = index;
                Name = name;
                Id = id;
                ToComplete = toComplete;
                Progress = progress;
                Queue = queue;
            }
        }
    }
}