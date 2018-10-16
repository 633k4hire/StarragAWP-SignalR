using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Browser.Templates.ControlPanel
{
    public partial class cp_left : System.Web.UI.UserControl
    {
        public void BuildTree()
        {
            TreeNode rootnode = new TreeNode();
            rootnode.Text="Control Panel";          
            //var ds = Pull.Transactions().OrderBy(w => w.Date).ToList();
            //ds.ForEach((t) => {
            //    TreeNode node = new TreeNode();
            //    node.Text = t.Date.ToShortDateString() + " - " + t.Name;
            //    node.Value = t.TransactionID;
            //    //node.NavigateUrl = t.TransactionID;
            //    DirTree.SelectedNodeChanged += DirTree_SelectedNodeChanged; ;
            //    rootnode.ChildNodes.Add(node);
            //});
            DirTree.Nodes.Add(rootnode);
        }

        private void DirTree_SelectedNodeChanged(object sender, EventArgs e)
        {
            //make hubdata and sync it
            WebAppHub.HubData data = new WebAppHub.HubData();

            Hub.Update(data);
        }

        private WebAppHub.Group m_group;
        public WebAppHub.Group Group
        {
            get { try { return Application["AppGroup"] as WebAppHub.Group; } catch { return null; } }
        }
        public WebAppHub.LeftPanel Hub = new WebAppHub.LeftPanel();
        protected void Page_Load(object sender, EventArgs e)
        {
            Hub.IncomingSyncData = HubSync;
            Hub.OnSync += Hub_OnSync;
            Hub.OnException += Hub_OnException;
        }

        private void Hub_OnException(object sender, ExceptionEvent e)
        {

        }

        private void Hub_OnSync(object sender, SyncEvent e)
        {

        }

        private WebAppHub.HubData HubSync(WebAppHub.HubData data)
        {
            return data;
        }
    }
}