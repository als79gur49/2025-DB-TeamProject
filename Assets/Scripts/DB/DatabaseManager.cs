using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine;

/// <summary>
/// SQLite 데이터베이스 연결 및 기본 관리를 담당하는 클래스
/// </summary>
public static class DatabaseManager
{
    private static IDbConnection connection;
    private static string dbPath;

    /// <summary>
    /// 데이터베이스 초기화
    /// </summary>
    public static void Initialize()
    {
        // StreamingAssets 폴더에 데이터베이스 파일 위치 설정
        dbPath = Path.Combine(Application.streamingAssetsPath, "uiux7.db");

        // 연결이 없거나 닫혀있으면 새로 연결
        if (connection == null || connection.State != ConnectionState.Open)
        {
            connection = new SqliteConnection("URI=file:" + dbPath);
            connection.Open();

            // 테이블 생성
            CreateTables();

            Debug.Log("데이터베이스가 성공적으로 초기화되었습니다");
        }
    }

    /// <summary>
    /// 데이터베이스 연결 종료
    /// </summary>
    public static void Close()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
            connection = null;
            Debug.Log("데이터베이스 연결이 종료되었습니다");
        }
    }

    /// <summary>
    /// 데이터베이스 연결 객체 반환
    /// </summary>
    public static IDbConnection GetConnection()
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            Initialize();
        }
        return connection;
    }

    /// <summary>
    /// 필요한 테이블들을 생성
    /// </summary>
    private static void CreateTables()
    {
        // 랭킹 테이블 생성
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Rankings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerName TEXT NOT NULL UNIQUE,
                Score INTEGER NOT NULL DEFAULT 0,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            )
        ");

        Debug.Log("데이터베이스 테이블이 성공적으로 생성되었습니다");
    }

    /// <summary>
    /// SELECT 쿼리 실행 후 DataReader 반환
    /// </summary>
    public static IDataReader ExecuteReader(string query, params (string name, object value)[] parameters)
    {
        var cmd = GetConnection().CreateCommand();
        cmd.CommandText = query;

        // 파라미터 추가
        foreach (var param in parameters)
        {
            var dbParam = cmd.CreateParameter();
            dbParam.ParameterName = param.name;
            dbParam.Value = param.value ?? DBNull.Value;
            cmd.Parameters.Add(dbParam);
        }

        return cmd.ExecuteReader();
    }

    /// <summary>
    /// INSERT, UPDATE, DELETE 등의 쿼리 실행
    /// </summary>
    public static int ExecuteNonQuery(string query, params (string name, object value)[] parameters)
    {
        using (var cmd = GetConnection().CreateCommand())
        {
            cmd.CommandText = query;

            // 파라미터 추가
            foreach (var param in parameters)
            {
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = param.name;
                dbParam.Value = param.value ?? DBNull.Value;
                cmd.Parameters.Add(dbParam);
            }

            return cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 단일 값 반환하는 쿼리 실행
    /// </summary>
    public static object ExecuteScalar(string query, params (string name, object value)[] parameters)
    {
        using (var cmd = GetConnection().CreateCommand())
        {
            cmd.CommandText = query;

            // 파라미터 추가
            foreach (var param in parameters)
            {
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = param.name;
                dbParam.Value = param.value ?? DBNull.Value;
                cmd.Parameters.Add(dbParam);
            }

            return cmd.ExecuteScalar();
        }
    }
}
