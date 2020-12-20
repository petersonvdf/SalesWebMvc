using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebMvcContext _context;

        public SalesRecordService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? initial, DateTime? final)
        {
            IQueryable<SalesRecord> result = from sale in _context.SalesRecord
                                             select sale;

            if (initial.HasValue)
            {
                result = result.Where(sale => sale.Date >= initial);
            }

            if (final.HasValue)
            {
                result = result.Where(sale => sale.Date <= final);
            }

            result = result.Include(sale => sale.Seller);
            result = result.Include(sale => sale.Seller.Department);
            result = result.OrderByDescending(sale => sale.Date);

            return await result.ToListAsync();
        }

        public async Task<List<IGrouping<Department, SalesRecord>>> FindByDateGroupingAsync(DateTime? initial, DateTime? final)
        {
            IQueryable<SalesRecord> result = from sale in _context.SalesRecord
                                             select sale;

            if (initial.HasValue)
            {
                result = result.Where(sale => sale.Date >= initial);
            }

            if (final.HasValue)
            {
                result = result.Where(sale => sale.Date <= final);
            }

            result = result.Include(sale => sale.Seller);
            result = result.Include(sale => sale.Seller.Department);
            result = result.OrderByDescending(sale => sale.Date);

            IQueryable<IGrouping<Department, SalesRecord>> retorno = result.GroupBy(sale => sale.Seller.Department);

            return await retorno.ToListAsync();
        }

        public async Task<SalesRecord> FindByIdAsync(int id)
        {
            return await _context.SalesRecord
                .Include(sale => sale.Seller)
                .Include(sale => sale.Seller.Department)
                .FirstOrDefaultAsync(sale => sale.Id == id);
        }

        public async Task InsertAsync(SalesRecord salesRecord)
        {
            _context.SalesRecord.Add(salesRecord);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                SalesRecord sale = await _context.SalesRecord.FindAsync(id);
                _context.SalesRecord.Remove(sale);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new IntegrityException(e.Message);
            }
        }

        public async Task UpdateAsync(SalesRecord sale)
        {
            bool hasAny = await _context.SalesRecord.AnyAsync(x => x.Id == sale.Id);

            if (!hasAny)
            {
                throw new NotFoundException("Id not found");
            }

            try
            {
                _context.SalesRecord.Update(sale);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
