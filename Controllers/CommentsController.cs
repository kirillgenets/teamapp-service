
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList;
using TeamAppService.Models;

namespace TeamAppService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : Controller
    {
        private readonly CommentContext _context;

        public CommentsController(CommentContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<List<Models.Comment>>> GetComments(
            [FromQuery(Name = "id")] long? id,
            [FromQuery(Name = "date")] DateTime? date,
            [FromQuery(Name = "text")] string? text,
            [FromQuery(Name = "authorName")] string? authorName,
            [FromQuery(Name = "authorId")] long? authorId,
            [FromQuery(Name = "teamId")] long? teamId
        )
        {
            var comments = await _context.GetComments(
                id,
                date,
                text,
                authorId,
                authorName,
                teamId
            );

            return comments;
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Comment>> GetComment(long id)
        {
            var comment = await _context.GetComment(id);

            if (comment == null || !CommentExists(id))
            {
                return NotFound();
            }

            return comment;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<ActionResult<Models.Comment>> PutComment(long id, Models.Comment comment)
        {
            var originalComment = await _context.GetComment(id);

            comment.id = id;
            comment.date = originalComment.date;
            comment._id = originalComment._id;

            await _context.Update(comment);

            try
            {
                await _context.SaveChangesAsync();

                return comment;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Comments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Models.Comment>> PostComment(Models.Comment comment)
        {
            await _context.Create(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Models.Comment>> DeleteComment(long id)
        {
            var comment = await _context.GetComment(id);
            if (comment == null)
            {
                return NotFound();
            }

            await _context.Remove(id);
            await _context.SaveChangesAsync();

            return comment;
        }

        private bool CommentExists(long id)
        {
            var comment = _context.GetComment(id);

            return comment == null ? false : true;
        }
    }
}
