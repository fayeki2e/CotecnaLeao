
function exportTableToExcel(tableID, filename = '') {
    debugger;
    var downloadLink;
    var dataType = 'application/vnd.ms-excel';
    var tableSelect = document.getElementById(tableID);
    var tableHTML = tableSelect.outerHTML.replace(/ /g, '%20');
    filename = filename ? filename + '.xls' : 'excel_data.xls';
    downloadLink = document.createElement("a");
    document.body.appendChild(downloadLink);
    if (navigator.msSaveOrOpenBlob) {
        var blob = new Blob(['\ufeff', tableHTML], {
            type: dataType
        });
        navigator.msSaveOrOpenBlob(blob, filename);
    } else {
        downloadLink.href = 'data:' + dataType + ', ' + tableHTML;
        downloadLink.download = filename;
        downloadLink.click();
    }
}

function exportData(tableID, filename = '') {
    /* Get the HTML data using Element by Id */
    var table = document.getElementById(tableID);

    /* Declaring array variable */
    var rows = [];

 
    //iterate through rows of table
    for (var i = 0, row; row = table.rows[i]; i++) {
        //rows would be accessed using the "row" variable assigned in the for loop
        //Get each cell value/column from the row

        var rdata = [];

        for (var j = 0; j < table.rows[i].cells.length; j++) {
            if (table.rows[0].cells[j].innerText != '') {
                rdata.push([table.rows[i].cells[j].innerText]);
            }
        }
        //column1 = row.cells[0].innerText;
        //column2 = row.cells[1].innerText;
        //column3 = row.cells[2].innerText;
        //column4 = row.cells[3].innerText;
        //column5 = row.cells[4].innerText;

        /* add a new records in the array */
        //rows.push(
        //    [
        //        column1,
        //        column2,
        //        column3,
        //        column4,
        //        column5
        //    ]
        //);

        rows.push(rdata);


    }
    csvContent = "data:text/csv;charset=utf-8,";
    /* add the column delimiter as comma(,) and each row splitted by new line character (\n) */
    rows.forEach(function (rowArray) {
        row = rowArray.join(",");
        csvContent += row + "\r\n";
    });

 




    /* create a hidden <a> DOM node and set its download attribute */
    var encodedUri = encodeURI(csvContent);
    var link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", filename);
    document.body.appendChild(link);
    /* download the data file named "Stock_Price_Report.csv" */
    link.click();
}


function exportToExcel1(tableID) {
    var tab_text = "<tr bgcolor='#87AFC6'>";
    var textRange; var j = 0, rows = '';
    tab = document.getElementById(tableID);
    tab_text = tab_text + tab.rows[0].innerHTML + "</tr>";
    var tableData = $('#' + tableID).DataTable().rows().data();
    for (var i = 0; i < tableData.length; i++) {
        rows += '<tr>'
            + '<td>' + tableData[i].value1 + '</td>'
            + '<td>' + tableData[i].value2 + '</td>'
            + '<td>' + tableData[i].value3 + '</td>'
            + '<td>' + tableData[i].value4 + '</td>'
            + '<td>' + tableData[i].value5 + '</td>'
            + '<td>' + tableData[i].value6 + '</td>'
            + '<td>' + tableData[i].value7 + '</td>'
            + '<td>' + tableData[i].value8 + '</td>'
            + '<td>' + tableData[i].value9 + '</td>'
            + '<td>' + tableData[i].value10 + '</td>'
            + '</tr>';
    }
    tab_text += rows;
    var data_type = 'data:application/vnd.ms-excel;base64,',
        template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table border="2px">{table}</table></body></html>',
        base64 = function (s) {
            return window.btoa(unescape(encodeURIComponent(s)))
        },
        format = function (s, c) {
            return s.replace(/{(\w+)}/g, function (m, p) {
                return c[p];
            })
        }

    var ctx = {
        worksheet: "Sheet 1" || 'Worksheet',
        table: tab_text
    }
    document.getElementById("dlink").href = data_type + base64(format(template, ctx));
    document.getElementById("dlink").download = "StudentDetails.xls";
    document.getElementById("dlink").traget = "_blank";
    document.getElementById("dlink").click();
}

