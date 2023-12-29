$(document).ready(function () {

    $("#select-all").click(function () {
        $(".checkBoxClass").prop('checked', $(this).prop('checked'));
    });

    $(".checkBoxClass").change(function () {
        if (!$(this).prop("checked")) {
            $("#select-all").prop("checked", false);
        }
    });


    //asterisk
    $('input[type=date],[type=text]').each(function () {
        var req = $(this).attr('data-val-required');
        if (undefined !== req) {
            var label = $('label[for="' + $(this).attr('id') + '"]');
            var text = label.text();
            if (text.length > 0) {
                label.append('<span style="color:red;"> *</span>');
            }
        }
    });

    // Expense or reuimbersement page
    $('#CurrencyId, #Comment, #BusinessActivityId, #CustomerMarketId, #EmployeeId').each(function () {
        var req = $(this).attr('data-val-required');
        if (undefined !== req) {
            var label = $('label[for="' + $(this).attr('id') + '"]');
            var text = label.text();
            if (text.length > 0) {
                label.append('<span style="color:red;"> *</span>');
            }
        }
    });

    // leave request form
   
});



function checkbox_test() {
    var counter = 0,
        i = 0,
        url = '',

        input_obj = document.getElementsByTagName('input');
    // loop through all collected objects
    for (i = 0; i < input_obj.length; i++) {
        if (input_obj[i].type === 'checkbox' && input_obj[i].checked === true) {
            counter++;
            url = url + '/' + input_obj[i].value;
        }
    }
    // display url string or message if there is no checked checkboxes
    if (counter > 0) {
        // remove first "&" from the generated url string
        url = url.substr(1);
        url_approve = url;
        $.ajax({
            type: "POST",
            url: url_approve,
            data: {},
            success: function (response) {
                console.log("posted..." + url_approve);
            }

        });
    }
    else {
        alert('You have not selected any timesheet');
    }
}