using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChangeLog.Core.Services;
using ChangeLog.Core;

namespace ChangeLog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskData>), StatusCodes.Status200OK)]
        public Task<IEnumerable<TaskData>> GetTasks([FromQuery] bool activeOnly = false, int count = 100)
        {
            return _taskService.GetTasks(activeOnly, count);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskData), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskData>> GetTask(int id)
        {
            var task = await _taskService.GetTask(id);
            if (task == null)
            {
                return NotFound();
            }
            return task;
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(TaskData), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskData>> AddTask(string targetBranch, string fromVersion, string toVersion)
        {
            var createdBy = "Anon";
            var payload = new TaskPayload()
            {
                TargetBranch = targetBranch,
                FromVersion = fromVersion,
                ToVersion = toVersion
            };

            var task = await _taskService.AddTask(createdBy, payload);
            if (task == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(AddTask), new { id = task.Id }, task);
        }

        [HttpDelete("{id}/delete")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskService.GetTask(id);
            if (task != null)
            {
                var result = await _taskService.RemoveTask(task.Id);
                if (result)
                {
                    return Accepted();
                }
                return BadRequest();
            }

            return NotFound();
        }

        [HttpPut("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task CancelTask(int id)
        {
            return _taskService.CancelTask(id);
        }
    }
}