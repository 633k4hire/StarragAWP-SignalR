using Helpers;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Web_App_Master.App_Start.SignalRHubs;

namespace Web_App_Master.Account
{
    public partial class CheckIn : System.Web.UI.Page
    {
        public void NotifyCheckInEmail(List<List<Asset>> assets)
        {
            if (Global.Library.Settings.TESTMODE == false)
            {
                foreach(var group in assets)
                {
                    try
                    {
                        EmailHelper.SendCheckOutNoticeAsync(group, Global.Library.Settings.CheckInMessage);
                        Global.LogEntry(DateTime.Now.ToString() + " User:" + Page.User.Identity.Name + ": " + "Checkin Email Sent ");
                    }
                    catch { }

                }
                
            }
        }
        public void SaveToUserPersistantLog()
        {
            try
            {
                var ud = Session["PersistingUserData"] as Data.UserData;
                string pdf = Session["CurrentCheckinReport"] as string;
                Data.Attachment a = new Data.Attachment();
                a.Email = ud.Email;
                a.Name = ud.Name;
                a.Files.Add(new Data.FileReference(Session["CurrentCheckInOrdernumber"] as string, pdf));
                ud.Attachments.Add(a);
                ud.Log.Add(new Data.LogEntry("Checked In Order #" + Session["CurrentCheckInOrdernumber"] as string, Session["CurrentCheckInOrdernumber"] as string));
               
                SettingsDBData db = new SettingsDBData();
                db.Appname = ud.Guid;
                db.XmlData = ud.SerializeToXmlString(ud);

                Push.Setting(db);
            }
            catch { }
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.SiteMaster().OnPanelUpdate += Checkout_OnPanelUpdate;
        }
        private void Checkout_OnPanelUpdate(object sender, UpdateRequestEvent e)
        {
            FCIrepeater.DataBind();
            CheckInPageUpdatePanel.Update();
        }

        public void BindCheckIn()
        {
            var checkin = Session["CheckIn"] as List<Asset>;
            if (checkin == null) checkin = new List<Asset>();
            FCIrepeater.DataSource = checkin;
            FCIrepeater.DataBind();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!User.Identity.IsAuthenticated)
                {
                   // Response.Redirect("/Account/Login");
                }
            }
                this.PreInit += Page_PreInit;
            BindCheckIn();
            Button btn = (Button)this.Master.FindControl("UpdateAllCarts");
            AsyncPostBackTrigger Trigger1 = new AsyncPostBackTrigger();
            Trigger1.ControlID = btn.ID;
            Trigger1.EventName = "Click";
            if (CheckInPageUpdatePanel.Triggers.Count == 1)
                CheckInPageUpdatePanel.Triggers.Add(Trigger1);
        }

        protected void Finalize_Click(object sender, EventArgs e)
        {
            //Generate report
            //sort assets in list into groups of OrderNumber
            var list = (from a in Session["CheckIn"] as List<Asset> select a.OrderNumber).Distinct().ToList();
            List<Asset> finalized = new List<Asset>();
            List<string> filenames = new List<string>();
            List<List<Asset>> subEmails = new List<List<Asset>>();
            foreach(var number in list)
            {
                var sublist = (from a in Session["CheckIn"] as List<Asset> where a.OrderNumber == number select a).ToList();
               var files =  CreateReceivingReport(sublist);
                SaveToUserPersistantLog();
                filenames.AddRange(files);
                finalized.AddRange(sublist);
                subEmails.Add(sublist);
            }            
            FinalizeCheckIn(finalized);
            Session["CheckInReportFileNameList"] = filenames;
            //combine all reports into one and display
            CombineReports(filenames);
            CheckInMultiView.ActiveViewIndex = 1;
            
                Finalize.Enabled = false;
                NotifyCheckInEmail(subEmails);
            
            ApplyChangesButton.Visible = false;
            LeavePlaceHolder.Visible = true;

            //Force all clients to update
            this.HubContext<ClientHub>().Clients.All.assetCacheChanged();

        }
        protected void FinalizeCheckIn(List<Asset> assets)
        {
            Push.Alert(assets.Count.ToString()+ " Tools Checked In: "+assets[0].OrderNumber);
            try
            {
                foreach (var asset in assets)
                {
                    asset.IsOut = false;
                    var history = asset.Clone() as Asset;
                    history.IsHistoryItem = true;
                    asset.History.History.Add(history);
                    asset.ShipTo = "";
                    asset.PersonShipping = "";
                    asset.ServiceEngineer = "";
                    asset.DateRecieved = DateTime.Now;
                    //save
                    Asset rem = null;
                    foreach (var a in Global.AssetCache)
                    {
                        if (a.AssetNumber == asset.AssetNumber) rem = a;
                    }
                    if (rem != null)
                    {
                        Global.AssetCache.Remove(rem);
                        Global.AssetCache.Add(asset);
                    }
                    AssetController.UpdateAsset(asset);
                }
                Session["CheckIn"] = new List<Asset>();
            }
            catch {}
            
        }

        protected void CombineReports(List<string> files)
        {
            try
            {
                var guid = Guid.NewGuid().ToString();
                var filename = guid + ".pdf";
                Session["CombinedReportPdf"] = ReportFrame.Src = "/Account/Receiving/" + filename; ;
                var dest = Server.MapPath("/Account/Receiving/" + filename);
                if (files.Count == 0) return;
                if (files.Count==1)
                {
                    
                    Pdf.Flatten(files[0],dest);
                    //File.Copy(files[0], dest);
                    ReportFrame.Src = "/Account/Receiving/" + filename;
                    return;
                }
               
               

                var result = Pdf.Merge(files, dest,true);
                if (result)
                {
                    ReportFrame.Src = "/Account/Receiving/" + filename;

                }
            }
            catch { }

        }


        protected void FinalCheckInRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "delete")
            {
                Asset rem = null;
                foreach (var ass in Session["CheckIn"] as List<Asset>)
                {
                    if (ass.AssetNumber == e.CommandArgument as string)
                    {
                        rem = ass;
                    }
                }
                try
                {
                    (Session["CheckIn"] as List<Asset>).Remove(rem);
                }
                catch { }
                this.UpdateAll();
            }
           
        }

        protected void Unnamed_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb.Checked)
            {
                foreach (var ass in Session["CheckIn"] as List<Asset>)
                {
                    if (ass.AssetNumber == cb.ID)
                    {
                        ass.IsDamaged = true;
                        AssetController.UpdateAsset(ass);
                    }
                }
                this.UpdateAll();
            }
            else
            {
                foreach (var ass in Session["CheckIn"] as List<Asset>)
                {
                    if (ass.AssetNumber == cb.ID)
                    {
                        ass.IsDamaged = false;
                        AssetController.UpdateAsset(ass);
                    }
                }
                this.UpdateAll();
            }
        }

        protected void CheckInViewBtn_Click(object sender, EventArgs e)
        {
            CheckInMultiView.ActiveViewIndex = 0;
        }

        protected void ReportViewBtn_Click(object sender, EventArgs e)
        {
            ReportIcon.Quiet();
            CheckInMultiView.ActiveViewIndex = 1;
        }

        int filecount = 1;
        protected List<string> CreateReceivingReport(List<Asset> assets)
        {
            List<string> filenames = new List<string>();
            try
            {
                //get filename
                string n = DateTime.Now.ToString().Replace("/", "-");
                n = n.Replace(":", "-");
                string info = n + "_" + assets.FirstOrDefault().OrderNumber;
                info = info.Replace("\\", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace(" ", "_");
                string path = "/Account/Receiving/" + info + ".pdf";
                Session["CurrentCheckInOrdernumber"] = assets[0].OrderNumber;
                var ud = Session["SessionUserData"] as Data.UserData;
                ud.Log.Add(new Data.LogEntry("Checked In Order#" + assets[0].OrderNumber, path));
                ReceivingLink.Text = path;
                var checker = Session["CurrentCheckinReport"] as string;
                if (checker==path)
                {
                    path = path.Replace(".pdf", "");
                    path += filecount.ToString();
                    filecount++;
                    path += ".pdf";
                }
                Session["CurrentCheckinReport"] = path;
                foreach(var ass in assets)
                {
                    ass.ReturnReport = "/Account/Receiving/" + info + ".pdf";
                    ass.Documents.Add("/Account/Receiving/" + info + ".pdf");
                }
                var path2 = Server.MapPath(path);
                filenames.Add(path2);
                string from = "";
                string customerattn = assets[0].ShipTo;
                try
                {
                    foreach (var customer in Global.Library.Settings.Customers)
                    {
                        if (customer.CompanyName == assets[0].ShipTo)
                        {
                            from = customer.CompanyName + Environment.NewLine;
                            from += customer.Address + Environment.NewLine;
                            from += customer.Address2 + Environment.NewLine;
                            from += customer.Address3 + Environment.NewLine;
                            from += customer.City + Environment.NewLine;
                            from += customer.State + ", " + customer.Postal + Environment.NewLine;
                            from += customer.Country + Environment.NewLine;
                            customerattn = customer.Attn;
                        }
                    }
                }
                catch { from += customerattn; }
                //From

                //Define Keys

                var headerKeys = new Dictionary<string, string>() {
            {"ShipDate", assets.FirstOrDefault().DateShippedString},
            {"Tracking", assets.FirstOrDefault().PersonShipping},
            {"Signature", customerattn},
            {"Attn", ""},
            {"Po", ""},
            {"To", from},
            {"From", ""},
            {"Weight","Report #::"+ info+".pdf"},
            {"Ordernumber","Order #:"+ assets.FirstOrDefault().OrderNumber} };

                var keys = new Dictionary<string, string>();
                headerKeys.ToList().ForEach(x => keys.Add(x.Key, x.Value));
                int i = 1;
                foreach (var asset in assets)
                {
                    if (i > 13)
                    {
                        //create templated pdf packingslip with default stamping action
                        Pdf.PackingSlip p = new Pdf.PackingSlip(new Templates.StarragReceivingReportTemplate());
                        p.CreateAndFill(path2, keys, p.StampAction);
                        //fill new page with new assets
                        keys = new Dictionary<string, string>();
                        headerKeys.ToList().ForEach(x => keys.Add(x.Key, x.Value));
                        i = 1;
                        path2 = path2.Replace(".pdf", "") + i.ToString() + ".pdf";
                        filenames.Add(path2);
                    }
                    keys.Add("Desc" + i.ToString(), asset.AssetName);
                    keys.Add("PartNo" + i.ToString(), asset.AssetNumber);
                    keys.Add("Qty" + i.ToString(), "1");
                    
                    ++i;
                }

                //create templated pdf packingslip with default stamping action
                Pdf.PackingSlip ps = new Pdf.PackingSlip(new Templates.StarragReceivingReportTemplate());
                var result = ps.CreateAndFill(path2, keys, ps.StampAction);
                
            }
            catch { }
            return filenames;
        }

        protected void TransferAssets_Click(object sender, EventArgs e)
        {
            Asset[] transfers = new Asset[] { };
            transfers = (Session["CheckIn"] as List<Asset>).ToArray();
            var list = (from a in Session["CheckIn"] as List<Asset> select a.OrderNumber).Distinct().ToList();
            List<Asset> finalized = new List<Asset>();
            List<string> filenames = new List<string>();
            List<List<Asset>> subEmails = new List<List<Asset>>();
            foreach (var number in list)
            {
                var sublist = (from a in Session["CheckIn"] as List<Asset> where a.OrderNumber == number select a).ToList();
                var files = CreateReceivingReport(sublist);
                SaveToUserPersistantLog();
                filenames.AddRange(files);
                finalized.AddRange(sublist);
                subEmails.Add(sublist);
            }
            FinalizeCheckIn(finalized);
            Session["CheckInReportFileNameList"] = filenames;
            //combine all reports into one and display
            CombineReports(filenames);

            Session["CheckOut"] = transfers.ToList();
            Response.Redirect("/Account/Outcart.aspx");
        }
    }
}