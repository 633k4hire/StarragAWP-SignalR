using System;

namespace Web_App_Master.Browser.Templates.ControlPanel
{
    public partial class cp_footer : System.Web.UI.UserControl
    {
        private WebAppHub.Group m_group;
        public WebAppHub.Group Group
        {
            get { try { return Application["AppGroup"] as WebAppHub.Group; } catch { return null; } }
        }
        public WebAppHub.Footer Hub = new WebAppHub.Footer();
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