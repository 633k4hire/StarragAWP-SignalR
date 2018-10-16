<%@ Page Title="" Async="true" EnableEventValidation="false" ViewStateEncryptionMode="Always" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="App.aspx.cs" Inherits="Web_App_Master.Browser.App1" %>




<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <link href="BrowserContent/awp.css" rel="stylesheet" />
    <link href="BrowserContent/browser.css" rel="stylesheet" />
    <link href="BrowserContent/jquery-ui.css" rel="stylesheet" />
    <link href="BrowserContent/jquery.contextMenu.css" rel="stylesheet" />
    <link href="BrowserContent/L0XX0R.css" rel="stylesheet" />

    <script src="BrowserScripts/jquery-ui.js"></script>
    <script src="BrowserScripts/jquery.contextMenu.js"></script>
    <script src="BrowserScripts/jquery.ui.position.js"></script>   
    <script src="BrowserScripts/browser.js"></script>
    <script src="BrowserScripts/jquery-resizable.js"></script>    
    
        <div id="appcontainer" style="z-index:900000 !important">
        <div id="toolbar">       
            <div class="l0x-toolbar">
                <asp:UpdatePanel runat="server" ID="AppToolbarUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true" >
                    <ContentTemplate>
                       <asp:PlaceHolder runat="server" ID="AppToolbarPlaceholder"  />
                    </ContentTemplate>
                </asp:UpdatePanel>
                
            </div>
        </div>
        <div id="browserbox">
            <div class="wrap">
                <div class="resizable resizable1">
                    <div class="inner">     
                         <asp:UpdatePanel runat="server" ID="AppLeftPanelUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true" >
                            <ContentTemplate>
                                <asp:PlaceHolder runat="server" ID="AppLeftPanelPlaceholder"  />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </div>  
                </div>
                <div class="resizable resizable2">
                    <div class="inner">   
                        <asp:UpdatePanel runat="server" ID="AppRightPanelUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true" >
                            <ContentTemplate>
                                <asp:PlaceHolder runat="server" ID="AppRightPanelPlaceholder"  />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                       
                    </div>  
                </div>
            </div>
         </div>
        <div id="footer" style="overflow:hidden !important;">
            <asp:UpdatePanel runat="server" ID="AppFooterUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true" >
                <ContentTemplate>
                    <asp:PlaceHolder runat="server" ID="AppFooterPlaceholder"  />
                </ContentTemplate>
            </asp:UpdatePanel>
          
        </div>
    </div>
    <asp:TextBox runat="server" ID ="AppArgument" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="AppCommand" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="SuperButtonArg" ClientIDMode="Static" style="display:none"></asp:TextBox>
<asp:Button ID="SuperButton" runat="server" Text="CLICK ME" OnClick="SuperButton_Click" style="display:none" ClientIDMode="Static"/>

</asp:Content>
