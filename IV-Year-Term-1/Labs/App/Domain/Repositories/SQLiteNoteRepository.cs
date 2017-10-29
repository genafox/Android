using System.Collections.Generic;
using App.Domain.Interfaces;
using App.Domain.Database.Models;
using SQLite;
using App.Domain.Database;
using System;
using App.Domain.Exceptions;

namespace App.Domain.Repositories
{
    public class SQLiteNoteRepository : INoteRepository
    {
        private readonly SQLiteConnection connection;

        public SQLiteNoteRepository(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Note> GetAll()
        {
            string query = $"SELECT * FROM {DbConstants.NotesTableName}";

            return connection.Query<Note>(query);
        }

        public Note GetByName(string noteName)
        {
            Note note = connection.Get<Note>(n => string.Equals(n.Name, noteName, StringComparison.InvariantCultureIgnoreCase));

            if (note == null)
            {
                throw new EntryNotFoundException($"Note with the name '{noteName}' was not found");
            }

            return note;
        }

        public void Create(Note note)
        {
            note.CreationDate = DateTime.Now;

            int result = this.connection.Insert(note);
            //throw new EntryAlreadyExistsException($"Note with the name '{noteName}' already exists");
        }

        public void Update(Note note)
        {
            int result = this.connection.Update(note);
            //throw new EntryNotFoundException($"Note with the name '{noteName}' was not found");
        }

        public void Delete(string noteName)
        {
            string command = $"DELETE FROM {DbConstants.NotesTableName} WHERE Name = {noteName}";

            int result = this.connection.Execute(command);
            //throw new EntryNotFoundException($"Note with the name '{noteName}' was not found");
        }
    }
}