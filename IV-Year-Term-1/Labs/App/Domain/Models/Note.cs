using System;

namespace App.Domain.Models
{
    public class Note
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public NoteImportance Importance { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string IconPath { get; set; }
    }
}