var selectedMonitorIndex = 0;
var monitorCount = 0;

$(document).ready(function () {

    function InitializeHub() {
        $.connection.hub.url = "/signalr";
        var hubMessenger = $.connection.pointerHub;


        $.connection.hub.start().done(function () {

            var lookHereImage = document.querySelector("#lookHereImage");
            var contentContainer = document.querySelector("#contentContainer");

            var xFinalPosition = 0;
            var yFinalPosition = 0;

            function getPosition(element) {
                var xPosition = 0;
                var yPosition = 0;

                while (element) {
                    xPosition += (element.offsetLeft - element.scrollLeft + element.clientLeft);
                    yPosition += (element.offsetTop - element.scrollTop + element.clientTop);
                    element = element.offsetParent;
                }
                return { x: xPosition, y: yPosition };
            }

            function movePointerPositionToNewLocation(xPosition, yPosition) {
                lookHereImage.style.left = xPosition + "px";
                lookHereImage.style.top = yPosition + "px";
            }

            function sendPointerServerNewLocation(selectedScreen, xPosition, yPosition) {

                var positions = selectedScreen + "," + xPosition + "," + yPosition;
                hubMessenger.server.send("position", positions);
            }



            function getClickPosition(e) {
                var parentPosition = getPosition(e.currentTarget);
                xFinalPosition = e.clientX - parentPosition.x - (lookHereImage.clientWidth / 2);
                yFinalPosition = e.clientY - parentPosition.y - (lookHereImage.clientHeight / 2);

                movePointerPositionToNewLocation(xFinalPosition, yFinalPosition);

                sendPointerServerNewLocation(selectedMonitorIndex, xFinalPosition, yFinalPosition);
            }

            function getDragOverPosition(e) {
                e = e || window.event;
                var parentPosition = getPosition(e.currentTarget);
                xFinalPosition = e.pageX - parentPosition.x - (lookHereImage.clientWidth / 2);
                yFinalPosition = e.pageY - parentPosition.y - (lookHereImage.clientHeight / 2);

                movePointerPositionToNewLocation(xFinalPosition, yFinalPosition);

                sendPointerServerNewLocation(selectedMonitorIndex, xFinalPosition, yFinalPosition);
            };

            contentContainer.addEventListener("click", getClickPosition);
            contentContainer.addEventListener("dragover", getDragOverPosition, false);

            hubMessenger.server.getMonitorCount().done(function (count) {
                for (var i = 0; i < count; i++) {
                    $("div[id^='monitor" + i + "']").toggleClass("hidden");
                };
            });
        });




    }

    InitializeHub();

});

function DeselectAllMonitors() {
    $("div[id^='monitor']").each(function () {
        if (!$(this).hasClass("hidden")) {
            $(this).prop("class", "inactiveMonitor");
        }
    });
}

function ActivateMonitor(monitorNumber) {
    DeselectAllMonitors();
    selectedMonitorIndex = monitorNumber;
    $("div[id^='monitor" + monitorNumber + "']").prop("class", "activeMonitor");

}