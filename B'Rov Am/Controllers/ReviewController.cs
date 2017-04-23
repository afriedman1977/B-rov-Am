using BRovAm.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace B_Rov_Am.Controllers
{
    public class ReviewController : TwilioController
    {
        SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
        private static int _index;
       

        public ActionResult Index()
        {
            //SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
            //Order currentOrder = manager.GetOrder(1);
            return View();
        }

        public TwiMLResult SearchForOrder(string From)
        {
            var response = new TwilioResponse();
            if (From == null)
            {
                response.BeginGather(new { action = "/Review/FindOrder", numDigits = "10" })
                    .Say("Please enter your 10 digit phone number", new { voice = "alice", language = "en-US" })
                    .EndGather();
                response.Redirect("/Review/SearchForOrder");
            }
            else
            {
                response.Redirect("/Review/FindOrder?digits=" + From.Substring(2));
            }
            return TwiML(response);
        }

        public TwiMLResult FindOrder(string digits)
        {
            var response = new TwilioResponse();
            SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            Customer customer = manager.GetAllCustomers().Where(c => c.PhoneNumber == digits).FirstOrDefault();
            int cId = customer != null ? customer.CustomerID : 0;
            Order order = REManager.GetAllOrders().Where(o => o.CustomerID == cId).FirstOrDefault();

            if (customer == null || order == null)
            {
                response.BeginGather(new { action = "/Review/ReEnterPhoneNumber", numDigits = "1" })
                    .Say("We couldn't find an order associated with your phone number. To search with a different phone number press 1, "
                    + " to go to the main menu press 2.", new { voice = "alice", language = "en-US" })
                    .EndGather();
                response.Redirect("/Review/FindOrder?digits=" + digits);
            }
            else
            {
                Session["customerId"] = customer.CustomerID;
                Session["orderId"] = order.OrderID;
                response.Redirect("/Review/ReviewOptions");
            }
            return TwiML(response);
        }

        public TwiMLResult ReEnterPhoneNumber(string digits)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/SearchForOrder?From=" + null);
            }
            else
            {
                response.Redirect("/Sales/Welcome");
            }
            return TwiML(response);
        }

        public TwiMLResult ReviewOptions()
        {
            _index = 0;
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/ReviewChoice", numDigits = 1 })
                .Say("To review your entire order press 1, to review a specific item in your order press 2, to add an item to your order press 3,"
                + " to return to the main menu press 4.", new { voice = "alice", language = "en-US" })
                .EndGather();
            response.Redirect("/Review/ReviewOptions");
            return TwiML(response);

        }

        public TwiMLResult ReviewChoice(string digits)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/ReviewEntireOrder");
            }
            else if (digits == "2")
            {
                response.Redirect("/Review/EnterDetail");
            }
            else if (digits == "3")
            {
                response.Redirect("/Sales/ChooseItem");
            }
            else if (digits == "4")
            {
                response.Redirect("/Sales/Welcome");
            }
            else
            {
                response.Say("invalid choice");
                response.Redirect("/Review/ReviewOptions");
            }
            return TwiML(response);
        }

        public TwiMLResult ReviewEntireOrder()
        {
            var response = new TwilioResponse();
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            List<OrderDetail> orderDetails = REManager.GetOrderDetailsByOrderId((int)Session["orderId"]).ToList();
            if(orderDetails.Count == 0)
            {
                response.Say("We could not find any items in this order");
                response.Redirect("/Review/ReviewOptions");
            }
            else if (_index == orderDetails.Count)
            {
                response.Say("there are no more items to review");
                response.Redirect("/Review/ReviewOptions");
            }
            else
            {
                Session["OrderDetailID"] = orderDetails[_index].OrderDetailID;
                List<Product> products = manager.GetAllProducts().ToList();
                Product product = products.Where(p => p.ProductId == orderDetails[_index].ProductID).FirstOrDefault();
                Color color = manager.GetAllColors().Where(c => c.ColorId == orderDetails[_index].ColorID).FirstOrDefault();
                Size size = manager.GetAllSizes().Where(s => s.SizeId == orderDetails[_index].SizeID).FirstOrDefault();
                response.BeginGather(new { action = "/Review/ChooseEdit", numDigits = "1" })
                    .Say("you chose " + orderDetails[_index].Quantity + " " + color.ProductColor + " " + product.Description + " size " + size.ProductSize
                    + " to change the quantity press 1, to change the color press 2, to change the size press 3, to delete this item from your cart "
                    + "press 4, to hear the next item in your cart press 5, to return to the previous menu press 6, to return to "
                    + "the main menu press 7.", new { voice = "alice", language = "en-US" })
                    .EndGather();
                response.Redirect("/Review/ReviewEntireOrder");
            }
            return TwiML(response);
        }

        public TwiMLResult ChooseEdit(string digits)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/EditQuantity");
            }
            else if (digits == "2")
            {
                response.Redirect("/Review/EditColor");
            }
            else if (digits == "3")
            {
                response.Redirect("/Review/EditSize");
            }
            else if (digits == "4")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.DeleteOrderDetail((int)Session["OrderDetailID"]);
            }
            else if (digits == "5")
            {
                _index++;
                response.Redirect("/Review/ReviewEntireOrder");
            }
            else if (digits == "6")
            {
                response.Redirect("/Review/ReviewOptions");
            }
            else if (digits == "7")
            {
                response.Redirect("/Sales/Welcome");
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Review/ReviewEntireOrder");
            }
            return TwiML(response);
        }

        public TwiMLResult EditQuantity()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/ReviewQuantity", numDigits = "2" })
                    .Say("Please enter a quantity", new { voice = "alice", language = "en-US" })
                    .EndGather();
            response.Redirect("/Review/EditQuantity");
            return TwiML(response);
        }

        public TwiMLResult ReviewQuantity(string digits)
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/ConfirmQuantity?qty=" + int.Parse(digits), numDigits = "1" })
                .Say("you have chosen to update the quantity to, " + digits + " , to enter the quantity again, press 1. to confirm and review the next item in your cart "
                + "press 2. to confirm and change the color of your item press 3. to confirm and change the size of your item press 4.", new { voice = "alice", language = "en-US" })
                .EndGather();
            response.Redirect("/Review/ReviewQuantity?digits=" + digits);
            return TwiML(response);
        }

        public TwiMLResult ConfirmQuantity(string digits, int qty)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/EditQuantity");
            }
            else if (digits == "2")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateQuantity(qty, (int)Session["OrderDetailID"]);
                decimal price = manager.AddQuantityToOrderDetail((int)Session["orderDetailId"], qty, (int)Session["orderId"]);
                response.Say("Quantity successfully updated. your updated price for this item is " + price + " dollars.");
                _index++;
                response.Redirect("/Review/ReviewEntireOrder");
            }
            else if (digits == "3")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateQuantity(qty, (int)Session["OrderDetailID"]);
                decimal price = manager.AddQuantityToOrderDetail((int)Session["orderDetailId"], qty, (int)Session["orderId"]);
                response.Say("Quantity successfully updated. your updated price for this item is " + price + " dollars.");
                response.Redirect("/Review/EditColor");
            }
            else if (digits == "4")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateQuantity(qty, (int)Session["OrderDetailID"]);
                decimal price = manager.AddQuantityToOrderDetail((int)Session["orderDetailId"], qty, (int)Session["orderId"]);
                response.Say("Quantity successfully updated. your updated price for this item is " + price + " dollars.");
                response.Redirect("/Review/EditSize");
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Review/ReviewQuantity?digits=" + qty.ToString());
            }
            return TwiML(response);
        }

        public TwiMLResult EditColor()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/ReviewColor", numDigits = "1" })
                    .Say("Please enter a color code", new { voice = "alice", language = "en-US" })
                    .EndGather();
            response.Redirect("/Review/EditColor");
            return TwiML(response);
        }

        public TwiMLResult ReviewColor(string digits)
        {
            var response = new TwilioResponse();
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            Color color = manager.GetAllColors().Where(c => c.ColorId.ToString() == digits).FirstOrDefault();
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            OrderDetail od = REManager.GetOrderDetailById((int)Session["OrderDetailID"]);
            List<ProductsColorsSizes> pcs = manager.GetAllProductSizeColors().Where(pc => pc.ProductId == od.ProductID && pc.SizeId == od.SizeID && pc.ColorId.ToString() == digits).ToList();
            if (color == null)
            {
                response.Say("invalid color code, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Review/EditColor");
            }
            else if (pcs.Count() == 0)
            {
                response.Say("Item not available in this color, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Review/EditColor");
            }
            else
            {
                response.BeginGather(new { action = "/Review/ConfirmColor?colorCode=" + int.Parse(digits), numDigits = "1" })
                    .Say("you have chosen to update the color to, " + color.ProductColor + " , to enter the color again, press 1. to confirm and review the next item in your cart "
                    + "press 2. to confirm and change the size of your item press 3.", new { voice = "alice", language = "en-US" })
                    .EndGather();
                response.Redirect("/Review/ReviewColor?digits=" + digits);
            }
            return TwiML(response);
        }

        public TwiMLResult ConfirmColor(string digits, int colorCode)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/EditColor");
            }
            else if (digits == "2")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateQuantity(colorCode, (int)Session["OrderDetailID"]);
                _index++;
                response.Redirect("/Review/ReviewEntireOrder");
            }
            else if (digits == "3")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateColor(colorCode, (int)Session["OrderDetailID"]);
                response.Redirect("/Review/EditSize");
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Review/ReviewColor?digits=" + colorCode.ToString());
            }
            return TwiML(response);
        }

        public TwiMLResult EditSize()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/ReviewSize", numDigits = "1" })
                    .Say("Please enter a size code", new { voice = "alice", language = "en-US" })
                    .EndGather();
            response.Redirect("/Review/EditSize");
            return TwiML(response);
        }

        public TwiMLResult ReviewSize(string digits)
        {
            var response = new TwilioResponse();
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            Size size = manager.GetAllSizes().Where(s => s.SizeId.ToString() == digits).FirstOrDefault();
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            OrderDetail od = REManager.GetOrderDetailById((int)Session["OrderDetailID"]);
            List<ProductsColorsSizes> pcs = manager.GetAllProductSizeColors().Where(pc => pc.ProductId == od.ProductID && pc.ColorId == od.ColorID && pc.SizeId.ToString() == digits).ToList();
            if (size == null)
            {
                response.Say("invalid size code, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Review/EditSize");
            }
            else if (pcs.Count() == 0)
            {
                response.Say("Item not available in this color, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Review/EditSize");
            }
            else
            {
                response.BeginGather(new { action = "/Review/ConfirmSize?sizeCode=" + int.Parse(digits), numDigits = "1" })
                    .Say("you have chosen to update the size to, " + size.ProductSize + " , to enter the size again, press 1. to confirm and review the next item in your cart "
                    + "press 2.", new { voice = "alice", language = "en-US" })
                    .EndGather();
                response.Redirect("/Review/ReviewSize?digits=" + digits);
            }
            return TwiML(response);
        }

        public TwiMLResult ConfirmSize(string digits, int sizeCode)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/EditSize");
            }
            else if (digits == "2")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateSize(sizeCode, (int)Session["OrderDetailID"]);
                _index++;
                response.Redirect("/Review/ReviewEntireOrder");
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Review/ReviewSize?digits=" + sizeCode.ToString());
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult EnterDetail()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/FindDetail", numDigits = "3" })
               .Say("please enter the item code of the item you want to edit.", new { voice = "alice", language = "en-US" })
               .EndGather();
            response.Redirect("Review/EnterDetail");
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult FindDetail(string digits)
        {
            var response = new TwilioResponse();
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            BRovAmManager BAmanager = new BRovAmManager(Properties.Settings.Default.constr);
            Product product = BAmanager.GetAllProducts().Where(p => p.ItemCode == digits).FirstOrDefault();
            List<OrderDetail> details = REManager.GetOrderDetailsByItemCode((int)Session["orderId"], product.ProductId).ToList();
            if (details == null || details.Count == 0)
            {
                response.Say("we could not find that item in your order", new { voice = "alice", language = "en-US" });
                response.Redirect("/Review/EnterDetail");
            }
            else if (details.Count > 1)
            {
                response.BeginGather(new { action = "/Review/FindBySize?productCode=" + int.Parse(digits), numDigits = "2" })
                    .Say("we found multiple of that item in your order, to narrow it down please enter the size code of the item you want to edit", new { voice = "alice", language = "en-US" })
                    .EndGather();
            }
            else
            {
                Session["OrderDetailID"] = details[0].OrderDetailID;
                response.Redirect("/Review/ReviewDetail");
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult FindBySize(int productCode, string digits)
        {
            var response = new TwilioResponse();
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            List<OrderDetail> details = REManager.GetOrderDetailsByItemCode((int)Session["orderId"], productCode).Where(d => d.SizeID == int.Parse(digits)).ToList();
            if (details == null || details.Count == 0)
            {
                response.Say("we could not find that item in that size in your order", new { voice = "alice", language = "en-US" });
                response.Redirect("/Review/FindDetail?digits=" + productCode);
            }
            else if (details.Count > 1)
            {
                response.BeginGather(new { action = "/Review/FindByColor?productCode=" + productCode + "&sizeCode=" + int.Parse(digits), numDigits = "2" })
                    .Say("we found multiple of that item in that size in your order, to narrow it down please enter the color code of the item you want to edit", new { voice = "alice", language = "en-US" })
                    .EndGather();
            }
            else
            {
                Session["OrderDetailID"] = details[0].OrderDetailID;
                response.Redirect("/Review/ReviewDetail");
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult FindByColor(int productCode, int sizeCode, string digits)
        {
            var response = new TwilioResponse();
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            List<OrderDetail> details = REManager.GetOrderDetailsByItemCode((int)Session["orderId"], productCode).Where(d => d.SizeID == sizeCode && d.ColorID == int.Parse(digits)).ToList();
            if (details == null || details.Count == 0)
            {
                response.Say("we could not find that item in that size and that color in your order", new { voice = "alice", language = "en-US" });
                response.Redirect("/Review/FindBySize?productCode=" + productCode + "&digits=" + sizeCode);
            }
            else
            {
                Session["OrderDetailID"] = details[0].OrderDetailID;
                response.Redirect("/Review/ReviewDetail");
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult ReviewDetail()
        {
            var response = new TwilioResponse();
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            OrderDetail orderDetail = REManager.GetOrderDetailById((int)Session["OrderDetailID"]);
            Product product = manager.GetAllProducts().Where(p => p.ProductId == orderDetail.ProductID).FirstOrDefault();
            Color color = manager.GetAllColors().Where(c => c.ColorId == orderDetail.ColorID).FirstOrDefault();
            Size size = manager.GetAllSizes().Where(s => s.SizeId == orderDetail.SizeID).FirstOrDefault();
            response.BeginGather(new { action = "/Review/ChooseEdit1", numDigits = "1" })
                .Say("you chose " + orderDetail.Quantity + " " + color.ProductColor + " " + product.Description + " size " + size.ProductSize
                + " to change the quantity press 1, to change the color press 2, to change the size press 3, to delete this item from your cart "
                + "press 4, to return to the previous menu press 5, to return to "
                + "the main menu press 6.", new { voice = "alice", language = "en-US" })
                .EndGather();
            response.Redirect("/Review/ReviewDetail");
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult ChooseEdit1(string digits)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/EditQuantity1");
            }
            else if (digits == "2")
            {
                response.Redirect("/Review/EditColor1");
            }
            else if (digits == "3")
            {
                response.Redirect("/Review/EditSize1");
            }
            else if (digits == "4")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.DeleteOrderDetail((int)Session["OrderDetailID"]);
            }
            else if (digits == "5")
            {
                response.Redirect("/Review/ReviewOptions");
            }
            else if (digits == "6")
            {
                response.Redirect("/Sales/Welcome");
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Review/ReviewDetail");
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult EditQuantity1()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/ReviewQuantity1", numDigits = "2" })
                    .Say("Please enter a quantity", new { voice = "alice", language = "en-US" })
                    .EndGather();
            response.Redirect("/Review/EditQuantity1");
            return TwiML(response);
        }

        public TwiMLResult ReviewQuantity1(string digits)
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/ConfirmQuantity1?qty=" + int.Parse(digits), numDigits = "1" })
                .Say("you have chosen to update the quantity to, " + digits + " , to enter the quantity again, press 1. to confirm and not make any more changes to this item "
                + "press 2. to confirm and change the color of your item press 3. to confirm and change the size of your item press 4.", new { voice = "alice", language = "en-US" })
                .EndGather();
            response.Redirect("/Review/ReviewQuantity1?digits=" + digits);
            return TwiML(response);
        }

        public TwiMLResult ConfirmQuantity1(string digits, int qty)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/EditQuantity1");
            }
            else if (digits == "2")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateQuantity(qty, (int)Session["OrderDetailID"]);
                decimal price = manager.AddQuantityToOrderDetail((int)Session["orderDetailId"], qty, (int)Session["orderId"]);
                response.Say("Quantity successfully updated. your updated price for this item is " + price + " dollars.");
                response.Redirect("/Review/ReviewOptions");
            }
            else if (digits == "3")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateQuantity(qty, (int)Session["OrderDetailID"]);
                decimal price = manager.AddQuantityToOrderDetail((int)Session["orderDetailId"], qty, (int)Session["orderId"]);
                response.Say("Quantity successfully updated. your updated price for this item is " + price + " dollars.");
                response.Redirect("/Review/EditColor1");
            }
                else if(digits == "4")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateQuantity(qty, (int)Session["OrderDetailID"]);
                decimal price = manager.AddQuantityToOrderDetail((int)Session["orderDetailId"], qty, (int)Session["orderId"]);
                response.Say("Quantity successfully updated. your updated price for this item is " + price + " dollars.");
                response.Redirect("/Review/EditSize1");
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Review/ReviewQuantity1?digits=" + qty.ToString());
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult EditColor1()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/ReviewColor1", numDigits = "1" })
                    .Say("Please enter a color code", new { voice = "alice", language = "en-US" })
                    .EndGather();
            response.Redirect("/Review/EditColor1");
            return TwiML(response);
        }

        public TwiMLResult ReviewColor1(string digits)
        {
            var response = new TwilioResponse();
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            Color color = manager.GetAllColors().Where(c => c.ColorId.ToString() == digits).FirstOrDefault();
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            OrderDetail od = REManager.GetOrderDetailById((int)Session["OrderDetailID"]);
            List<ProductsColorsSizes> pcs = manager.GetAllProductSizeColors().Where(pc => pc.ProductId == od.ProductID && pc.SizeId == od.SizeID && pc.ColorId.ToString() == digits).ToList();
            if (color == null)
            {
                response.Say("invalid color code, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Review/EditColor1");
            }
            else if (pcs.Count() == 0)
            {
                response.Say("Item not available in this color, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Review/EditColor1");
            }
            else
            {
                response.BeginGather(new { action = "/Review/ConfirmColor1?colorCode=" + int.Parse(digits), numDigits = "1" })
                    .Say("you have chosen to update the color to, " + color.ProductColor + " , to enter the color again, press 1. to confirm and not make any more changes to this item "
                    + "press 2. to confirm and change the size of your item press 3.", new { voice = "alice", language = "en-US" })
                    .EndGather();
                response.Redirect("/Review/ReviewColor1?digits=" + digits);
            }
            return TwiML(response);
        }

        public TwiMLResult ConfirmColor1(string digits, int colorCode)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/EditColor1");
            }
            else if (digits == "2")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateQuantity(colorCode, (int)Session["OrderDetailID"]);
                response.Say("Color successfully updated.");
                response.Redirect("/Review/ReviewOptions");
            }
            else if (digits == "3")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateColor(colorCode, (int)Session["OrderDetailID"]);
                response.Say("Color successfully updated.");
                response.Redirect("/Review/EditSize1");
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Review/ReviewColor1?digits=" + colorCode.ToString());
            }
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult EditSize1()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = "/Review/ReviewSize1", numDigits = "1" })
                    .Say("Please enter a size code", new { voice = "alice", language = "en-US" })
                    .EndGather();
            response.Redirect("/Review/EditSize1");
            return TwiML(response);
        }

        public TwiMLResult ReviewSize1(string digits)
        {
            var response = new TwilioResponse();
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            Size size = manager.GetAllSizes().Where(s => s.SizeId.ToString() == digits).FirstOrDefault();
            ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
            OrderDetail od = REManager.GetOrderDetailById((int)Session["OrderDetailID"]);
            List<ProductsColorsSizes> pcs = manager.GetAllProductSizeColors().Where(pc => pc.ProductId == od.ProductID && pc.ColorId == od.ColorID && pc.SizeId.ToString() == digits).ToList();
            if (size == null)
            {
                response.Say("invalid size code, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Review/EditSize1");
            }
            else if (pcs.Count() == 0)
            {
                response.Say("Item not available in this size, please try again", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Redirect("/Review/EditSize1");
            }
            else
            {
                response.BeginGather(new { action = "/Review/ConfirmSize1?sizeCode=" + int.Parse(digits), numDigits = "1" })
                    .Say("you have chosen to update the size to, " + size.ProductSize + " , to enter the size again, press 1. to confirm "
                    + "press 2.", new { voice = "alice", language = "en-US" })
                    .EndGather();
                response.Redirect("/Review/ReviewSize1?digits=" + digits);
            }
            return TwiML(response);
        }

        public TwiMLResult ConfirmSize1(string digits, int sizeCode)
        {
            var response = new TwilioResponse();
            if (digits == "1")
            {
                response.Redirect("/Review/EditSize1");
            }
            else if (digits == "2")
            {
                ReviewEditManager REManager = new ReviewEditManager(Properties.Settings.Default.constr);
                REManager.UpdateSize(sizeCode, (int)Session["OrderDetailID"]);
                response.Say("Size successfully updated.");
                response.Redirect("/Review/ReviewOptions");
            }
            else
            {
                response.Say("Invalid choice");
                response.Redirect("/Review/ReviewSize1?digits=" + sizeCode.ToString());
            }
            return TwiML(response);
        }
    }
}
