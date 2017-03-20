using BRovAm.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B_Rov_Am.Models
{
    public class EnterColorAndSizesViewModel
    {
        public Product CurrentProduct { get; set; }
        public List<Color> AllColors { get; set; }
        public List<Size> AllSizes { get; set; }
        public List<ProductsColorsSizes> CollorsSizesForproduct { get; set; }
        public ProductsColorsSizes CurrentproductColorsize { get; set; }
        public int ColorId { get; set; }
    }
}