using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;
using System.IO;

public static class TestSql
{

    private static IDbConnection conn;

    private static string dbPath = Path.Combine(Application.streamingAssetsPath, "TestSql1.db");
    
    public static void Init()
    {
        if(conn == null)
        {
            conn = new SqliteConnection("URI=file:" + dbPath);
            conn.Open();
        }
    }

    public static void Close()
    {
        if(conn != null)
        {
            conn.Close();
            conn = null;
        }
    }

    public static void GetScore(string name)
    {   
        var cmd = conn.CreateCommand();

        cmd.CommandText = "SELECT TestName FROM TestEntity WHERE Testname = @name";

        var param = cmd.CreateParameter();
        param.ParameterName = "@name";
        param.Value = name;
        cmd.Parameters.Add(param);

        using(var reader = cmd.ExecuteReader())
        {
            if(reader.Read())
            {
                Debug.Log(reader["TestName"].ToString());
            }
        }
    }
    
}
