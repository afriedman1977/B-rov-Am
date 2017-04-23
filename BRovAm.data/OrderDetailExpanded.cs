using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRovAm.data
{
    public class OrderDetailExpanded
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Decimal TotalPrice { get; set; }
    }
}
