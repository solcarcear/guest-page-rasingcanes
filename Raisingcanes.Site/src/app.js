
$(function () {
    $("#cbHostEvent").on("change", (e) => {

        if ($("#cbHostEvent").val() === '8c4aaf82-1a32-42f2-bade-30247d276594') {
            showMap();
        }

        //let target = $(e.currentTarget);
        //console.log(target.val());
    });


});

function showMap() {

    const tilesProvider = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
    const myPosition = [37.09024, -95.712891];
    let myMap = L.map('osm-map').setView(myPosition, 18);

    L.tileLayer(tilesProvider, {
        maxZoom: 18
    }).addTo(myMap);


    let marker = new L.marker(myPosition, {
        draggable: true,
        autoPan: true
    }).addTo(myMap);



    marker.bindPopup("<b>Cowork!</b><br>USA.");

    //let iconMarker = L.icon({
    //    iconUrl: 'https://raisingcanes.creatio.com/0/conf/content/img/CasePage-RCMarkerIcon.png?hash=ba571cb2640466bee36058717a517538',
    //    iconSize: [50, 50],
    //    iconAnchor: [40, 25]
    //});

    //let marker2 = L.marker([-17.760, -63.1979327], { icon: iconMarker }).addTo(myMap);
    //marker2.bindPopup("<b>Sucursal muertes lenta</b><br>San Martin chuy.");

    let circle = L.circle(myPosition, {
        color: 'gray',
        fillColor: '#808080',
        fillOpacity: 0.5,
        radius: 500,
    }).addTo(myMap);
}


function degreesToRadians(degrees) {
    return degrees * Math.PI / 180;
}
function getDistanceBetweenPoints(lat1, lng1, lat2, lng2) {
    // El radio del planeta tierra en metros.
    let R = 6378137;
    let dLat = degreesToRadians(lat2 - lat1);
    let dLong = degreesToRadians(lng2 - lng1);
    let a = Math.sin(dLat / 2)
        *
        Math.sin(dLat / 2)
        +
        Math.cos(degreesToRadians(lat1))
        *
        Math.cos(degreesToRadians(lat1))
        *
        Math.sin(dLong / 2)
        *
        Math.sin(dLong / 2);

    let c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    let distance = R * c;
    const result = distance === 0 ? 0 : distance / 1609.34;

    return result;//resultado en millas
}

function metersToMiles(meters) {

    return meters === 0 ? 0 : meters / 1609.34;
}
function milesToMeter(meters) {
    return meters * 1609.34;
}