jQuery(document).ready(function ($) {
    $(".clickable-row").click(function () {
        var moduleID = $(this).data("moduleid");
        localStorage.setItem('moduleid', moduleID);
        window.location = $(this).data("href");
    });
});