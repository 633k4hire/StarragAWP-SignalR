<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CheckIn.aspx.cs" Inherits="Web_App_Master.Account.CheckIn" %>
<asp:Content ID="CheckInContent" ContentPlaceHolderID="MainContent" runat="server">


     <nav class="navbar navbar-inverse bg-grayDark" style="margin-top:0px !important; height:auto; width:100%; left:0px!important; border-radius:0px!important; position:fixed !important; z-index:25000;">
         
        <ul class="nav navbar-nav starrag-menu">
            <li class="fg-white"><h2>Checkin</h2> </li>
           <li>
                    <asp:LinkButton ID ="CheckInViewBtn" OnClick="CheckInViewBtn_Click" ToolTip="Check In" ClientIDMode="Static" runat="server"><span id="CheckInIcon" runat="server" class="glyphicon glyphicon-cloud-upload md-glyph"></span></asp:LinkButton>
            </li>
            <li>
                    <asp:LinkButton ID ="ReportViewBtn" OnClick="ReportViewBtn_Click" ToolTip="Reports" ClientIDMode="Static" runat="server"><span id="ReportIcon" runat="server" class=" glyphicon glyphicon-file md-glyph"></span></asp:LinkButton>
            </li>
        </ul>
           <asp:PlaceHolder ID ="ApplyChangesButton" runat="server" Visible="true">
               <ul class="nav navbar-nav navbar-right starrag-menu">
                   <li>
                    <asp:LinkButton runat="server" OnClick="TransferAssets_Click"  ID="TransferAssets" ClientIDMode="Static" style="vertical-align:top; text-decoration:none"><span class="btn bg-green "><strong>Transfer</strong></span></asp:LinkButton>
                </li>
                   <li>
                <asp:LinkButton runat="server" OnClick="Finalize_Click"  ID="Finalize" ClientIDMode="Static" style="vertical-align:top;text-decoration:none"><span class="bg-starrag btn"><strong >Finalize</strong></span></asp:LinkButton>
                </li>
                
               </ul>

            </asp:PlaceHolder>  
         <asp:PlaceHolder ID="LeavePlaceHolder" runat="server" Visible="false">
             <ul class="nav navbar-nav navbar-right starrag-menu">
                <li>
                   <a href="/Default">
                   <span class="shadow-metro-black"><i title="Leave"  style="font-size:1em;" class="mif-cross av-hand-cursor shadow-metro-black"></i><strong>&nbsp&nbsp Leave</strong></span></a>
                </li>   
                 </ul>
            </asp:PlaceHolder>
    </nav>



    <div class="bg-metro" style=" padding-top:60px !important"></div>
    <asp:MultiView ID="CheckInMultiView" ActiveViewIndex="0" runat="server">
        <asp:View ID="CheckInView" runat="server">
                <div class="row">
                   <div class="col-md-12">
                         <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Items:</span></div>
                            <div class="awp_box_content bg-sg-box"">
                                <div class="row">
                                    <asp:UpdatePanel runat="server" ID="CheckInPageUpdatePanel" ClientIDMode="Static" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                        <ContentTemplate>   
                                            <asp:Repeater runat="server" ID ="FCIrepeater" ClientIDMode="Static" OnItemCommand="FinalCheckInRepeater_ItemCommand" >                    
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
                                        </ContentTemplate>
                                       
                                    </asp:UpdatePanel>
                                  
                                </div>
                            </div>
                        </div>
                     </div>

              </div>

        </asp:View>
        <asp:View ID="ReportView" runat="server">
             <script type="text/javascript">

                $(document).ready(function () {
                   
                    
                    function calcIframeDivHeight() {
                        try {
                            var newHeight = 0;
                        var myViewHeight = jQuery(".main-content").height();
                        newHeight = myViewHeight - 160;
                        $("#IframeDiv").height(newHeight);
                        $("#MainContent_ReportFrame").height(newHeight);
                        $("#MainContent_ReportFrame").width($("#IframeDiv").width());
                        } catch (er) { }
                            
                    }

                    // run on page load   
                    try {calcIframeDivHeight(); } catch (er) { }
                    
                    
                    // run on window resize event
                    $(window).resize(function () {
                        try {calcIframeDivHeight(); } catch (er) { } 
                    });

                });

            </script>
            <div class="row">
            <div class="col-md-12">
                <!--    <span class="awp-save-btn bg-green fg-white shadow  mif-ani-flash " onclick="printpackingslip()"><i title="Print"  style="font-size:2em; vertical-align:top" class="mif-printer av-hand-cursor fg-white shadow-metro-black"></i></span>--> 
                <div class="awp_box rounded bg-sg-title shadow">
                    <div class="awp_box_title bg-sg-title">
                       <span class="fg-white shadow-metro-black">Receiving Report:</span>
                    </div>
                    <div class="awp_box_content bg-sg-box" style="text-align:left!important;">
                         <div id="ReceivingHidden" style="display:none"><asp:Literal ID="ReceivingLink" runat="server"></asp:Literal> </div>
                        <div id="IframeDiv" style="min-height:100px;height:300px; text-align: left!important">
                         <iframe frameborder="0" style="position:absolute; min-height:100px;" id="ReportFrame" runat="server" src="/Account/Templates/blank.pdf" ></iframe>
                         </div>
                    </div>
                </div>
            </div>  
        </div>
        </asp:View>
    </asp:MultiView>

        
</asp:Content>
