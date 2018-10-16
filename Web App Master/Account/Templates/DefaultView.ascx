<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultView.ascx.cs" Inherits="Web_App_Master.Account.Templates.icon_view" %>
<!--Icon Template-->

<div  class="file " title='' draggable="true"  onclick="ShowLoader();BarcodeScanned('<%# Eval("AssetNumber")%>', '<%# Eval("IsHistoryItem")%>','<%# Eval("DateShippedTicks")%>');">
           <div id =" file-color " class="file-color <%# Eval("Color") %>" style="width:100%; height:100%;"></div>

    <div class="i-icon">   
        <img style="box-shadow: rgba(0, 0, 0, 0.70) 0px 0px 10px;" src='<%# Eval("FirstImage")+".r?w=133&h=100" %>' />
    </div>

    <div runat="server" class="i-title"><strong class="fg-white shadow-metro-black"><%# Eval("AssetName") %></strong> </div>
  
    <div class="i-check">
<span id="InOutIndicator"  runat="server" class="edge-badge fg-white shadow-metro-black icon-badge " style="z-index:3 !important;"><strong><%# Eval("AssetNumber")%></strong></span></div>
    <div runat="server" id="DownloadLink" class="i-download" style="margin-top:-4px;padding-left:2px">         <i title="Add To Cart" id="dl_link"  style="font-size:1.5em;" class="fa fa-shopping-cart c-lime av-add-cursor" onclick="<%# prepareAjax(Eval("AssetNumber").ToString(),Eval("IsOut").ToString())%>"></i>
</div>

</div>



