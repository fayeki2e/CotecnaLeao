var helpers =
{
    buildDropdown: function (result, dropdown, emptyMessage, defaultId, idParam, nameParam) {
        // Remove current options
        for (i = 0; i < dropdown.length; i++) {
            // Add the empty option with the empty message
            if (dropdown[i].selectedOptions.length === 1) {
                defaultId = dropdown[i].selectedOptions[0].value;
            }
            
            var c;
            for (c = dropdown[i].options.length - 1; c >= 0; c--) {
                dropdown[i].options.remove(c);
            }
            if (emptyMessage !== null) {
                dropdown[i].options.add(new Option(emptyMessage, ""));//'<option value="">' + emptyMessage + '</option>');
            }
            // Check result isnt empty
            if (result !== '') {
                // Loop through each of the results and append the option to the dropdown
                $.each(result, function (k, v) {
                    var selected = false;
                    if (v["id"] !== undefined && v["id"].toString() === defaultId) {
                        selected = true;
                    }
                    dropdown[i].options.add(new Option(v[nameParam], v[idParam], false, selected));
                });
            }
        }
    }
};

function LoadCurrency() {
    data = null;
    if ($('#EmployeeId').val() === "") return;
    urlPath = '/api/CurrencyJson/' + $('#EmployeeId').val();
    $.ajax({
        type: "GET",
        url: urlPath,
        success: function (data) {
            helpers.buildDropdown(data, $('#CurrencyId'), null, null, "id", "name");
            CheckSettleAdvance();
        }
    });
}

function CheckSettleAdvance() {
    selected_val = $('#CurrencyId option:selected').val();
    if (selected_val === "1") {
        $('#AdvancePaymentRequestId').val("");
        $('#AdvancePaymentRequestId').attr("disabled", true);
        $('#AdvancePaymentRequestId').css('pointer-events', 'none');
    } else {
        $('#AdvancePaymentRequestId').css('pointer-events', 'auto');
        $('#AdvancePaymentRequestId').attr("disabled", false);
    }
}

function CheckRejectionReason() {
    selected_val = $('#RejectionReasonId option:selected').val();
    if (selected_val === "4" || selected_val === "11") {
        $('#RejectionReasonOther').css('pointer-events', 'auto');
        $('#RejectionReasonOther').attr("disabled", false);
        
    } else {
        $('#RejectionReasonOther').attr("disabled", true);
        $('#RejectionReasonOther').css('pointer-events', 'none');
    }
}


function CheckCreditCard() {
    selected_val = $('#CreditCard:checkbox:checked').length>0;
    if (selected_val) {
        $('#AdvancePaymentRequestId').val("");
        $('#AdvancePaymentRequestId').attr("disabled", true);
        $('#AdvancePaymentRequestId').css('pointer-events', 'none');
    } else {
        $('#AdvancePaymentRequestId').css('pointer-events', 'auto');
        $('#AdvancePaymentRequestId').attr("disabled", false);
    }
}

function LoadPendingAdvances() {
    if ($('#EmployeeId').val() === null || $('#EmployeeId').val() === "") return;
    if ($('#CurrencyId').val() === null || $('#CurrencyId').val() === "") return;
    urlPath = '/api/PaymentRequestsJson/';
    selected_val = $('#AdvancePaymentRequestId option:selected').val();
    $.ajax({
        type: "GET",
        url: urlPath,
        data: { employeeId: $('#EmployeeId').val(), currencyId: $('#CurrencyId').val()},
        success: function (data) {
            helpers.buildDropdown(data, $('#AdvancePaymentRequestId'), 'Select advance if applicable', selected_val, "id", "name");
        }
    });
}


// fetch expense Head
var ExpenseHeads = [];
//fetch categories from database
function LoadExpenseHead() {
    url_EH = '/api/ExpenseHeadsJson/' + $('#EmployeeId').val();

    $.ajax({
        type: "GET",
        url: url_EH,
        success: function (data) {
            ExpenseHeads = data;
            helpers.buildDropdown(data, $(".expensehead"), 'select expense head', null, "id", "expenseHeadDesc");
        }
    });
}

//fetch products
function LoadBusinessActivity(expenseHead) {
    /*
    console.log("Running LoadBusinessActivity");
    url_BA = '/api/BusinessActivitiesJson/' + $('#Employee').val();

    $.ajax({
        type: "GET",
        url: url_BA,
        success: function (data) {
            helpers.buildDropdown(data, $(expenseHead).parents('.mycontainer').find('select.businessactivity'), 'Select Business Activity', null, "id", "name");
        },
        error: function (error) {
            console.log(error);
        }
    })
    */
}

//fetch customer activity
function LoadCustomerMarket(businessActivity) {
    url_CM = '/api/CustomerMarketsJson/' + $('#EmployeeId').val();

    $.ajax({
        type: "GET",
        url: url_CM,
        data: { 'businessActivity': $(businessActivity).val() },
        success: function (data) {
            helpers.buildDropdown(data, $(businessActivity).parents('.mycontainer').find('select.customermarket'), 'Select Customer Market', null, "id", "name");
        },
        error: function (error) {
            console.log(error);
        }
    });
}

//fetch employee list
function MyselfOnbehalf() {
    selected_val = $('#Type option:selected').val();
    if (selected_val === "1") {
        var employeeId = $('#LoggedInEmployeeId').val();
        var employee = employeeId;
        $('#EmployeeId').val(employee);
        $('#EmployeeId').css('pointer-events', 'none');
    } else {
        $('#EmployeeId').css('pointer-events', 'auto');
    }
    LoadEmployeedetails();
    LoadExpenseHead();
    LoadCurrency();
    LoadPendingAdvances();    
}

function OperationNumber(operationtype) {
       
    //selected_val = $('#OperationType option:selected').val();
    selected_val = operationtype.options[operationtype.selectedIndex].value;
    var index = operationtype.id.split("_")[1];

    if (selected_val === "1") {
 
        $('#ExpenseLineItems_' + operationtype.id.split("_")[1] + '__operationno')[0].style.visibility = "hidden"
        $("#validation_" + index + "__operationnumber").css("visibility", "hidden");
    }
    else {
 
        $('#ExpenseLineItems_' + operationtype.id.split("_")[1] + '__operationno')[0].style.visibility = "visible";
        $("#validation_" + index + "__operationnumber").css("visibility", "visible");
    }





 
    //if (txt.value == "") {

    //    $("#validation_" + index + "__operationnumber").css("visibility", "visible");
    //} else {
    //    $("#validation_" + index + "__operationnumber").css("visibility", "hidden");
    //}


   
}


function LoadEmployeedetails() {
    if (!$('select#EmployeeId option:selected').val()) {
        $("#Behalf_Emp_Designation").text("");
        $("#Behalf_Emp_Location").text("");
        return;
    }

    employeeId = $('select#EmployeeId option:selected').val() === "" ?
        0: $('select#EmployeeId option:selected').val();
    currencyId = $('select#CurrencyId option:selected').val() === "" ?
        1 : $('select#CurrencyId option:selected').val();
    url_var = '/api/EmployeesJson/' + employeeId;
    $.ajax
        ({
            type: 'GET',
            url: url_var,
            dataType: 'json',
            data: {
                id: employeeId,
                currencyId: currencyId
            },
            success: function (data) {
                var res = data;
                $("#Behalf_Emp_Designation").text(res.designation);
                $("#Behalf_Emp_Location").text(res.location);
                $("#Behalf_Emp_Location").val(res.basicsalary);
               // $("#Behalf_Emp_Advance").text(res.inrAdvanceReceived);
                $("#Behalf_Emp_Advance").text(res.inrAdvanceReceived.toFixed(2));

               // $("#Behalf_Emp_Reimbursement").text(res.inrReimbursementApproved);
                $("#Behalf_Emp_Reimbursement").text(res.inrReimbursementApproved.toFixed(2));

                $("#Behalf_Emp_Balance").text(res.inrPendingAmount.toFixed(2));
                $("#Behalf_Emp_Balance").text(res.inrPendingAmount.toFixed(2));
                $(".currencycode").text(res.currencyCode);
                
                if (res.inrPendingAmount > 0) {
                    $("#Behalf_Emp_Balance").addClass("red");
                    $("#Behalf_Emp_Balance").removeClass("green");
                } else {
                    $("#Behalf_Emp_Balance").addClass("green");
                    $("#Behalf_Emp_Balance").removeClass("red");
                }

                if ($('#CreditCardDiv').length===1) {
                    if (res.canHoldCreditCard) {
                        $('#CreditCardDiv')[0].style.visibility = "visible";
                    } else {
                        $('#CreditCardDiv')[0].style.visibility = "hidden";
                    }
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
}

// check box to select all records
$(document).ready(function () {
    var form_state = "";
    setInterval(function () {
        var form_data = $("form").serialize();
        if (form_state !== form_data) {
            form_state = form_data;
            var url = "/api/ExpenseDraftJson/";
            if ($("#AdvancePaymentRequestId").length === 0){
                url = "/api/AdvanceDraftJson/";
            }
            $.ajax({
                type: "POST",
                url: url,
                data: $("form").serialize(),
                success: function (data) {
                    $("#DraftId").val(data);
                },
                error: function (error) {
                    console.log(error);
                }
            });

        } 
    }, 10000);

    var CanApplyOnBehalf = $('#Employee_CanApplyOnBehalf').val();
    if (CanApplyOnBehalf !== 'True') {
        $("#Type").val(1);
        $("#Type option[value='2']").remove();
    }
    else {
        $("#Type").val(2);
        $("#Type option[value='1']").remove();
    }
    MyselfOnbehalf();

    if ($('#EmployeeId').val() === null || $('#EmployeeId').val() === "") return;
    LoadEmployeedetails();
    LoadCurrency();
    LoadPendingAdvances();
    //LoadExpenseHead();


});

//check to date is smaller than from date
function FromAndToDate() {
    var isValid = true;
    var toDate = $('#ToDate').val();
    var fromDate = $('#FromDate').val();
    // condition check
    if (toDate < fromDate) {
        isValid = false;
        $('#ToDate').siblings('span.error1').css('display', 'block');
    }
    else {
        $('#ToDate').siblings('span.error1').css('display', 'none');
        $('#ToDate').attr("aria-invalid", false);
    }
}

function ExpenseCreateDate() {
    var isValid = true;
    var toDate = $('#ToDate').val();
    var fromDate = $('#FromDate').val();
    createDate = $('.expenseDate').val();
    //condition check
    if (createDate >= fromDate && createDate <= toDate) {
        isValid = false;
        $('.expenseDate').siblings('span.error1').css('display', 'none');
        $('.expenseDate').attr("aria-invalid", false);
    }
    else {
        $('.expenseDate').siblings('span.error1').css('display', 'block');
        $('.expenseDate').attr("aria-invalid", true);
    }
}

function ExpenseCreateDateVal(element) {
    var isValid = true;
    var toDate = $('#ToDate').val();
    var fromDate = $('#FromDate').val();
    createDate = $('.expenseDate').val();
    //condition check
    if (createDate >= fromDate && createDate <= toDate) {
        isValid = false;
        $('.expenseDate').siblings('span.error1').css('display', 'none');
    }
    else {
        $('.expenseDate').siblings('span.error1').css('display', 'block');
    }
}

$('#ExpenseForm').submit(function (e) {
    //e.preventDefault();
    
    var date_elements = $('.expenseDate');
    date_elements.trigger('change');
    for (var i = 0; i < date_elements.length; i++) {
        var element = date_elements[i];
    }
});

$('#advanceform').submit(function(e) {
    var currentForm = this;
    e.preventDefault();
    var basicsalary = parseInt($("#BasicSalary").val());
    var advanceamount = parseInt($("#Amount").val());
    if (advanceamount >= 10000 || basicsalary > 0 && advanceamount > basicsalary) {
        bootbox.confirm("Do you want to continue with the Submit this request? You are requesting for more than Rs 10000 or your Basic Salary.", function (result) {
            if (result) {
                if ($('#advanceform').valid()) {
                    currentForm.submit();
                }
            }
        });
        return;
    }
    if ($('#advanceform').valid()) {
        currentForm.submit();
    }
});

 
    function get_balance()
    {       
        var oTable = document.getElementById('myTable');
        var rowLength = oTable.rows.length;
        var balance = 0;
        for (i = 0; i < rowLength-2; i++){
            var input_amount = 'ExpenseLineItems_' + i + '__Amount'
            var row_amount = 0;
            try {
                row_amount = document.getElementById(input_amount).value;
            }
            catch (err) {
                
            }
              
            if (row_amount == '') { row_amount = 0; }
         
           // balance = parseInt(balance) + parseInt(row_amount);
            balance = parseFloat(parseFloat(balance) + parseFloat(row_amount)).toFixed(2)

        }
        $('#lbl_total').text(balance);

 

    }

 

 
	$(document).ready(function() { 	 
	get_balance();

    

	});
 
