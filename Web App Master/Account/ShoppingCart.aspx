<%@ Page Title="Checkout" Language="C#" MasterPageFile="~/Site.Master" Debug="true" EnableViewStateMac="false" EnableEventValidation="false" ViewStateEncryptionMode="Never"  AutoEventWireup="true" CodeBehind="ShoppingCart.aspx.cs" Inherits="Web_App_Master.Account.ShoppingCart" %>
<asp:Content ID="ShopCartContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="bg-shaded"></div>
     <nav class="navbar navbar-inverse bg-grayDark" style="margin-top:0px !important; height:auto; width:100%; left:0px!important; border-radius:0px!important; position:fixed !important; z-index:25000;">
     
           <asp:PlaceHolder ID ="ApplyChangesButton" runat="server" Visible="true">
               <ul class="nav navbar-nav navbar-right fg-white">
                     <li class="bg-green fg-white">
            <asp:LinkButton runat="server" OnClick="ContinueToCheckOutBtn_Click"  ID="ContinueToCheckOutBtn" ClientIDMode="Static"><b style="vertical-align:bottom" class="fg-white fg-hover-black"> Place Order</b>&nbsp&nbsp<i style="vertical-align:top" class="glyphicon glyphicon-arrow-right md-glyph fg-white"></i></asp:LinkButton>
                                 
                    </li>
               </ul>

            </asp:PlaceHolder>     
    </nav>
        <asp:PlaceHolder ID ="MessagePlaceHolder" ClientIDMode="Static" runat="server" Visible="false">
        <div class="row">
                <div class="col-md-12">
                    <div class="awp_box rounded bg-sg-title shadow">
                        <div class="awp_box_title bg-sg-title">
                           <span class="fg-white shadow-metro-black"><span class="mif-warning mif-ani-flash mif-ani-slow"></span></span>
                        </div>
                        <div class="awp_box_content bg-red fg-white shadow-metro-black">
                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                       </div>
                    </div>
                </div>
            </div>

    </asp:PlaceHolder>
    <div class="bg-metro" style=" padding-top:60px !important"></div>
            <div class="row">
                     <div class="col-md-3" id="ShippingOptionsBox">
                         <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Shipping Options</span></div>
                            <div class="awp_box_content bg-sg-box" style="text-align:left;" >   
                                <h4 class="fg-white shadow-metro-black"> <%= Page.User.Identity.GetUserName() %></h4>
                                <h4 class="fg-white shadow-metro-black">
                                    <asp:Label Text="Your Email" CssClass="fg-white shadow-metro-black" runat="server" ID="AnonLabel" Visible="false"  />
                                    <asp:TextBox Visible="false" ID="AnonUserInput" CssClass="form-control" runat="server" />
                                </h4>
                                <hr />
                                <h4 class="fg-white shadow-metro-black">Ship To</h4>
                                <div id ="CustomerAddressInput">
                                    <asp:DropDownList Width="100%" ClientIDMode="Static" ID='checkout_ShipTo' AppendDataBoundItems="true" runat="server"  CssClass="form-control">
                                        <asp:ListItem Text="--Select One--" Value="" /> 
                                    </asp:DropDownList></div>
                                <span><input id="cb_CustomAddress" runat="server" type="checkbox" name="cb_CustomAddress" value="" onchange="CustomCheckoutAddressChecked()" style="display:inline-block; vertical-align:middle; margin-right:5px;" /><h5 style="display:inline-block;"  class="fg-white shadow-metro-black">Custom Address</h5>
</span>
                                <div id="CustomAddressInput" style="display:none">
                                  <input runat="server" id="SprCompany" type="text" class="form-control" placeholder="CompanyName">
                                                <input runat="server" id="SprAddr" type="text" class="form-control" placeholder="Address">
                                                <input runat="server" id="SprAddr2" type="text" class="form-control" placeholder="Address Line #2">
                                                <input runat="server" id="SprCty" type="text" class="form-control" placeholder="City">
                                                <input runat="server" id="SprState" type="text" class="form-control" placeholder="State">
                                                <input runat="server" id="SprPostal" type="text" class="form-control" placeholder="Postal">
                                                <input runat="server" id="SprCountry" type="text" class="form-control" placeholder="Country">
                                                <input runat="server" id="SprName" type="text" class="form-control" placeholder="Attn">
                                                <input runat="server" id="SprEmail" type="text" class="form-control" placeholder="Email">
                                                <input runat="server" id="SprPhone" type="text" class="form-control" placeholder="Phone">  
                                </div>
                                <hr />
                                <div class="">                                    
                                    <input runat="server" id="checkout_OrderNumber" type="text" class="form-control" placeholder="Comment" />
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
                                                                <asp:Repeater runat="server"  EnableViewState="false" ID ="FinalCheckoutRepeater" ClientIDMode="Static" OnItemCommand="FinalCheckoutRepeater_ItemCommand" >                    
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
                                                            </ContentTemplate>
                                    
                                                        </asp:UpdatePanel>
                                  
                                                    </div>
                                                </div>
                                            </div>
                             </div>

                     
             </div>


        
    
</asp:Content>
