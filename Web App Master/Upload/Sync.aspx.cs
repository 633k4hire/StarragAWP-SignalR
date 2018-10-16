using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Upload
{
    public partial class Sync : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string assetnumber = Request.QueryString["a"];
                
                string pass = Request.QueryString["p"];
                var files = Request.Files;
                if (pass == "5H7fYYgftm54")
                {
                    
                    var asset = Pull.Asset(assetnumber);
                    if (asset.AssetNumber=="0000")
                    {
                        asset.AssetNumber = assetnumber;                        
                    }
                    if (!Directory.Exists(Server.MapPath("/Account/Images/" + assetnumber + "/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("/Account/Images/" + assetnumber + "/"));
                    }
                    foreach (string f in files.Keys)
                    {
                        var file = files[f];
                        file.SaveAs(Server.MapPath("/Account/Images/"+assetnumber+"/" + file.FileName));
                        asset.Images += file.FileName + ",";
                    }
                    Push.Asset(asset);
                    var status = Application["syncstatus"] as string;
                    status += "<br>Asset "+asset.AssetNumber+" Updated at " + DateTime.Now.ToString();
                    Application["syncstatus"] = status;
                    StatusLiteral.Text = status;
                    SyncUpdatePanel.Update();
                }
            }
        }
    }
}