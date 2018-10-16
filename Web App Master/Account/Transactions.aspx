<%@ Page Title="Pending Transactions" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Transactions.aspx.cs" Inherits="Web_App_Master.Account.Transactions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .main-content{
            padding-left:0px !important;
            padding-right:0px !important;
        }
        .main-content_inner{
            padding-left:0px !important;
            padding-right:0px !important;
            margin:0px !important
        }
        .awp_box{
            margin:0px !important;
        }
        .row{
            padding:0px !important;
            margin:0px !important
        }
        body{
           padding-left:0px !important;
            padding-right:0px !important;
            margin:0px !important
        }
        html{
            padding-left:0px !important;
            padding-right:0px !important;
            margin:0px !important
        }
    </style>
    <div id="AssetBox"  class="row">
        <div class="col-md-4" style="padding:0px !important;">
                <div class="awp_box rounded">           
                <div class="awp_box_title bg-sg-title">
                    <span class="fg-white shadow-metro-black">Transactions</span>
                </div>
                <div class="awp_box_content bg-transparent">
                    <div id="menu">

                    </div>
                    <asp:UpdatePanel ID ="TransactionUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true" runat="server">
                        <ContentTemplate>  
                             <asp:Repeater ID="TransactionRepeater" ClientIDMode="Static" runat="server" OnItemDataBound="TransactionRepeater_ItemDataBound">
                                <ItemTemplate>
                                    <div class=" bg-sg-box" style="border-bottom: 1px solid grey; overflow:hidden">                                        
                                            <div class="col-sm-12 bg-sg-box fg-white" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                <asp:Button Height="25" Width="15" ToolTip="Delete Transaction" ID="DeleteTransactionBtn" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandName='<%#Eval("Name")%>' CommandArgument='<%#Eval("TransactionID")%>'  OnCommand="DeleteTransactionBtn_Command"/>
                                                <a title="Complete Transaction" style="font-weight:bold" class="btn btn-sm fg-white shadow-metro-black" href='/Account/OutCart.aspx?pend=<%#Eval("TransactionID")%>'>></a>    
                                                <a style="text-decoration:none" href='/Account/Transactions.aspx?tid=<%#Eval("TransactionID")%>'><span class="fg-white shadow-metro-black"><%# ((DateTime)Eval("Date")).ToShortDateString()%></span>&nbsp-&nbsp <span class="fg-white shadow-metro-black"><%# Eval("Name")%></span><span class="fg-white shadow-metro-black"> &nbsp-&nbsp  <span class="fg-white shadow-metro-black"><%# (Eval("Customer") as Helpers.Customer).CompanyName%></span> <asp:Literal Text="" runat="server" ID="TransactionLiteral"  /></a>
                                            </div>  
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                           
                </div>
                </div>
         </div>       
            <div class="col-md-8"  style="padding:0px !important;">
                <div class="awp_box rounded ">           
                    
                <div class="awp_box_title bg-sg-title">
                    <span class="fg-white shadow-metro-black">
                        <asp:Label Text="" runat="server" ID="TransactionIdLabel" /></span>
                </div>
                <div class="awp_box_content bg-transparent">
                  
        <div class="col-md-12 fg-white shadow-metro-black" >
           <div style="width:auto; opacity:0.75 !important; font-weight:bold" class="bg-sg-title rounded">
                             <span class=" top-right-btn bg-green"><a style="text-decoration:none!important;" class="fg-white" title="Complete Transaction" href='/Account/OutCart.aspx?pend=<%= TID%>'>Complete</a></span>

                <div>
                    <span>Confimation Number:  <%= TID %></span>
                </div>
                <div>
                <span>Ship To:  <%= ShipToName %></span>
                </div>
         
                <div>
                    <span><a href='mailto:<%= Email %>'> Email:  <%= Email %></a></span>
                </div>
                <div>
                    <span>Comment:  <%= OrderNumber %></span>
                </div>
            </div>
        
        </div>
        <div class="col-md-12">
            <div>
                <asp:Repeater ID="TransactionItemRepeater"  runat="server" OnItemCommand="TransactionItemRepeater_ItemCommand">
                    <ItemTemplate>
                            <div  class="file " title='' draggable="true"  onclick="BarcodeScanned('<%# Eval("AssetNumber")%>', '<%# Eval("IsHistoryItem")%>','<%# Eval("DateShippedTicks")%>');">
                                    <div id =" file-color " class="file-color <%# Eval("Color") %>" style="width:100%; height:100%;"></div>

                            <div class="i-icon">   
                                <img style="box-shadow: rgba(0, 0, 0, 0.70) 0px 0px 10px;" src='<%# Eval("FirstImage")+".r?w=133&h=100" %>' />
                            </div>

                            <div runat="server" class="i-title"><strong class="fg-white shadow-metro-black"><%# Eval("AssetName") %></strong> </div>
  
                            <div class="i-check">
                        <span id="InOutIndicator"  runat="server" class="edge-badge fg-white shadow-metro-black icon-badge " style="z-index:3 !important;"><strong><%# Eval("AssetNumber")%></strong></span></div>
                                <div runat="server" id="DownloadLink" class="i-download" style="margin-top:-4px;padding-left:2px" onclick="event.stopPropagation();">  <asp:LinkButton ID="RemoveFromCheckOutButton" CommandName="Delete" CommandArgument='<%# Eval("AssetNumber")%>' runat="server"  > <i title="Add To Task List" id="dl_link"  style="font-size:1.5em;" class="glyphicon glyphicon-remove c-red  av-hand-cursor"></i></asp:LinkButton>       

   
                        </div>

                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>				


                </div>
                </div>
         </div>             
    

</asp:Content>
<%--<asp:Repeater ID="TransactionItemRepeater"  runat="server">
                    <ItemTemplate>
                            <div  class="file " title='' draggable="true"  onclick="BarcodeScanned('<%# Eval("AssetNumber")%>', '<%# Eval("IsHistoryItem")%>','<%# Eval("DateShippedTicks")%>');">
                                    <div id =" file-color " class="file-color <%# Eval("Color") %>" style="width:100%; height:100%;"></div>

                            <div class="i-icon">   
                                <img style="box-shadow: rgba(0, 0, 0, 0.70) 0px 0px 10px;" src='<%# Eval("FirstImage")+".r?w=133&h=100" %>' />
                            </div>

                            <div runat="server" class="i-title"><strong class="fg-white shadow-metro-black"><%# Eval("AssetName") %></strong> </div>
  
                            <div class="i-check">
                        <span id="InOutIndicator"  runat="server" class="edge-badge fg-white shadow-metro-black icon-badge " style="z-index:3 !important;"><strong><%# Eval("AssetNumber")%></strong></span></div>
                                <div runat="server" id="DownloadLink" class="i-download" style="margin-top:-4px;padding-left:2px" onclick="event.stopPropagation();">  <asp:LinkButton ID="RemoveFromCheckOutButton" CommandName="Delete" CommandArgument='<%# Eval("AssetNumber")%>' runat="server"  > <i title="Add To Task List" id="dl_link"  style="font-size:1.5em;" class="glyphicon glyphicon-remove c-red  av-hand-cursor"></i></asp:LinkButton>       

   
                        </div>

                        </div>
                    </ItemTemplate>
                </asp:Repeater>
--%>
