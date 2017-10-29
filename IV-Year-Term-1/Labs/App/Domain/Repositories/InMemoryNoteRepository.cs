using System;
using System.Collections.Generic;
using App.Domain.Exceptions;
using App.Domain.Interfaces;
using App.Domain.Database.Models;

namespace App.Domain.Repositories
{
    public class InMemoryNoteRepository : INoteRepository
    {
        private static Dictionary<string, Note> Storage;

        static InMemoryNoteRepository()
        {
            Storage = new Dictionary<string, Note>();
            Seed();
        }

        public IEnumerable<Note> GetAll()
        {
            return Storage.Values;
        }

        public Note GetByName(string noteName)
        {
            CheckExistance(noteName, shouldExist: true);

            return Storage[noteName];
        }

        public void Create(Note note)
        {
            CheckExistance(note.Name, shouldExist: false);

            note.CreationDate = DateTime.Now;
            Storage.Add(note.Name, note);
        }

        public void Update(Note note)
        {
            CheckExistance(note.Name, shouldExist:true);

            Storage[note.Name] = note;
        }

        public void Delete(string noteName)
        {
            CheckExistance(noteName, shouldExist: true);

            Storage.Remove(noteName);
        }

        private static void CheckExistance(string noteName, bool shouldExist)
        {
            bool exists = Storage.ContainsKey(noteName);

            if (shouldExist && !exists)
            {
                throw new EntryNotFoundException($"Note with the name '{noteName}' was not found");
            }

            if (!shouldExist && exists)
            {
                throw new EntryAlreadyExistsException($"Note with the name '{noteName}' already exists");
            }
        }

        private static void Seed()
        {
            var note1 = new Note
            {
                Name = "First Note",
                Description = "Thid is very cool and poverfull note that is capable to concure the world!",
                Importance = NoteImportance.High,
                CreationDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(4)
            };

            var note2 = new Note
            {
                Name = "Second Note",
                Description = "Thid is very cool and poverfull note that is capable to concure the world! /r/n Splitted on two rows.",
                Importance = NoteImportance.Medium,
                CreationDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(4)
            };

            var note3 = new Note
            {
                Name = "Third Note",
                Description = "Thid is very cool and poverfull note that is capable to concure the world! /r/n Splitted on three rows. /r/n With Low importance.",
                Importance = NoteImportance.Low,
                CreationDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(4)
            };

            Storage.Add(note1.Name, note1);
            Storage.Add(note2.Name, note2);
            Storage.Add(note3.Name, note3);
        }
    }
}