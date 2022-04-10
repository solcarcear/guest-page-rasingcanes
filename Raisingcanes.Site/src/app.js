const defaultPosition = [39.814349, -100.537016];
const mapOsm = getMap();
const marker = new L.marker(defaultPosition, {
    draggable: true,
    autoPan: true
});
const searchArea = L.circle(defaultPosition, {
    color: 'gray',
    fillColor: '#808080',
    fillOpacity: 0.5,
    radius: 8046.72
})
let raisingAvailableRestaurants = [];
let raisingRestaurantMarkers = [];

$(function () {
    mapOsm.options.activo = false;
    $("#box-radius").hide();
    $("#box-osm-map").hide();
    $("#cbHostEvent").on("change", (e) => {
        let map = null;
        if ($("#cbHostEvent").val() === '8c4aaf82-1a32-42f2-bade-30247d276594') {
            drawPosition(mapOsm);
            drawSearchRadius(mapOsm);
            $("#box-radius").show();
            $("#box-osm-map").show();
            mapOsm.options.activo = true;
            setCustomerPosition();
        } else {
            cleanMap();
            $("#box-radius").hide();
            $("#box-osm-map").hide();
            mapOsm.options.activo = false;
        }
    });

    $("#cbSearchRadius").on("change", (e) => {
        const value = $(e.target).val();
        searchArea.setRadius(value);
        loadAvailableRestaurants();
    });
    $("#cbState").on("change", (e) => { setCustomerPosition() });
    $("#txtCity").on("change", (e) => { setCustomerPosition() });
    $("#txtStreet").on("change", (e) => { setCustomerPosition() });
    $('#Div_PreferredRestaurant').hide();
});

function getMap() {
    const tilesProvider = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
    let myMap = L.map('osm-map');
    myMap.setView(defaultPosition, 10);
    L.tileLayer(tilesProvider, {
        maxZoom: 18
    }).addTo(myMap);

    return myMap;
}

function drawPosition(map) {
    marker.addTo(map);
    marker.bindPopup("<b>You!</b>");
    marker.on("dragend", (evt) => {
        let marker = evt.target;
        let position = marker.getLatLng();
        const { lat, lng } = position;
        searchArea.setLatLng([lat, lng]);
        loadAvailableRestaurants();
    });
}

function drawSearchRadius(map) {
    searchArea.addTo(map);
}

function getRadius() {

    const selectRadius = $("#cbSearchRadius").val();
    return selectRadius;
}

function cleanMap() {
    searchArea.remove();
    marker.remove();
}

function setCustomerPosition() {
    if (mapOsm.options.activo) {
        const state = $("#cbState option:selected").text();
        const city = $("#txtCity").val();
        const street = $("#txtStreet").val();
        $.blockUI({ message: '<h1>Looking your address on the map</h1>' });

        PageMethods.GetPositionByCustomerAddress(state, city, street, (result) => {
            $.unblockUI();

            const obj = JSON.parse(result);
            if (obj.length) {
                const { lat, lon } = obj[0];
                mapOsm.setView([lat, lon], 10);
                marker.setLatLng([lat, lon]);
                searchArea.setLatLng([lat, lon]);
                loadAvailableRestaurants();
            }
        });
    }
}

function drawAvailableRestaurants(restaurants) {


    for (ram of raisingRestaurantMarkers) {
        mapOsm.removeLayer(ram);
    }
    raisingRestaurantMarkers = [];

    for (item of restaurants) {
        let iconMarker = L.icon({
            iconUrl: 'https://raisingcanes.creatio.com/0/conf/content/img/CasePage-RCMarkerIcon.png?hash=ba571cb2640466bee36058717a517538',
            iconSize: [30, 40],
            iconAnchor: [20, 25]
        });
        const pos = [parseFloat(item.Lat), parseFloat(item.Lon)];
        let markerRaising = L.marker(pos, {
            icon: iconMarker,
            customStreet: item.Street,
            customCity: item.CityId,
            customState: item.StateId,
            customRestaurantId: item.RestaurantCode,
            customZip: item.Zip,
            customDisplayName: item.DisplayName

        }).addTo(mapOsm);

        raisingRestaurantMarkers.push(markerRaising);
        markerRaising.bindTooltip(markerRaising.options.customDisplayName);
        markerRaising.on("click", (e) => {
            const { customStreet, customCity, customState, customRestaurantId, customZip } = e.target.options;


            $('#cbPreferredRestaurant').val(customRestaurantId);
            $('#txtEventStreet').val(customStreet);
            $('#txtEventCity').val(customCity);
            $('#txtEventState').val(customState);
            $('#txtEventZip').val(customZip);

        });

    };
}

function loadAvailableRestaurants() {
    const { lat, lng } = marker.getLatLng();
    const searchRadiusVal = $("#cbSearchRadius option:selected").val();
    const radius = metersToMiles(searchRadiusVal);
    raisingAvailableRestaurants = [];
    for (const r of raisingRestaurants) {
        const distance = getDistanceBetweenPoints(lat, lng, r.Lat, r.Lon);
        if (distance <= radius) {
            raisingAvailableRestaurants.push(r);
        }
    }

    drawAvailableRestaurants(raisingAvailableRestaurants);


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
function metersToMiles(meters) {

    return meters === 0 ? 0 : meters / 1609.34;
}

function blockMap() {
    $.blockUI({ message: '<h1>Looking for nearby restaurants</h1>' });
}

function unblockMap() {

    $.unblockUI();
}