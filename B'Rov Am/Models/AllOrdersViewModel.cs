using BRovAm.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B_Rov_Am.Models
{
    public class AllOrdersViewModel
    {
        public IEnumerable<OrderWithCustomer> Orders { get; set; }
    }
}