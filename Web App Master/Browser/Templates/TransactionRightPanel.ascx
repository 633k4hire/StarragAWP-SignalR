<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TransactionRightPanel.ascx.cs" Inherits="Web_App_Master.Browser.Templates.TransactionRightPanel" %>
<div id="TopPane" style="height:100px;width:100%;  box-shadow: rgba(0, 175, 240, 0.70) 0px 0px 15px; border-bottom: 2px solid rgba(0, 175, 240,1);">
    <h2 class="fg-black shadow-metro-black"><asp:Label Text="Transaction" runat="server" /></h2>
</div>
<div id="BottomPane" style="width:100%; height:100%">
    <asp:Repeater ID="TransactionItemRepeater"  runat="server">
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