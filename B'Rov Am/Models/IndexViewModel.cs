using BRovAm.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B_Rov_Am.Models
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            AllProducts = new List<ProductModel>();
        }

        public List<ProductModel> AllProducts { get; set; }         
        public IEnumerable<Category> AllCategories { get; set; }        
    }
}