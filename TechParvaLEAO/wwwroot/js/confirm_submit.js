$('#masters_form').submit(function(e) {
    var currentForm = this;
    e.preventDefault();
    bootbox.confirm("Do you want to submit these changes?", function (result) {
        if (result) {
                currentForm.submit();
        }
    });
    return;
});
