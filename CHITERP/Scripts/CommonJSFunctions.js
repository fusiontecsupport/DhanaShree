jQuery.fn.exists = function () { return ($(this).length > 0); }

$("input").on('keydown', function () {
    //alert(this.maxLength);
    //alert(this.value.length);
    if (this.maxLength > 0) {
        if (this.value.length >= this.maxLength) this.value = this.value.slice(0, this.maxLength);
    }
});

//$("input[type=date]").on('change', function () {
//    //alert(this.maxLength);
//    //alert(this.value.length);
//    var tdt = $(this).val();
//    //alert(tdt);
//    if (tdt != "") {
//        if (isValidDate(tdt) == false) {
//            alert('Please select a Valid Date');
//            this.focus();
//        }
//    }
//});

//function isValidDate(txtDate) {
//    //return txtDate instanceof Date && !isNaN(date.getTime());
//    var currVal = txtDate;
//    var dtVal = txtDate.split("/");
//    console.log(dtVal);
//    if (dtVal == null)
//        dtVal = txtDate.split("-");
//    //Declare Regex  
//    console.log(dtVal);
//    var rxDatePattern = /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/
//    var dtArray = currVal.match(rxDatePattern); // is format OK?
//    //alert(dtVal);
//    //Checks for mm/dd/yyyy format.
//    if (dtVal[0].length > 3) {
//        //alert('yyyy-mm-dd');
//        dtMonth = eval(dtVal[1]);
//        dtDay = eval(dtVal[2]);
//        dtYear = eval(dtVal[0]);
//    }
//    else {
//        //alert('dd-mm-yyyy');
//        dtMonth = eval(dtVal[1]);
//        dtDay = eval(dtVal[0]);
//        dtYear = eval(dtVal[2]);
//    }
    
//    //alert(dtDay);
//    //alert(dtMonth);
//    //alert(dtYear);
//    if (currVal == '' || dtArray == null)
//        return false;
//    else if (dtMonth < 1 || dtMonth > 12)
//        return false;
//    else if (dtDay < 1 || dtDay > 31)
//        return false;
//    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
//        return false;
//    else if (dtMonth == 2) {
//        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
//        if (dtDay > 29 || (dtDay == 29 && !isleap))
//            return false;
//    }
//    else
//        return true;
//}

function CheckContr($obj, $val) {
    // alert($($obj).val());
    //alert($($val).val());

    var sel_cntnrno = $($obj).val();
    if (sel_cntnrno.length > 11 || sel_cntnrno.length < 11) {
        alert("Please Enter Complete Container No...!");
        $($obj).val("");
        $($obj).focus();
        return false;
    }

}

// ----------------- Save Form Data -------------------------------- //

function submitFormData(ActionName, controllername, pagetitle, formData) {
    var baseUrl = $("#BaseUrl").data("baseurl");
    var url = baseUrl + "/" + controllername + "/" + ActionName;
    var pg = pagetitle.split("~");
    var pgtl = "";
    var redirurl = "";
    //alert(pg.length);
    //alert(pagetitle);
    if (pg.length > 1) {
        pgtl = pg[0];
        redirurl = pg[1];
    }
    else
        pgtl = pagetitle;
    //alert(url)
    var Message = "";
    $.ajax({
        //url: "@Url.Action("SaveData", "CompanyAccountingDetail")",
        url: url,
        data: formData,
        type: "POST",
        success: function (status) {
            console.log(status);
            //alert(status);
            if (status == "Success") {
                Message = "" + pgtl + " Saved Successfully";
                //alert(Message);

                Swal.fire({
                    
                    //position: 'center',
                    type: 'success',
                    title: Message,
                    showConfirmButton: true,
                    //timer: 3500
                }).then((result) => {
                    if (result.value) {

                        ActionName = "Index";
                        var returnurl = baseUrl + "/" + controllername + "/" + ActionName;
                        console.log(returnurl);
                        //alert(returnurl);
                       // alert(redirurl);
                        //alert(returnurl);
                        if (redirurl != "")
                            returnurl = redirurl;
                        //var returnurl = "@Url.Action("Index", "CompanyAccountingDetail")";
                        window.location.href = returnurl;
                    }
                });
            }
            else if (status == "Existing") {

                Message = "" + pgtl + " Already Exists";
                Swal.fire({
                    //position: 'top-center',
                    type: 'info',
                    title: Message,
                    showConfirmButton: true,
                    //timer: 3500
                }).then((result) => {
                    if (result.value) {
                        ActionName = "Form";
                        var returnurl = baseUrl + "/" + controllername + "/" + ActionName;
                        if (redirurl != "")
                            returnurl = redirurl;
                        //var returnurl = "@Url.Action("Form", "CompanyAccountingDetail")";
                        window.location.href = returnurl;
                    }
                });
            }
            else if (status == "Continue") {

                Message = "" + pgtl + " Saved. Please Continue...";
                Swal.fire({
                    //position: 'top-center',
                    type: 'info',
                    title: Message,
                    showConfirmButton: true,
                    //timer: 3500
                }).then((result) => {
                    if (result.value) {
                        ActionName = "Form";
                        var returnurl = baseUrl + "/" + controllername + "/" + ActionName;
                        //var returnurl = window.location.href;
                        //console.log(returnurl);
                        if (redirurl != "")
                            returnurl = redirurl;
                        //console.log(returnurl);
                        //var returnurl = "@Url.Action("Form", "CompanyAccountingDetail")";
                        window.location.href = returnurl;
                    }
                });
            }
            else {
                Message = "" + pgtl + " Not Saved";

                Swal.fire({
                    //position: 'top-center',
                    type: 'error',
                    title: Message,
                    showConfirmButton: true,
                    //timer: 3500
                }).then((result) => {
                    if (result.value) {
                        ActionName = "Form";
                       //var returnurl = "" + baseUrl + "/" + controllername + "/" + ActionName + "";                        
                        //window.location.href = returnurl;
                    }
                });
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}


// ----------------- Save Form and File Data -------------------------------- //

function submitFormFileData(ActionName, controllername, pagetitle, formData, fileData, filActionName,id) {
    var baseUrl = $("#BaseUrl").data("baseurl");
    var url = baseUrl + "/" + controllername + "/" + ActionName;
    var furl = baseUrl + "/" + controllername + "/" + filActionName+"/?ids="+id;
    //alert(url)
    //alert(furl)
    var Message = "";
    $.ajax({
        //url: "@Url.Action("SaveData", "CompanyAccountingDetail")",
        url: url,
        data: formData,
        type: "POST",
        success: function (status) {
            //alert(status);
            //alert('bef fil data');
            var osts = status;
            $.ajax({
                url: furl, 
                data: fileData, //,
                type: "POST",
                cache: false,
                contentType: false,
                processData: false,
                success: function (status) {
                    //alert('after fil data');
                    Message = "" + pagetitle + " Saved Successfully";
                    if (status == "FileError") {
                        Swal.fire({
                            position: 'top-center',
                            type: 'error',
                            title: 'Attachments NOT Saved Successfully, File(s) not uploaded properly?',
                            showConfirmButton: true,
                            //timer: 3500
                        }).then((result) => {
                            if (result.value) {
                                ActionName = "Index";
                                var returnurl = baseUrl + "/" + controllername + "/" + ActionName;

                                //var returnurl = "@Url.Action("Index", "CompanyAccountingDetail")";
                                window.location.href = returnurl;
                            }
                        });
                    }
                    else if (osts == "Success") {
                        Message = "" + pagetitle + " Saved Successfully";
                        //alert(Message);

                        Swal.fire({

                            //position: 'center',
                            type: 'success',
                            title: Message,
                            showConfirmButton: true,
                            //timer: 3500
                        }).then((result) => {
                            if (result.value) {

                                ActionName = "Index";
                                var returnurl = baseUrl + "/" + controllername + "/" + ActionName;

                                //var returnurl = "@Url.Action("Index", "CompanyAccountingDetail")";
                                window.location.href = returnurl;
                            }
                        });
                    }
                    else if (osts == "Existing") {

                        Message = "" + pagetitle + " Already Exists";
                        Swal.fire({
                            //position: 'top-center',
                            type: 'info',
                            title: Message,
                            showConfirmButton: true,
                            //timer: 3500
                        }).then((result) => {
                            if (result.value) {
                                ActionName = "Form";
                                var returnurl = baseUrl + "/" + controllername + "/" + ActionName;

                                //var returnurl = "@Url.Action("Form", "CompanyAccountingDetail")";
                                window.location.href = returnurl;
                            }
                        });
                    }
                    else if (osts == "Continue") {

                        Message = "" + pagetitle + " Saved. Please Continue...";
                        Swal.fire({
                            //position: 'top-center',
                            type: 'info',
                            title: Message,
                            showConfirmButton: true,
                            //timer: 3500
                        }).then((result) => {
                            if (result.value) {
                                ActionName = "Form";
                                var returnurl = baseUrl + "/" + controllername + "/" + ActionName;

                                //var returnurl = "@Url.Action("Form", "CompanyAccountingDetail")";
                                window.location.href = returnurl;
                            }
                        });
                    }
                    else {
                        Message = "" + pagetitle + " Not Saved";

                        Swal.fire({
                            //position: 'top-center',
                            type: 'error',
                            title: Message,
                            showConfirmButton: true,
                            //timer: 3500
                        }).then((result) => {
                            if (result.value) {
                                ActionName = "Form";
                                //var returnurl = "" + baseUrl + "/" + controllername + "/" + ActionName + "";                        
                                //window.location.href = returnurl;
                            }
                        });
                    }
                },
                //xhr: function () {  // Custom XMLHttpRequest
                //    var myXhr = $.ajaxSettings.xhr();
                //    if (myXhr.upload) { // Check if upload property exists
                //        // Progress code if you want
                //    }
                //    return myXhr;
                //}
            });

            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}



function isNumberOnlyKey(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if ((charCode >= 48 && charCode <= 57) || ((charCode <= 110 && charCode >= 96))) {
        return true;
    }
    else
        return false;

}

function isAlphaNumberOnlyKey(e) {

    var key = e.which || e.keyCode;
    if (e.shiftKey && key >= 48 && key <= 57) {
        return false;
    }
    else {
        if (key >= 186 && key <= 187 || key >= 191 && key <= 222 || key == 32) {
            return false;
        }
        else {
            return false;
        }

    }
}

function isAlphaNumberOnlySpaceKey(e) {

    var key = e.which || e.keyCode;
    if (e.shiftKey && key >= 48 && key <= 57) {
        return false;
    }
    else {
        if (key >= 186 && key <= 187 || key >= 191 && key <= 222) {
            return false;
        }
        else {
            return false;
        }

    }
}

function isAlpha(evt) {
    var keyCode = (evt.which) ? evt.which : evt.keyCode
    if ((keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 123) && keyCode != 32)
        return false;
    else
        return true;
}

function validateFloatKeyPress(el, evt) {

    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57)) {
        return false;
    }

    if (charCode == 46 && el.value.indexOf(".") !== -1) {
        return false;
    }

    return true;
}

function isNumerDecimalOnly(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && !((charCode >= 48 && charCode <= 57) || ((charCode <= 110 && charCode >= 96))) && !(charCode == 46 || charCode == 8))
        return false;
    else {
        var len = $(element).val().length;
        var index = $(element).val().indexOf('.');
        if (index > 0 && charCode == 46) {
            return false;
        }
        if (index > 0) {
            var CharAfterdot = (len + 1) - index;
            if (CharAfterdot > 3) {
                return false;
            }
        }

    }
    return true;
}

function isNumberCommaDot(evt) {
    //alert('hi');
    var theEvent = evt || window.event;
    var key = theEvent.keyCode || theEvent.which;

    if (key === 9) { //TAB was pressed
        return;
    }

    key = String.fromCharCode(key);
    if (key.length == 0) return;
    var regex = /^[0-9,\9\b]*\.?[0-9]*$/;
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (!regex.test(key) && !((charCode >= 48 && charCode <= 57) || ((charCode <= 110 && charCode >= 96)))) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

function isNumerDecimalFourOnly(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && !((charCode >= 48 && charCode <= 57) || ((charCode <= 110 && charCode >= 96))) && !(charCode == 46 || charCode == 8))
        return false;
    else {
        var len = $(element).val().length;
        var index = $(element).val().indexOf('.');
        if (index > 0 && charCode == 46) {
            return false;
        }
        if (index > 0) {
            var CharAfterdot = (len + -1) - index;
            if (CharAfterdot > 3) {
                return false;
            }
        }

    }
    return true;
}


//function isAlphaNumberOnlyKey(e) {
//    var k = evt.keyCode || evt.which;
//    var ok = k >= 65 && k <= 90 || // A-Z
//        k >= 96 && k <= 105 || // a-z
//        k >= 35 && k <= 40 || // arrows
//        k == 9 || //tab
//        k == 46 || //del
//        k == 8 || // backspaces
//        (!evt.shiftKey && k >= 48 && k <= 57); // only 0-9 (ignore SHIFT options)

//    if (!ok || (evt.ctrlKey && evt.altKey)) {
//        evt.preventDefault();
//    }
//}

function isAlphaNumberOnlyKey1(evt) {
    var keyCode = e.keyCode || e.which;

    //Regex for Valid Characters i.e. Alphabets and Numbers.
    var regex = /^[A-Za-z0-9]+$/;

    var isValid = regex.test(String.fromCharCode(keyCode));
    if (!isValid) {
        SwalErrMsg("Only Alphabets and Numbers allowed !!");
        return false;
    }
    else {
        return true;
    }
}

function emailvalidate(email) {
    var aemail = email;
    var expr = /^([\w-\.]+)@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (!expr.test(aemail)) {
        SwalErrMsg("Invalid email address. !!");
        return false;
    }
    else {
        return true;
    }
}

function dfdemailvalidate(email) {
    var expr = /^([\w-\.]+)@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (!expr.test(email)) {
        SwalErrMsg("Invalid email address.");
    }
}

function add_autocomplete($obj, controls) {
    var oldFn = $.ui.autocomplete.prototype._renderItem;
    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        var re = new RegExp(this.term, "i");
        var t = item.label.replace(re, "<strong style='font-weight:bold;background:#C7DFFE;border-radius:2px;border:1px solid #98B8E1'>" + this.term + "</strong>");
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };
    $obj.autocomplete({
        source: function (request, response) {
            $.ajax({
                url: $obj.data("autocomplete-url"),
                type: "POST",
                dataType: "json",
                max: 10,
                scrollable: true,
                data: {
                    term: request.term
                },
                success: function (data) {

                    response($.map(data, function (item) {
                        count = 0;
                        item_str = "";
                        var jsonArg = new Object();
                        count = 0;
                        $.each(item, function (i, data) {
                            switch (count) {
                                case 0:
                                    jsonArg.label = data;// + " (" + item['WRDCODE'] + ")";
                                    jsonArg.value = data;
                                    break;
                                case 1:
                                    jsonArg.id = data;
                                    break;
                                case 2:
                                    jsonArg.desc = data;
                                    break;
                                case 3:
                                    jsonArg.xparam1 = data;
                                    break;
                                case 4:
                                    jsonArg.xparam2 = data;
                                    break;
                                case 5:
                                    jsonArg.xparam3 = data;
                                    break
                                case 6:
                                    jsonArg.xparam4 = data;
                                    break


                            }
                            count++
                        });
                        return jsonArg
                    }))
                }
            })
        },
        search: function () {
            var term = extractLast(this.value);
            if (term.length < 2) {
                return false
            }
        },
        select: function (event, ui) {
            $(this).val(ui.item.label);
            count = 0;
            $.each(controls.split(','), function (index, value) {
                switch (count) {

                    case 1:
                        $("#" + value).val(ui.item.id);
                        break;
                    case 2:
                        $("#" + value).val(ui.item.desc);
                        break;
                    case 3:
                        $("#" + value).val(ui.item.xparam1);
                        break;
                    case 4:
                        $("#" + value).val(ui.item.xparam2);
                        break;
                    case 5:
                        $("#" + value).val(ui.item.xparam3);
                        break
                    case 6:
                        $("#" + value).val(ui.item.xparam4);
                        break



                }
                count++
            });



            return false
        },
        change: function (event, ui) {
            //debugger;
            var opt = $(this).val();

            var crntval = event.currentTarget.value;
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: $obj.data("autocomplete-url"),
                data: "{'term':'" + crntval + "'}",
                dataType: 'json',
                success: function (data) {
                    //debugger;
                    if (data.length == 0) {
                        //$(event.currentTarget).val('');
                        alert('Select Items from the list.')
                        //SwalErrMsg('Select Items from the list.');
                    }
                },
                error: function (data) {
                    $(event.currentTarget).val('');
                    console.log('Error retrieving options.');
                }
            });
        },
        messages: {
            noResults: "",
            results: ""
        }
    })
}

function isAlphaNumeric(chkval) {
    var TCode = chkval;
    var Exp = /((^[0-9]+[a-z]+)|(^[a-z]+[0-9]+))+[0-9a-z]+$/i;
    if (!TCode.match(Exp))
        return false;
    else
        return true;
}

function isGstNumber(chkgst) {
    var gst_value = chkgst.toUpperCase();
    var reg = /^([0-9]{2}[a-zA-Z]{4}([a-zA-Z]{1}|[0-9]{1})[0-9]{4}[a-zA-Z]{1}([a-zA-Z]|[0-9]){3}){0,15}$/;
    if (gst_value.match(reg)) {
        return true;
    } else {
        return false;
    }
}


function GetFormattedDate(obj) {
    var MyDate_String_Value = obj;
    var value1 = new Date
        (
            parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
        );
    var dat = value1.getDate() +
        "-" +
        eval(value1.getMonth() + 1) +
        "-" +
        value1.getFullYear();
    return dat;
}
function GetFormattedDateTime(obj) {
    var MyDate_String_Value = obj;
    var value1 = new Date
        (
            parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
        );
    var dat = value1.getDate() +
        "-" +
        eval(value1.getMonth() + 1) +
        "-" +
        value1.getFullYear() + " " + value1.getHours() + ":" + value1.getMinutes();
    return dat;
}

function SwalErrMsg(msg) {
    //Swal.fire(
    //    'Error!',
    //    '<strong>'+ msg+'</strong>',
    //    'error'
    //);
    alert(msg);

}

function SwalSucMsg(msg) {
    //Swal.fire(
    //    'Success!',
    //    '<strong>' + msg + '</strong>',
    //    'success'
    //);
    alert(msg);

}
function SwalInfMsg(msg) {
    //Swal.fire(
    //    'Information!',
    //    '<strong>' + msg + '</strong>',
    //    'info'
    //);
    alert(msg);

}

function SwalAlert(msg) {
    //Swal.fire(
    //    'Alert!',
    //    '<strong>' + msg + '</strong>',
    //    'info'
    //);
    alert(msg);

}
