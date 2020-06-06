using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamAppService.Helpers;
using TeamAppService.Models;

namespace TeamAppService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<dynamic>> GetUsers(
            [FromQuery(Name = "id")] long? id,
            [FromQuery(Name = "date")] DateTime? date,
            [FromQuery(Name = "teamName")] string? teamName
        )
        {
            var users = await _context.GetUsers(
                id,
                date,
                teamName
            );

            return users;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.GetUser(id);

            if (user == null || !UserExists(id))
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<ActionResult<Models.User>> PutUser(long id, User user)
        {
            bool isUnique = await _context.IsUnique(user.login, id, user.teamId);

            if (!isUnique)
            {
                return ValidationProblem(null, null, null, "Login must be unique");
            }

            var originalTask = await _context.GetUser(id);

            user.id = id;
            user.date = originalTask.date;
            user._id = originalTask._id;

            await _context.Update(user);

            try
            {
                await _context.SaveChangesAsync();

                return user;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return ValidationProblem();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<UserPostResponse>> PostUser(PostUser user, [FromQuery(Name = "isSignIn")] bool isSignIn = false)
        {
            if (isSignIn)
            {
                var authUser = await _context.IsAuth(user.login, user.password, user.teamName);
                
                if(authUser != null)
                {
                    return new UserPostResponse(authUser);
                }

                return ValidationProblem("Team name, login or password is incorrect.");
            }

            bool isUnique = await _context.IsUnique(user.login, null, user.teamId, user.teamName);
            bool isTeamAuth = await _context.IsTeamAuth(user.teamName, user.teamPassword);

            if (!isUnique)
            {
                return ValidationProblem("Login must be unique.");
            }

            if (!isTeamAuth)
            {
                return ValidationProblem("Team name or team password is incorrect.");
            }

            User correctUser = new User(user.login, user.password, user.teamName);

            await _context.Create(correctUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.id }, new UserPostResponse(correctUser));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(long id)
        {
            var user = await _context.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }

            await _context.Remove(id);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(long id)
        {
            var user = _context.GetUser(id);

            return user == null ? false : true;
        }
    }
}
