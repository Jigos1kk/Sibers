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
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="request">Project creation data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Created project details.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] EmployeeRequestDto request, CancellationToken ct)
        {
            var employee = await _employeeService.CreateAsync(request, ct);

            var response = MapToResponseDto.MapEmployee(employee);

            return CreatedAtAction(
                actionName: nameof(GetById), 
                routeValues: new { id = employee.Id }, 
                value: response);
        }

        /// <summary>
        /// Gets a project by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Project details.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var employee = await _employeeService.ReadAsync(id, ct);

            var response = MapToResponseDto.MapEmployee(employee);

            return Ok(response);
        }   
    }
}