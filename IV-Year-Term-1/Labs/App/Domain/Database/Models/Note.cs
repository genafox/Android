using SQLite;
using System;

namespace App.Domain.Database.Models
{
    [Table(DbConstants.NotesTableName)]
    public class Note
    {
        [PrimaryKey, AutoIncrement, Column("Id")]
        private int Id { get; set; }

        [Indexed(Name = "NoteName", Unique = true)]
        public string Name { get; set; }

        public string Description { get; set; }

        public NoteImportance Importance { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string IconPath { get; set; }
    }
}