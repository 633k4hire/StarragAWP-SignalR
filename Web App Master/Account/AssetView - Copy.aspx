<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssetView.aspx.cs" Inherits="Web_App_Master.Account.AssetView" %>
<%@ MasterType VirtualPath="~/Site.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <!--menu-->
    <div class="app-bar shadow-bottom" style="margin-top:0px !important; left:0px!important; position:fixed !important; z-index:25000;">
    
    <ul class="app-bar-menu">
        <li><h1></h1> </li>
        <li>
            <a href="#" class="dropdown-toggle">View</a>
            <ul class="d-menu" data-role="dropdown">
                <li><a href="#" onclick="ChangeAssetView('list'); ShowLoader()">Default</a></li>
                <li><a href="#" onclick="ChangeAssetViewListType('list-type-icons'); ShowLoader()">Icon</a></li>
                <li><a href="#" onclick="ChangeAssetViewListType('list-type-tiles'); ShowLoader()">Detail</a></li>
                <li><a href="#" onclick="ChangeAssetViewListType('list-type-listing'); ShowLoader()">Listing</a></li>
                <li><a href="#" onclick="ChangeAssetView('smtile'); ShowLoader()">List Tile</a></li>
                <li><a href="#" onclick="ChangeAssetView('mdtile'); ShowLoader()">Medium Tile</a></li>
                <li><a href="#" onclick="ChangeAssetView('lgtile'); ShowLoader()">large Tile</a></li>
            </ul>
        </li>
       <li>
           <div class="input-control text" data-role="input" style="width:250px !important;">
               <asp:TextBox ID="avSearchString" runat="server" ClientIDMode="Static" CssClass="text"></asp:TextBox>
  
           </div>

       </li>
       <li>             
            <asp:LinkButton ID ="AssetSearchBtn" ClientIDMode="Static" runat="server" OnClientClick="ShowLoader()" OnClick="AssetSearchBtn_Click"><span class="glyphicon glyphicon-search""></span></asp:LinkButton>
             
       </li>
        <li>
            <a href="#" style="text-decoration:none" class="dropdown-toggle">&nbsp<span title="Search Filter" class="mif-settings-ethernet"></span></a>            
            <ul class="d-menu" data-role="dropdown">
                <li><a id ="avAssetSearchBtn" runat="server" onclick="SetSearchType('asset')" >Asset Search</a> </li>
                <li><a id ="avHistorySearchBtn" runat="server" onclick="SetSearchType('history')" >History Search</a> </li>

            </ul>
        </li>
        <li>
            <a href="#" style="text-decoration:none" class="dropdown-toggle">&nbsp<span title="View Filter" class="glyphicon glyphicon-filter"></span></a>            
            <ul class="d-menu" data-role="dropdown">
                <li><asp:LinkButton ID ="ViewAll" ClientIDMode="Static" runat="server" OnClientClick="ShowLoader()" OnClick="ViewAll_Click">View All</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterIsOut" ClientIDMode="Static" runat="server" OnClientClick="ShowLoader()" OnClick="FilterIsOut_Click">Checked Out Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterIsIn" ClientIDMode="Static" runat="server" OnClientClick="ShowLoader()" OnClick="FilterIsIn_Click">Checked In Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterIsDamaged" ClientIDMode="Static" runat="server" OnClientClick="ShowLoader()" OnClick="FilterIsDamaged_Click">Damaged Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterOnHold" ClientIDMode="Static" runat="server" OnClientClick="ShowLoader()" OnClick="FilterOnHold_Click">On Hold Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterCalibrated" ClientIDMode="Static" runat="server" OnClientClick="ShowLoader()" OnClick="FilterCalibrated_Click">Calibrated Only</asp:LinkButton> </li>

            </ul>
        </li>
       
        
        
    </ul>
   
</div>
<!--updatepanel-->
                    <asp:UpdatePanel  runat="server" ID="CheckoutUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>                
               <!--REPEATER-->
    <div style=" padding-top:50px !important">
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
            </Triggers>
         </asp:UpdatePanel>


  
    <div id="maincontentTiles" style="height:100%;width:100%;min-height:100%;min-width:100%;background-color:#2d89ef;">
        </div>
<asp:TextBox runat="server" ID ="avSelectedSearch" ClientIDMode="Static" style="display:none"></asp:TextBox>
<asp:TextBox runat="server" ID ="avSelectedView" ClientIDMode="Static" style="display:none"></asp:TextBox>
<asp:Button ID="avChangeView" runat="server" Text="CLICK ME" OnClick="ChangeView_Click" style="display:none" ClientIDMode="Static"/>


    

    </asp:Content>
