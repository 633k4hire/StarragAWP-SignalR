<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultView.ascx.cs" Inherits="WebVault.Templates.icon_view" %>
<!--Icon Template-->
<link href="../Content/jquery.contextMenu.css" rel="stylesheet" />
<script src="../Scripts/jquery.contextMenu.js"></script>
<script src="../Scripts/jquery.ui.position.js"></script>
<script src="../Scripts/FileBrowserContextMenuActions.js"></script>
<div  class="app-file" title='<%# Eval("ToolTip") %>' draggable="true">
    <asp:LinkButton ID="IconLink"  CommandName='<%# Eval("Id") %>' runat="server" CommandArgument="" >
    <div class="i-icon icon-folder">   
        <img src='<%# Eval("Image") %>' />
    </div>

    <div runat="server" class="i-title"><%# Eval("Title") %></div>
    </asp:LinkButton>
    <div class="i-check">
        <asp:CheckBox Text="" runat="server" ID="ItemCheckBox"  ClientIDMode="Static"/></div>
    <div runat="server" id="DownloadLink" class="i-download"><a target="_blank" href='<%# Eval("Url") %>'><span class="glyphicon glyphicon-download"></span></a></div>
</div>

