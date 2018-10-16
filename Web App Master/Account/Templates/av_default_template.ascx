<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="av_default_template.ascx.cs" Inherits="Web_App_Master.Account.Templates.av_default_template" %>

<div class="list shadow" style="overflow:hidden;" onclick="BarcodeScanned('<%# Eval("AssetNumber")%>', '<%# Eval("IsHistoryItem")%>','<%# Eval("DateShippedTicks")%>');">
    <i title="Add To Task List"  style="font-size:1.5em;" class="glyphicon glyphicon-plus c-lime av-add-cursor" onclick="<%# prepareAjax(Eval("AssetNumber").ToString(),Eval("IsOut").ToString())%>"></i>
    <img src='<%# Eval("FirstImage") %>' class="list-icon">
    <span id="InOutIndicator" runat="server" class="list-title fg-white shadow-metro-black" style="margin-left:5px;padding-left:5px;"><strong><%# Eval("AssetNumber") %> - <%# Eval("AssetName") %></strong></span>
</div>