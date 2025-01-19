
using System;
using Npgsql;

// dotnet add package Npgsql

public class DBConnection
{
    private string Host = "localhost";
    private string User = "postgres";
    private string DBname = "hors_jeu";
    private string Password = "postgres";
    private string Port = "5432";

    public NpgsqlConnection GetConnection()
    {
        string connString = $"Host={Host};Username={User};Password={Password};Database={DBname};Port={Port}";
        NpgsqlConnection conn = new NpgsqlConnection(connString);
        return conn;
    }
}