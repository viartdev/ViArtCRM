﻿$(document).ready(function () {
    $('.moveButton').click(function (e) {
        e.preventDefault();
        console.log("click");
        var data = {
            taskID: $(this).data("taskid"),
            currentTaskStatus: $(this).data("taskstatus")
        };

        //$.ajax({
        //    type: "POST",
        //    url: "/DashBoard/Move",
        //    content: "application/json; charset=utf-8",
        //    dataType: "json",
        //    data: JSON.stringify(data),
        //    success: function (d) {
        //        console.log(d);
        //    },
        //    error: function (xhr, textStatus, errorThrown) {
        //        console.log("Error");
        //    }
        //});
        $.ajax({
            url: '/DashBoard/Move',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: "application/json",
            dataType: 'json',
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            },
            success: function (result) {
                console.log(result);
            },
            async: true,
            processData: false
        });
    });
});
