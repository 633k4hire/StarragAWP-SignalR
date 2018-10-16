using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task RefreshAsync()
        {
            var currentview = Session["CurrentAvView"] as string;
            var assetCache = await Pull.AssetsAsync();

            Application[(Session["guid"] as string)] = assetCache;
            Session["SessionAssets"] = assetCache.OrderBy(w => w.AssetNumber).ToList();
            var list = Session["SessionAssets"] as List<Asset>;
            if (Page.User.IsInRole("Admins")|| Page.User.IsInRole("superadmin"))
            {
                UpdateView(currentview, list.ToList().OrderBy(w => w.AssetNumber));
            }
            else
            {
                //var inonly = from a in list where a.IsOut == false select a;
                //UpdateView(currentview, inonly.ToList().OrderBy(w => w.AssetNumber));
                UpdateView(currentview, list.ToList().OrderBy(w => w.AssetNumber));

            }



        }
        public async Task UpdateAsync()
        {
            var currentview = Session["CurrentAvView"] as string;
            var assetCache = Application[(Session["guid"] as string)] as List<Asset>;
            Session["SessionAssets"] = assetCache.OrderBy(w => w.AssetNumber).ToList();
            var list = Session["SessionAssets"] as List<Asset>;
            if (Page.User.IsInRole("Admins")|| Page.User.IsInRole("superadmin"))
            {
                UpdateView(currentview, list.ToList().OrderBy(w => w.AssetNumber));
            }
            else
            {
                //var inonly = from a in list where a.IsOut == false select a;
                //UpdateView(currentview, inonly.ToList().OrderBy(w => w.AssetNumber));
                UpdateView(currentview, list.ToList().OrderBy(w => w.AssetNumber));

            }



        }
        public async Task SearchAsync()
        {
            var searchfilter = "asset";
            var userinput = avSelectedSearch.Text;
            if (userinput != null && userinput != "")
            {
                searchfilter = userinput;
            }            
            var currentview = Session["CurrentAvView"] as string;
            var assetCache = Application[(Session["guid"] as string)] as List<Asset>;
            Session["SessionAssets"] = assetCache.OrderBy(w => w.AssetNumber).ToList();
            var search = Request.QueryString["q"];
            UpdateView(currentview, AssetSearch(search, searchfilter).OrderBy(w => w.AssetNumber));
        }
        public string IsSelected(object num)
        {

            if ((bool)num == true)
                return "element-selected";
            else
                return "";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.SiteMaster().OnAssetViewUpdate += AssetView_OnAssetViewUpdate;
            if (IsAsync)
            {

            }
            if (!IsPostBack)
            {
                if (Request.QueryString["q"]!=null)
                {
                    RegisterAsyncTask(new PageAsyncTask(SearchAsync));

                }
                else
                {
                    // RegisterAsyncTask(new PageAsyncTask(UpdateAsync));
                    var currentview = Session["CurrentAvView"] as string;
                    var assetCache = Application[(Session["guid"] as string)] as List<Asset>;
                    if (assetCache==null)
                    {
                       Application[(Session["guid"] as string)] = new List<Asset>();
                        assetCache = new List<Asset>();
                    }
                    Session["SessionAssets"] = assetCache.OrderBy(w => w.AssetNumber).ToList();
                    var list = Session["SessionAssets"] as List<Asset>;
                    if (Page.User.IsInRole("Admins") || Page.User.IsInRole("superadmin"))
                    {
                        UpdateView(currentview, list.ToList().OrderBy(w => w.AssetNumber));
                    }
                    else
                    {
                        //var inonly = from a in list where a.IsOut == false select a;
                        //UpdateView(currentview, inonly.ToList().OrderBy(w => w.AssetNumber));
                        UpdateView(currentview, list.ToList().OrderBy(w => w.AssetNumber));

                    }
                }
            }
            
          
        }

        private void AssetView_OnAssetViewUpdate(object sender, UpdateRequestEvent e)
        {
            var currentview = Session["CurrentAvView"] as string;           
            var list = Session["SessionAssets"] as List<Asset>;
            if (Page.User.IsInRole("Admins") || Page.User.IsInRole("superadmin"))
            {
                UpdateView(currentview, Global.AssetCache.OrderBy(w => w.AssetNumber));
            }
        }

        private List<Asset> AssetSearch(string SearchTerm, string SearchFilter="asset")
        {
            try
            {
                List<Asset> Results = new List<Asset>();
                if (SearchFilter.ToLower() == "asset")
                {

                    foreach (var asset in Session["SessionAssets"] as List<Asset>)
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
                    Results = Results.GroupBy(x => x.AssetNumber).Select(x => x.First()).ToList();
                    return Results;
                }
                if (SearchFilter.ToLower() == "history")
                {

                    foreach (var aaa in Session["SessionAssets"] as List<Asset>)
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

                        }                    
                        if ((e.Item.DataItem as Asset).OnHold)
                        {
                            var icl = isoutcontainer.Attributes["class"];
                            icl += " element-onhold";
                            isoutcontainer.Attributes["class"] = icl;
                        }
                        if ((e.Item.DataItem as Asset).IsDamaged)
                        {
                            var icl = isoutcontainer.Attributes["class"];
                            icl += " element-damaged";
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
                            if ((e.Item.DataItem as Asset).OnHold)
                            {
                                var icl = ioi.Attributes["class"];
                                icl += " bg-violet";
                                ioi.Attributes["class"] = icl;
                            }
                            if ((e.Item.DataItem as Asset).IsDamaged)
                            {
                                var icl = ioi.Attributes["class"];
                                icl += " bg-amber";
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
                            
                        }
                        if ((e.Item.DataItem as Asset).OnHold)
                        {
                            var icl = isoutcontainer.Attributes["class"];
                            icl = icl.Replace("fg-white", "fg-violet");
                            isoutcontainer.Attributes["class"] = icl;

                        }
                        if ((e.Item.DataItem as Asset).IsDamaged)
                        {
                            var icl = isoutcontainer.Attributes["class"];
                            icl = icl.Replace("fg-white", "fg-amber");
                            isoutcontainer.Attributes["class"] = icl;

                        }
                        return;
                    }
                    if (a is Templates.icon_view)
                    {
                        var template = e.Item.Controls[0] as Templates.icon_view;
                        var dl_link = template.Controls[5] as HtmlGenericControl;
                        var inoutcontainer = template.Controls[3] as HtmlGenericControl;
                        if ((e.Item.DataItem as Asset).IsOut)
                        {
                            dl_link.InnerHtml=dl_link.InnerHtml.Replace("c-lime", "c-red");
                            dl_link.InnerHtml = dl_link.InnerHtml.Replace("download", "upload");
                        }
                        //if ((e.Item.DataItem as Asset).OnHold)
                        //{
                        //    var icl = inoutcontainer.Attributes["class"];
                        //    icl += " bg-sg-violet";
                        //    inoutcontainer.Attributes["class"] = icl;

                        //}
                        //if ((e.Item.DataItem as Asset).IsDamaged)
                        //{
                        //    var icl = inoutcontainer.Attributes["class"];
                        //    icl += " bg-sg-red";
                        //    inoutcontainer.Attributes["class"] = icl;

                        //}
                        return;
                    }
                }
            }
            catch { }
        }
     
        protected void UpdateView(string view, object datasource=null)
        {
            if (datasource == null)
            { datasource = AssetController.GetAllAssets().OrderBy(w => w.AssetNumber); }
            switch (view)
            {
                case "icon":
                    AssetViewRepeater.HeaderTemplate = null;
                    AssetViewRepeater.FooterTemplate = null;
                    AssetViewRepeater.ItemTemplate = Page.LoadTemplate("/Account/Templates/DefaultView.ascx");
                    AssetViewRepeater.DataSource = datasource;
                    AssetViewRepeater.DataBind();
                    Session["CurrentAvView"] = avSelectedView.Text;
                    break;
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
                    AssetViewRepeater.HeaderTemplate = null;
                    AssetViewRepeater.FooterTemplate = null;
                    AssetViewRepeater.ItemTemplate = Page.LoadTemplate("/Account/Templates/DefaultView.ascx");
                    AssetViewRepeater.DataSource = datasource;
                    AssetViewRepeater.DataBind();
                    Session["CurrentAvView"] = avSelectedView.Text;
                    break;
            }
            CheckoutUpdatePanel.Update();
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
            UpdateView((Session["CurrentAvView"] as string), AssetSearch(avSearchString.Text,searchfilter).OrderBy(w => w.AssetNumber));
        }
        protected void FilterIsOut_Click(object sender, EventArgs e)
        {
            var ret = from a in Session["SessionAssets"] as List<Asset> where a.IsOut==true select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList().OrderBy(w => w.AssetNumber));
          
        }
        protected void FilterIsIn_Click(object sender, EventArgs e)
        {
            var ret = from a in Session["SessionAssets"] as List<Asset> where a.IsOut==false select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList().OrderBy(w => w.AssetNumber));
        }

        protected void FilterIsDamaged_Click(object sender, EventArgs e)
        {
            var ret = from a in Session["SessionAssets"] as List<Asset> where a.IsDamaged == true select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList().OrderBy(w => w.AssetNumber));
        }

        protected void FilterOnHold_Click(object sender, EventArgs e)
        {
            var ret = from a in Session["SessionAssets"] as List<Asset> where a.OnHold == true select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList().OrderBy(w => w.AssetNumber));
        }

        protected void FilterCalibrated_Click(object sender, EventArgs e)
        {
            var ret = from a in Session["SessionAssets"] as List<Asset> where a.IsCalibrated == true select a;
            UpdateView((Session["CurrentAvView"] as string), ret.ToList().OrderBy(w => w.AssetNumber));
        }

        protected void ViewAll_Click(object sender, EventArgs e)
        {
            var x = Session["SessionAssets"] as List<Asset>;
            x = x.OrderBy(w => w.AssetNumber).ToList();
            UpdateView((Session["CurrentAvView"] as string), x);

        }      
        protected void RefreshBtn_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(RefreshAsync));
        }

        protected void Refresher_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(RefreshAsync));
        }

        protected void CreateAssetBtn_Click(object sender, EventArgs e)
        {

        }
    }
}