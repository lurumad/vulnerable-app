using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using VulnerableNotesApp.Infrastructure.Data;
using VulnerableNotesApp.Model;

namespace VulnerableNotesApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notes")]
    public class NotesController : ControllerBase
    {
        private readonly NotesDbContext dbContext;

        public NotesController(NotesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Note>>> GetAll(string content, CancellationToken cancellationToken)
        {
            var connection = dbContext.Database.GetDbConnection();

            try
            {
                await connection.OpenAsync(cancellationToken);
                var sql = $@"
                    SELECT [Id]
                      ,[Content]
                      ,[Date]
                      ,[Important]
                      ,[UserId]
                  FROM [dbo].[Notes]
                  WHERE UserId = {User.GetSubjectId()}
                  {(string.IsNullOrEmpty(content) ? string.Empty : $" AND [Content] LIKE '%{content}%'")}";

                return Ok(await connection.QueryAsync<Note>(sql));
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Note>> Create(Note note, CancellationToken cancellationToken)
        {
            note.UserId = User.GetSubjectId();
            await dbContext.AddAsync(note);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Created($"{Request.Scheme}://{Request.Host}/api/notes/{note.Id}", note);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> Update(int id, Note model, CancellationToken cancellationToken)
        {
            var note = await dbContext.Notes
                .SingleOrDefaultAsync(n => n.Id == id && n.UserId == User.GetSubjectId(), cancellationToken);

            if (note is null)
            {
                return NotFound();
            }

            note.Important = model.Important;

            await dbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var note = await dbContext.Notes.SingleOrDefaultAsync(n => n.Id == id, cancellationToken);

            if (note is null)
            {
                return NotFound();
            }

            dbContext.Remove(note);

            await dbContext.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete]
        [Route("")]
        public async Task<ActionResult> DeleteAll(CancellationToken cancellationToken)
        {
            var notes = dbContext.Notes.Where(n => n.UserId == User.GetSubjectId());
            dbContext.RemoveRange(notes);
            await dbContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
    }
}
