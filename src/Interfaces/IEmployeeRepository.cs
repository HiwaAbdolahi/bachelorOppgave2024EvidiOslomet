using bachelorOppgave13.Models;

namespace bachelorOppgave13.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllEmployeesAsync();
    Task<Employee> GetEmployeeByIdAsync(string id);
    Task<Employee> GetEmployeeByNameAsync(string firstName, string lastName);
    Task<List<Employee>> GetCheckedInEmployeesAsync();
    Task<List<Employee>> GetCheckedOutEmployeesAsync();
    Task<Employee> CreateEmployeeAsync(Employee employee, List<IFormFile> files);
    Task<bool> UpdateEmployeeAsync(string personId,Employee employee, List<IFormFile> files);
    Task<bool> UpdateCheckedInStatusAsync(Employee employee, bool checkedIn);
    Task<bool> DeleteEmployeeAsync(string personId);
    Task<List<Employee>> GetFilteredEmployeesAsync(string sortBy, string department, string role, Boolean checker);
}