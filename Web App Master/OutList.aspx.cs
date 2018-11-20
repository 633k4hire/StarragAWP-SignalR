using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Notification.NotificationSystem;

namespace Web_App_Master
{
    public partial class OutList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(OutListSuperButton);
            if (!IsPostBack)
            {
                BindList((from a in Global.AssetCache where a.IsOut == true select a).OrderBy((x) => x.AssetNumber));
            }
        }

        private void BindList(object datasource)
        {
            OutListRepeater.DataSource = datasource;
            OutListRepeater.DataBind();
            OutListUpdatePanel.Update();
        }

        protected void OutListSuperButton_Click(object sender, EventArgs e)
        {
            var cmd = OutListCommand.Text;
            var arg = OutListArgument.Text;
            var arg2 = OutListSuperButtonArg.Text;
            switch (cmd)
            {
                case "sort":
                    switch (arg)
                    {
                        case "assetnumber":
                            BindList((from a in Global.AssetCache where a.IsOut == true select a).OrderBy((x) => x.AssetNumber));
                            break;
                        case "engineer":
                            BindList((from a in Global.AssetCache where a.IsOut == true select a).OrderBy((x) => x.ServiceEngineer));
                            break;
                        case "customer":
                            BindList((from a in Global.AssetCache where a.IsOut == true select a).OrderBy((x) => x.ShipTo));
                            break;
                        default:
                            BindList((from a in Global.AssetCache where a.IsOut == true select a).OrderBy((x) => x.AssetNumber));
                            break;
                    }
                    break;
                default:
                    BindList((from a in Global.AssetCache where a.IsOut == true select a).OrderBy((x) => x.AssetNumber));
                    break;
            }
            
        }

        protected void OutListRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var btn = e.Item.Controls[1];
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btn);
        }

        protected void OutListRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var b = Global.AssetCache;
            var asset = b.FindAssetByNumber(e.CommandName);
            EmailNotice notice = new EmailNotice();
            notice.Data = asset.ShipTo;
            notice.Scheduled = DateTime.Now.AddDays(30);
            notice.NoticeType = NoticeType.Checkout;
            notice.NoticeAction = Global.CheckoutAction;
            
                notice.Assets.Add(e.CommandName);

            notice.NoticeControlNumber = asset.OrderNumber;
            notice.Body = Global.Library.Settings.NotificationMessage;
            notice.Subject = "Asset Return Reminder";
            
            var engineer = (from d in Global.Library.Settings.ServiceEngineers where d.Name == Session["Engineer"] as string select d).FirstOrDefault();
            var statics = (from d in Global.Library.Settings.StaticEmails select d).ToList();
            if (engineer == null)
            {
                engineer = new EmailAddress();
            }
            notice.Emails.Add(engineer);
            notice.Emails.AddRange(statics);
            notice.EmailAddress = engineer;


            EmailHelper.SendNotificationSystemNotice(notice);
        }
    }
}