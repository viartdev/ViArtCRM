jQuery(document).ready(function ($) {
    $(".clickable-row").click(function () {
        var moduleID = $(this).data("moduleid");
        window.location.href = "/DashBoard/Index/" + moduleID;
    });
});