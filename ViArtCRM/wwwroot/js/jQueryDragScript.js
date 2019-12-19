var cardID = -1;

function OnControlsInitialized(s, e) {
    $('.draggbleCard').draggable({ //http://api.jqueryui.com/draggable/
        helper: 'clone',
        start: function (ev, ui) {
            var $sourceElement = $(ui.helper.context);
            var $draggingElement = $(ui.helper);
            //var sourceGrid = ASPxClientGridView.Cast($draggingElement.hasClass("left") ? "gridFrom" : "gridTo"); // Cast to zone
            var draggbleEelement = $draggingElement.get();
            
            //style elements
            $sourceElement.addClass("draggingStyle");
            $draggingElement.addClass("draggingStyle");
            $draggingElement.width(draggbleEelement.clientWidth);

            cardID = $draggingElement.attr("id");
            //find key
            //cardID = sourceGrid.GetRowKey(sourceGrid.GetTopVisibleIndex() + $sourceElement.index() - 1);
        },
        stop: function (e, ui) {
            $(".draggingStyle").removeClass("draggingStyle");
        }
    });

    var settings = function (className) {
        return {
            tolerance: "intersect",
            accept: className,
            drop: function (ev, ui) {
                $(".targetGrid").removeClass("targetGrid");
                var leftToRight = ui.helper.hasClass("left");
                //cbPanel.PerformCallback(cardID + "|" + leftToRight); ajax
            },
            over: function (ev, ui) {
                $(this).addClass("targetGrid");
            },
            out: function (ev, ui) {
                $(".targetGrid").removeClass("targetGrid");
            }
        };
    };

    //http://api.jqueryui.com/droppable/
    $(".droppableLeft").droppable(settings(".right"));
    $(".droppableRight").droppable(settings(".left"));

}