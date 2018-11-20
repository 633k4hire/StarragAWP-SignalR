var hub;
var output;
var OnChanged = false;
$(document).ready(function () {
    InitSignalRClient();
});

function InitSignalRClient() {
    try {
        hub = $.connection.clientHub; //set hub with the server side class         
        //output = $("#output");

        hub.client.usersConnected = function (numUsers) { //this instanciate the      usersConnected function receiving the numUsers parameter which is the number of users connected
            //$("#count").text(numUsers); //show the number of users connected inside a div 
            var cc = 0;
        };

        hub.client.incomingMsg = function (name, message) { //this instanciate the shapeMoved function          receiving x, y
            var bb = 0;
            //HideLocalLoader();
            //output.html(output.html() + name + ":" + message + "<br>");//.css({ left: x, top: y }); //this moves the shape in the clients to          50.the coords x, y
        };

        hub.client.imgReady = function (client) { //this instanciate the shapeMoved function          receiving x, y
            var bb = 0;
            //$("#img_" + client.Id).attr('src', 'data:image/jpeg;base64,'+client.Data);
            //Super("c_img", client.Data, client.Id);
            //HideLocalLoader();
            //output.html(output.html() + name + ":" + message + "<br>");//.css({ left: x, top: y }); //this moves the shape in the clients to          50.the coords x, y
        };

        hub.client.assetCacheChanged = function (client) { //this instanciate the shapeMoved function          receiving x, y
            var bb = 0;
            MasterSuper("asset-cache", "refresh", "");
        };

        hub.client.onSuper = function (cmd, arg1, arg2) {
            if (cmd === "new connection" || cmd === "end connection") {

            }
            MasterSuper(cmd, arg1, arg2);
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
    } catch(err){ }
} 

function MasterSuper(cmd, arg, arg2) {
    $("#AppCommand").val(cmd);
    $("#AppArgument").val(arg);
    $("#AppArgument2").val(arg2);
    $("#MasterSuperBtn").click();
}
function Ajax(methodurl, arg, success, error) {
    $.ajax({
        type: 'POST',
        url: methodurl,
        data: "{'arg':'" + arg + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: success,
        error: error

    });
}

function SetChange() {
    OnChanged = true;
}
function HubLoop(e, a) {
    if (OnChanged === true) {
        hub.server.sendMsg("", m.val());
        OnChanged = false;
    }
    
}
