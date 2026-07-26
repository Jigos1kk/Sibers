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
    /// Controller responsible for managing employees.
    /// Provides CRUD operations for employee entities including creation, retrieval, update, and deletion.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeController"/> class.
        /// </summary>
        /// <param name="employeeService">The employee service for business logic operations.</param>
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Creates a new employee with the specified personal details.
        /// </summary>
        /// <param name="req">The employee creation data including first name, last name, middle name, and email.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 201 Created response with the created employee details.</returns>
        /// <response code="201">Returns the newly created employee.</response>
        /// <response code="400">If the request data is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] EmployeeRequestDto req, CancellationToken ct)
        {
            var employee = await _employeeService.CreateAsync(req, ct);

            var response = MapToResponseDto.MapEmployee(employee);

            return CreatedAtAction(
                actionName: nameof(GetById), 
                routeValues: new { id = employee.Id }, 
                value: response);
        }

        /// <summary>
        /// Retrieves a specific employee by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response with the employee details.</returns>
        /// <response code="200">Returns the requested employee.</response>
        /// <response code="404">If the employee with the specified ID does not exist.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var employee = await _employeeService.ReadAsync(id, ct);

            var response = MapToResponseDto.MapEmployee(employee);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves all employees.
        /// </summary>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response with a list of all employees.</returns>
        /// <response code="200">Returns the list of employees.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var employee = await _employeeService.ReadAsync(ct);

            var response = employee.ConvertAll(e => MapToResponseDto.MapEmployee(e));

            return Ok(response);
        }

        /// <summary>
        /// Searches employees by name with partial matching.
        /// Used for autocomplete dropdowns in the project wizard.
        /// </summary>
        /// <param name="term">The search term to filter employees by (matches first name, last name, or middle name).</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response with a list of matching employees.</returns>
        /// <response code="200">Returns the list of matching employees.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<EmployeeResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string term, CancellationToken ct)
        {
            var employees = await _employeeService.SearchAsync(term, ct);

            var response = employees.ConvertAll(e => MapToResponseDto.MapEmployee(e));

            return Ok(response);
        }

        /// <summary>
        /// Updates an existing employee's personal details.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to update.</param>
        /// <param name="req">The updated employee data.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response if the update was successful.</returns>
        /// <response code="200">If the employee was successfully updated.</response>
        /// <response code="404">If the employee with the specified ID does not exist.</response>
        /// <response code="400">If the request data is invalid.</response>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeRequestDto req, CancellationToken ct)
        {
            await _employeeService.UpdateAsync(id, req, ct);

            return Ok();
        }

        /// <summary>
        /// Deletes an employee by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to delete.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response if the deletion was successful.</returns>
        /// <response code="200">If the employee was successfully deleted.</response>
        /// <response code="404">If the employee with the specified ID does not exist.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _employeeService.DeleteAsync(id, ct);

            return Ok();
        }
    }
}