$(document).ready(function() {

    function Initialize() {
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

            function getSelectedScreen() {
                var screenId = $('input[name=screen]:checked').val();
                if (screenId === undefined)
                    return '0';
                return screenId;
            }
            function getClickPosition(e) {
                var parentPosition = getPosition(e.currentTarget);
                var selectedScreen = getSelectedScreen();
                xFinalPosition = e.clientX - parentPosition.x - (lookHereImage.clientWidth / 2);
                yFinalPosition = e.clientY - parentPosition.y - (lookHereImage.clientHeight / 2);

                movePointerPositionToNewLocation(xFinalPosition, yFinalPosition);

                sendPointerServerNewLocation(selectedScreen, xFinalPosition, yFinalPosition);
            }

            contentContainer.addEventListener("click", getClickPosition);

        });


    }

    Initialize();
    $("#screen1").prop('checked', true);
});