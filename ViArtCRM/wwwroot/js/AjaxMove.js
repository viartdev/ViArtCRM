
$(document).ready(function () {
    $('.moveButton').click(function (e) {
        e.preventDefault();
        console.log("click");
        var data = {
            taskID: $(this).data("taskid"),
            currentTaskStatus: $(this).data("taskstatus")
        };
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