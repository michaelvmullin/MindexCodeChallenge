using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/reportingstructure")]
    public class ReportingStructureController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public ReportingStructureController(ILogger<ReportingStructureController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpGet("{id}", Name = "getReportingStructureByEmployeeId")]
        public IActionResult GetReportingStructureByEmployeeId(String id)
        {
            _logger.LogDebug($"Received reporting get request for '{id}'");

            ReportingStructure reportingStructure = new ReportingStructure();
            reportingStructure.Employee = _employeeService.GetById(id);

            if (reportingStructure.Employee == null)
                return NotFound();

            reportingStructure.NumberOfReports = GetNumberOfReports(reportingStructure.Employee.DirectReports);

            return Ok(reportingStructure);
        }

        public int GetNumberOfReports(List<Employee> employees)
        {
            int numberOfReports = 0;

            if (employees == null || employees.Count < 1)
            {
                return numberOfReports;
            }

            foreach (Employee employee in employees)
            {
                numberOfReports++;
                // Use recursion to include DirectReports of all employees in the hierarchy.
                numberOfReports += GetNumberOfReports(_employeeService.GetById(employee.EmployeeId)?.DirectReports);
            }

            return numberOfReports;
        }
    }
}
