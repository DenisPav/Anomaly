using Bogus;
using EmbedioSample.ApiModels;
using EmbedioSample.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmbedioSample.Services
{
    public class NoteRepository : INoteRepository
    {
        readonly static IList<Note> AllNotes;

        static NoteRepository()
        {
            AllNotes = new Faker<Note>()
                .RuleFor(note => note.Id, f => f.Random.Int())
                .RuleFor(note => note.Text, f => f.Lorem.Paragraph())
                .RuleFor(note => note.CreateAt, f => f.Date.Recent())
                .Generate(100);
        }

        public IEnumerable<Note> GetAll()
        {
            return AllNotes;
        }

        public Note Find(int id)
        {
            return AllNotes.FirstOrDefault(note => note.Id == id);
        }

        public Note AddNote(NoteApiModel model)
        {
            var randomizer = new Randomizer();
            var newNote = new Note
            {
                CreateAt = DateTime.UtcNow,
                Id = randomizer.Int(),
                Text = model.Text
            };

            AllNotes.Add(newNote);

            return newNote;
        }

        public Note UpdateNote(int id, NoteApiModel model)
        {
            var wantedNote = AllNotes.FirstOrDefault(note => note.Id == id);

            if (wantedNote != null)
            {
                wantedNote.Text = model.Text;
                wantedNote.CreateAt = DateTime.UtcNow;
            }

            return wantedNote;
        }

        public Note DeleteNote(int id)
        {
            var wantedNote = AllNotes.FirstOrDefault(note => note.Id == id);

            if(wantedNote != null)
            {
                AllNotes.Remove(wantedNote);
            }

            return wantedNote;
        }
    }
}
