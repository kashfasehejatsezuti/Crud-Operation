using Microsoft.EntityFrameworkCore;

namespace curd_dotnet_api.Data
{
    public class EmployeeRepository
    { private readonly AppDbContext _appDbContext;
        //Add Dependency to manupulate data
        public EmployeeRepository(AppDbContext appDbContext) {
           _appDbContext = appDbContext;
        }
        //post api
        public async Task AddEmployeeAsync(Employee employee)
        {  
            //Add Employee Model
            await _appDbContext.Set<Employee>().AddAsync(employee);
            //Sql Query run and generate
            await _appDbContext.SaveChangesAsync();
        }

        // Method return the Employee List
        public async Task<List<Employee>> GetAllEmployeeAsync()
        {
            //Add Employee Model
            //    await _appDbContext.Set<Employee>().ToList();

          return  await _appDbContext.Employees.ToListAsync();
        }
        //Method to The Get Employee by Id
        public async Task <Employee> GetEmployeeByIdAsync(int id)
        {
            return await _appDbContext.Employees.FindAsync(id);
        }
        //Method Update the Employee by Id
        public async Task UpdateEmployeeAsync(int id, Employee model)
        {
            var employeee = await _appDbContext.Employees.FindAsync(id);
            if (employeee == null)
            {
                throw new Exception("Employee not found");
            }
            employeee.Name = model.Name;
            employeee.Phone = model.Phone;
            employeee.Age = model.Age;
            employeee.Email = model.Email;  
            employeee.Salary = model.Salary;
            await _appDbContext.SaveChangesAsync();
        }
        //Method Delete the Employee by Id
        public async Task DeleteEmployeeAsnyc(int id)
        {
            var employeee = await _appDbContext.Employees.FindAsync(id);
            if (employeee == null)
            {
                throw new Exception("Employee not found");
            }
            _appDbContext.Employees.Remove(employeee);
            await _appDbContext.SaveChangesAsync();
        }



    }
}
