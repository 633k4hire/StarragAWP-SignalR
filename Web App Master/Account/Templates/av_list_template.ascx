<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="av_list_template.ascx.cs" Inherits="Web_App_Master.Account.Templates.av_list_template" %>

      <div class="col-md-12"   >
          <span class="av-lv-linkbadge"><i title="Add To Task List"  style="font-size:1.5em;" class="glyphicon glyphicon-plus c-lime av-add-cursor" onclick="<%# prepareAjax(Eval("AssetNumber").ToString(),Eval("IsOut").ToString())%>"></i></span>
          <div class=" av-lv rounded bg-sg-box shadow" onclick="BarcodeScanned('<%# Eval("AssetNumber")%>', '<%# Eval("IsHistoryItem")%>','<%# Eval("DateShippedTicks")%>');">
              <div class="av-lv-row"  >
              <div  class=" av-lv-thumb" ><img id="av_list_img" src="<%# Eval("FirstImage")%>" width="80" height="70"></img></div>
             <div   class="av-lv-seperator"></div>    
             <div class="av-lv-content fg-white shadow-metro-black"  >
                <strong><%# Eval("AssetName")%> - <%# Eval("Description")%></strong>
             </div>    
              </div>
             <span  id="InOutIndicator" runat="server" class="edge-badge fg-white shadow"><strong><%# Eval("AssetNumber")%></strong></span>
          </div>
      </div>

