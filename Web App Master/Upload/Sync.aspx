<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sync.aspx.cs" Inherits="Web_App_Master.Upload.Sync" %>

<html>
    <body>
        <asp:UpdatePanel ID="SyncUpdatePanel" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <asp:Literal ID="StatusLiteral" runat="server" Visible="false">

                </asp:Literal>
            </ContentTemplate>
        </asp:UpdatePanel>
    </body>
</html>