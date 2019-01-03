using EmbedioSample.ApiModels;
using EmbedioSample.Services;
using System.Net;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;

namespace EmbedioSample.Controllers
{
    public class NoteController : WebApiController
    {
        readonly INoteRepository NoteRepository;

        public NoteController(
            IHttpContext context,
            INoteRepository noteRepository
        ) : base(context)
        {
            NoteRepository = noteRepository;
        }

        [WebApiHandler(HttpVerbs.Get, "/notes")]
        public bool Get()
        {
            var notes = NoteRepository.GetAll();

            return this.JsonResponse(notes);
        }

        [WebApiHandler(HttpVerbs.Get, "/notes/{id}")]
        public bool GetSingle(int id)
        {
            var note = NoteRepository.Find(id);

            if(note == null)
            {
                this.Response.StatusCode = (int)HttpStatusCode.NotFound;
                this.JsonResponse(new { });

                return false;
            }

            return this.JsonResponse(note);
        }

        [WebApiHandler(HttpVerbs.Post, "/notes")]
        public bool CreateNew()
        {
            var model = this.ParseJson<NoteApiModel>();
            var newNote = NoteRepository.AddNote(model);

            return this.JsonResponse(newNote);
        }

        [WebApiHandler(HttpVerbs.Put, "/notes/{id}")]
        public bool Update(int id)
        {
            var model = this.ParseJson<NoteApiModel>();
            var updatedNote = NoteRepository.UpdateNote(id, model);

            if (updatedNote == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                this.JsonResponse(new { });

                return false;
            }

            return this.JsonResponse(updatedNote);
        }

        [WebApiHandler(HttpVerbs.Delete, "/notes/{id}")]
        public bool Delete(int id)
        {
            var deletedNote = NoteRepository.DeleteNote(id);

            if(deletedNote == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                this.JsonResponse(new { });

                return false;
            }

            return this.JsonResponse(deletedNote);
        }
    }
}
