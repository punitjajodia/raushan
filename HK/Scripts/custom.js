var popupFormOptions = {
    autoOpen: false,
    modal: true,
    width: 700,
    title: 'Add Data'
};

function ReloadUIComponents() {
    $("input[type=datetime").datepicker();
}

function ShowPopup(data){
    $("#popupForm").dialog(popupFormOptions).dialog("open");
    $("#popupForm").empty().append(data);
}

$(document).on("click", "#add-importer", function () {
    $.get("/Importers/Create", ShowPopup);
});

$(document).on("click","#add-exporter", function () {
    $.ajax({
        //Call CreatePartialView action method

        url: "/Exporters/Create",
        type: 'Get',
        success: function (data) {
            $("#popupForm").dialog(popupFormOptions).dialog("open");
            $("#popupForm").empty().append(data);
        },
        error: function () {
            alert("something seems wrong");
        }
    });
});

function AddFromPopup(data, status, xhr) {
    if (xhr && xhr.getResponseHeader('X-Error')) {
        //validation error occurred
        $("#popupForm").empty().append(data);
    } else {
        RefreshContainerForm();
    }
}


$(function () {
    ReloadUIComponents();
    hideEditableInvoiceFields();
});

function focusOnNextElement() {
    var index = parseInt(localStorage.getItem("lastFocusInputElement")) + 1;
    $("input[type=text], textarea").eq(index).select();
}

function RefreshContainerForm() {
    
        $("#popupForm").dialog(popupFormOptions).dialog("close");
        var params = {};
        $("#EditContainerForm :input").each(function () {
            params[this.name] = $(this).val();
        })

        $.post('/Containers/Edit', params, function (data) {
            $("#CreateEditContainer").html(data);
            ReloadUIComponents();
            focusOnNextElement();
        });
}



$(document).on("change", "#EditContainerForm :input", function () {
    RefreshContainerForm();
});

function hideEditableInvoiceFields() {
    $(".ShowEdit_Edit").hide();
}

$(document).on("change", "input[type=text], textarea", function () {
    var currentIndex = $("input[type=text], textarea").index(this);
    localStorage.setItem("lastFocusInputElement", currentIndex);
});

$(document).on("change", "#Invoices :input", function () {
    var ProductID = $(this).data("productid");
    var params = {};
    params.ProductID = ProductID;
    params.ProductContainerPriceID = $(this).data("productcontainerpriceid");

    if (params.ProductContainerPriceID == 0) { // In case we are changing the value for the first time, we only need to change the current value
        params[$(this).data("modelname")] = $(this).val();
    } else {
        $("#Invoices :input[data-productcontainerpriceid=" + params.ProductContainerPriceID + "]").each(function () { //If we are editing, we need to send the other values too
            params[$(this).data("modelname")] = $(this).val();
        });
    }
    
    $.post('/ProductContainerPrices/Edit', params)
        .done(function (data) {
            RefreshData(function () {
                hideEditableInvoiceFields();
             //   console.log((parseInt(localStorage.getItem("lastFocusInputElement")) + 1));
                
            });
            
        })
        .fail(function (error) {
            console.log(error);
        });
});

$(document).on("change", ".buyerInvoiceForm :input", function () {
    
    $(this).parent("form").submit();
});

$(document).on("click", ".ShowEdit_Show", function () {
    $(this).siblings(".ShowEdit_Edit").show().focus();
    $(this).hide();
});

$(document).on("blur", ".ShowEdit_Edit", function () {
    $(this).siblings(".ShowEdit_Show").show();
    $(this).hide();
});

$("#CreateNewContainer").click(function () {
    $("#CreateEditContainer").load("/Containers/Create", function () {
        ReloadUIComponents();
    });
    
    $("#PackingList").html("");
    $("#AddContainerItemForm").hide();
})


function RefreshData(callback) {
    $.get("/Dashboard/List", function (data) {
        $("#AddContainerItemForm").show();
        $("#PackingList").html(data);
        focusOnNextElement();
        if (callback && typeof(callback) === "function") callback();
    });
    
}

$(function () {
    $("#tabs").tabs();
    $(document).ajaxStart(function () {
        $("#loading").show();
    });

    $(document).ajaxComplete(function () {
        $("#loading").hide();
    });

});