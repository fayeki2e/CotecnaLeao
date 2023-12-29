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
                if (v.id === defaultId)
                    dropdown.append('<option value="' + v[idParam] + '" selected>' + v[nameParam] + '</option>');
                else
                    dropdown.append('<option value="' + v[idParam] + ' ">' + v[nameParam] + '</option>');
            });
        }
    }
};


$(document).ready(function () {
    form_state = "";

    if (document.getElementById('TimeSheetEntries_0__OutTime') != null) {
        document.getElementById('TimeSheetEntries_0__OutTime').onfocusout = function () {
            inTime = document.getElementById('TimeSheetEntries_0__InTime').valueAsDate;
            outTime = document.getElementById('TimeSheetEntries_0__OutTime').valueAsDate;
            if (outTime < inTime) {
                outTime = new Date(outTime + (60 * 60 * 24 * 1000));
            }
            document.getElementById('TimeSheetEntries_0__HoursWorked').valueAsDate =
                new Date(outTime - inTime);
        };
    }


    if (document.getElementById('TimeSheetEntries_1__OutTime') != null) {
        document.getElementById('TimeSheetEntries_1__OutTime').onfocusout = function () {
            inTime = document.getElementById('TimeSheetEntries_1__InTime').valueAsDate;
            outTime = document.getElementById('TimeSheetEntries_1__OutTime').valueAsDate;
            if (outTime < inTime) {
                outTime = new Date(outTime + (60 * 60 * 24 * 1000));
            }
            document.getElementById('TimeSheetEntries_1__HoursWorked').valueAsDate =
                new Date(outTime - inTime);
        };
    }
    
    if (document.getElementById('TimeSheetEntries_2__OutTime') != null) {
        document.getElementById('TimeSheetEntries_2__OutTime').onfocusout = function () {
            inTime = document.getElementById('TimeSheetEntries_2__InTime').valueAsDate;
            outTime = document.getElementById('TimeSheetEntries_2__OutTime').valueAsDate;
            if (outTime < inTime) {
                outTime = new Date(outTime + (60 * 60 * 24 * 1000));
            }
            document.getElementById('TimeSheetEntries_2__HoursWorked').valueAsDate =
                new Date(outTime - inTime);
        };
    }

    if (document.getElementById('TimeSheetEntries_3__OutTime') != null) {
        document.getElementById('TimeSheetEntries_3__OutTime').onfocusout = function () {
            inTime = document.getElementById('TimeSheetEntries_3__InTime').valueAsDate;
            outTime = document.getElementById('TimeSheetEntries_3__OutTime').valueAsDate;
            if (outTime < inTime) {
                outTime = new Date(outTime + (60 * 60 * 24 * 1000));
            }
            document.getElementById('TimeSheetEntries_3__HoursWorked').valueAsDate =
                new Date(outTime - inTime);
        };
    }

    if (document.getElementById('TimeSheetEntries_4__OutTime') != null) {
        document.getElementById('TimeSheetEntries_4__OutTime').onfocusout = function () {
            inTime = document.getElementById('TimeSheetEntries_4__InTime').valueAsDate;
            outTime = document.getElementById('TimeSheetEntries_4__OutTime').valueAsDate;
            if (outTime < inTime) {
                outTime = new Date(outTime + (60 * 60 * 24 * 1000));
            }
            document.getElementById('TimeSheetEntries_4__HoursWorked').valueAsDate =
                new Date(outTime - inTime);
        };
    }

    if (document.getElementById('TimeSheetEntries_5__OutTime') != null) {
        document.getElementById('TimeSheetEntries_5__OutTime').onfocusout = function () {
            inTime = document.getElementById('TimeSheetEntries_5__InTime').valueAsDate;
            outTime = document.getElementById('TimeSheetEntries_5__OutTime').valueAsDate;
            if (outTime < inTime) {
                outTime = new Date(outTime + (60 * 60 * 24 * 1000));
            }
            document.getElementById('TimeSheetEntries_5__HoursWorked').valueAsDate =
                new Date(outTime - inTime);
        };
    }

    if (document.getElementById('TimeSheetEntries_6__OutTime') != null) {
        document.getElementById('TimeSheetEntries_6__OutTime').onfocusout = function () {
            inTime = document.getElementById('TimeSheetEntries_6__InTime').valueAsDate;
            outTime = document.getElementById('TimeSheetEntries_6__OutTime').valueAsDate;
            if (outTime < inTime) {
                outTime = new Date(outTime + (60 * 60 * 24 * 1000));
            }
            document.getElementById('TimeSheetEntries_6__HoursWorked').valueAsDate =
                new Date(outTime - inTime);
        };
    }

    setInterval(function () {
        var form_data = $("form").serialize();
        if (form_state !== form_data) {
            form_state = form_data;
            console.log(form_data);
            var url = $("timesheet_form").val();
            $.ajax({
                type: "POST",
                url: url,
                data: $("form").serialize(),
                success: function (data) {
                    console.log("Successful");
                },
                error: function (error) {
                    console.log(error);
                }
            });

        } else {
            console.log("Form data unchanged");
        }
    }, 10000);

    //Add button click event
    $('#add').click(function () {
        //validation and add order items
        var isAllValid = true;
        if (isAllValid) {
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.day_of_week', $newRow).val($('#dayofWeek').val());
            $('.date_of_week', $newRow).val($('#dateofWeek').val());

            //Replace add button with remove button
            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');

            //remove id attribute from new clone row
            $('#dayofWeek,#dateofWeek,#jobNumber,#clientName,#id_in_time,#id_out_time,#workedHours,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();
            //append clone row
            $('#orderdetailsItems').append($newRow);

            //clear select data
            $('#dayofWeek,#dateofWeek').val('0');
            $('#jobNumber,#clientName').val('');
            $('#orderItemError').empty();
        }

    });

    //remove button click event
    $('#orderdetailsItems').on('click', '.remove', function () {
        $(this).parents('tr').remove();
    });

    LoadEmployeedetails();
});



$('#submit').click(function () {
    var isAllValid = true;
    var list = [];
    var orderItem = {
        EmployeeID: $('select.employee').val(),
        WorkedHours: $('.workedhours').val(),
        Holiday: $('.workedholiday').val(),
        StartdDate: parseInt($('.startdate').val()),
        EndDate: parseInt($('.enddate').val()),
        BasicSalary: $('.basicsalary').val(),
        OvertimeAmount: $('.overtimeamount').val()
    };
    list.push(orderItem);
    console.log(list);

});

// Load Employee Details
function LoadEmployeedetails() {
    console.log("function load employee");
    url_var = '/api/EmployeesJson/' + $('select#EmployeeId option:selected').val();
    $.ajax
        ({
            type: 'GET',
            url: url_var,
            dataType: 'json',
            data: {},
            success: function (data) {
                var res = data;
                console.log(res.Id);
                $("#Behalf_Emp_Designation").val(res.designation);
                $("#Behalf_Emp_Code").val(res.employeeCode);

            },
            error: function (error) {
                console.log(error);
            }
        });

}

//sr no increment
var add = (function () {
    var counter = 0;
    return function () { counter += 1; return counter; };
})();

function myFunction() {
    document.getElementById("demo").innerHTML = add();
}


//total Hours count

function pad(num) {
    return ("0" + num).slice(-2);
}

function diffTime(start, end) {
    var s = start.split(":"), sMin = +s[1] + s[0] * 60,
        e = end.split(":"), eMin = +e[1] + e[0] * 60,
        diff = eMin - sMin;
    if (diff < 0) { sMin -= 12 * 60; diff = eMin - sMin; }
    var h = Math.floor(diff / 60),
        m = diff % 60;
    return "" + pad(h) + ":" + pad(m);
}
