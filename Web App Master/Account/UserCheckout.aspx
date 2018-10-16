<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation="false" ViewStateEncryptionMode="Never" EnableViewStateMac="false" AutoEventWireup="true" CodeBehind="UserCheckout.aspx.cs" Inherits="Web_App_Master.Account.UserCheckout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  
     <div class="bg-shaded"></div>
    <div class="row">
        <div class="col-md-offset-2 col-md-4 fg-white shadow-metro-black" >
            <h1>Receipt</h1>
           
            <div>
                <span>Confimation Number:  <%= TID %></span>
            </div>
            <div>
                
            </div>
            <div>
                <span>Name:  <%= Name %></span>
            </div>
            <div>
                <span>Email:  <%= Email %></span>
            </div>
            <div>
                <span>Comment:  <%= OrderNumber %></span>
            </div>
            <div id="CustomAddressInput" style="display:normal">
                <input runat="server" id="SprCompany" type="text" readonly="readonly" class="form-control" placeholder="CompanyName">
                <input runat="server" id="SprAddr" type="text" readonly="readonly" class="form-control" placeholder="Address">
                <input runat="server" id="SprAddr2" type="text" readonly="readonly" class="form-control" placeholder="Address Line #2">
                <input runat="server" id="SprCty" type="text" readonly="readonly" class="form-control" placeholder="City">
                <input runat="server" id="SprState" type="text" readonly="readonly" class="form-control" placeholder="State">
                <input runat="server" id="SprPostal" type="text" readonly="readonly" class="form-control" placeholder="Postal">
                <input runat="server" id="SprCountry" type="text" readonly="readonly" class="form-control" placeholder="Country">
                <input runat="server" id="SprName" type="text" readonly="readonly" class="form-control" placeholder="Attn">
                <input runat="server" id="SprEmail" type="text" readonly="readonly" class="form-control" placeholder="Email">
                <input runat="server" id="SprPhone" type="text" readonly="readonly" class="form-control" placeholder="Phone">  
            </div>
        </div>
        <div class="col-md-4">
            <div>
                <asp:Repeater  EnableViewState="false" ID="ReceiptRepeater"  runat="server">
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

</asp:Content>
