using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamAppService.Models;

namespace TeamAppService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly TeamContext _context;

        public TeamsController(TeamContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<dynamic>> GetTeams(
            [FromQuery(Name = "id")] long? id,
            [FromQuery(Name = "date")] DateTime? date,
            [FromQuery(Name = "title")] string? title,
            [FromQuery(Name = "name")] string? name,
            [FromQuery(Name = "password")] string? password
        )
        {
            if (password != null)
            {
                return await _context.IsAuth(name, password);
            }

            var teams = await _context.GetTeams(
                id,
                date,
                name,
                title
            );

            return teams;
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamPostResponse>> GetTeam(long id)
        {
            var team = await _context.GetTeam(id);

            if (team == null || !TeamExists(id))
            {
                return NotFound();
            }

            return new TeamPostResponse(team);
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<ActionResult<Models.Team>> PutTeam(long id, Team team)
        {
            bool isUnique = await _context.IsUnique(team.name, id);

            if (!isUnique)
            {
                return ValidationProblem("Name must be unique");
            }

            var originalTask = await _context.GetTeam(id);

            team.id = id;
            team.date = originalTask.date;
            team._id = originalTask._id;

            await _context.Update(team);

            try
            {
                await _context.SaveChangesAsync();

                return team;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
                {
                    return ValidationProblem();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Teams
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            bool isUnique = await _context.IsUnique(team.name);

            if (!isUnique)
            {
                return ValidationProblem("Name must be unique");
            }

            await _context.Create(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeam), new { id = team.id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Team>> DeleteTeam(long id)
        {
            var team = await _context.GetTeam(id);
            if (team == null)
            {
                return NotFound();
            }

            await _context.Remove(id);
            await _context.SaveChangesAsync();

            return team;
        }

        private bool TeamExists(long id)
        {
            var team = _context.GetTeam(id);

            return team == null ? false : true;
        }
    }
}
