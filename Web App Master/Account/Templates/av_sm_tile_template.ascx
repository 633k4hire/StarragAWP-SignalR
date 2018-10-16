<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="av_sm_tile_template.ascx.cs" Inherits="Web_App_Master.Account.Templates.av_sm_tile_template" %>
<div id="TESTTEST"></div>
<div id="TESTSERVER" runat="server">

<div id="DataItemTile" class="tile tile-small-y bg-metro-light" data-role="tile"  onclick="BarcodeScanned('<%# Eval("AssetNumber")%>', '<%# Eval("IsHistoryItem")%>','<%# Eval("DateShippedTicks")%>');">
    <div id="IsOutContainer" runat="server" class="tile-content zooming bg-metro-dark ">
                    <div class="image-container zooming bg-metro-light" style="margin-top:25px">
                        <div class="frame">
                            <img src="<%# Eval("FirstImage")%>">
                        </div>
                        
                    </div>
          <span class="av-smtile-linkbadge shadow-metro-black"><i title="Add To Task List"  style="font-size:1.5em;" class="glyphicon glyphicon-plus av-add-cursor c-lime" onclick="<%# prepareAjax(Eval("AssetNumber").ToString(),Eval("IsOut").ToString())%>"></i></span>
                    <span  id="InOutIndicator" runat="server"   class="tile-badge fg-white bg-metro-dark shadow-metro-black"><strong><%# Eval("AssetNumber")%></strong></span>
                    <div class="tile-label fg-white shadow-metro-black bg-metro-dark" style="width:100%; text-align:center"><span style="z-index:1004"><strong><%# Eval("AssetName")%></strong></span></div>
                </div>
            </div>     
    </div>