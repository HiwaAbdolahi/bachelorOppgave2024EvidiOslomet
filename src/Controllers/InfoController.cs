using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Azure.Cosmos;
using bachelorOppgave13.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Container = Microsoft.Azure.Cosmos.Container;
using bachelorOppgave13.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace bachelorOppgave13.Controllers;


public class InfoController : Controller
{
    private readonly IEmployeeRepository _employeeRepository;

    public InfoController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }
    
    [HttpGet("/Info/GetAllEmployees")]
    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllEmployeesAsync();
        return employees;
    }

    [HttpGet("/Info/GetEmployee/{id}")]
    public async Task<Employee> GetEmployeeByIdAsync(string id)
    {
        var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
        return employee;
    }

    [HttpGet("/Info/GetEmployeeByName/{firstName}/{lastName}")]
    public async Task<Employee> GetEmployeeByNameAsync(string firstName, string lastName)
    {
        var employee = await _employeeRepository.GetEmployeeByNameAsync(firstName, lastName);
        return employee;
    }

    [HttpGet("/Info/GetCheckedInEmployees")]
    public async Task<List<Employee>> GetCheckedInEmployeesAsync()
    {
        var employees = await _employeeRepository.GetCheckedInEmployeesAsync();
        return employees;
    }

    [HttpGet("/Info/GetCheckedOutEmployees")]
    public async Task<List<Employee>> GetCheckedOutEmployeesAsync()
    {
        var employees = await _employeeRepository.GetCheckedOutEmployeesAsync();
        return employees;
    }

    [HttpPost("/Info/CreateEmployee")]
    public async Task<Employee> CreateEmployeeAsync([FromForm] Employee employee,[FromForm] List<IFormFile> files)
    {
        var response = await _employeeRepository.CreateEmployeeAsync(employee, files);
        return response;
    }

    [HttpPut("/Info/UpdateEmployee/")]
    public async Task<bool> UpdateEmployeeAsync([FromForm] Employee employee, [FromForm] List<IFormFile> files )
    {
        var response = await _employeeRepository.UpdateEmployeeAsync(employee.personId.ToString(),employee, files);
        return response;
    }

    [HttpPut("/Info/UpdateCheckedInStatus/{employee}/{checkedIn}")]
    public async Task<bool> UpdateCheckedInStatusAsync(Employee employee, bool checkedIn)
    {
        var response = await _employeeRepository.UpdateCheckedInStatusAsync(employee, checkedIn);
        return response;
    }

    [HttpDelete("/Info/DeleteEmployee")]
    public async Task<bool> DeleteEmployeeAsync(string personId)
    {
        var response = await _employeeRepository.DeleteEmployeeAsync(personId);
        return response;
    }

    [HttpGet("/Info/FilterEmployees")]
    public async Task<List<Employee>> GetFilteredEmployeesAsync(string sortBy, string department, string role, bool checker)
    {
        var filteredEmployees = await _employeeRepository.GetFilteredEmployeesAsync(sortBy, department, role, checker);
        foreach (var emp in filteredEmployees)
        {
            Console.WriteLine(emp.personFirstName + " " + emp.personLastName);
        }

        return filteredEmployees;
    }

}