using App.Domain.Models;
using System.Collections.Generic;

namespace App.Domain.Interfaces
{
    public interface INoteRepository
    {
        IEnumerable<Note> GetAll();

        Note GetByName(string noteName);

        void Create(Note note);

        void Update(Note note);

        void Delete(string noteName);
    }
}