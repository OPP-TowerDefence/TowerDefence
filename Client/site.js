const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:7041/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

let mapWidth = 0;
let mapHeight = 0;

async function joinRoom() {
    const roomCode = document.getElementById('roomCode').value;
    const username = document.getElementById('username').value;

    if (roomCode && username) {
        await connection.invoke("JoinRoom", roomCode, username);

        document.getElementById('loginForm').classList.add('hidden');
        document.getElementById('gameInterface').classList.remove('hidden');

        const roomInfo = document.getElementById('roomInfo');
        roomInfo.textContent = `Room: ${roomCode} | Username: ${username}`;
        roomInfo.classList.remove('hidden');

        clearMap();
    }
}

function clearMap() {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = '';
}

connection.on("InitializeMap", function (width, height, map) {
    mapWidth = width;
    mapHeight = height;

    const gameMap = document.getElementById('gameMap');

    gameMap.style.gridTemplateColumns = `repeat(${width}, 50px)`;
    gameMap.style.gridTemplateRows = `repeat(${height}, 50px)`;

    gameMap.style.width = `${width * 50}px`;
    gameMap.style.height = `${height * 50}px`;

    renderMap(map);
});

connection.on("UserJoined", function (username) {
    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = `${username} has joined the room!`;
    messageList.appendChild(listItem);
});

connection.on("OnTick", function (map) {
    renderMap(map);
});

function renderMap(map) {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = '';

    map.forEach(tower => {
        const cell = document.createElement('div');
        cell.className = 'grid-cell tower';
        gameMap.appendChild(cell);

        cell.style.gridColumnStart = tower.x + 1;
        cell.style.gridRowStart = tower.y + 1;
    });
}

connection.start().catch(function (err) {
    console.error(err.toString());
});

document.getElementById('gameMap').addEventListener('click', function (event) {
    const bounds = event.target.getBoundingClientRect();
    const x = event.clientX - bounds.left;
    const y = event.clientY - bounds.top;
    const gridX = Math.floor(x / 50);
    const gridY = Math.floor(y / 50);

    if (gridX >= 0 && gridX < mapWidth && gridY >= 0 && gridY < mapHeight) {
        const roomCode = document.getElementById('roomCode').value;
        connection.invoke("PlaceTower", roomCode, gridX, gridY);
    } else {
        console.log('Tower placement is outside the grid boundaries.');
    }
});