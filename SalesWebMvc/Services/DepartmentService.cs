using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services
{
    public class DepartmentService
    {
        private readonly SalesWebMvcContext _context;

        public DepartmentService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> FindAllAsync()
        {
            return await _context.Department.ToListAsync();
        }

        public async Task InsertAsync(Department department)
        {
            _context.Department.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task<Department> FindByIdAsync(int id)
        {
            return await _context.Department.FirstOrDefaultAsync(department => department.Id == id);
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                Department department = await _context.Department.FindAsync(id);
                _context.Department.Remove(department);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new IntegrityException("Can't delete department because it has sellers");
            }
        }

        public async Task UpdateAsync(Department department)
        {
            bool hasAny = await _context.Department.AnyAsync(x => x.Id == department.Id);

            if (!hasAny)
            {
                throw new NotFoundException("Id not found");
            }

            try
            {
                _context.Department.Update(department);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
