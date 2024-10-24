const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:7041/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

let mapWidth = 0;
let mapHeight = 0;
let paths = [];

let activeTowerCategory = 0;
let activeSelectionDiv = document.getElementById('longDistanceTowerDiv');

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

        const activePlayersContainer = document.getElementById('activePlayersContainer');
        activePlayersContainer.classList.remove('hidden');

        const towerSelectionBar = document.getElementById('towerSelectionBar');
        towerSelectionBar.classList.remove('hidden');
        towerSelectionBar.style.display = 'flex';

        activeSelectionDiv.classList.add('active');

        clearMap();
    }
}

function clearMap() {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = '';
}

connection.on("InitializeMap", function (width, height, map, mapEnemies, newPaths) {
    mapWidth = width;
    mapHeight = height;
    paths = newPaths;  // Store the array of paths

    const gameMap = document.getElementById('gameMap');

    gameMap.style.gridTemplateColumns = `repeat(${width}, 10px)`;
    gameMap.style.gridTemplateRows = `repeat(${height}, 10px)`;
    gameMap.style.width = `${width * 10}px`;
    gameMap.style.height = `${height * 10}px`;

    renderMap(map, mapEnemies);
});

connection.on("OnTick", function (map, mapEnemies, newPaths) {
    paths = newPaths;  // Update the array of paths on each tick
    renderMap(map, mapEnemies);
    console.log('Map Enemies:', JSON.stringify(mapEnemies, null, 2)); // Log the enemies
});

connection.on("UserJoined", function (username, players) {
    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = `${username} has joined the room!`;
    messageList.appendChild(listItem);

    updateActiveUsersList(players);
});

connection.on("UserLeft", function (username, players) {
    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = `${username} has left the room!`;
    messageList.appendChild(listItem);

    updateActiveUsersList(players);
});

function updateActiveUsersList(players) {
    const activeUsersList = document.getElementById('activeUsersList');
    activeUsersList.innerHTML = ''; // Clear the list first

    players.forEach(player => {
        const userItem = document.createElement('li');
        userItem.textContent = `${player.username} - ${player.towerType}`;
        activeUsersList.appendChild(userItem);
    });
}

function renderMap(map, mapEnemies) {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = ''; // Clear the map before re-rendering

    // Render towers
    map.forEach(tower => {
        const cell = document.createElement('div');
        cell.className = 'grid-cell tower';
        cell.classList.add(tower.type.toLowerCase());
        cell.classList.add(tower.category.toLowerCase());
        gameMap.appendChild(cell);

        cell.style.gridColumnStart = tower.x + 1;
        cell.style.gridColumnEnd = tower.x + 11;  // Towers are 10x10
        cell.style.gridRowStart = tower.y + 1;
        cell.style.gridRowEnd = tower.y + 11;  // Towers are 10x10
    });

    // Render all paths
    paths.forEach(path => {
        renderPath(path);
    });

    // Render enemies
    mapEnemies.forEach(enemy => {
        const cell = document.createElement('div');
        cell.className = 'grid-cell enemy';

        // Apply specific classes based on enemy type
        switch (enemy.type) {
            case 'FastEnemy':
                cell.classList.add('fast-enemy'); // Match with your CSS for fast enemies
                break;
            case 'StrongEnemy':
                cell.classList.add('strong-enemy'); // Match with your CSS for strong enemies
                break;
            case 'FlyingEnemy':
                cell.classList.add('flying-enemy'); // Match with your CSS for flying enemies
                break;
            default:
                cell.classList.add('enemy'); // Fallback to general enemy class
                break;
        }

        gameMap.appendChild(cell);
        cell.style.gridColumnStart = enemy.x + 1;
        cell.style.gridRowStart = enemy.y + 1;
    });
}


function renderPath(path) {
    const gameMap = document.getElementById('gameMap');

    path.forEach(point => {
        // Create a path cell
        const cell = document.createElement('div');
        cell.className = 'grid-cell path';  // Use the unified path class

        // Apply specific classes based on tile type
        switch (point.type) { // Make sure to match the key correctly
            case 'Normal':
                cell.classList.add('normal-tile');
                break;
            case 'Ice':
                cell.classList.add('ice-tile');
                break;
            case 'Mud':
                cell.classList.add('mud-tile');
                break;
            case 'PinkHealth':
                cell.classList.add('pinkhealth-tile');
                break;
            case 'Objective':
                cell.classList.add('objective-tile');
                break;
            default:
                cell.classList.add('normal-tile'); // Fallback class
                break;
        }

        gameMap.appendChild(cell);

        // Set grid positions based on path coordinates
        cell.style.gridColumnStart = point.x + 1;  // Access x
        cell.style.gridRowStart = point.y + 1;     // Access y
    });
}

connection.start().catch(function (err) {
    console.error(err.toString());
});

document.getElementById('gameMap').addEventListener('click', function (event) {
    const bounds = event.target.getBoundingClientRect();
    const x = event.clientX - bounds.left;
    const y = event.clientY - bounds.top;
    const gridX = Math.floor(x / 10);  // Adjusted for the new grid size
    const gridY = Math.floor(y / 10);  // Adjusted for the new grid size

    const towerX = gridX - 1;  // Shift to center the tower
    const towerY = gridY - 1;  // Shift to center the tower

    if (towerX >= 0 && towerX + 2 < mapWidth && towerY >= 0 && towerY + 2 < mapHeight) {
        const roomCode = document.getElementById('roomCode').value;
        
        if (!isPathBlocked(towerX, towerY)) {
            connection.invoke("PlaceTower", roomCode, towerX, towerY, activeTowerCategory);
        } else {
            console.log('Cannot place the tower on the path or another tower.');
        }
    } else {
        console.log('Tower placement is outside the grid boundaries.');
    }
});

function isPathBlocked(towerX, towerY) {
    const allPaths = paths.flat();  // Flatten the paths array

    for (let i = 0; i < allPaths.length; i++) {
        const pathPoint = allPaths[i];
        for (let dx = 0; dx < 3; dx++) {
            for (let dy = 0; dy < 3; dy++) {
                if (towerX + dx === pathPoint.X && towerY + dy === pathPoint.Y) {
                    return true;
                }
            }
        }
    }

    const existingTowers = document.querySelectorAll('.grid-cell.tower');
    for (let i = 0; i < existingTowers.length; i++) {
        const towerElement = existingTowers[i];
        const towerGridX = parseInt(towerElement.style.gridColumnStart) - 1;
        const towerGridY = parseInt(towerElement.style.gridRowStart) - 1;

        for (let dx = 0; dx < 3; dx++) {
            for (let dy = 0; dy < 3; dy++) {
                for (let ex_dx = 0; ex_dx < 3; ex_dx++) {
                    for (let ex_dy = 0; ex_dy < 3; ex_dy++) {
                        if (towerX + dx === towerGridX + ex_dx && towerY + dy === towerGridY + ex_dy) {
                            return true;
                        }
                    }
                }
            }
        }
    }

    return false;
}

function selectTowerCategory(towerCategory) {
    activeTowerCategory = towerCategory;

    if (activeSelectionDiv) {
        activeSelectionDiv.classList.remove('active');
    }

    if (towerCategory === 0) {
        activeSelectionDiv = document.getElementById('longDistanceTowerDiv');
    } else {
        activeSelectionDiv = document.getElementById('heavyTowerDiv');
    }

    activeSelectionDiv.classList.add('active');
}
