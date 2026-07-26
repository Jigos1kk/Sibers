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
    public class ProjectTaskController : ControllerBase
    {
        private readonly IProjectTaskService _taskService;

        public ProjectTaskController(IProjectTaskService taskService)
        {
            _taskService = taskService;
        }

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

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProjectTaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var task = await _taskService.ReadAsync(id, ct);

            var response = MapToResponseDto.MapTask(task);

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProjectTaskResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? projectId, CancellationToken ct)
        {
            var tasks = await _taskService.ReadAsync(projectId, ct);

            var response = tasks.ConvertAll(t => MapToResponseDto.MapTask(t));

            return Ok(response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ProjectTaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectTaskRequestDto req, CancellationToken ct)
        {
            await _taskService.UpdateAsync(id, req, ct);

            return Ok();
        }

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