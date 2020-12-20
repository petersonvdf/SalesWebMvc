using System.Collections.Generic;

namespace SalesWebMvc.Models.ViewModels
{
    public class SalesRecordFormViewModel
    {
        public List<Seller> Sellers { get; set; }
        public SalesRecord SalesRecord { get; set; }
    }
}
