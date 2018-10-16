<%@ Page Title Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintBarcodes.aspx.cs" Inherits="Web_App_Master.Account.PrintBarcodes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
                    


    <!--menu
    <div class="app-bar shadow-bottom" style="margin-top:0px !important; left:0px!important; position:fixed !important; z-index:25000;">
    
    <ul class="app-bar-menu">
        <li><h1></h1> </li>
        <li>
            <a href="#" class="dropdown-toggle">View</a>
            <ul class="d-menu" data-role="dropdown">
                <li><a href="#" onclick="ChangeAssetView('list'); ">Default</a></li>
                <li><a href="#" onclick="ChangeAssetViewListType('list-type-icons');">Icon</a></li>
                <li><a href="#" onclick="ChangeAssetViewListType('list-type-tiles');">Detail</a></li>
                <li><a href="#" onclick="ChangeAssetViewListType('list-type-listing');">Listing</a></li>
                <li><a href="#" onclick="ChangeAssetView('smtile'); ">List Tile</a></li>
                <li><a href="#" onclick="ChangeAssetView('mdtile'); ">Medium Tile</a></li>
                <li><a href="#" onclick="ChangeAssetView('lgtile'); ">large Tile</a></li>
            </ul>
        </li>
       <li>
           <div class="input-control text" data-role="input" style="width:250px !important;">
               <asp:TextBox ID="avSearchString" runat="server" ClientIDMode="Static" CssClass="text"></asp:TextBox>
  
           </div>

       </li>
       <li>             
            <asp:LinkButton ID ="AssetSearchBtn" ClientIDMode="Static" runat="server"  ><span class="glyphicon glyphicon-search""></span></asp:LinkButton>
             
       </li>
        <li>
            <a href="#" style="text-decoration:none" class="dropdown-toggle">&nbsp<span title="Search Filter" class="mif-settings-ethernet"></span></a>            
            <ul class="d-menu" data-role="dropdown">
                <li><a id ="avAssetSearchBtn" runat="server" onclick="SetSearchType('asset')" >Asset Search</a> </li>
                <li><a id ="avHistorySearchBtn" runat="server" onclick="SetSearchType('history')" >History Search</a> </li>

            </ul>
        </li>
        <li>
            <a href="#" style="text-decoration:none" class="dropdown-toggle">&nbsp<span title="View Filter" class="glyphicon glyphicon-filter"></span></a>            
            <ul class="d-menu" data-role="dropdown">
                <li><asp:LinkButton ID ="ViewAll" ClientIDMode="Static" runat="server"  >View All</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterIsOut" ClientIDMode="Static" runat="server"  >Checked Out Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterIsIn" ClientIDMode="Static" runat="server"  >Checked In Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterIsDamaged" ClientIDMode="Static" runat="server"  >Damaged Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterOnHold" ClientIDMode="Static" runat="server"  >On Hold Only</asp:LinkButton> </li>
                <li><asp:LinkButton ID ="FilterCalibrated" ClientIDMode="Static" runat="server"  >Calibrated Only</asp:LinkButton> </li>

            </ul>
        </li>
       
        
        
    </ul>
   
</div>
--><!--updatepanel-->
     <div class="row">
               
               <!--REPEATER-->

        
    <script type="text/javascript">

                $(document).ready(function () {
                   
                    
                    function calcIframeDivHeight() {
                        var newHeight = 0;
                        var myViewHeight = jQuery(".main-content").height();
                        newHeight = myViewHeight - 105;
                        $("#PrintoutDiv").height(newHeight);
                        $("#PrintControlDiv").height(newHeight);
                       // $("#MainContent_ReportFrame").height(newHeight);
                      //  $("#MainContent_ReportFrame").width($("#PrintoutDiv").width());     
                    }

                    // run on page load    
                    calcIframeDivHeight();

                    // run on window resize event
                    $(window).resize(function () {
                        calcIframeDivHeight();
                    });

                });

            </script>
        <div class="col-md-3">
                <!--    <span class="awp-save-btn bg-green fg-white shadow  mif-ani-flash " onclick="printpackingslip()"><i title="Print"  style="font-size:2em; vertical-align:top" class="mif-printer av-hand-cursor fg-white shadow-metro-black"></i></span>--> 
               
            <div class="awp_box rounded bg-sg-title shadow">
                <span class="awp-bar " style="padding-bottom:-10px!important;">
                     <span class="awp-bar-btn bg-green fg-white shadow">      
                         <asp:button runat="server" ClientIDMode="Static" ID="hiddenModeSwitch" OnClick="hiddenModeSwitch_Click" style="display:none;"></asp:button>
                         <label class="awp-switch" title="Toggle Text/Barcode Label" style="padding-left:2px!important;">
                            <input id="TestModeSwitch"  type="checkbox" checked="checked" onchange="ChangeLabelDisplay()">
                            <span class="check"></span>
                        </label>  
                     </span>
                 </span>       
                    <div class="awp_box_title bg-sg-title">
                       <span class="fg-white shadow-metro-black">Label Options</span>
                    </div>
                    <div class="awp_box_content bg-sg-box fg-white">
                        <div id="PrintControlDiv" style="min-height:500px;height:300px;">
                            <span><strong>Start Position</strong></span>
                            <asp:TextBox runat="server" ID="StartPos" CssClass="form-control" TextMode="Number" Text="1" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="StartPos"
                                CssClass="text-danger" ErrorMessage="Must be a number." />
                                <div class="awp_box rounded bg-sg-title shadow">
                                    <span class="awp-bar ">
                                  <span class="awp-bar-btn bg-green fg-white shadow"><asp:LinkButton runat="server" ID="SelectAllBtn"  OnClick="SelectAllBtn_Click"> <i title="Select All" class="glyphicon glyphicon-check fg-white"></i></asp:LinkButton></span>
                              </span>
                                <div class="awp_box_title bg-sg-title">
                                   <span class="fg-white shadow-metro-black">Assets</span>
                                </div>
                                <div class="awp_box_content bg-sg-box">                 
                                        <div class ="ShowScroll" style="height:250px; width:100%;">
                                            <asp:UpdatePanel ChildrenAsTriggers="true" UpdateMode="Conditional" ID="AssetChecklistUpdatePanel" runat="server">
                                                <ContentTemplate>   
                                                     <asp:CheckBoxList  ID="AssetCheckList" runat="server" Width="100%" Height="100%"  style="text-align:left">
                                           </asp:CheckBoxList>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="SelectAllBtn" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                          
                                        </div>
                                </div>
                            </div>
                            <asp:Button CssClass="form-control" ID="PreviewBtn" OnClick="PreviewBtn_Click" Width="100%"  Text="Preview" runat="server"/>
                         </div>
                    </div>
                </div>
            </div>  
            <div class="col-md-9">
                <!--    <span class="awp-save-btn bg-green fg-white shadow  mif-ani-flash " onclick="printpackingslip()"><i title="Print"  style="font-size:2em; vertical-align:top" class="mif-printer av-hand-cursor fg-white shadow-metro-black"></i></span>--> 
                <div class="awp_box rounded bg-sg-title shadow">
                    <div class="awp_box_title bg-sg-title">
                       <span class="fg-white shadow-metro-black">Labels</span>
                    </div>
                    <div class="awp_box_content bg-sg-box">
                        <asp:UpdatePanel  runat="server" ID="BarcodePrintUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>   
                                 <div id="ReceivingHidden" style="display:none"><asp:Literal ID="ReceivingLink" runat="server"></asp:Literal> </div>
                                        <div id="PrintoutDiv" style="min-height:100px;height:300px;">
                                       <!-- <iframe frameborder="0" style="position:absolute; min-height:100px;" id="ReportFrame" runat="server" src="#" ></iframe>--> 
                                            <object  id="PdfEmbeddedViewer" data='<%= userid %>' type="application/pdf" width="100%" height="100%">
                                             <p>This browser does not support PDFs. Please download the PDF to view it: <a href="/pdf/sample-3pp.pdf">Download PDF</a>.</p>
                                            </object>s
                                         </div>
                             </ContentTemplate>
                                <Triggers>
                                   
                                </Triggers>
                             </asp:UpdatePanel>
                        
                    </div>
                </div>
            </div>  

         </div>
   
<asp:TextBox runat="server" ID ="avSelectedSearch" ClientIDMode="Static" style="display:none"></asp:TextBox>
<asp:TextBox runat="server" ID ="avSelectedView" ClientIDMode="Static" style="display:none"></asp:TextBox>
<asp:Button ID="avChangeView" runat="server" Text="CLICK ME"  style="display:none" ClientIDMode="Static"/>


    </asp:Content>
