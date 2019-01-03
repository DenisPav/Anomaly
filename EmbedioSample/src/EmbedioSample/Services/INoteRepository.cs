using EmbedioSample.ApiModels;
using EmbedioSample.Data;
using System.Collections.Generic;

namespace EmbedioSample.Services
{
    public interface INoteRepository
    {
        IEnumerable<Note> GetAll();
        Note Find(int id);
        Note AddNote(NoteApiModel model);
        Note UpdateNote(int id, NoteApiModel model);
        Note DeleteNote(int id);
    }
}
