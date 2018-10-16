
var geocoder;
var map;
var markers = [];
$(document).ready(function () {
       
   

});
 function ListReceived(msg)
 {
     var x = 0;
     function go() {
         codeAddress(msg.d[x]);
         if (x++ < msg.d.length-1) {
             setTimeout(go, 120);
         }
         else {
             map.setCenter(new google.maps.LatLng(37.6872,- 97.3301));
             map.setZoom(4);
         }
     }
     go();     
}

function GeneralFailure(msg)
{
    alert("Ajax Failure 0x010023c");
}
function ChangeMapView() {
    $("#hiddenModeSwitch").click();
}

function myMap() {
    var myCenter = new google.maps.LatLng(37.6872, -97.3301);
    var mapCanvas = document.getElementById("map");
    var mapOptions = { center: myCenter, zoom: 5 };
    map = new google.maps.Map(mapCanvas, mapOptions);
    //SetMarkers(map);
    var num = "test";
    //codeAddress("hawthorne, ca"); url: '/Account/AssetController.aspx/GetCustomerAddresses',
    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/GetOutAssetAddresses',
        data: "{'num':'" + num + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: ListReceived,
        error: GeneralFailure

    });

}

function MarkerClick() {
    var myCenter = new google.maps.LatLng(51.508742, -0.120850);
    var marker = new google.maps.Marker({ position: myCenter });
    map.setZoom(9);
    map.setCenter(marker.getPosition());
}

function SetMarkers(map)
{
    var myCenter = new google.maps.LatLng(51.508742, -0.120850);
    var marker = new google.maps.Marker({ position: myCenter });
    marker.setMap(map);
    new google.maps.event.addListener(marker, 'click', MarkerClick);

}
function SetMarkerLatLong(map, lat, lng) {
    var myCenter = new google.maps.LatLng(lat, lng);
    var marker = new google.maps.Marker({ position: myCenter });
    marker.setMap(map);
}

function codeAddress(addr) {    
    geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'address': addr }, GeoCodeCallBack );
}
function GeoCodeCallBack(results, status) {
    if (status == google.maps.GeocoderStatus.OK) {
        map.setCenter(results[0].geometry.location);
        var marker = new google.maps.Marker({
            map: map,
            position: results[0].geometry.location
        });
        markers.push(marker);
        //TODO here attach your events to show info when marker is clicked
    }
    else {
        var s = status;
       // alert('Geocode was not successful for the following reason: ' + status);
    }
}
function setMapOnAll(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}

// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
    setMapOnAll(null);
}

// Shows any markers currently in the array.
function showMarkers() {
    setMapOnAll(map);
}

// Deletes all markers in the array by removing references to them.
function deleteMarkers() {
    clearMarkers();
    markers = [];
}
