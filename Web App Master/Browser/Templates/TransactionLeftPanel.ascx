<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TransactionLeftPanel.ascx.cs" Inherits="Web_App_Master.Browser.Templates.TransactionLeftPanel" %>
 <asp:TreeView 
     EnableViewState="false"
     ViewStateMode="Disabled"
     OnTreeNodeCheckChanged="DirTree_TreeNodeCheckChanged" 
     OnSelectedNodeChanged="DirTree_SelectedNodeChanged" 
     ShowLines="false" 
     runat="server"
     ID ="DirTree" 
     ClientIDMode="Static" 
     NodeWrap="false"
      OnDataBound="DirTree_DataBound"
      OnTreeNodeDataBound ="DirTree_TreeNodeDataBound"
></asp:TreeView>    