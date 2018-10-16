<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MasterLog.aspx.cs" Inherits="Web_App_Master.MasterLog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function ()
        {
            setTimeout(PollMessages, 1000);
        });
        function PollMessages() {
            $("#LogArgument").val("Poll Message");
            $("#LogSuperButtonArg").val("");
            $("#LogCommand").val("");
            $("#LogSuperButton").click();
            setTimeout(PollMessages, 1000);
        }
    </script>
    <asp:UpdatePanel ID="LogUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>   
            <asp:Repeater ID="LogRepeater" runat="server">
                <ItemTemplate>  
                    <div class="row">
                        <div class="col-md-12">
                            <div class="border-bottom-blue">
                                <%# Eval("Entry") %>
                            </div>   
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </ContentTemplate>
    </asp:UpdatePanel>
      <asp:TextBox runat="server" ID ="LogArgument" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="LogCommand" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="LogSuperButtonArg" ClientIDMode="Static" style="display:none"></asp:TextBox>
<asp:Button ID="LogSuperButton" runat="server" Text="CLICK ME" OnClick="LogSuperButton_Click" style="display:none" ClientIDMode="Static"/>
</asp:Content>
