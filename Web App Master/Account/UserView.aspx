<%@ Page Title="AWP" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserView.aspx.cs" Inherits="Web_App_Master.Account.UserView" %>


<asp:Content ID="AWP" ContentPlaceHolderID="MainContent" runat="server">
                
    <link href="../Content/browser.css" rel="stylesheet" />
<%--    <link href="../Content/jquery-ui.css" rel="stylesheet" />
    <script src="../Scripts/jquery-ui.js"></script>

    <link href="../Content/jquery.contextMenu.css" rel="stylesheet" />
<script src="../Scripts/jquery.contextMenu.js"></script>
<script src="../Scripts/jquery.ui.position.js"></script>
<script src="../Scripts/FileBrowserContextMenuActions.js"></script>--%>
    <script type="text/javascript">
        $(document).ready(function () {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(InitialiseSettings);
        });
        function InitialiseSettings() {
            HideLoader();
        }
    </script>

    <!--menu-->
<nav class="navbar navbar-inverse navbar-fixed-top shadow-bottom bg-grayDark" style=" padding-right:25px; margin-top:50px !important; text-align:left !important; left:0px!important; position:fixed !important; z-index:25000; vertical-align:middle">
  <div class="container-fluid">
    <div class="navbar-header">
      <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#AVmenu" style="float:left !important">
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>                        
      </button>
      
    </div>
    <div id="AVmenu" class="collapse navbar-collapse" >
      <ul class="nav navbar-nav starrag-menu">
          <li class="dropdown">
            <a class="dropdown-toggle" data-toggle="dropdown" href="#">View
            <span class="caret"></span></a>
            <ul class="dropdown-menu starrag-menu">
                <li><a href="#" onclick="ChangeAssetView('list'); ">Default</a></li>
                <li><a href="#" onclick=" ChangeAssetView('icon')">Icon</a></li>
                <li><a href="#" onclick="ChangeAssetViewListType('list-type-tiles'); ">Detail</a></li>
                <li><a href="#" onclick="ChangeAssetViewListType('list-type-listing'); ">Listing</a></li>
                <li><a href="#" onclick="ChangeAssetView('smtile'); ">List Tile</a></li>
                <li><a href="#" onclick="ChangeAssetView('mdtile'); ">Medium Tile</a></li>
<%--                <li><a href="#" onclick="ChangeAssetView('lgtile'); ">large Tile</a></li>--%>
            </ul>
          </li>
          <li style="margin-top:10px;">
               <div class="  text" data-role="input" style="width:250px !important; vertical-align:middle!important">
                   <asp:TextBox ID="avSearchString" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
               </div>
          </li>
          <li>             
               <asp:LinkButton ID ="AssetSearchBtn" ClientIDMode="Static" runat="server"  OnClick="AssetSearchBtn_Click"><span class="glyphicon glyphicon-search""></span></asp:LinkButton>
          </li>
      
          <li class="dropdown">
            <a class="dropdown-toggle" data-toggle="dropdown" href="#">Filter
            <span class="caret"></span></a>
            <ul class="dropdown-menu starrag-menu">
                <li><asp:LinkButton OnClientClick="ShowLoader()" ID ="ViewAll" ClientIDMode="Static" runat="server"  OnClick="ViewAll_Click">View All</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterIsOut" ClientIDMode="Static" runat="server"  OnClick="FilterIsOut_Click">Checked Out Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterIsIn" ClientIDMode="Static" runat="server"  OnClick="FilterIsIn_Click">Checked In Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterIsDamaged" ClientIDMode="Static" runat="server"  OnClick="FilterIsDamaged_Click">Damaged Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterOnHold" ClientIDMode="Static" runat="server"  OnClick="FilterOnHold_Click">On Hold Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterCalibrated" ClientIDMode="Static" runat="server"  OnClick="FilterCalibrated_Click">Calibrated Only</asp:LinkButton> </li>
            </ul>
          </li>
           <li>
            <asp:LinkButton runat="server" OnClientClick="ShowLoader()" ID="RefreshBtn" OnClick="RefreshBtn_Click" ><span class="mif-loop2"></span> </asp:LinkButton>
        </li>
      </ul>   
    </div>
  </div>
</nav>

    <div class="bg-shaded"></div>
<!--updatepanel-->
                    <asp:UpdatePanel  runat="server" ID="CheckoutUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>                
               <!--REPEATER-->
    <div style=" padding-top:50px !important; background-color:black !important">
    <asp:Repeater ID="AssetViewRepeater" runat="server" EnableViewState="false" OnItemDataBound="AssetViewRepeater_ItemDataBound">
        <HeaderTemplate> </HeaderTemplate>
        <ItemTemplate></ItemTemplate>
        <FooterTemplate> </FooterTemplate>
    </asp:Repeater>
        </div>
            </ContentTemplate>
            <Triggers>
               <asp:AsyncPostBackTrigger ControlID="avChangeView" EventName="Click" />
               <asp:AsyncPostBackTrigger ControlID="AssetSearchBtn" EventName="Click" />
               <asp:AsyncPostBackTrigger ControlID="FilterIsOut" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="FilterIsIn" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="FilterIsDamaged" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="FilterOnHold" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="FilterCalibrated" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="RefreshBtn" EventName="Click" />
            </Triggers>
         </asp:UpdatePanel>


  
    <div id="maincontentTiles" style="height:100%;width:100%;min-height:100%;min-width:100%;background-color:#2d89ef;">
        </div>
<asp:TextBox runat="server" ID ="avSelectedSearch" ClientIDMode="Static" style="display:none"></asp:TextBox>
<asp:TextBox runat="server" ID ="avSelectedView" ClientIDMode="Static" style="display:none"></asp:TextBox>
<asp:Button OnClientClick="ShowLoader()" ID="avChangeView" runat="server" Text="CLICK ME" OnClick="ChangeView_Click" style="display:none" ClientIDMode="Static"/>
<asp:Button OnClientClick="ShowLoader()" ID="AjaxRefresherBtn" runat="server" Text="CLICK ME" OnClick="Refresher_Click" style="display:none" ClientIDMode="Static"/>


    

    </asp:Content>