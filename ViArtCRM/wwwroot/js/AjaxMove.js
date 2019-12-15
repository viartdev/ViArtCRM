$(document).ready(function () {
    $('.moveButton').click(function (e) {
        e.preventDefault();
        console.log("click");
        var data = {
            taskID: $(this).data("taskID"),
            currentTaskStatus: $(this).data("taskStatus")
        };

        $.ajax({
            type: "POST",
            url: "/DashBoard/Move",
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            success: function (d) {
                console.log(d);
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("Error");
            }
        });
    });
});
