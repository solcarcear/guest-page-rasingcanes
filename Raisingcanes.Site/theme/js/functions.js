$(function () {

    $(window).scroll(function () {
        if ($(this).scrollTop() !== 0) {
            $('#toTop').fadeIn();
        } else {
            $('#toTop').fadeOut();
        }
    });

    $('#toTop').click(function () {
        $('body,html').animate({ scrollTop: 0 }, 500);
    });

    $("#Div_PreferredRestaurant").hide();

    $('#divLoader').hide();    

    $("#divAccountCategoryOthers").hide();

    $("#divHowSupportedEvent").hide();

    $('#chooseFileAdditionalInf').bind('change', function () {
        var filename = $("#chooseFileAdditionalInf").val();
        if (/^\s*$/.test(filename)) {
            $(".file-upload").removeClass('active');
            $("#noFileAdditionalInf").text("No file chosen...");
        }
        else {
            $(".file-upload").addClass('active');
            $("#noFileAdditionalInf").text(filename.replace("C:\\fakepath\\", ""));
        }
    });

    $('#chooseFileW9').bind('change', function () {
        var filename = $("#chooseFileW9").val();
        if (/^\s*$/.test(filename)) {
            $(".file-upload").removeClass('active');
            $("#noFileW9").text("No file chosen...");
        }
        else {
            $(".file-upload").addClass('active');
            $("#noFileW9").text(filename.replace("C:\\fakepath\\", ""));
        }
    });

    $('.rowData').find('input').not(':radio').val('');
    //$('.rowData :input').val('');

});

function ValidateFRM() {    

    if ($('input#website').val().length !== 0) {
        return false;
    }

    var inputs = $('#divformDetail :input');
    if (!inputs.length || !!inputs.valid()) {        
        $("#divformDetail").hide();
        $("#divLoader").show();
        return true;
    }
}


$(document).ready(function () {
    
    var useragent = navigator.userAgent || navigator.vendor || window.opera;
    var ismobile = /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|zh-cn|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(useragent) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(useragent.substr(0, 4));
    if (ismobile) {
        if (window.matchMedia("(orientation: portrait)").matches) {
            //alert("For a better visualization use your device in landscape mode");
        }
    }

    $('.modal').modal({
        dismissible: false, // Modal can be dismissed by clicking outside of the modal        
        onCloseEnd: function () { // Callback for Modal close
            
        }
    }
    );    

    $.extend($.validator.messages, {
        required: "Required field.",
        phone: "Invalid phone format (xxx-xxx-xxxx).",
        remote: "Please fix this field.",
        email: "Invalid email format.",
        url: "Please enter a valid URL.",
        date: "Please enter a valid date.",
        dateISO: "Please enter a valid date (ISO).",
        number: "Please enter a valid number.",
        digits: "Please enter only digits.",
        creditcard: "Please enter a valid credit card number.",
        equalTo: "Please enter the same value again.",
        maxlength: $.validator.format("Please enter no more than {0} characters."),
        minlength: $.validator.format("Please enter at least {0} characters."),
        rangelength: $.validator.format("Please enter a value between {0} and {1} characters long."),
        range: $.validator.format("Please enter a value between {0} and {1}."),
        max: $.validator.format("Invalid value (max: 99.999)."),
        min: $.validator.format("Invalid value (min. 1)."),
        emailssame: "The emails entered must be the same."
    });

    $('#txtContactPhone').mask('(000) 000-0000');
    $('#txtAccountPhone').mask('(000) 000-0000');
      
    $.validator.addMethod('emailssame', function (value, element) {
        return value === $("#txtEmail").val();
    });
               
    $('#wrapped').validate({
        rules: {
            'txtEmail2': {
                required: true,
                email: true,
                emailssame: true
            }
        }
    });
    
    $('#cbAccountCategory').on('change', function () {

        if (this.value === 'd147b4b6-6eb9-4c79-b66e-dcf056b13670') {
            $("#divAccountCategoryOthers").show();
            $('#txtAccountCategoryOthers').addClass("required");
        }
        else {
            $("#divAccountCategoryOthers").hide();
            $("#txtAccountCategoryOthers").removeClass("required");
            $("#txtAccountCategoryOthers").val('');
        }
    });

    $('#txtStreet').on('change', function () {

        if ($("#cbHostEvent").val() === 'edbfdd08-96f8-4a9a-b636-808ae93677d0')
        {
            $('#txtEventStreet').val($('#txtStreet').val());
        }        
    });

    $('#txtCity').on('change', function () {

        if ($("#cbHostEvent").val() === 'edbfdd08-96f8-4a9a-b636-808ae93677d0') {
            $('#txtEventCity').val($('#txtCity').val());
        }
    });

    $('#cbState').on('change', function () {

        if ($("#cbHostEvent").val() === 'edbfdd08-96f8-4a9a-b636-808ae93677d0') {
            $('#txtEventState').val($('#cbState option:selected').text());
        }
    });

    $('#txtZip').on('change', function () {

        if ($("#cbHostEvent").val() === 'edbfdd08-96f8-4a9a-b636-808ae93677d0') {
            $('#txtEventZip').val($('#txtZip').val());
        }
    });

    //Combobox HostEvent
    $('#cbHostEvent').on('change', function () {
        //At your organization’s address
        if ($("#cbHostEvent").val() === 'edbfdd08-96f8-4a9a-b636-808ae93677d0') {
            $('#txtEventStreet').val($('#txtStreet').val());
            $('#txtEventCity').val($('#txtCity').val());
            $('#txtEventState').val($('#cbState option:selected').text());
            $('#txtEventZip').val($('#txtZip').val());
            $("#Div_PreferredRestaurant").hide();
            $("#Div_HostEvent").show();

            $('#txtEventStreet').addClass("required");
            $('#txtEventCity').addClass("required");
            $('#txtEventState').addClass("required");
            $('#txtEventZip').addClass("required");
        }
        //At a Raising Cane’s Restaurant closest to your organization's address
        else if ($("#cbHostEvent").val() === '72e26321-f6b2-44a5-b603-aa334803a5ee')
        {
            $('#txtEventStreet').val('');
            $('#txtEventCity').val('');
            $('#txtEventState').val('');
            $('#txtEventZip').val('');
            $("#Div_HostEvent").hide();
            $("#Div_PreferredRestaurant").hide();

            $('#txtEventStreet').removeClass("required");
            $('#txtEventCity').removeClass("required");
            $('#txtEventState').removeClass("required");
            $('#txtEventZip').removeClass("required");
        }
        //At a Raising Cane’s Restaurant of my preference
        else if ($("#cbHostEvent").val() === '8c4aaf82-1a32-42f2-bade-30247d276594') {
            $('#txtEventStreet').val('');
            $('#txtEventCity').val('');
            $('#txtEventState').val('');
            $('#txtEventZip').val('');
            $("#Div_PreferredRestaurant").show();

            $("#Div_HostEvent").show();

            $('#txtEventStreet').addClass("required");
            $('#txtEventCity').addClass("required");
            $('#txtEventState').addClass("required");
            $('#txtEventZip').addClass("required");
        }
        //Not Sure	
        else if ($("#cbHostEvent").val() === 'a971744d-2223-4b1a-9a75-02888d0152e9') {
            $('#txtEventStreet').val('');
            $('#txtEventCity').val('');
            $('#txtEventState').val('');
            $('#txtEventZip').val('');
            $("#Div_PreferredRestaurant").hide();

            $("#Div_HostEvent").hide();

            $('#txtEventStreet').removeClass("required");
            $('#txtEventCity').removeClass("required");
            $('#txtEventState').removeClass("required");
            $('#txtEventZip').removeClass("required");
        }

    });

    $('#cbPreferredRestaurant').on('change', function () {

        if (this.value !== '')
        {
            PageMethods.GetRestaurants(this.value, function (result) {
                var obj = JSON.parse(result);
                var arr = objectToArray(obj);
               
                for (var i = 0; i < arr.length; i++)
                {
                    $('#cbPreferredRestaurant').append($('<option>').text(arr[i].Street + ', ' + arr[i].Zip).val(arr[i].RestaurantCode));
                    $('#txtEventStreet').val(arr[i].Street);
                    $('#txtEventCity').val(arr[i].CityId);
                    $('#txtEventState').val(arr[i].StateId);
                    $('#txtEventZip').val(arr[i].Zip);
                }
            });
        }        
    });
   

    //$("input:radio[name='EventOtherAddressAtRestaurant']:radio").change(function () {
    //    if ($("input[name='EventOtherAddressAtRestaurant']:checked").val() === 'other') {
    //        $('#txtEventStreet').addClass("required");
    //        $('#txtEventCity').addClass("required");
    //        $('#cbEventState').addClass("required");
    //        $('#txtEventZip').addClass("required");
    //        $(".clsAddress").show();
    //    }
    //    else {
    //        $("#txtEventStreet").removeClass("required");
    //        $("#txtEventCity").removeClass("required");
    //        $("#cbEventState").removeClass("required");
    //        $("#txtEventZip").removeClass("required");

    //        $("#txtEventStreet").val('');
    //        $("#txtEventCity").val('');
    //        $("#txtEventZip").val('');
    //        $("#cbEventState").val('');
    //        $(".clsAddress").hide();
    //    }
    //});    

    //$("input:radio[name='EventOtherAddress']:radio").change(function () {
    //    if ($("input[name='EventOtherAddress']:checked").val() === 'Yes') {
    //        $(".clsOtherAddress").show();
    //    }
    //    else
    //    {
    //        $(".clsOtherAddress").hide();
    //        $("#txtEventStreet").removeClass("required");
    //        $("#txtEventCity").removeClass("required");
    //        $("#cbEventState").removeClass("required");
    //        $("#txtEventZip").removeClass("required");

    //        $("#txtEventStreet").val('');
    //        $("#txtEventCity").val('');
    //        $("#txtEventZip").val('');
    //        $("#cbEventState").val('');

    //        $(".clsAddress").hide();
    //    }
    //});    


    function GetCityValue(stateId)
    {
        PageMethods.GetCitiesByState(stateId, function (result)
        {
            var obj = JSON.parse(result);
            var arr = objectToArray(obj);

            $('#cbCity option').remove();
            $('#cbCity').append($('<option>').text("").val(""));

            for (var i = 0; i < arr.length; i++) {
                $('#cbCity').append($('<option>').text(arr[i].Text).val(arr[i].Id));
            }            
        });        
    }

    function GetRestaurants(CityId) {
        PageMethods.GetRestaurants(CityId, function (result) {
            var obj = JSON.parse(result);
            var arr = objectToArray(obj);

            $('#cbPreferredRestaurant option').remove();
            $('#cbPreferredRestaurant').append($('<option>').text("").val(""));

            for (var i = 0; i < arr.length; i++) {
                $('#cbPreferredRestaurant').append($('<option>').text(arr[i].Street + ', ' + arr[i].Zip).val(arr[i].RestaurantCode));
            }
        });
    }

    function objectToArray(obj) {
        var array = [];
        for (prop in obj) {
            if (obj.hasOwnProperty(prop)) {
                array.push(obj[prop]);
            }
        }
        return array;
    }
     

    $("input:radio[name='rdSupportEvent']:radio").change(function () {
        if ($("input[name='rdSupportEvent']:checked").val() === 'Yes')
        {
            $("#divHowSupportedEvent").show();
            $("#txtHowSupportedEvent").addClass("required");
        }
        else {
            $("#divHowSupportedEvent").hide();
            $("#txtHowSupportedEvent").removeClass("required");
            $("#txtHowSupportedEvent").val('');
        }
    });    


    $("#txtAboutTheAccount").on("keypress", function (e) {
        if ($(this).val().length === 250) {
            e.preventDefault();
        }
    });

    $("#txtHowSupportedEvent").on("keypress", function (e) {
        if ($(this).val().length === 500) {
            e.preventDefault();
        }
    });

    $("#txtPurposeEvent").on("keypress", function (e) {
        if ($(this).val().length === 500) {
            e.preventDefault();
        }
    });

    $("#txtIdeasRCEvent").on("keypress", function (e) {
        if ($(this).val().length === 500) {
            e.preventDefault();
        }
    });

    $("#txtAdditionalInformationEvent").on("keypress", function (e) {
        if ($(this).val().length === 500) {
            e.preventDefault();
        }
    });    

});



