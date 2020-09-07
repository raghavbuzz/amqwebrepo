using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using amqwebapp.Models;
using Apache.NMS;
using Apache.NMS.Util;

namespace amqwebapp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CheckAMQ(AmqModel model)
        {
            var url = model.Url;
            
            if (url != null && url.Length > 0)
            {
                try
                {
                    OperatorRequestObject operatorRequestObject = new OperatorRequestObject
                    {
                        ShortCode = "ABC"
                    };
                    Uri uri = new Uri("activemq:" + url);
                    IConnectionFactory factory = new Apache.NMS.ActiveMQ.ConnectionFactory(uri);
                    IConnection connection = factory.CreateConnection("admin", "admin");
                    connection.Start();
                    ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                    IDestination queueDestination = SessionUtil.GetDestination(session, "ExampleQueue");
                    IMessageProducer messageProducer = session.CreateProducer(queueDestination);
                    var objMessage = session.CreateObjectMessage(operatorRequestObject);
                    messageProducer.Send(objMessage);
                    session.Close();
                    connection.Stop();
                    ViewData["Message"] = "All Fine";
                }
                catch(Exception ex)
                {
                    ViewData["Message"] = ex.Message;
                }
                
            }
            else
            {
                ViewData["Message"] = "No URL";
            }

            

            return View("Index");
        }
    }

    [Serializable]
    public class OperatorRequestObject
    {
        private string shortcode;

        public string ShortCode
        {
            get { return shortcode; }
            set { shortcode = value; }
        }
    }
}
