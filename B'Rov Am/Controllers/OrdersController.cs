using B_Rov_Am.Models;
using BRovAm.data;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace B_Rov_Am.Controllers
{
    public class OrdersController : Controller
    {
        //
        // GET: /Orders/

        public ActionResult AllOrders()
        {
            OrdersManager OManager = new OrdersManager(Properties.Settings.Default.constr);
            AllOrdersViewModel oavm = new AllOrdersViewModel();
            oavm.Orders = OManager.AllOrders();
            return View(oavm);
        }

        public ActionResult OrderDetails(int orderId, int customerId)
        {
            OrdersManager OManager = new OrdersManager(Properties.Settings.Default.constr);
            OrderDetailsViewModel odvm = new OrderDetailsViewModel();
            odvm.OrderDetails = OManager.GetOrderDetailsByOrderId(orderId);
            odvm.Customer = OManager.GetCustomerById(customerId);
            odvm.Order = new SalesManager(Properties.Settings.Default.constr).GetOrder(orderId);
            return View(odvm);
        }

        public ActionResult OrderDetailsForPrint(int orderId, int customerId)
        {
            OrdersManager OManager = new OrdersManager(Properties.Settings.Default.constr);
            OrderDetailsViewModel odvm = new OrderDetailsViewModel();
            odvm.OrderDetails = OManager.GetOrderDetailsByOrderId(orderId);
            odvm.Customer = OManager.GetCustomerById(customerId);
            odvm.Order = new SalesManager(Properties.Settings.Default.constr).GetOrder(orderId);
            return View(odvm);
        }

        public ActionResult PrintInvoice(int orderId, int customerId)
        {
            return new ActionAsPdf("OrderDetailsForPrint", new { orderId = orderId,  customerId =  customerId });
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            //AdministratorRepository repo = new AdministratorRepository(Properties.Settings.Default.constr);
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            Administrator admin = manager.Signin(username, password);
            if (admin == null)
            {
                return Redirect("/Orders/Login");
            }

            FormsAuthentication.SetAuthCookie(username, true);
            return Redirect("/Home/Index");
        }

    }
}
