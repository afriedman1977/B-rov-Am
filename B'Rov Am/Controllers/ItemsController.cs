using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;
using BRovAm.data;

namespace B_Rov_Am.Controllers
{
    public class ItemsController : TwilioController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public TwiMLResult Choose(string digits)
        {
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            List<Product> products = manager.GetAllProducts().ToList();
            Product product = products.Where(p => p.ItemCode == digits).FirstOrDefault();

            var response = new TwilioResponse();
            if (product == null)
            {
                response.Say("invalid item code please try again", new { voice = "alice", language = "en-GB" });
                response.Redirect("/Sales/ChooseItem");
            }
            else
            {
                response.BeginGather(new { action = "/Items/ConfirmItem?pID=" + product.ProductId + "&price=" + product.Price + "&itemCode=" + digits, numDigits = "1" })
                   .Say("You have chosen, " + product.Description + " , to confirm choice, press 1 ."
                   + " to cancel press 2.", new { voice = "alice", language = "en-GB"})
                   .EndGather();
                response.Redirect("/Items/Choose?digits=" + digits);
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult ConfirmItem(int pID, decimal price, int itemCode, string digits)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
                Session["orderDetailId"] = manager.CreateOrderDetail((int)Session["orderId"], pID, price);
                response.Redirect("/Items/ChooseSize?pID=" + pID);
            }
            else if(digits == "2")
            {
                response.Redirect("/Sales/ChooseItem");
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Items/Choose?digits=" + itemCode);
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult ChooseSize(int pID)
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Items/VerifySize?pID=" + pID, numDigits = "2" })
                    .Say("Please enter a size code", new { voice = "alice", language = "en-GB" })
                    .EndGather();
            response.Redirect("/Items/ChooseSize?pID=" + pID);
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult VerifySize(int pID, string digits)
        {
            var response = new TwilioResponse();
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            Size size = manager.GetAllSizes().Where(s => s.SizeId.ToString() == digits).FirstOrDefault();
            List<ProductsColorsSizes> pcs = manager.GetAllProductSizeColors().Where(pc => pc.ProductId == pID && pc.SizeId.ToString() == digits).ToList();
            if (size == null || pcs.Count() == 0)
            {
                response.Say("invalid size code, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Items/ChooseSize?pID=" + pID);
            }
            else
            {
                response.BeginGather(new { action = "/Items/ConfirmSize?pID=" + pID + "&sId=" + size.SizeId, numDigits = "1" })
                    .Say("You have chosen size," + size.ProductSize + ", to confirm press 1, to try again press 2.", new { voice = "alice", language = "en-GB" })
                    .EndGather();
                response.Redirect("/Items/VerifySize?pID=" + pID + "&digits=" + digits);
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult ConfirmSize(int pID, int sId, string digits)
        {
            var response = new TwilioResponse();
            if (digits == "2")
            {
                response.Redirect("/Items/ChooseSize?pID=" + pID);
            }
            else if (digits == "1")
            {
                SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
                manager.AddSizeToOrderDetail((int)Session["orderDetailId"], sId);
                response.Redirect("/Items/ChooseColor?pID=" + pID + "&sId=" + sId);
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Items/VerifySize?pID=" + pID + "&digits=" + sId);
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult ChooseColor(int pID, int sId)
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Items/VerifyColor?pID=" + pID + "&sId=" + sId, numDigits = "1" })
                    .Say("Please enter a color code.", new { voice = "alice", language = "en-GB", timeout = "3" })
                    .EndGather();
            response.Redirect("/Items/ChooseColor?pID=" + pID + "&sId=" + sId);
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult VerifyColor(int pID, int sId, string digits)
        {
            var response = new TwilioResponse();
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            Color color = manager.GetAllColors().Where(c => c.ColorId.ToString() == digits).FirstOrDefault();
            List<ProductsColorsSizes> pcs = manager.GetAllProductSizeColors().Where(pc => pc.ProductId == pID && pc.SizeId == sId && pc.ColorId.ToString() == digits).ToList();
            if (color == null)
            {
                response.Say("invalid color code, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Items/ChooseColor?pID=" + pID + "&sId=" + sId);
            }
            else if (pcs.Count() == 0)
            {
                response.Say("Item not available in this color, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Items/ChooseColor?pID=" + pID + "&sId=" + sId);
            }
            else
            {
                response.BeginGather(new { action = "/Items/ConfirmColor?pID=" + pID + "&sId=" + sId + "&cId=" + color.ColorId, numDigits = "1" })
                    .Say("You have chosen color," + color.ProductColor + ", to confirm press 1, to try again press 2.", new { voice = "alice", language = "en-GB", timeout = "100" })
                    .EndGather();
                response.Redirect("/Items/VerifyColor?pID=" + pID + "&sId=" + sId + "&digits=" + digits);
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult ConfirmColor(int pID, int sId, int cId, string digits)
        {
            var response = new TwilioResponse();
            if (digits == "2")
            {
                response.Redirect("/Items/ChooseColor?pID=" + pID + "&sId=" + sId);
            }
            else if (digits == "1")
            {
                SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
                manager.AddColorToOrderDetail((int)Session["orderDetailId"], cId);
                response.Redirect("/Items/ChooseQuantity?pID=" + pID + "&sId=" + sId);
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Items/VerifyColor?pID=" + pID + "&sId=" + sId + "&digits=" + cId);
            }
            return TwiML(response);
        }

        public TwiMLResult ChooseQuantity()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Items/VerifyQuantity", numDigits = "2", })
                .Say("Please enter a quantity.", new { voice = "alice", language = "en-US" })
                .EndGather();
            response.Redirect("/Items/ChooseQuantity");
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult VerifyQuantity(string digits)
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Items/ConfirmQuantity?qty=" + digits, numDigits = "1" })
                .Say("You have chosen, " + digits + " items to confirm press 1, to re-enter quantity press 2.", new { voice = "alice", language = "en-US" })
                .EndGather();
            response.Redirect("/Items/VerifyQuantity?digits=" + digits);
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult ConfirmQuantity(string qty, string digits)
        {
            var response = new TwilioResponse();
            if (digits != "1" || digits != "2")
            {
                response.Say("inavlid choice", new { voice = "alice", language = "en-US" });
                response.Redirect("/Items/VerifyQuantity?digits=" + qty);
            }
            else if (digits == "2")
            {
                response.Redirect("/Items/ChooseQuantity");
            }
            else
            {
                SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
                decimal price = manager.AddQuantityToOrderDetail((int)Session["orderDetailId"], int.Parse(qty));
                response.BeginGather(new { action = "/Items/ConfirmOrderDetail", numDigits = "1" })
                    .Say("Your total cost for this item is " + price + " dollars. To add another item press 1, to checkout press 2", new { voice = "alice", language = "en-US" })
                    .EndGather();
                response.Redirect("Items/ConfirmQuantity?qty=" + qty + "&digits=" + digits);
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult ConfirmOrderDetail(string digits)
        {
            var response = new TwilioResponse();
            if (digits != "1" || digits != "2")
            {
                response.BeginGather(new { action = "/Items/ConfirmOrderDetail", numDigits = "1" })
                   .Say("To add another item press 1, to checkout press 2", new { voice = "alice", language = "en-US" })
                   .EndGather();
                response.Redirect("Items/ConfirmOrderDetail?digits=" + 3);
            }
            else if (digits == "1")
            {
                response.Redirect("/Sales/ChooseItem");
            }
            else if(digits == "2")
            {

            }
            return TwiML(response);
        }
    }
}
