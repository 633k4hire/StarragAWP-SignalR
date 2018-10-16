using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Account
{
    public partial class PdfViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var a = Server.UrlEncode("/Account/Receiving/1-19-2018_5-58-48_PM_11111.pdf");
            if (!IsPostBack)
            {
                try
                {
                    var b =Request["file"];
                    PdfTitle.Text = Path.GetFileName(b);
                    ReportFrame.Src = b;
                }
                catch { }
            }
        }
    }
}