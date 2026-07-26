using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;

namespace SibersTest.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing project tasks.
    /// Provides CRUD operations for task entities including creation, retrieval, update, and deletion.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTaskController : ControllerBase
    {
        private readonly IProjectTaskService _taskService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTaskController"/> class.
        /// </summary>
        /// <param name="taskService">The project task service for business logic operations.</param>
        public ProjectTaskController(IProjectTaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Creates a new task within a project.
        /// </summary>
        /// <param name="request">The task creation data including name, comment, priority, status, author, assignee, and project.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 201 Created response with the created task details.</returns>
        /// <response code="201">Returns the newly created task.</response>
        /// <response code="400">If the request data is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectTaskResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ProjectTaskRequestDto request, CancellationToken ct)
        {
            var task = await _taskService.CreateAsync(request, ct);

            var response = MapToResponseDto.MapTask(task);

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = task.Id },
                value: response);
        }

        /// <summary>
        /// Retrieves a specific task by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response with the task details.</returns>
        /// <response code="200">Returns the requested task.</response>
        /// <response code="404">If the task with the specified ID does not exist.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProjectTaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var task = await _taskService.ReadAsync(id, ct);

            var response = MapToResponseDto.MapTask(task);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves all tasks, optionally filtered by project ID.
        /// </summary>
        /// <param name="projectId">Optional project ID to filter tasks by project.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response with a list of tasks.</returns>
        /// <response code="200">Returns the list of tasks.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProjectTaskResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? projectId, CancellationToken ct)
        {
            var tasks = await _taskService.ReadAsync(projectId, ct);

            var response = tasks.ConvertAll(t => MapToResponseDto.MapTask(t));

            return Ok(response);
        }

        /// <summary>
        /// Updates an existing task with the specified changes.
        /// </summary>
        /// <param name="id">The unique identifier of the task to update.</param>
        /// <param name="req">The updated task data.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response if the update was successful.</returns>
        /// <response code="200">If the task was successfully updated.</response>
        /// <response code="404">If the task with the specified ID does not exist.</response>
        /// <response code="400">If the request data is invalid.</response>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ProjectTaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectTaskRequestDto req, CancellationToken ct)
        {
            await _taskService.UpdateAsync(id, req, ct);

            return Ok();
        }

        /// <summary>
        /// Deletes a task by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the task to delete.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response if the deletion was successful.</returns>
        /// <response code="200">If the task was successfully deleted.</response>
        /// <response code="404">If the task with the specified ID does not exist.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ProjectTaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _taskService.DeleteAsync(id, ct);

            return Ok();
        }
    }
}