<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentViewer.ascx.cs" Inherits="Web_App_Master.DocumentViewer" %>
    <script>
        function doSomething(arg) {
            $("#ASuperBtnArg").val(arg);
            var a = $("#ASuperBtn");
            $("#ASuperBtn").click();
    }
    </script>
    <asp:UpdatePanel ID ="AssetDocumentsViewer" UpdateMode="Conditional" runat="server">
        <ContentTemplate> 
             <div class="row">     
                 <div class="col-md-3 bg-starrag">
                    <asp:UpdatePanel runat="server" ID="AssetDocumentTreeUpdatePanel" UpdateMode="Conditional">
                        <ContentTemplate>
                             
                            <div class="awp_box rounded bg-sg-title shadow">
                                <div class="awp_box_title bg-sg-title">
                                   <span class="fg-white shadow-metro-black">Packing Slip + Tracking: <asp:Label ID="Label1" runat="server" Text=""></asp:Label></span>
                                </div>
                                <div class="awp_box_content bg-sg-box" style="min-height:400px;">
                                    <asp:TreeView 
                                    EnableViewState="true"                                    
                                    ViewStateMode="Enabled"  
                                    ShowLines="false" 
                                    runat="server"
                                    ID ="AssetDocumentTree" 
                                    ClientIDMode="Static" 
                                    NodeWrap="false"                                       
                                    ></asp:TreeView>   
                                </div>
                            </div>


                        </ContentTemplate>                            
                    </asp:UpdatePanel>

                 </div>
                 <div class="col-md-9 bg-starrag">
                     <asp:UpdatePanel ID="AssetDocumentIframeUpdatePanel" UpdateMode="Conditional" runat="server">
                         <ContentTemplate>  
                            <div class="awp_box rounded bg-sg-title shadow">
                                <div class="awp_box_title bg-sg-title">
                                   <span class="fg-white shadow-metro-black">Packing Slip + Tracking: <asp:Label ID="TrackingNumberLabel" runat="server" Text=""></asp:Label></span>
                                </div>
                                <div class="awp_box_content bg-sg-box" style="min-height:400px">
                                     <div id="SPD" style="text-align:center; height:800px; width:100%">  
                                         <div id="CurrentAssetHidden" style="display:none"><asp:Literal ID="CurrentAssetDocumentHidden" runat="server"></asp:Literal> </div>
                               
                                         <iframe style="height:90%; width:90%; position:relative; z-index:2000;" id="CurrentAssetDocumentSrc" runat="server" src="/Account/Pdfs/blank.pdf"  ></iframe>
                                     </div>
                                </div>
                            </div>

                         </ContentTemplate>
                     </asp:UpdatePanel>
                 </div>
             </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:TextBox ID="ASuperBtnArg" ClientIDMode="Static" style="display:none"  runat="server" />  
    <asp:Button Text="" ID="ASuperBtn" ClientIDMode="Static" style="display:none" runat="server" OnClick="ASuperBtn_Click" />