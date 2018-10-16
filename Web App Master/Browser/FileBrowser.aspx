<%@ Page Title="Browser" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FileBrowser.aspx.cs" Inherits="Web_App_Master.Browser.App" %>
<asp:Content ID="BrowserContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="BrowserContent/awp.css" rel="stylesheet" />
    <link href="BrowserContent/browser.css" rel="stylesheet" />
    <link href="BrowserContent/jquery-ui.css" rel="stylesheet" />
    <link href="BrowserContent/jquery.contextMenu.css" rel="stylesheet" />
    <link href="BrowserContent/L0XX0R.css" rel="stylesheet" />

    <script src="BrowserScripts/jquery-ui.js"></script>
    <script src="BrowserScripts/jquery.contextMenu.js"></script>
    <script src="BrowserScripts/jquery.ui.position.js"></script>   
    <script src="BrowserScripts/browser.js"></script>
    <script src="BrowserScripts/jquery-resizable.js"></script>    
    <script src="BrowserScripts/webapp.js"></script>
    

        <div id="appcontainer" style="z-index:900000 !important">
        <div id="toolbar">
          
           
            <div class="l0x-toolbar">
                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white flat-link font-2-5x"  runat="server" ID="NavBack" OnClick= "NavBack_Click" ><span title="Back" class="fa font-1x fa-angle-left v-align-middle"  style="left:10px"></span></asp:LinkButton></div>
                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="NavHome" OnClick="NavHome_Click" >  <span title="Home" class="fa font-1x fa-home v-align-middle" style="left:2px"></span></asp:LinkButton></div>
                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="ShowListBtn" OnClick="ShowListBtn_Click" OnClientClick="ToggleList()" >  <span title="Show List" class="fa font-1x fa-list-alt v-align-middle" style="top:-8px !important"></span></asp:LinkButton></div>

                <div class="l0x-toolbar-Seperator bg-white"></div>              

                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="SelectAllBtn" OnClick="SelectAllBtn_Click" >  <span title="Select All" class="fa font-1x fa-check-square v-align-middle" style="left:4px"></span></asp:LinkButton></div>                
                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white flat-link font-2-5x"  runat="server" ID="CutBtn" OnClick= "CutBtn_Click" ><span title="Cut" class="fa font-1x fa-cut v-align-middle"  style="left:2px"></span></asp:LinkButton></div>
                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="CopyBtn" OnClick="CopyBtn_Click" >  <span title="Copy" class="fa font-1x fa-copy v-align-middle"></span></asp:LinkButton></div>
                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="PasteBtn" OnClick="PasteBtn_Click" >  <span title="Paste" class="fa font-1x fa-paste v-align-middle"></span></asp:LinkButton></div>
                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="DeleteBtn" OnClick="DeleteBtn_Click" >  <span title="Delete" class="fa font-1x fa-trash v-align-middle" style="left:4px"></span></asp:LinkButton></div>
                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="NewFolderBtn" OnClick="NewFolderBtn_Click" >  <span title="New Folder" class="fa font-1x fa-plus-square v-align-middle" style="left:3px"></span></asp:LinkButton></div>
                <div class="l0x-toolbar-btn"><asp:LinkButton CssClass="fg-white  flat-link font-2-5x"  runat="server" ID="DownloadZipBtn" OnClick="DownloadZipBtn_Click" >  <span title="Download Zip" class="fa font-1x fa-file-archive v-align-middle" style="left:5px"></span></asp:LinkButton></div>
</div>
        </div>
        <div id="browserbox">
                  <div class="wrap">
                    <div class="resizable resizable1">
                      <div class="inner">
                         <asp:UpdatePanel OnPreRender="DirectoryTreeUpdatePanel_PreRender" ID="DirectoryTreeUpdatePanel" ClientIDMode="Static" UpdateMode="Conditional" ChildrenAsTriggers="true" runat="server">
                            <ContentTemplate>
                              <asp:TreeView EnableViewState="false" ViewStateMode="Disabled" OnTreeNodeCheckChanged="DirTree_TreeNodeCheckChanged" 
                                  OnSelectedNodeChanged="DirTree_SelectedNodeChanged" 
                                  ShowLines="true" 
                                  runat="server"
                                  ID ="DirTree" 
                                  ClientIDMode="Static" 
                                  NodeWrap="false"
                                    >
                              </asp:TreeView>    
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="SelectAllBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="CutBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="CopyBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="PasteBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="DeleteBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="SuperButton" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="NavBack" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="NavHome" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="ShowListBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="NewFolderBtn" EventName="click" />
                                

                            </Triggers>
                        </asp:UpdatePanel>

                      </div>  
                    </div>
                    <div class="resizable resizable2">
                      <div class="inner">
                        <asp:UpdatePanel ID="FileBoxUpdatePanel" ClientIDMode="Static" UpdateMode="Conditional" ChildrenAsTriggers="true" runat="server">
                            <ContentTemplate>
                                <asp:MultiView ID="BrowserMulti" ActiveViewIndex="0" runat="server">
                                    <asp:View ID="DirViewer" ClientIDMode="Static" runat="server">
                                        <asp:Repeater  OnItemCreated="FileRepeater_ItemCreated" ID="FileRepeater" runat="server" EnableViewState="false">
                                            <ItemTemplate>
                                             
                                            </ItemTemplate>
                                        </asp:Repeater>    
                                    </asp:View>
                                    <asp:View ID ="FileViewer" ClientIDMode="Static" runat="server">
                                      File View
                                    </asp:View>
                                </asp:MultiView>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="DirClick" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="FileClick" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="SelectAllBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="SuperButton" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="CutBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="CopyBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="PasteBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="DeleteBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="NavBack" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="NavHome" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="ShowListBtn" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="NewFolderBtn" EventName="click" />
                                

                            </Triggers>
                        </asp:UpdatePanel>
                      </div>  
                    </div>
                </div>

         </div>
        <div id="footer" style="overflow:hidden !important;">
            <asp:UpdatePanel runat="server" ID ="FooterUpdatePanel" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                   <span style="display:inline-block"><asp:Label Text=":" ID="FooterLabel" CssClass="fg-white shadow-metro-black" runat="server" ClientIDMode="Static"></asp:Label></span>
                   <div style="display:none" runat="server" id="InputModal" visible="true">
                    <span style="display:inline-block">
                        <asp:TextBox Visible="true" runat="server"  ID="RenameInputBox" ClientIDMode="Static" Width="100px" Height="20px" OnTextChanged="RenameInputBox_TextChanged"></asp:TextBox>
                    </span> 
                       <span onclick="SuperButtonAsync('','inputaccept,')" class="sm-btn bg-green fg-white fa fa-check" style="display:inline-block;text-align:center;" >

                           </span>                       
                       <span onclick="SuperButtonAsync('','inputcancel,')" class="sm-btn bg-red fg-white fa fa-times" style="display:inline-block;text-align:center;">
                           
                           </span>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <asp:TextBox runat="server" ID ="avSelectedView" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="DirID" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="FileID" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="SuperButtonArg" ClientIDMode="Static" style="display:none"></asp:TextBox>
<asp:Button ID="avChangeView" runat="server" Text="CLICK ME" OnClick="avChangeView_Click" style="display:none" ClientIDMode="Static"/>
<asp:Button ID="DirClick" runat="server" Text="CLICK ME" OnClick="DirClick_Click" style="display:none" ClientIDMode="Static"/>
<asp:Button ID="FileClick" runat="server" Text="CLICK ME" OnClick="FileClick_Click" style="display:none" ClientIDMode="Static"/>
<asp:Button ID="SuperButton" runat="server" Text="CLICK ME" OnClick="SuperButton_Click" style="display:none" ClientIDMode="Static"/>

</asp:Content>
