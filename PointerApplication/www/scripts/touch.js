var selectedMonitorIndex = 0;
var monitorCount = 0;

$(document).ready(function () {

    function InitializeHub() {
        $.connection.hub.url = "/signalr";
        var hubMessenger = $.connection.pointerHub;


        $.connection.hub.start().done(function () {

            var lookHereImage = document.querySelector("#lookHereImage");
            var canvasContainer = document.querySelector("#canvasContainer");
            
            function movePointerPositionToNewLocation(xPosition, yPosition) {
                lookHereImage.style.left = xPosition + "px";
                lookHereImage.style.top = yPosition + "px";
            }

      
            function sendPointerServerNewLocation(selectedScreen, xPosition, yPosition) {

                var positions = selectedScreen + "," + xPosition + "," + yPosition;
                hubMessenger.server.send("position", positions);
            }

            function getTouchMovePosition(e) {
                e.preventDefault();
                var pointer = e.changedTouches[0];
                var xFinalPosition = pointer.pageX;
                var yFinalPosition = pointer.pageY;

                movePointerPositionToNewLocation(xFinalPosition, yFinalPosition);

                sendPointerServerNewLocation(selectedMonitorIndex, xFinalPosition, yFinalPosition);
            }
          
            canvasContainer.addEventListener("touchmove", getTouchMovePosition, false);
            canvasContainer.addEventListener("touchstart", getTouchMovePosition, false);
            
        });

    }

    InitializeHub();

});

