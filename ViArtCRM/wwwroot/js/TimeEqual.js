$(document).ready(function () {
    var one_day = 1000 * 60 * 60 * 24;
    $(".card #date").each(function (index) {
        
        date = new Date($(".card #date")[index].innerHTML);
        datems = date.getTime();
        datenow = Date.now();
        diff = datems - datenow;
        diffdays = Math.round(diff / one_day);
        console.log(diffdays);
        item = $(".card .card-header").eq(index);
        if (diffdays <= 2)
            item.addClass("redline");
        else if (diffdays > 2 && diffdays <= 6)
            item.addClass("yellowline");
        else if (diffdays > 6)
            item.addClass("greenline");

    });
});