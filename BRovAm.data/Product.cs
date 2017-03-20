using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRovAm.data
{
    public class Product
    {
        public int ProductId { get; set; }
        public string StyleNumber { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        //public string Color { get; set; }
        //public string Size { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string ItemCode { get; set; }
    }
}
