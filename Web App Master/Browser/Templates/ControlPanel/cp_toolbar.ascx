<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="cp_toolbar.ascx.cs" Inherits="Web_App_Master.Browser.Templates.ControlPanel.cp_toolbar" %>

<div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white flat-link font-2-5x"  runat="server" ID="NavBack" ><span title="Back" class="fa font-1x fa-angle-left v-align-middle"  style="left:10px"></span></asp:LinkButton></div>
<div class="l0x-toolbar-btn"><asp:LinkButton  CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="NavHome"  >  <span title="Home" class="fa font-1x fa-home v-align-middle" style="left:2px"></span></asp:LinkButton></div>
<div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="ShowListBtn"  OnClientClick="ToggleList()" >  <span title="Show List" class="fa font-1x fa-list-alt v-align-middle" style="top:-8px !important"></span></asp:LinkButton></div>

<div class="l0x-toolbar-Seperator bg-white"></div>           