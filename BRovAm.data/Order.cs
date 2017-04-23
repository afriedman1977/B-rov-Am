using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRovAm.data
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalCost { get; set; }
        public decimal? TotalAmountPaid { get; set; }
        public int? TotalQuantity { get; set; }
    }
}
