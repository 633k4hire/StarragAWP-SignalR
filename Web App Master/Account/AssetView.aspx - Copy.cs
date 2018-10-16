using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

namespace Web_App_Master.Account
{
    public partial class AssetView : System.Web.UI.Page
    {
        public string IsSelected(object num)
        {

            if ((bool)num == true)
                return "element-selected";
            else
                return "";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!Page.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Account/Login");
            }
               
            
                var currentview = Session["CurrentAvView"] as string;
                UpdateView(currentview);

            

            
           // PlaceHolder lbl_UserName = this.Master.FindControl("RibbonBar") as PlaceHolder;
            //lbl_UserName.Visible = true;
        }
        private List<Asset> AssetSearch(string SearchTerm, string SearchFilter="asset")
        {
            try
            {
                List<Asset> Results = new List<Asset>();
                if (SearchFilter.ToLower() == "asset")
                {

                    foreach (var asset in AssetController.GetAllAssets())
                    {

                        try
                        {
                            if (asset.AssetNumber != null)
                            {
                                if (asset.AssetNumber.ToUpper().Contains(SearchTerm.ToUpper()))
                                {

                                    if (!Results.Contains(asset))
                                        Results.Add((Asset)asset.Clone());
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            if (asset.AssetName != null)
                            {
                                if (asset.AssetName.ToUpper().Contains(SearchTerm.ToUpper()))
                                {
                                    if (!Results.Contains(asset))
                                        Results.Add((Asset)asset.Clone());
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            if (asset.Description != null)
                            {
                                if (asset.Description.ToUpper().Contains(SearchTerm.ToUpper()))
                                {
                                    if (!Results.Contains(asset))
                                        Results.Add((Asset)asset.Clone());
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            if (asset.ShipTo != null)
                            {
                                if (asset.ShipTo.ToUpper().Contains(SearchTerm.ToUpper()))
                                {
                                    if (!Results.Contains(asset))
                                        Results.Add((Asset)asset.Clone());
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            if (asset.ServiceEngineer != null)
                            {
                                if (asset.ServiceEngineer.ToUpper().Contains(SearchTerm.ToUpper()))
                                {
                                    if (!Results.Contains(asset))
                                        Results.Add((Asset)asset.Clone());
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            if (asset.PersonShipping != null)
                            {
                                if (asset.PersonShipping.ToUpper().Contains(SearchTerm.ToUpper()))
                                {
                                    if (!Results.Contains(asset))
                                        Results.Add((Asset)asset.Clone());
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            if (asset.OrderNumber != "-1")
                            {
                                if (asset.OrderNumber.ToString().ToUpper().Contains(SearchTerm.ToUpper()))
                                {
                                    if (!Results.Contains(asset))
                                        Results.Add((Asset)asset.Clone());
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                    return Results;
                }
                if (SearchFilter.ToLower() == "history")
                {

                    foreach (var aaa in AssetController.GetAllAssets())
                    {
                        try
                        {
                            foreach (var asset in aaa.History.History)
                            {
                                try
                                {
                                    if (asset.AssetNumber != null)
                                    {
                                        if (asset.AssetNumber.ToUpper().Contains(SearchTerm.ToUpper()))
                                        {
                                            if (!Results.Contains(asset))
                                                Results.Add((Asset)asset.Clone());

                                        }
                                    }
                                }
                                catch
                                {

                                }
                                try
                                {
                                    if (asset.AssetName != null)
                                    {
                                        if (asset.AssetName.ToUpper().Contains(SearchTerm.ToUpper()))
                                        {
                                            if (!Results.Contains(asset))
                                                Results.Add((Asset)asset.Clone());
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                try
                                {
                                    if (asset.Description != null)
                                    {
                                        if (asset.Description.ToUpper().Contains(SearchTerm.ToUpper()))
                                        {
                                            if (!Results.Contains(asset))
                                                Results.Add((Asset)asset.Clone());
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                try
                                {
                                    if (asset.ShipTo != null)
                                    {
                                        if (asset.ShipTo.ToUpper().Contains(SearchTerm.ToUpper()))
                                        {
                                            if (!Results.Contains(asset))
                                                Results.Add((Asset)asset.Clone());
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                try
                                {
                                    if (asset.ServiceEngineer != null)
                                    {
                                        if (asset.ServiceEngineer.ToUpper().Contains(SearchTerm.ToUpper()))
                                        {
                                            if (!Results.Contains(asset))
                                                Results.Add((Asset)asset.Clone());
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                try
                                {
                                    if (asset.PersonShipping != null)
                                    {
                                        if (asset.PersonShipping.ToUpper().Contains(SearchTerm.ToUpper()))
                                        {
                                            if (!Results.Contains(asset))
                                                Results.Add((Asset)asset.Clone());
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                try
                                {
                                    if (asset.OrderNumber != "-1")
                                    {
                                        if (asset.OrderNumber.ToString().ToUpper().Contains(SearchTerm.ToUpper()))
                                        {
                                            if (!Results.Contains(asset))
                                                Results.Add((Asset)asset.Clone());
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch { }
                    }
                    foreach (var item in Results)
                    {
                        item.IsHistoryItem = true;
                    }
                    return Results;
                }

                if (SearchFilter == "Date")
                {
                    return Results;
                }
            }
            catch { }
            return new List<Asset>();
        }

        protected void AssetViewRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    var a =e.Item.Controls[0];
                    if (a == null) return;
                    if (a is Templates.av_sm_tile_template)
                    {
                        var smtile= e.Item.Controls[0] as Templates.av_sm_tile_template;                       
                        var isoutcontainer = smtile.FindControl("IsOutContainer") as HtmlGenericControl;
                        if ((e.Item.DataItem as Asset).IsOut)
                        {
                            var icl = isoutcontainer.Attributes["class"];
                            icl += " element-selected";
                            isoutcontainer.Attributes["class"] = icl;

                        }else
                        {
                            var icl = isoutcontainer.Attributes["class"];
                            icl += " element-available";
                            isoutcontainer.Attributes["class"] = icl;
                        }
                    }
                    if (a is Templates.av_list_template)
                    {
                        var avlist = e.Item.Controls[0] as Templates.av_list_template;
                        var ioi = avlist.FindControl("InOutIndicator") as HtmlGenericControl;
                        if (ioi!=null)
                        {
                            if ((e.Item.DataItem as Asset).IsOut)
                            {
                                var icl = ioi.Attributes["class"];
                                icl += " bg-red";
                                ioi.Attributes["class"] = icl;
                            }
                            else
                            {
                                var icl = ioi.Attributes["class"];
                                icl += " bg-black";
                                ioi.Attributes["class"] = icl;
                            }
                                
                        }
                        var isoutcontainer = avlist.FindControl("IsOutContainer") as HtmlGenericControl;
                        if ((e.Item.DataItem as Asset).IsOut)
                        {
                            var icl = isoutcontainer.Attributes["class"];
                            icl += " element-selected";
                            isoutcontainer.Attributes["class"] = icl;

                        }
                        else
                        {
                            var icl = isoutcontainer.Attributes["class"];
                            icl += " element-available";
                            isoutcontainer.Attributes["class"] = icl;
                        }
                    }
                    if (a is Templates.av_default_template)
                    {
                        var d = e.Item.Controls[0] as Templates.av_default_template;
                        var isoutcontainer = d.Controls[1] as HtmlGenericControl;
                        if ((e.Item.DataItem as Asset).IsOut)
                        {
                            var icl = isoutcontainer.Attributes["class"];
                            icl = icl.Replace("fg-white", "fg-red");
                            isoutcontainer.Attributes["class"] = icl;
                            return;
                        }

                    }
                }
            }
            catch { }
        }
        
        protected void UpdateView(string view, object datasource=null)
        {
            if (datasource == null)
            { datasource = AssetController.GetAllAssets(); }
            switch (view)
            {
                case "list":
                    AssetViewRepeater.HeaderTemplate = Page.LoadTemplate("/Account/Templates/av_default_header_template.ascx");
                    AssetViewRepeater.ItemTemplate = Page.LoadTemplate("/Account/Templates/av_default_template.ascx");
                    AssetViewRepeater.FooterTemplate = Page.LoadTemplate("/Account/Templates/av_default_footer_template.ascx");
                    AssetViewRepeater.DataSource = datasource;
                    AssetViewRepeater.DataBind();
                    Session["CurrentAvView"] = avSelectedView.Text;
                    break;
                case "detail":

                    break;
                case "smtile":
                    AssetViewRepeater.ItemTemplate = Page.LoadTemplate("/Account/Templates/av_list_template.ascx");
                    AssetViewRepeater.DataSource = datasource;
                    AssetViewRepeater.DataBind();
                    Session["CurrentAvView"] = avSelectedView.Text;
                    break;
                case "mdtile":
                    AssetViewRepeater.HeaderTemplate = Page.LoadTemplate("/Account/Templates/av_header_template.ascx");
                    AssetViewRepeater.ItemTemplate = Page.LoadTemplate("/Account/Templates/av_sm_tile_template.ascx");
                    AssetViewRepeater.FooterTemplate = Page.LoadTemplate("/Account/Templates/av_footer_template.ascx");
                    AssetViewRepeater.DataSource = datasource;
                    AssetViewRepeater.DataBind();
                    Session["CurrentAvView"] = avSelectedView.Text;
                    break;
                case "lgtile":
                    AssetViewRepeater.HeaderTemplate = Page.LoadTemplate("/Account/Templates/av_header_template.ascx");
                    AssetViewRepeater.ItemTemplate = Page.LoadTemplate("/Account/Templates/av_sm_tile_template.ascx");
                    AssetViewRepeater.FooterTemplate = Page.LoadTemplate("/Account/Templates/av_footer_template.ascx");
                    AssetViewRepeater.DataSource = datasource;
                    AssetViewRepeater.DataBind();
                    Session["CurrentAvView"] = avSelectedView.Text;
                    break;
                default:
                    AssetViewRepeater.HeaderTemplate = Page.LoadTemplate("/Account/Templates/av_default_header_template.ascx");
                    AssetViewRepeater.ItemTemplate = Page.LoadTemplate("/Account/Templates/av_default_template.ascx");
                    AssetViewRepeater.FooterTemplate = Page.LoadTemplate("/Account/Templates/av_default_footer_template.ascx");
                    AssetViewRepeater.DataSource = datasource;
                    AssetViewRepeater.DataBind();
                    Session["CurrentAvView"] = avSelectedView.Text;
                    break;
            }
        }
        protected void ChangeView_Click(object sender, EventArgs e)
        {
            UpdateView(avSelectedView.Text);
        }

        protected void AssetSearchBtn_Click(object sender, EventArgs e)
        {
            var searchfilter = "asset";
            var userinput = avSelectedSearch.Text;
            if (userinput!=null && userinput!="")
            {
                searchfilter = userinput;
            }
            UpdateView((Session["CurrentAvView"] as string), AssetSearch(avSearchString.Text,searchfilter));
        }
        protected void FilterIsOut_Click(object sender, EventArgs e)
        {
            var ret = from a in AssetController.GetAllAssets() where a.IsOut==true select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList());
          
        }
        protected void FilterIsIn_Click(object sender, EventArgs e)
        {
            var ret = from a in AssetController.GetAllAssets() where a.IsOut==false select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList());
        }

        protected void FilterIsDamaged_Click(object sender, EventArgs e)
        {
            var ret = from a in AssetController.GetAllAssets() where a.IsDamaged == true select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList());
        }

        protected void FilterOnHold_Click(object sender, EventArgs e)
        {
            var ret = from a in AssetController.GetAllAssets() where a.OnHold == true select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList());
        }

        protected void FilterCalibrated_Click(object sender, EventArgs e)
        {
            var ret = from a in AssetController.GetAllAssets() where a.IsCalibrated == true select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList());
        }

        protected void ViewAll_Click(object sender, EventArgs e)
        {
            UpdateView((Session["CurrentAvView"] as string), AssetController.GetAllAssets());

        }
    }
}