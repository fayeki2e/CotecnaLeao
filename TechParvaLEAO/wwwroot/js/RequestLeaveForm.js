 

medical_mandatory = false;
var helpers =
{
    buildDropdown: function (result, dropdown, emptyMessage, defaultId, idParam, nameParam) {
        // Remove current options
        dropdown.empty();
        // Add the empty option with the empty message
        if (emptyMessage !== null) {
            dropdown.append('<option value="">' + emptyMessage + '</option>');
        }
        // Check result isnt empty
        if (result !== '') {
            // Loop through each of the results and append the option to the dropdown
            $.each(result, function (k, v) {
                if (v.id === defaultId || result.length === 1)
                    dropdown.append('<option value="' + v[idParam] + '" selected>' + v[nameParam] + '</option>');
                else
                    dropdown.append('<option value="' + v[idParam] + ' ">' + v[nameParam] + '</option>');
            });
        }
    }
};

function LoadCompOffs() {
    urlPath = '/api/CompOffJson/' + $('#EmployeeId').val();
    $.ajax({
        type: "GET",
        url: urlPath,
        success: function (data) {
            helpers.buildDropdown(data, $('#CompOffAgainstDateId'), null, 1, "id", "name");
        }
    });
    if ($('#LeaveTypeId option:selected').val().trim() === "2") {
        $('#compoff').show();
    } else {
        $('#compoff').hide();
    }
}

// ajax call
function LoadLeaveTypes(element) {
    if ($('select#EmployeeId option:selected').val() === "") return;
    //ajax function for fetch data
    $.ajax({
        type: "GET",
        url: '/api/LeaveTypesJson/' + $('select#EmployeeId option:selected').val(),
        success: function (data) {
            //render catagory
            helpers.buildDropdown(data, $('#LeaveTypeId'), "Select Leave Type", $('#LeaveTypeId').val(), "id", "name");
        }
    });
}

function LoadCategories(categoryDD) {
    var url = "/api/LeaveCategoriesJson/?leaveType=" + $('select#LeaveTypeId option:selected').val(); 
    $.ajax({
        type: "GET",
        url: url,
        data: { 'categoryId': $(categoryDD).val() },
        success: function (data) {
            //render products to appropriate dropdown
            renderDependentDropdown($(categoryDD).parents('.mycontainer').find('select.leaveCategory'), data);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function LoadSubCategory(categorySB) {
    var url = "/api/LeaveSubCategoriesJson/?leaveType=" + $('select#LeaveTypeId option:selected').val() + "&leaveCategory=" + $('select#LeaveCategoryId option:selected').val(); 

    $.ajax({
        type: "GET",
        url: url,
        dataType: 'json',
        success: function (data) {
            //render products to appropriate dropdown
            renderDependentDropdown($(categorySB).parents('.mycontainer').find('select.leaveSubCategory'), data);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function renderDependentDropdown(element, data) {
    //render product
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('Select'));
    $.each(data, function (i, val) {
        var option = $('<option/>').val(val.id).text(val.text);
        if (data.length === 1) option.attr('selected', 'selected');
        $ele.append(option);
    });
    element.trigger("change");
}

function MyselfOnbehalf() {
    selected_val = $('#Type option:selected').val();
    if (selected_val.trim() === "1") {
        var employeeId = $('#LoggedInEmployeeId').val();
        var employee = employeeId;
        $('#EmployeeId').val(employee);
        $('#EmployeeId').css('pointer-events', 'none');
        LoadLeaveTypes($('#LeaveTypeId'));
    } else {
        //document.getElementById("EmployeeId").value = null;
        $('#EmployeeId').css('pointer-events', 'auto');
    }
    LoadEmployeedetails();
}

function LoadEmployeedetails() {
    if (!$('select#EmployeeId option:selected').val()) return;
    url_var = '/api/EmployeesJson/' + $('select#EmployeeId option:selected').val();
    $.ajax
        ({
            type: 'GET',
            url: url_var,
            dataType: 'json',
            data: {},
            success: function (data) {
                var res = data;
                $("#Behalf_Emp_Designation").text(res.designation);
                $("#Behalf_Emp_Location").text(res.location);

            },
            error: function (error) {
                console.log(error);
            }
        });
}

function leaveDetail() {
    var startDate = $("#StartDate").val();
    $("#EndDate").show();
    $("#EndDate").val(startDate);
}

function switchhalfday(selectedhalfday) {

    if (selectedhalfday == 'start') {
        $("#HalfDayEnd").prop("checked", false);
    }

    if (selectedhalfday == 'end') {
        $("#HalfDayStart").prop("checked", false);
    }

}

function GetLeaveNumberOfDays() {
    var startDate = $("#StartDate").val();
    var endDate = $("#EndDate").val();
    var employeeId = $('select#EmployeeId option:selected').val();
    var leaveTypeId = $('select#LeaveTypeId option:selected').val();
    var halfDayStart = $("#HalfDayStart:checked").length > 0 ? true : false;
    var halfDayEnd = $("#HalfDayEnd:checked").length > 0 ? true : false;



    url_var = '/api/LeaveCalculation/';
    $.ajax
        ({
            type: 'GET',
            url: url_var,
            dataType: 'json',
            data: {
                StartDate: startDate, EndDate: endDate,
                EmployeeId: employeeId,
                LeaveTypeId: leaveTypeId,
                HalfDayStart: halfDayStart,
                HalfDayEnd: halfDayEnd
            },
            success: function (data) {
                $("#TotalNumberOfDays").val(data.numberOfDays);
                $("#EndDate").val(data.endDate);
                CheckMedical();
            },
            error: function (error) {
                console.log(error);
            }
        });
}

function CheckMaternityLeave() {
    selected_val = $('#LeaveSubCategoryId option:selected').val();
    if (selected_val.trim() === "7") {
        $('#LeaveNature')[0].value = "Maternity Leave";
        $('#LeaveNature').css('pointer-events', 'none');
    }
    else {
        $('#LeaveNature').css('pointer-events', 'auto');
        $('#LeaveNature')[0].value = "";
    }
}

// select all check box to approve multiple leave
$(document).ready(function () {
    var form_state = "";
    $('#medical-doc').hide();
    $('#compoff').hide();
    $("#select-all").click(function () {
        $(".checkBoxClass").prop('checked', $(this).prop('checked'));
    });
    $(".checkBoxClass").change(function () {
        if (!$(this).prop("checked")) {
            $("#select-all").prop("checked", false);
        }
    });

    //only Co-ordinator can see the behalf option for leave request
    var can_apply_onbehalf = $('#Employee_CanApplyOnBehalf').val();
    if (can_apply_onbehalf !== 'True') {
        $("#Type").val(1);
        $("#Type option[value='2']").remove();
        MyselfOnbehalf();
    }
    else {
        $("#Type").val(2);
        //$("#EmployeeId").val(null);
        $("#Type option[value='1']").remove();
        MyselfOnbehalf();
    }
    if ($('#LeaveTypeId').val() === "") {
        LoadLeaveTypes($('#LeaveTypeId'));
    }
    GetLeaveNumberOfDays();

    $("#RequestLeaveForm").submit(function (event) {
        if ($("#RequestLeaveForm").validate().checkForm() === false) {
            event.preventDefault();
            //form was NOT ok - optionally add some error script in here
            return false; //for old browsers 
        } else {
            if (medical_mandatory && $('#supportings-reuim')[0].value==="") {
                event.preventDefault();
                alert('Please upload medical certificate');

            }
            //form was OK - you can add some pre-send script in here
        }

        //you don't have to submit manually if you didn't prevent the default event before
    });

    setInterval(function () {
        var form_data = $("form").serialize();
        if (form_state !== form_data) {
            form_state = form_data;
            console.log(form_data);
            var url = "/api/LeaveDraftJson/";
            $.ajax({
                type: "POST",
                url: url,
                data: $("form").serialize(),
                success: function (data) {
                    //render products to appropriate dropdown
                    console.log("Successful");
                    $("#DraftId").val(data);
                },
                error: function (error) {
                    console.log(error);
                }
            });

        } else {
            console.log("Form data unchanged");
        }
    }, 10000);
});

// medical document attachment
function CheckMedical() {
    var Select_leavetype = $('#LeaveSubCategoryId option:selected').val();
    medical_mandatory = false;
    $('#medical-doc').hide();

    if (Select_leavetype.trim() === "4") {
        var totalday_count = $('#TotalNumberOfDays').val();
        medical_mandatory = parseFloat(totalday_count) > 2;
        if (medical_mandatory)
            $('#medical-doc').show();
    }
}

function CheckRejectionReason() {
    selected_val = $('#RejectionReasonId option:selected').val();
    if (selected_val!==null && selected_val.trim() === "8") {
        $('#RejectionReasonOther').css('pointer-events', 'auto');
        $('#RejectionReasonOther').attr("disabled", false);
    } else {
        $('#RejectionReasonOther').attr("disabled", true);
        $('#RejectionReasonOther').css('pointer-events', 'none');
    }
}

function approveall() {

    var counter = 0,
        i = 0,
      
        input_obj = document.getElementsByTagName('input');
         var ids = [];

    // loop through all collected objects
    for (i = 0; i < input_obj.length; i++) {
        if (input_obj[i].type === 'checkbox' && input_obj[i].checked === true) {
            counter++;
            ids.push(input_obj[i].value);
        }
    }
 
    if (counter > 0) {

        setTimeout(function () {
            location.reload();
        }, 3000);

        $.ajax({
            url: '/Leave/LeaveRequests/ApproveConfirmedAll',
            type: "GET",
            data: { s_id: JSON.stringify(ids) },
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                alert("Successfully Leave Approved");
                location.reload();

            }
        });
    }

}


function check_team_leaves() {


    dataSource = [];
     

    var startDate = $("#StartDate").val();
    var endDate = $("#EndDate").val();

        var test = "test";
        $.ajax(
            {
                method: "POST",
                url: '/Leave/LeaveRequests/Check_Team_Leaves',
                data: {
            
                     fromdate: startDate, todate: endDate
                },
 

                success: function (data) {

                 
                    if (data.length > 0) {
                        alert("Another Team Member is on Leave between selected date");
                    }
                  
                },
                error: function (msg) {
                    alert("Error in getting details: " + msg);
                    console.log(msg);
                }
            });

   

}