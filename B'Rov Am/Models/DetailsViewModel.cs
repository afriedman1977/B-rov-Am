using BRovAm.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B_Rov_Am.Models
{
    public class DetailsViewModel
    {
        public Product CurrentProduct { get; set; }
        public IEnumerable<ColorSizeQuantity> DetailsForProduct { get; set; }
    }
}