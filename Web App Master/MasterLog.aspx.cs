using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master
{
    public partial class MasterLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindLog();
        }

        protected void LogSuperButton_Click(object sender, EventArgs e)
        {
            BindLog();
        }

        protected void BindLog()
        {
            List<LogEntry> entries = new List<LogEntry>();
            var logs = Pull.MasterLog();
            foreach (var item in logs.)
            {

            }
            LogUpdatePanel.Update();
        }
        public class LogEntry
        {
            public string Entry { get; set; }
        }
    }
}