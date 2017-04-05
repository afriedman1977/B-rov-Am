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
    public class SalesController : TwilioController
    {
        //
        // GET: /Sales/

        public ActionResult Index()
        {
            return View();
        }

        public void AddCustomer()
        {

        }

        [HttpPost]
        public TwiMLResult Welcome()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = Url.Action("Menu", "Sales"), numDigits = "1" })
                .Say("Welcome To the Berov am clothing Hotline. press 1 to start shopping, "
                + "press 2 to review a previous order, press 3 to hear deadline and pickup location, "
                + "press 4 to leave a message.", new { voice = "alice", language = "en-GB", timeout = "3" })
                .EndGather();
            response.Redirect("/Sales/Welcome");
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult Menu(string digits, string From)
        {
            var selectedOption = digits;
            //var optionActions = new Dictionary<string, Func<TwiMLResult>>()
            //{
            //    {"1", ChooseItem},
            //    //{"2", ReviewOrder}
            //};

            //return optionActions.ContainsKey(selectedOption) ?
            //return  optionActions[selectedOption]();
            //RedirectWelcome();
            if (selectedOption == "1")
            {
                // return GetCustomer(From);
               return TwiML(new TwilioResponse().Redirect("/Sales/GetCustomer"));
            }
            else if (selectedOption == "2")
            {
                return TwiML(new TwilioResponse().Redirect("/Review/SearchForOrder"));
            }
            return TwiML(new TwilioResponse().Say("Invalid choice").Redirect("/Sales/Welcome"));
        }

        [HttpPost]
        public TwiMLResult GetCustomer(string From)
        {
            var response = new TwilioResponse();
            if (From != null)
            {
                SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
                Customer customer = manager.GetAllCustomers().Where(c => c.PhoneNumber == From.Substring(2)).FirstOrDefault();
                if (customer != null)
                {
                    Session["orderId"] = manager.CreateOrder(customer.CustomerID);
                    return ChooseItem();
                }
                else
                {
                    //customer = new Customer
                    //{
                    //    FirstName = "x",
                    //    LastNmae = "x",
                    //    Address = "x",
                    //    PhoneNumber = From.Substring(2)
                    //};
                    //int cId = manager.AddCustomer(customer);
                    //Session["orderId"] = manager.CreateOrder(cId);
                    //return ChooseItem();
                    response.Redirect("/Sales/VerifyCustomer?phoneNumber=" + From.Substring(2) + "&digits=1");
                    return TwiML(response);
                }
            }
            else
            {
                response.BeginGather(new { action = Url.Action("VerifyNumber", "Sales"), numDigits = "10" })
                    .Say("Please enter your 10 digit Phone Number", new { voice = "alice", language = "en-GB", timeout = "5" })
                    .EndGather();
                response.Redirect("/Sales/GetCustomer");
                return TwiML(response);
            }
        }

        [HttpPost]
        public TwiMLResult VerifyNumber(string digits)
        {
            var response = new TwilioResponse();
            if (digits.Length != 10)
            {
                response.Say("invalid phone number");
                response.Redirect("/Sales/GetCustomer");
                return TwiML(response);
            }
            response.BeginGather(new { action = "/Sales/VerifyCustomer?phoneNumber=" + digits, numDigits = 1 })
                .Say("You entered, " + digits[0] + "," + digits[1] + "," + digits[2] + "," + digits[3] + "," + digits[4] + "," + digits[5] + "," + digits[6] + "," + digits[7] + ","
                + digits[8] + "," + digits[9] + ". To confirm press 1, to re enter your phone number press 2.", new { voice = "alice", language = "en-US", timeout = "100" })
                .EndGather();
            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult VerifyCustomer(string phoneNumber, string digits)
        {
            var response = new TwilioResponse();
            if (digits == "2")
            {
                return TwiML(response.Redirect("/Sales/GetCustomer"));
            }

            //SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
            //Customer customer = manager.GetAllCustomers().Where(c => c.PhoneNumber == phoneNumber).FirstOrDefault();
            //if (customer != null)
            //{
            //    Session["orderId"] = manager.CreateOrder(customer.CustomerID);
            //    return ChooseItem();
            //}
            else if (digits == "1")
            {
                response.Say("Please record your name and address after the beep. press the pound key when you are done", new { voice = "alice", language = "en-GB", timeout = "100" });
                response.Record(new { action = "/Sales/CaptureRecording?phoneNumber=" + phoneNumber, method = "GET", finishOnKey = "#" });
                return TwiML(response);
            }
            else
            {
                return TwiML(response.Say("Invalid choice").Redirect("/Sales/VerifyNumber?digits=" + phoneNumber));
            }
            // return GetCustomer();
        }

        [HttpGet]
        public TwiMLResult CaptureRecording(string phoneNumber, string RecordingUrl)
        {
            Customer customer = new Customer
             {
                 FirstName = "x",
                 LastNmae = "x",
                 Address = "x",
                 PhoneNumber = phoneNumber
             };
            SalesManager manager = new SalesManager(Properties.Settings.Default.constr);
            int cId = manager.AddCustomer(customer);
            Session["orderId"] = manager.CreateOrder(cId);
            return ChooseItem();
        }

        [HttpPost]
        public TwiMLResult ChooseItem()
        {
            var response = new TwilioResponse();
            response.BeginGather(new { action = Url.Action("Choose", "Items"), numDigits = "3" })
                .Say("Please enter an item code.", new { voice = "alice", language = "en-GB", timeout = "100" })
                .EndGather();
            response.Redirect("/Sales/ChooseItem");
            return TwiML(response);
        }

        //private static TwiMLResult ReviewOrder()
        //{
        //    var response = new TwilioResponse();
        //    return TwiML(response);
        //}
    }
}
