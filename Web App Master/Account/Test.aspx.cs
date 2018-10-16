using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Account
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ASuperBtn);

                TreeNode rootnode = new TreeNode("root");
                rootnode.NavigateUrl = "javascript:doSomething('root');";
                TreeNode node = new TreeNode("sub");
                node.NavigateUrl = "javascript:doSomething('sub');";
                rootnode.ChildNodes.Add(node);
                AssetDocumentTree.Nodes.Add(rootnode);
                AssetDocumentTreeUpdatePanel.Update();
            }
        }

   
        protected void ASuperBtn_Click(object sender, EventArgs e)
        {
            var arg = ASuperBtnArg.Text;
            CurrentAssetDocumentSrc.Src = arg;
            AssetDocumentIframeUpdatePanel.Update();
            AssetDocumentsViewer.Update();
        }
    }
}