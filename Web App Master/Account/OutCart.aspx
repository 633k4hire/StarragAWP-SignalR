<%@ Page Title="Out Cart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OutCart.aspx.cs" Inherits="Web_App_Master.Account.Checkout" %>
<asp:Content ID="OutCartContent" ContentPlaceHolderID="MainContent" runat="server">

     <nav class="navbar navbar-inverse bg-grayDark" style="margin-top:0px !important; height:auto; width:100%; left:0px!important; border-radius:0px!important; position:fixed !important; z-index:25000;">
     
           <asp:PlaceHolder ID ="ApplyChangesButton" runat="server" Visible="true">
               <ul class="nav navbar-nav navbar-right starrag-menu">
                     <li>
            <asp:LinkButton runat="server" OnClientClick="ShowLoader();" OnClick="ContinueToCheckOutBtn_Click"  ID="ContinueToCheckOutBtn" ClientIDMode="Static" style="vertical-align:bottom; text-decoration:none"><strong>Continue To Shipping&nbsp&nbsp</strong><i style="vertical-align:top" class="glyphicon glyphicon-arrow-right md-glyph"></i></asp:LinkButton>
            
        </li>
               </ul>

            </asp:PlaceHolder>     
    </nav>

    <div class="bg-metro" style=" padding-top:60px !important"></div>
            <div class="row">
                     <div class="col-md-3">
                         <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Options</span></div>
                            <div class="awp_box_content bg-sg-box" style="height:auto !important; vertical-align:middle; text-align:left !important;">   
                                <h4 class="fg-white shadow-metro-black">Customer</h4>
                                <asp:PlaceHolder ID ="CustomCustomerPlaceHolder" ClientIDMode="Static" runat="server" Visible="false">

                                     <span class=" fg-white shadow-metro-black"><asp:Literal  ID="CustomCustomer" runat="server"></asp:Literal></span> 
                                    </asp:PlaceHolder>
                                    <asp:DropDownList OnTextChanged="checkout_ShipTo_TextChanged" Width="100%" ClientIDMode="Static" ID='checkout_ShipTo' AppendDataBoundItems="true" runat="server"  CssClass="form-control">
                                        <asp:ListItem Text="--Select One--" Value="" /> 
                                    </asp:DropDownList>
                                <hr />
                                <h4 class="fg-white shadow-metro-black">Engineer</h4>
                                 <asp:PlaceHolder ID ="CustomEngineerPlaceHolder" ClientIDMode="Static" runat="server" Visible="false">
                                      <span class=" fg-white shadow-metro-black"><asp:Literal ID="CustomEngineer" runat="server"></asp:Literal></span>
                                       
                                    </asp:PlaceHolder>
                                    <asp:DropDownList Width="100%" ClientIDMode="Static" ID='checkout_ServiceEngineer' AppendDataBoundItems="true" runat="server"   CssClass=" form-control">
                                        <asp:ListItem Text="--Select One--" Value="" /> 
                                    </asp:DropDownList>
                                <hr />
                                <h4 class="fg-white shadow-metro-black">User</h4>
                                 <asp:DropDownList Width="100%" ClientIDMode="Static" ID='checkout_ShippingPerson' AppendDataBoundItems="true" runat="server"  CssClass=" form-control">
                                        <asp:ListItem Text="--Select One--" Value="" /> 
                                    </asp:DropDownList>
                                <hr />
                                <h4 class="fg-white shadow-metro-black">Order Number</h4>

                                 <div class="">                                    
                                    <input runat="server" id="checkout_OrderNumber" type="text" class="form-control" placeholder="Order Number" />
                                </div>
                            </div>
                        </div>
                     </div>
           
                     
            
                     
                  
                                     <div class="col-md-9">
                  <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Items:</span></div>
                            <div class="awp_box_content bg-sg-box"">
                                <div class="row">
                                    <asp:UpdatePanel runat="server" ID="OutCartUpdatePanel" ClientIDMode="Static" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                        <ContentTemplate>   
                                            <asp:Repeater runat="server" ID ="FinalCheckoutRepeater" ClientIDMode="Static" OnItemCommand="FinalCheckoutRepeater_ItemCommand" >                    
                                                <ItemTemplate>    
                                                    <div  class="file " title='' draggable="true"  onclick="ShowLoader();BarcodeScanned('<%# Eval("AssetNumber")%>', '<%# Eval("IsHistoryItem")%>','<%# Eval("DateShippedTicks")%>');">
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
                                        </ContentTemplate>
                                    
                                    </asp:UpdatePanel>
                                  
                                </div>
                            </div>
                        </div>
         </div>

            </div>



        
    
</asp:Content>
