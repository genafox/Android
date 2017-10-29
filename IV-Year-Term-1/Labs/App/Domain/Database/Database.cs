using System;
using SQLite;
using System.IO;
using App.Domain.Exceptions;
using App.Domain.Database.Models;

namespace App.Domain.Database
{
    public class Database
    {
        private static readonly string DatabaseFullPath;

        public const string Name = "NotesDB.db3";
        public const int Version = 1;

        static Database()
        {
            DatabaseFullPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                Name);
        }

        public static void Init()
        {
            if (!File.Exists(DatabaseFullPath))
            {
                using (var connection = EstablishConnection())
                {
                    try
                    {
                        connection.CreateTable<Note>();
                    }
                    catch(Exception ex)
                    {
                        throw new DatabaseInitializationException($"Could not creat database due to error: {ex.Message}", ex);
                    }
                }
            }
        }

        public static SQLiteConnection EstablishConnection(string dbPath = null)
        {
            dbPath = dbPath ?? DatabaseFullPath;

            try
            {
                return new SQLiteConnection(dbPath);
            }
            catch(SQLiteException ex)
            {
                throw new DatabaseConnectionException($"Could not establish connection with database {dbPath}", ex);
            }
        }
    }
}