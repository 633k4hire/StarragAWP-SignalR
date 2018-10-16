<%@ Page Title="Create Asset" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateAsset.aspx.cs" Inherits="Web_App_Master.Account.CreateAsset" %>
<asp:Content ID="CreateAsset" ContentPlaceHolderID="MainContent" runat="server">
           
         <nav class="navbar navbar-inverse bg-grayDark" style="margin-top:0px !important; height:auto; width:100%; left:0px!important; border-radius:0px!important; position:fixed !important; z-index:25000;">
        <ul class="nav navbar-nav starrag-menu">
            <li class="fg-white"><h2>Create</h2> </li>
              
        </ul>
        <ul class="nav navbar-nav navbar-right starrag-menu">
            <asp:PlaceHolder ID="saveBtnPlaceholder" runat="server" >
                <li>
                    <asp:LinkButton runat="server" OnClick="CreateAssetBtn_Click"  ID="CreateAssetBtn" ClientIDMode="Static" style="vertical-align:bottom; text-decoration:none"><strong>Save&nbsp&nbsp</strong><i style="vertical-align:top" class="glyphicon glyphicon-floppy-disk md-glyph"></i></asp:LinkButton>
                </li>
            </asp:PlaceHolder>
        </ul>           
    </nav>
    
    




    <div style="padding-bottom:60px!important"></div>
    <div class="col-md-3">
        <div class="awp_box rounded bg-sg-title shadow">
      
            <div class="awp_box_title bg-sg-title">
               <span class="fg-white shadow-metro-black">Images</span>
            </div>
            <div class="awp_box_content bg-sg-box" style="text-align:left !important;">
                                <div class="row bg-sg-box fg-white ">
                    <div class="col-md-12" style="text-align:center">
                         <span style="font-weight:bold;" class="av-hand-cursor" onclick="ShowDiv('UploadImage')" ><span class="mif-file-upload mif-2x"></span><strong>Upload Image</strong></span>
                          
                                <asp:Image BorderStyle="None" ImageUrl="/Images/transparent.png" runat="server" ID="AssetImgBox"  ImageAlign="Middle" Width="100%" />
                            
                 
                    </div>
                </div>
            </div>
        </div>
    </div>
     <div class="col-md-9">
        <div runat="server" id="CErrorBox" class=" transition-bottom bg-starrag rounded" style="z-index:99999;" visible="false">
                       
                        <div id="Cerror-box-label"><span class="mif-warning mif-2x fg-white shadow-metro-black"></span>
                        <asp:Label Text="" ID="CErrorLabel" ClientIDMode="Static" CssClass="fg-white shadow-metro-black" runat="server" />
                        </div>          
                        <div id="error-box-content">
                            <asp:Label ID="CErrorMessage" ClientIDMode="Static" CssClass="fg-white shadow-metro-black" runat="server" />
                        </div>
                    </div>  
        <div class="awp_box rounded bg-sg-title shadow">
      
            <div class="awp_box_title bg-sg-title">
               <span class="fg-white shadow-metro-black">Asset</span>
            </div>
            <div class="awp_box_content bg-sg-box" style="text-align:left !important;">
                <asp:UpdatePanel ID="createIdUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
                    <ContentTemplate>
                <div class="row bg-sg-box fg-white ">
                    <div class="col-md-10">
                         <span style="font-weight:bold;" ><strong>Asset Name</strong></span>
                         <asp:TextBox runat="server" ID="AssetName" CssClass="form-control" TextMode="SingleLine" />
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="AssetName" CssClass="text-danger" ErrorMessage="This field is required." />    
                    </div>
                    <div class="col-md-2">
                         <span style="font-weight:bold;" ><strong>Asset Number</strong></span>
                         <asp:TextBox runat="server" ID="AssetNumber" CssClass="form-control" TextMode="SingleLine" />
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="AssetNumber" CssClass="text-danger" ErrorMessage="This field is required." />    
                    </div>
                </div> 

                <div class="row bg-sg-box fg-white ">
                <div class="col-md-3">
                    <span style="font-weight:bold;" >Weight (lbs)</span>
                    <asp:TextBox runat="server" ID="Weight" CssClass="form-control" TextMode="Number" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Weight" CssClass="text-danger" ErrorMessage="This field is required." />    

               </div>
                <div class="col-md-9 " style="display:none">
                    <div class="row">
                        <div class="col-md-2" style="text-align:center">
                        <span style="font-weight:bold;" >Calibrated</span>
                            <div><asp:CheckBox ID="CalibratedCheckBox" runat="server" />
</div>
                        </div>
                        <div class="col-md-5">
                            <span style="font-weight:bold;" >Calibration Company</span>
                            <asp:TextBox runat="server" ID="CalCompanyText" CssClass="form-control" TextMode="SingleLine" />
                        </div>
                        <div class="col-md-5">
                            <span style="font-weight:bold;" >Calibration Period</span>
                            <asp:TextBox runat="server" ID="CalPeriodText" CssClass="form-control" TextMode="Number" />
                        </div>
                        
                    </div>
                </div>
                </div>
                <div class="row bg-sg-box fg-white ">
                    <div class="col-md-12">
                         <span style="font-weight:bold;" ><strong>Description</strong></span>
                         <asp:TextBox Height="150px" runat="server" ID="DescriptionText" CssClass="form-control" TextMode="MultiLine" />
                    </div>
                </div> 

                    </ContentTemplate>
                   
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
 
<div id="UploadImage" class="awp-outer-dialog" style="display:none; z-index:99998" >
        <div class=" awp-inner-dialog" style="opacity:initial !important">
                    <span class="awp-dialog-close-btn bg-red fg-white shadow " onclick="HideDiv('UploadImage')"><i title="Close"  style="vertical-align:top" class="mif-cross av-hand-cursor fg-white shadow-metro-black"></i></span>
                    <div class="awp_box rounded bg-sg-title shadow" style="left:50% !important; top:30%">
                        <div class="awp_box_title bg-sg-title">
                           <span class="fg-white shadow-metro-black"><span class="mif-file-upload mif-2x"></span>Upload Image</span>
                        </div>
                        <div class="awp_box_content bg-sg-box">  
                            <asp:FileUpload ID="creatorImageUploader" runat="server" />                           
                            <asp:Button CausesValidation="false"  runat="server" ID="UploadImg" ClientIDMode="Static" OnClick="UploadImg_Click" Text="Upload" />
                       </div>
                    </div>      
        </div>
    </div>

</asp:Content>
