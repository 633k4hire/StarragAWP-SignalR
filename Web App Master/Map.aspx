<%@ Page Title="" Language="C#" Async="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Map.aspx.cs" Inherits="Web_App_Master.Map" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
 <script src="Scripts/Map.js"></script>   
        <script type="text/javascript">

                $(document).ready(function () {
                   
                    
                    function calcDivHeight() {
                        var newHeight = 0;
                        var myViewHeight = jQuery(".main-content").height();
                        newHeight = myViewHeight - 100;
                        $("#MapDiv").height(newHeight);
                        
                    }

                    // run on page load    
                    calcDivHeight();

                    // run on window resize event
                    $(window).resize(function () {
                        calcDivHeight();
                    });

                });

            </script>
    <div class="bg" style="opacity:0.2;"></div>
     <div class="row">
            <div class="col-md-12">
                <div class="awp_box rounded bg-sg-title shadow">
                    <span class="awp-bar " style="padding-bottom:-10px!important;">
                     <span class="awp-bar-btn bg-green fg-white shadow">      
                         <asp:button runat="server" ClientIDMode="Static" ID="hiddenModeSwitch" OnClick="hiddenModeSwitch_Click" style="display:none;"></asp:button>
                         <label class="awp-switch" title="Toggle Text/Barcode Label" style="padding-left:2px!important;">
                            <input id="TestModeSwitch"  type="checkbox" checked="checked" onchange="ChangeMapView()">
                            <span class="check"></span>
                        </label>  
                     </span>
                 </span>   
                    <div class="awp_box_title bg-sg-title">
                       <span class="fg-white shadow-metro-black"><asp:Literal runat="server" ID="PdfTitle" Text="Asset Map" ></asp:Literal></span>
                    </div>
                    <div class="awp_box_content bg-sg-box">
                         <div id="MapHiddenDiv" style="display:none"><asp:Literal ID="MapHiddenData" runat="server"></asp:Literal> </div>
                        <div id="MapDiv" style="min-height:100px">
                       <div id="map" style="width:100%;height:100%"></div>
                         </div>
                    </div>
                </div>
            </div>  
        </div>
    
<script src='https://maps.googleapis.com/maps/api/js?callback=myMap&key=<%=ConfigurationManager.AppSettings["GoogleAPIKey"] %>'></script>


</asp:Content>
