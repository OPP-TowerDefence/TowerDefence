const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

connection.start().catch(err => console.error(err.toString()));

connection.on("ReceiveTowerPlacement", function (x, y) {
    placeTowerOnUI(x, y);
});

function placeTowerOnUI(x, y) {
    const cell = document.querySelector(`#cell-${x}-${y}`);
    cell.classList.add('tower');
}

function placeTower(x, y) {
    connection.invoke("PlaceTower", x, y).catch(err => console.error(err.toString()));
}

