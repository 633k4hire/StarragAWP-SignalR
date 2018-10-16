<%@ Page Title="Check Out" Language="C#" EnableViewStateMac="false" MasterPageFile="~/Site.Master" Async="true" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="Web_App_Master.Account.Checkout1" %>
<asp:Content ID="CheckOutContent" ContentPlaceHolderID="MainContent" runat="server">

              <nav class="navbar navbar-inverse bg-grayDark shadow-bottom" style="margin-top:0px !important;  width:100%; left:0px!important; height:auto; z-index:25000; border-radius:0px !important; position:fixed;">
                  <div class="container-fluid">
                    <div class="navbar-header">
                      <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#CheckoutMenu" style="float:left !important">
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>                        
                      </button>      
                    </div>
                    <div id="CheckoutMenu" class="collapse navbar-collapse" >
                        <ul class="nav navbar-nav starrag-menu fg-white">
                            <li class="fg-white"><h2>Checkout</h2> </li>
                            <li>
                                <asp:LinkButton ID ="ShippingViewBtn" OnClick="ShippingViewBtn_Click" ToolTip="Shipping Options" ClientIDMode="Static" runat="server"><span id="ShipLink" runat="server" class=" glyphicon glyphicon-cloud-download md-glyph"></span></asp:LinkButton>
                            </li>           
                            <li>
                                <asp:LinkButton ID ="PackingSlipViewBtn" OnClick="PackingSlipViewBtn_Click" ToolTip="Report" ClientIDMode="Static" runat="server"><span id="ReportLink" runat="server" class=" glyphicon glyphicon-file md-glyph"></span></asp:LinkButton>
                            </li>    
                        </ul>
                        <ul class="nav navbar-nav starrag-menu navbar-right" style="padding-right:18px;">
                            <asp:PlaceHolder ID="FinalizeHolder" runat="server" Visible="true">
                                <li>
                                   <asp:LinkButton CssClass="control-button" ID="FinalizeNoModalBtn" runat="server" OnClientClick="ShowLoader()" OnClick="FinalizeBtn_Click" >
                                   <span class="shadow-metro-black"><i title="Finalize"  style="font-size:1em;" class="glyphicon glyphicon-floppy-disk av-hand-cursor shadow-metro-black"></i><strong>&nbsp&nbsp Finalize</strong></span></asp:LinkButton>
                                </li>   
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="LeavePlaceHolder" runat="server" Visible="false">
                                <li>
                                   <a href="/Account/AssetView">
                                   <span class="shadow-metro-black"><i title="Leave"  style="font-size:1em;" class="mif-cross av-hand-cursor shadow-metro-black"></i><strong>&nbsp&nbsp Leave</strong></span></a>
                                </li>   
                            </asp:PlaceHolder>
                        </ul>
                    </div>
                  </div>
                </nav>





    <div style="padding-top:60px;"></div>
    <asp:PlaceHolder ID ="MessagePlaceHolder" ClientIDMode="Static" runat="server" Visible="false">
        <div class="row">
                <div class="col-md-12">
                    <div class="awp_box rounded bg-sg-title shadow">
                        <div class="awp_box_title bg-sg-title">
                           <span class="fg-white shadow-metro-black"><span class="mif-warning mif-ani-flash mif-ani-slow"></span></span>
                        </div>
                        <div class="awp_box_content bg-red fg-white shadow-metro-black">
                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                       </div>
                    </div>
                </div>
            </div>

    </asp:PlaceHolder>
    <asp:MultiView ID="CheckoutView" ActiveViewIndex="0" runat="server">
        <asp:View ID="ShippingView" ClientIDMode="Static" runat="server">
            <div id ="ShipmentInformationGroup" class="row">

                 <asp:UpdatePanel ID ="AddressUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true" runat="server">
                     <ContentTemplate>
                          <div class="col-md-3">

                         <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black bold"><span class="mif-home mif-2x"></span>Source</span>
                            </div>
                            <div class="awp_box_content bg-sg-box">   
                                    <input runat="server" id="SprCompany" type="text" class="form-control" placeholder="CompanyName">
                                    <input runat="server" id="SprAddr" type="text" class="form-control" placeholder="Address">
                                    <input runat="server" id="SprAddr2" type="text" class="form-control" placeholder="Address Line #2">
                                    <input runat="server" id="SprCty" type="text" class="form-control" placeholder="City">
                                    <input runat="server" id="SprState" type="text" class="form-control" placeholder="State">
                                    <input runat="server" id="SprPostal" type="text" class="form-control" placeholder="Postal">
                                    <input runat="server" id="SprCountry" type="text" class="form-control" placeholder="Country">
                                    <input runat="server" id="SprName" type="text" class="form-control" placeholder="Attn">
                                    <input runat="server" id="SprEmail" type="text" class="form-control" placeholder="Email">
                                    <input runat="server" id="SprPhone" type="text" class="form-control" placeholder="Phone">  
                            </div>
                        </div>
                     </div>
                     <div class="col-md-3">
                       
                         <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black bold"><span class="mif-my-location mif-2x"></span>Destination</span></div>
                            <div class="awp_box_content bg-sg-box">   
                                    <asp:DropDownList ViewStateMode="Disabled"  AutoPostBack="true" OnTextChanged="checkout_ShipTo_TextChanged" Width="100%" ClientIDMode="Static" ID='checkout_ShipTo' AppendDataBoundItems="true" runat="server"  CssClass="form-control">
                                        <asp:ListItem Text="--Select One--" Value="" /> 
                                    </asp:DropDownList>
                                    <input runat="server" id="ToCompany" type="text" class="form-control" placeholder="CompanyName">
                                    <input runat="server" id="ToAddr" type="text" class="form-control" placeholder="Address">
                                    <input runat="server" id="ToAddr2" type="text" class="form-control" placeholder="Address Line #2">
                                    <input runat="server" id="ToCty" type="text" class="form-control" placeholder="City">
                                    <input runat="server" id="ToState" type="text" class="form-control" placeholder="State">
                                    <input runat="server" id="ToPostal" type="text" class="form-control" placeholder="Postal">
                                    <input runat="server" id="ToCountry" type="text" class="form-control" placeholder="Country">
                                    <input runat="server" id="ToName" type="text" class="form-control" placeholder="Attn">
                                    <input runat="server" id="ToEmail" type="text" class="form-control" placeholder="Email">
                                    <input runat="server" id="ToPhone" type="text" class="form-control" placeholder="Phone">                                                          

                            </div>
                        </div>
                     </div>
                     </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="checkout_ShipTo" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                    

                    



                     <div class="col-md-3">
                           
                         <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black bold"><span class="mif-widgets mif-2x"></span>Shipping Options</span></div>
                            <div class="awp_box_content bg-sg-box" style="padding-left:0px !important; padding-right:0px !important; text-align:left !important">  
                               
                                    <div class="col-md-12" style="padding-left:0px; padding-top:15px; text-align:left!important">
                                        <asp:DropDownList OnSelectedIndexChanged="ShippingMethodDropDownList_SelectedIndexChanged" ForeColor="Black" AutoPostBack="true" ClientIDMode="Static" ID='ShippingMethodDropDownList' AppendDataBoundItems="true" runat="server" CssClass="input-control text">
                                                <asp:ListItem Text="No Label" Value="00-NoLabel" /> 
                                                <asp:ListItem Text="01-Next Day" Value="01-NextDayAir" /> 
                                                <asp:ListItem Text="02-2nd Day" Value="02-2ndDayAir" /> 
                                                <asp:ListItem Text="03-Ground" Value="03-Ground" /> 
                                                <asp:ListItem Text="12-3 Day Select" Value="12-3DaySelect" /> 
                                                <asp:ListItem Text="13-Next Day Air Saver" Value="13-NextDayAirSaver" /> 
                                                <asp:ListItem Text="14-Next Day Air Saver Early" Value="14-NextDayAirEarly" /> 
                                                <asp:ListItem Text="59-2nd Day Early AM" Value="59-2ndDayAirA.M." /> 
                                                <asp:ListItem Text="65-UPS Saver" Value="65-UPSSaver" /> 
                                        </asp:DropDownList>
                                    </div>
                              
                                <div class="row">
                                  <div class="col-md-12" style="padding-left:3px !important; padding-right:3px!important;"> 
                                    <span class="awp-save-btn fg-white shadow">
                                        <asp:LinkButton runat="server" ID="RefreshCost" CssClass="fg-white" ClientIDMode="Static" OnClick="RefreshCost_Click">
                                        <i title="Save"  style="font-size:1em; padding:3px" class="mif-loop2 av-hand-cursor bg-green rounded shadow"></i></asp:LinkButton>
                                    </span>                                      
                                     <div class="awp_box rounded bg-sg-title shadow">           
                                        <div class="awp_box_title bg-sg-title">
                                           <span class="fg-white shadow-metro-black bold"><span class="mif-dollars mif-2x"></span>Estimated Cost</span></div>
                                        <div class="awp_box_content bg-sg-box">   
                                            <input runat="server" id="EstimatedCost" type="text" class="form-control" placeholder="$0.00">
                                        </div>
                                    </div>
                                 </div>
                                  <div class="col-md-12" style="padding-left:3px !important; padding-right:3px!important;">                                       
                                    <span class="awp-save-btn fg-white shadow">
                                        <asp:LinkButton runat="server" ID="RefreshArrival" CssClass="fg-white" ClientIDMode="Static" OnClick="RefreshArrival_Click">
                                        <i title="Save"  style="font-size:1em; padding:3px" class="mif-loop2 av-hand-cursor bg-green rounded shadow"></i></asp:LinkButton>
                                    </span>                                       
                                      <div class="awp_box rounded bg-sg-title shadow">           
                                        <div class="awp_box_title bg-sg-title">
                                           <span class="fg-white shadow-metro-black bold"><span class="mif-calendar mif-2x"></span>Estimated Arrival</span></div>
                                        <div class="awp_box_content bg-sg-box">   
                                            <input runat="server" id="ArrivalDate" type="text" class="form-control" placeholder="Arrival Date">
                                            <input runat="server" id="ArrivalTime" type="text" class="form-control" placeholder="Arrival Time">
                                        </div>
                                    </div>
                                 </div>

                                </div>
                            </div>
                        </div>
                     </div>
                     <div class="col-md-3">
                          
                         <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black bold"><span class="mif-dropbox mif-2x"></span>Package Options</span></div>
                            <div class="awp_box_content bg-sg-box">   
                                    <input runat="server" id="PkgWeight" type="text" class="form-control" placeholder="Weight">
                                    <input runat="server" id="PkgHeight" type="text" class="form-control" placeholder="Height">
                                    <input runat="server" id="PkgWidth" type="text" class="form-control" placeholder="Width">
                                    <input runat="server" id="PkgLength" type="text" class="form-control" placeholder="Length">
                                    <input runat="server" id="PkgReference" type="text" class="form-control" placeholder="Reference">
                                    <input runat="server" id="PkgReference2" type="text" class="form-control" placeholder="2nd Reference">
                                    <input runat="server" id="PkgTracking" type="text" class="form-control" disabled="disabled" placeholder="Tracking Number"> 
                            </div>
                        </div>
                     </div>

            </div>

    
        </asp:View>
        <asp:View ID="PackingSlipView"  ClientIDMode="Static" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

         

            // create function to calculate ideal height values
            function CalcDiv() {
                try {
                    var newHeight = 0;
                    var myViewHeight = jQuery(".main-content").height();
                    newHeight = myViewHeight - 180;
                    $("#SPD").height(newHeight);
                    var aa = $("#SPD").height();
                    var a = $("#SPD").width();
                    var p = $("#PF");
                    //$("#PF").width(aa - 30);
                    var c = $("#MainContent_PF").width(a);
                    var d = $("#MainContent_PF").height(aa);
                    var e = $("#MainContent_PF");
                    $("#PF").height(a-30);
                } catch (er)
                { var eee = er; }
            } // end calcHeights function

            // run on page load    
           
            CalcDiv();
          
            // run on window resize event
            $(window).resize(function () {
                CalcDiv();
            });

        });
       

    </script>
            <div class="row">
            <div class="col-md-12" style="margin-bottom:0px!important;">
                <!--    <span class="awp-save-btn bg-green fg-white shadow  mif-ani-flash " onclick="printpackingslip()"><i title="Print"  style="font-size:2em; vertical-align:top" class="mif-printer av-hand-cursor fg-white shadow-metro-black"></i></span>--> 
                <div class="awp_box rounded bg-sg-title shadow">
                    <span class="top-right-btn" ><asp:LinkButton OnClick="RemakeLabelsBtn_Click" ID="RemakeLabelsBtn" runat="server" CssClass="btn bg-green fg-white">Re-Make Labels</asp:LinkButton><asp:LinkButton OnClick="ShowLabelsBtn_Click" ID="ShowLabelsBtn" runat="server" CssClass="btn bg-green fg-white">Show</asp:LinkButton> </span>
                    <div class="awp_box_title bg-sg-title">
                       <span class="fg-white shadow-metro-black">Packing Slip + Tracking: <asp:Label ID="TrackingNumberLabel" runat="server" Text=""></asp:Label></span>
                    </div>
                    <div class="awp_box_content bg-sg-box">
                             <asp:UpdatePanel runat="server" ID="PdfUpdatePanel" UpdateMode="Conditional">
                                 <ContentTemplate>
                                     <div id="SPD" style="text-align:center;">  
                                         <div id="PackingSlipHidden" style="display:none"><asp:Literal ID="PackingSlipLink" runat="server"></asp:Literal> </div>
                                         <iframe style="height:80%; position:relative; z-index:2000;" id="PF" runat="server" src="/Account/Pdfs/blank.pdf"  ></iframe>
                                     </div>
                                 </ContentTemplate>
                             </asp:UpdatePanel>
                             
                         </div>
                    </div>
                </div>
            </div>         
        </asp:View>
        <asp:View ID="ShippingLabelView"  ClientIDMode="Static" runat="server">   
           
            <div class="row">
                <div class="col-md-12">
                    <span class="awp-save-btn bg-green fg-white shadow  mif-ani-flash " onclick="printimage()"><i title="Print"  style="font-size:2em; vertical-align:top" class="mif-printer av-hand-cursor fg-white shadow-metro-black"></i></span>
                    <div class="awp_box rounded bg-sg-title shadow">
                        <div class="awp_box_title bg-sg-title">
                           <span class="fg-white shadow-metro-black"></span>
                        </div>
                        <div class="awp_box_content bg-sg-box">            
                            <div id="UpsDiv" style="min-height:100px;height:600px;">
                                <div id="ups_label_only">
                                    <div id="imgcontainer" runat="server"><img id ="UpsLabelImgBox" runat="server" src="/Images/transparent.png" /></div>                                
                                </div>
                            </div>
                       </div>
                    </div>
                </div>
            </div>
         
        </asp:View>
    </asp:MultiView>
    <div id="FinalizeConfirmDialog" class="awp-outer-dialog" style="display:none" >
        <div class=" awp-inner-dialog" style="opacity:initial !important">

                    <span class="awp-dialog-close-btn bg-red fg-white shadow " onclick="HideDiv('FinalizeConfirmDialog')"><i title="Close"  style="vertical-align:top" class="mif-cross av-hand-cursor fg-white shadow-metro-black"></i></span>
                    <div class="awp_box rounded bg-sg-title shadow" style="left:50% !important; top:30%">
                        <div class="awp_box_title bg-sg-title">
                           <span class="fg-white shadow-metro-black">Confirm Label Creation</span>
                        </div>
                        <div class="awp_box_content bg-sg-box">                            
                            <asp:Button CssClass="control-button" ID="but_OK" runat="server" Text="Create Labels" OnClientClick="ShowLoader()" OnClick="FinalizeBtn_Click"  /><br />
                       </div>
                    </div>
      
        </div>
    </div>
      
   
</asp:Content>
