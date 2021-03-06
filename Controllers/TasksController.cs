﻿
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
    public class TasksController : Controller
    {
        private readonly TaskContext _context;

        public TasksController(TaskContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<List<Models.Task>>> GetTasks(
            [FromQuery(Name = "id")] long? id,
            [FromQuery(Name = "date")] DateTime? date,
            [FromQuery(Name = "title")] string? title,
            [FromQuery(Name = "category")] string? category,
            [FromQuery(Name = "assigneeId")] long? assigneeId,
            [FromQuery(Name = "isCompleted")] bool? isCompleted,
            [FromQuery(Name = "teamId")] long? teamId,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            [FromQuery(Name = "page")] int page = 1,
            [FromQuery(Name = "showAll")] bool showAll = false
        )
        {
            var tasks = await _context.GetTasks(
                id,
                date, 
                title, 
                category, 
                assigneeId,
                isCompleted,
                teamId
            );

            return showAll ? tasks : tasks.ToPagedList(page, pageSize).ToList();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Task>> GetTask(long id)
        {
            var task = await _context.GetTask(id);

            if (task == null || !TaskExists(id))
            {
                return NotFound();
            }

            return task;
        }

        // PUT: api/Tasks/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<ActionResult<Models.Task>> PutTask(long id, Models.Task task)
        {
            var originalTask = await _context.GetTask(id);

            task.id = id;
            task.date = originalTask.date;
            task._id = originalTask._id;

            await _context.Update(task);

            try
            {
                await _context.SaveChangesAsync();

                return task;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Tasks
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Models.Task>> PostTask(Models.Task task)
        {
            await _context.Create(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.id }, task);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Models.Task>> DeleteTask(long id)
        {
            var task = await _context.GetTask(id);
            if (task == null)
            {
                return NotFound();
            }

            await _context.Remove(id);
            await _context.SaveChangesAsync();

            return task;
        }

        private bool TaskExists(long id)
        {
            var task = _context.GetTask(id);

            return task == null ? false : true;
        }
    }
}
