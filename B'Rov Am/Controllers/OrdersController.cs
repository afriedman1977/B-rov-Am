using B_Rov_Am.Models;
using BRovAm.data;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            odvm.OrderNumber = orderId;
            return View(odvm);
        }

        public ActionResult OrderDetailsForPrint(int orderId, int customerId)
        {
            OrdersManager OManager = new OrdersManager(Properties.Settings.Default.constr);
            OrderDetailsViewModel odvm = new OrderDetailsViewModel();
            odvm.OrderDetails = OManager.GetOrderDetailsByOrderId(orderId);
            odvm.Customer = OManager.GetCustomerById(customerId);
            odvm.OrderNumber = orderId;
            return View(odvm);
        }

        public ActionResult PrintInvoice(int orderId, int customerId)
        {
            return new ActionAsPdf("OrderDetailsForPrint", new { orderId = orderId,  customerId =  customerId });
        }

    }
}
