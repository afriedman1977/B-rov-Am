using BRovAm.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B_Rov_Am.Models
{
    public class OrderDetailsViewModel
    {
        public IEnumerable<OrderDetailExpanded> OrderDetails { get; set; }
        public Customer Customer { get; set; }
        public Order Order { get; set; }
    }
}