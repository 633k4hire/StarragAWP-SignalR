using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_App_Master.Browser.Templates;
using Web_App_Master.Browser.Templates.ControlPanel;

namespace Web_App_Master.Browser
{
    public partial class App1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            //MyControl is the Custom User Control with a code behind file
            var AppId = Request.QueryString["appid"] as string;
            if (AppId == null) return;
            if (AppId=="tr")
            {
                LoadTransactionApp();

                //ToggleLeftPanel();
            }
            if (AppId=="ctrl")
            {
                LoadControlPanelApp();
            }
            
        }

        private void LoadControlPanelApp()
        {
            cp_toolbar Header = (cp_toolbar)Page.LoadControl("Templates/ControlPanel/cp_toolbar.ascx");
            AppToolbarPlaceholder.Controls.Add(Header);

            cp_left LeftPanel = (cp_left)Page.LoadControl("Templates/ControlPanel/cp_left.ascx");
            var dirtree = LeftPanel.Controls[0] as TreeView;

            var rootnode = new TreeNode("Control Panel");

            TreeNode SettingsNode = new TreeNode();
            SettingsNode.Value = "GeneralSettings";
            TreeNode GeneralSettingsNode = new TreeNode("General");
            GeneralSettingsNode.Value = "GeneralSettings";
            SettingsNode.ChildNodes.Add(GeneralSettingsNode);

            TreeNode TransactionNode = new TreeNode();
            TransactionNode.Value = "TransactionNode";

            TreeNode CustomerNode = new TreeNode();
            CustomerNode.Value = "CustomerNode";

            TreeNode PersonnelNode = new TreeNode();
            PersonnelNode.Value = "PersonnelNode";

            TreeNode NoticesNode = new TreeNode();
            NoticesNode.Value = "NoticesNode";

            TreeNode AssetsNode = new TreeNode();
            AssetsNode.Value = "AssetsNode";

            dirtree.ShowLines = true;
            //Settings
            
            //transactions
            try
            {
                var ds = Pull.Transactions().OrderBy(w => w.Date).ToList();
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.Date.ToShortDateString() + " - " + t.Name;
                    node.Value = t.TransactionID;
                    //node.NavigateUrl = "/Account/OutCart.aspx?pend=" + t.TransactionID;
                    dirtree.SelectedNodeChanged += Dirtree_SelectedNodeChanged;
                    TransactionNode.ChildNodes.Add(node);

                });
            }
            catch
            { }

            //Customers
            try
            {
                Pull.Globals();

                var ds = Global.Library.Settings.Customers.OrderBy(w => w.CompanyName).ToList();
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.CompanyName;
                    node.ToolTip = t.Address;
                    node.Value = t.CompanyName + "###" + t.Address;
                    dirtree.SelectedNodeChanged += CustomerNode_Selected;
                    CustomerNode.ChildNodes.Add(node);

                });
            }
            catch 
            {

                throw;
            }
            //Personnel

            //Notices

            //Assets


            rootnode.ChildNodes.Add(SettingsNode);
            rootnode.ChildNodes.Add(TransactionNode);
            rootnode.ChildNodes.Add(CustomerNode);
            rootnode.ChildNodes.Add(PersonnelNode);
            rootnode.ChildNodes.Add(NoticesNode);
            rootnode.ChildNodes.Add(AssetsNode);


            dirtree.Nodes.Add(rootnode);
            AppLeftPanelPlaceholder.Controls.Add(LeftPanel);
            cp_right RightPanel = (cp_right)Page.LoadControl("Templates/ControlPanel/cp_right.ascx");
            AppRightPanelPlaceholder.Controls.Add(RightPanel);
            cp_footer Footer = (cp_footer)Page.LoadControl("Templates/ControlPanel/cp_footer.ascx");
            AppFooterPlaceholder.Controls.Add(Footer);

           
        }

        private void CustomerNode_Selected(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LoadTransactionApp()
        {
            TransactionToolbar Header = (TransactionToolbar)Page.LoadControl("Templates/TransactionToolbar.ascx");
            AppToolbarPlaceholder.Controls.Add(Header);
            TransactionLeftPanel LeftPanel = (TransactionLeftPanel)Page.LoadControl("Templates/TransactionLeftPanel.ascx");
            var dirtree = LeftPanel.Controls[0] as TreeView;
            var rootnode = new TreeNode();
            rootnode.Text = "Transactions";
            dirtree.ShowLines = true;
            //bind the tree
            try
            {
                var ds = Pull.Transactions().OrderBy(w => w.Date).ToList();
                ds.ForEach((t) => {
                    TreeNode node = new TreeNode();
                    node.Text = t.Date.ToShortDateString() + " - " + t.Name;
                    node.Value = t.TransactionID;
                    //node.NavigateUrl = t.TransactionID;
                    dirtree.SelectedNodeChanged += Dirtree_SelectedNodeChanged;
                    rootnode.ChildNodes.Add(node);
                });
                dirtree.Nodes.Add(rootnode);
            }
            catch
            { }
            AppLeftPanelPlaceholder.Controls.Add(LeftPanel);
            TransactionRightPanel RightPanel = (TransactionRightPanel)Page.LoadControl("Templates/TransactionRightPanel.ascx");
            AppRightPanelPlaceholder.Controls.Add(RightPanel);
            TransactionFooter Footer = (TransactionFooter)Page.LoadControl("Templates/TransactionFooter.ascx");
            AppFooterPlaceholder.Controls.Add(Footer);
        }

        public void ToggleLeftPanel()
        {
            var script = @"$(document).ready(function (){ ToggleList();});";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToggleListScript", script, true);
        }
        private void Dirtree_SelectedNodeChanged(object sender, EventArgs e)
        {
            var tree = (TreeView)sender;
            var node = tree.SelectedNode;
            var transaction = Pull.Transaction(node.Value);
            var trp = AppRightPanelPlaceholder.Controls[0] as TransactionRightPanel;
            var repeater = trp.Controls[3] as Repeater;
            var transLabel = trp.Controls[1] as Label;
            transLabel.Text = transaction.Name;
            List<Helpers.Asset> assets = new List<Helpers.Asset>();
            var lib = Pull.Assets();
            foreach (var item in transaction.Assets)
            {
                assets.Add((from a in lib where a.AssetNumber == item select a).First());
            }
            repeater.DataSource = assets;
            repeater.DataBind();
            //ToggleLeftPanel();
            AppRightPanelUpdatePanel.Update();
        }

        protected void SuperButton_Click(object sender, EventArgs e)
        {

        }
    }


}