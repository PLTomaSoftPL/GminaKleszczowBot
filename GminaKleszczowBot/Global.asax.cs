using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Timers;
using System.Data;
using System.Threading.Tasks;

namespace GksKatowiceBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        [NonSerialized]
        static DataTable dt = new DataTable();
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);


            Helpers.BaseDB.AddToLog("Wywołanie metody Application_Start");

            var aTimer = new System.Timers.Timer();
            aTimer.Interval = 3 * 60 * 1000;

            aTimer.Elapsed += OnTimedEvent;
            aTimer.Start();
        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (DateTime.UtcNow.Hour == 14 && (DateTime.UtcNow.Minute > 30 && DateTime.UtcNow.Minute <= 33))
            {
                dt = Helpers.BaseGETMethod.GetUser();
                foreach (DataRow dr in dt.Rows)
                {
                    Task.Run(() => Controllers.ThreadClass.SendThreadMessage(dr));
                }
            }
        }
    }
}
