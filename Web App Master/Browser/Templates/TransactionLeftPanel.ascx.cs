using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Browser.Templates
{
    public partial class TransactionLeftPanel : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DirTree_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {

        }

        protected void DirTree_SelectedNodeChanged(object sender, EventArgs e)
        {
            var tree = (TreeView)sender;
            var node = tree.SelectedNode;
            
        }

        protected void DirTree_DataBound(object sender, EventArgs e)
        {

        }

        protected void DirTree_TreeNodeDataBound(object sender, TreeNodeEventArgs e)
        {

        }
    }
}