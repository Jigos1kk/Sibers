using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;

namespace SibersTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="request">Project creation data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Created project details.</returns>
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
        /// Gets a project by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Project details.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var project = await _projectService.ReadAsync(id, ct);

            var response = MapToResponseDto.MapProject(project);

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProjectResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var projects = await _projectService.ReadAsync(ct);

            var response = projects.ConvertAll(p => MapToResponseDto.MapProject(p));

            return Ok(response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectRequestDto req, CancellationToken ct)
        {
            await _projectService.UpdateAsync(id, req, ct);

            return Ok();
        }

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