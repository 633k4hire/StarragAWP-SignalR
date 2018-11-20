<%@ Page Title="Out List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OutList.aspx.cs" Inherits="Web_App_Master.OutList" %>
<asp:Content ID="OutListContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .inner {
            padding-left:0px !important;
            padding-right:0px !important;
            padding-top:0px !important;
        }
        .panel-heading{
            padding:2px 15px 2px 15px!important;
        }
        a{
            color: black;
        }
        a:hover a:focus{
            text-decoration:none;
            color: rgba(0, 175, 240, 1) !important;
            text-shadow: 0px 0px 5px rgba(0, 0, 0, 1) !important;
        }
    </style>
    <div class=" border-bottom-blue bg-white" style="margin:0px !important; padding-left:15px">
    <span class="shadow-metro-black"><strong><h3>Asset Out List</h3></strong></span>
        Sort by: <a onclick="OutListSuper('sort','assetnumber','');" href="#" title="Asset Number">Asset Number</a> |
        <a onclick="OutListSuper('sort','engineer','');" href="#" title="Service Engineer">Service Engineer</a> |
        <a onclick="OutListSuper('sort','customer','');" href="#" title="Customer">Customer</a>
    </div>
    <asp:UpdatePanel ID="OutListUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
             <asp:Repeater ID="OutListRepeater" runat="server" OnItemDataBound="OutListRepeater_ItemDataBound" OnItemCommand="OutListRepeater_ItemCommand">
                <ItemTemplate>  
                    <div class="row">
                        <div class="col-md-12">
                            <div class="border-bottom-blue bg-white text-left">
                                <asp:Button ID="SendReminderEmailBtn" Text="Send Reminder" runat="server" CommandName='<%# Eval("AssetNumber") %>' />|<%# Eval("AssetNumber") %> | <%# Eval("AssetName") %> | <%# Eval("ShipTo") %> | <%# Eval("ServiceEngineer") %>
                                
                            </div>   
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:TextBox runat="server" ID ="OutListArgument" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="OutListCommand" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="OutListSuperButtonArg" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:Button ID="OutListSuperButton" runat="server" Text="CLICK ME" OnClick="OutListSuperButton_Click" style="display:none" ClientIDMode="Static"/>
        <script type="text/javascript">
        $(document).ready(function ()
        {
            $("#BackgoundDiv").hide();
            //setTimeout(PollMessages, 1000);
        });
        function PollMessages() {
            $("#OutListArgument").val("Poll Message");
            $("#OutListSuperButtonArg").val("");
            $("#OutListCommand").val("");
            $("#OutListSuperButton").click();
            //setTimeout(PollMessages, 1000);
            }
        function OutListSuper(cmd,arg,arg2) {
            $("#OutListArgument").val(arg);
            $("#OutListSuperButtonArg").val(arg2);
            $("#OutListCommand").val(cmd);
            $("#OutListSuperButton").click();
        }

        function InitSignalRClient() {
            try {
                
                hub = $.connection.clientHub; //set hub with the server side class         
                //output = $("#output");
                
                hub.client.assetCacheChanged = function (client) { //this instanciate the shapeMoved function          receiving x, y
                    var bb = 0;
                    OutListSuper("asset-cache", "refresh", "");
                };             

                $.connection.hub.qs = { 'user': 'admin' };
                $.connection.hub.start().done(function () {

                    // hub.server.changeAssetCache("");
                    //set timeout to check client status
                    //window.onkeypress = function (e) {
                    //    if (e.charCode == 13) {
                    //        var m = $("#input");
                    //        hub.server.sendMsg("User", m.val());
                    //        e.stopPropagation();
                    //    }
                    //};
                });
            } catch( err){ }
        } 

    </script>
</asp:Content>
