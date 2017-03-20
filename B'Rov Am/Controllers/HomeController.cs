using B_Rov_Am.Models;
using BRovAm.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Speech.Synthesis;

namespace B_Rov_Am.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            IndexViewModel ivm = new IndexViewModel();
            foreach(Product p in manager.GetAllProducts())
            {
                ivm.AllProducts.Add(new ProductModel
                {
                    Product = p,
                    Colors = manager.GetColorForProduct(p.Id),
                    Sizes = manager.GetSizesForProduct(p.Id)
                });
            }            
            ivm.AllCategories = manager.GetAllCategories();
            return View(ivm);
        }

        [HttpPost]
        public ActionResult AddProduct(string styleNumber, string brand, string description, decimal price, int categoryId)
        {
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            int currentId = manager.AddProduct(styleNumber, brand, description, price, categoryId);
            return Redirect("/Home/EnterColorAndSizes?id=" + currentId);            
        }

        public ActionResult EnterColorAndSizes(int id ,int? colorId)
        {
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            EnterColorAndSizesViewModel ecsvm = new EnterColorAndSizesViewModel();
            ecsvm.CurrentProduct = manager.GetProductById(id);
            ecsvm.AllColors = manager.GetAllColors().ToList();
            ecsvm.AllSizes = manager.GetAllSizes().ToList();
            ecsvm.CollorsSizesForproduct = manager.GetColorsAndSizesForProduct(id).ToList();
            if (colorId == null)
            {
                ecsvm.ColorId = 1;
                
            }
            else
            {
                ecsvm.ColorId = colorId.Value;
            }
            return View(ecsvm);
           // return Json(ecsvm, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult SubmitColorAndSizes(List<ProductsColorsSizes> pcs)
        {
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            manager.AddColorSizes(pcs);
            return Redirect("/Home/EnterColorAndSizes?id=" +pcs[0].ProductId);
        }

        public ActionResult GetProductById(int id)
        {
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            return Json(manager.GetProductById(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditProduct(Product p)
        {
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            manager.EditProduct(p);
            return Redirect("/Home/EnterColorAndSizes?id=" + p.Id);
        }

        public ActionResult GetDetails(int id)
        {
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            DetailsViewModel dvm = new DetailsViewModel();
            dvm.DetailsForProduct = manager.GetDetailsForProduct(id);
            dvm.CurrentProduct = manager.GetProductById(id);
            return View(dvm);
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);            
            manager.DeleteProduct(id);
            return Redirect("/Home/Index");
        }

        public ActionResult SpeakDescription(string description)
        {
            using(SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                synth.Speak(description);
            }
            return View();
        }
    }
}
