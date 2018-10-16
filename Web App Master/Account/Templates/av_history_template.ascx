<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="av_history_template.ascx.cs" Inherits="Web_App_Master.Account.Templates.av_history_template" %>

<div class="list shadow" style="overflow:hidden; text-align:left" onclick="BarcodeScanned('<%# Eval("AssetNumber")%>', '<%# Eval("IsHistoryItem")%>','<%# Eval("DateShippedTicks")%>');">
    <span id="InOutIndicator" runat="server" class="list-title fg-white shadow-metro-black" style="margin-left:5px;padding-left:5px;"><strong><%# Eval("AssetNumber") %> - <%# Eval("AssetName") %> - <%# Eval("DateRecievedString") %> - <%# Eval("DateShippedString") %></strong></span>
</div>
