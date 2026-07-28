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
    /// Controller responsible for managing projects.
    /// Provides CRUD operations for project entities including creation, retrieval, update, and deletion.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectController"/> class.
        /// </summary>
        /// <param name="projectService">The project service for business logic operations.</param>
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Creates a new project with the specified details.
        /// </summary>
        /// <param name="request">The project creation data including name, dates, priority, companies, manager, and employees.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 201 Created response with the created project details.</returns>
        /// <response code="201">Returns the newly created project.</response>
        /// <response code="400">If the request data is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ProjectRequestDto request, CancellationToken ct)
        {
            var project = await _projectService.CreateAsync(request, ct);

            var response = MapToResponseDto.MapProject(project);

            return CreatedAtAction(
                actionName: nameof(GetById), 
                routeValues: new { id = project.Id }, 
                value: response);
        }

        /// <summary>
        /// Retrieves a specific project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response with the project details.</returns>
        /// <response code="200">Returns the requested project.</response>
        /// <response code="404">If the project with the specified ID does not exist.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var project = await _projectService.ReadAsync(id, ct);

            var response = MapToResponseDto.MapProject(project);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves all projects with optional filtering and sorting.
        /// </summary>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <param name="filter">Optional filter and sort parameters for querying projects.</param>
        /// <returns>A 200 OK response with a list of projects.</returns>
        /// <response code="200">Returns the list of projects.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProjectResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct, [FromQuery] ProjectFilterQueryDto? filter)
        {
            var projects = await _projectService.ReadAsync(filter, ct);

            var response = projects.ConvertAll(p => MapToResponseDto.MapProject(p));

            return Ok(response);
        }

        /// <summary>
        /// Updates an existing project with the specified changes.
        /// </summary>
        /// <param name="id">The unique identifier of the project to update.</param>
        /// <param name="req">The updated project data.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response if the update was successful.</returns>
        /// <response code="200">If the project was successfully updated.</response>
        /// <response code="404">If the project with the specified ID does not exist.</response>
        /// <response code="400">If the request data is invalid.</response>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectRequestDto req, CancellationToken ct)
        {
            await _projectService.UpdateAsync(id, req, ct);

            return Ok();
        }

        /// <summary>
        /// Deletes a project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project to delete.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response if the deletion was successful.</returns>
        /// <response code="200">If the project was successfully deleted.</response>
        /// <response code="404">If the project with the specified ID does not exist.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _projectService.DeleteAsync(id, ct);

            return Ok();
        }
    }
}