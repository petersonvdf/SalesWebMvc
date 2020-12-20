using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;

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
    }
}
