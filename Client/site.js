const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:7041/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

let mapWidth = 0;
let mapHeight = 0;
let path = [];  // Define path globally

let activeTowerCategory = 0;
let activeSelectionDiv = document.getElementById('longDistanceTowerDiv');

// Function to handle joining a room
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
        activePlayersContainer.classList.remove('hidden'); // Ensure active players are shown

        const towerSelectionBar = document.getElementById('towerSelectionBar');
        towerSelectionBar.classList.remove('hidden');
        towerSelectionBar.style.display = 'flex';

        activeSelectionDiv.classList.add('active');

        clearMap();
    }
}

// Function to clear the game map
function clearMap() {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = '';
}

// Initialize the map and path when the game starts
connection.on("InitializeMap", function (width, height, map, mapEnemies, newPath) {
    mapWidth = width;
    mapHeight = height;
    path = newPath;  // Update the global path variable

    const gameMap = document.getElementById('gameMap');

    // Update the grid size to be 10x10 for towers and 1x1 for enemies
    gameMap.style.gridTemplateColumns = `repeat(${width}, 10px)`;
    gameMap.style.gridTemplateRows = `repeat(${height}, 10px)`;

    gameMap.style.width = `${width * 10}px`;
    gameMap.style.height = `${height * 10}px`;

    renderMap(map, mapEnemies, path);
});

// Update the map on every tick
connection.on("OnTick", function (map, mapEnemies, newPath) {
    path = newPath;  // Update the global path variable on each tick
    renderMap(map, mapEnemies, path);
});

// Event handler for when a new user joins the room
connection.on("UserJoined", function (username, players) {
    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = `${username} has joined the room!`;
    messageList.appendChild(listItem);

    // Update the active users list
    const activeUsersList = document.getElementById('activeUsersList');
    activeUsersList.innerHTML = ''; // Clear the list first

    players.forEach(player => {
        const userItem = document.createElement('li');
        userItem.textContent = `${player.username} - ${player.towerType}`;
        activeUsersList.appendChild(userItem);
    });
});

// Event handler for when a user leaves the room
connection.on("UserLeft", function (username, players) {
    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = `${username} has left the room!`;
    messageList.appendChild(listItem);

    // Update the active users list
    const activeUsersList = document.getElementById('activeUsersList');
    activeUsersList.innerHTML = ''; // Clear the list first

    players.forEach(player => {
        const userItem = document.createElement('li');
        userItem.textContent = `${player.username} - ${player.towerType}`;
        activeUsersList.appendChild(userItem);
    });
});

// Function to render the game map
function renderMap(map, mapEnemies, path) {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = ''; // Clear the map before re-rendering

    // Render the towers
    map.forEach(tower => {
        const cell = document.createElement('div');
        cell.className = 'grid-cell tower';
        cell.classList.add(tower.type.toLowerCase());
        cell.classList.add(tower.category.toLowerCase());
        gameMap.appendChild(cell);

        // Set grid positions for the tower, which should occupy 10x10 cells
        cell.style.gridColumnStart = tower.x + 1;
        cell.style.gridColumnEnd = tower.x + 11;  // Towers are 10x10
        cell.style.gridRowStart = tower.y + 1;
        cell.style.gridRowEnd = tower.y + 11;  // Towers are 10x10
    });

    // Render the path
    renderPath(path);

    // Render the enemies
    mapEnemies.forEach(enemy => {
        const cell = document.createElement('div');
        cell.className = 'grid-cell enemy';

        if (enemy.speed == 1) {
            cell.classList.add('strong-enemy');
        } else if (enemy.speed == 2) {
            cell.classList.add('flying-enemy');
        } else {
            cell.classList.add('fast-enemy');
        }

        gameMap.appendChild(cell);

        // Set grid positions for the enemies, which are 1x1
        cell.style.gridColumnStart = enemy.x + 1;
        cell.style.gridRowStart = enemy.y + 1;
    });
}

// Function to render the path
function renderPath(path) {
    const gameMap = document.getElementById('gameMap');

    // Filter out any duplicate coordinates
    const uniquePath = path.filter((point, index, self) =>
        index === self.findIndex((p) => p.x === point.x && p.y === point.y)
    );

    uniquePath.forEach((point) => {
        const cell = document.createElement('div');
        cell.className = 'grid-cell path';  // Assign the path cell class
        gameMap.appendChild(cell);

        // Set grid positions based on path coordinates
        cell.style.gridColumnStart = point.x + 1;
        cell.style.gridRowStart = point.y + 1;
    });
}

// Start the connection to the server
connection.start().catch(function (err) {
    console.error(err.toString());
});

// Add the click event listener for placing towers
document.getElementById('gameMap').addEventListener('click', function (event) {
    const bounds = event.target.getBoundingClientRect();
    const x = event.clientX - bounds.left;
    const y = event.clientY - bounds.top;
    const gridX = Math.floor(x / 10);  // Adjusted for the new grid size
    const gridY = Math.floor(y / 10);  // Adjusted for the new grid size

    // Calculate the top-left corner of the 3x3 tower (centered around the click)
    const towerX = gridX - 1;  // Shift to center the tower
    const towerY = gridY - 1;  // Shift to center the tower

    // Check boundaries
    if (towerX >= 0 && towerX + 2 < mapWidth && towerY >= 0 && towerY + 2 < mapHeight) {
        const roomCode = document.getElementById('roomCode').value;
        
        // Check if any part of the tower overlaps with the path or other towers
        if (!isPathBlocked(towerX, towerY)) {
            connection.invoke("PlaceTower", roomCode, towerX, towerY, activeTowerCategory);
        } else {
            console.log('Cannot place the tower on the path or another tower.');
        }
    } else {
        console.log('Tower placement is outside the grid boundaries.');
    }
});

// Function to check if the 3x3 area overlaps with the path or existing towers
function isPathBlocked(towerX, towerY) {
    // Check against path
    for (let i = 0; i < path.length; i++) {
        const pathPoint = path[i];
        
        // Check if any of the 3x3 area around (towerX, towerY) intersects with the path
        for (let dx = 0; dx < 3; dx++) {
            for (let dy = 0; dy < 3; dy++) {
                if (towerX + dx === pathPoint.x && towerY + dy === pathPoint.y) {
                    return true;  // Block the tower if any part overlaps with the path
                }
            }
        }
    }

    // Check against existing towers
    const existingTowers = document.querySelectorAll('.grid-cell.tower');
    for (let i = 0; i < existingTowers.length; i++) {
        const towerElement = existingTowers[i];
        const towerGridX = parseInt(towerElement.style.gridColumnStart) - 1;
        const towerGridY = parseInt(towerElement.style.gridRowStart) - 1;

        // Check if the new 3x3 tower overlaps with any existing 3x3 tower
        for (let dx = 0; dx < 3; dx++) {
            for (let dy = 0; dy < 3; dy++) {
                for (let ex_dx = 0; ex_dx < 3; ex_dx++) {
                    for (let ex_dy = 0; ex_dy < 3; ex_dy++) {
                        if (towerX + dx === towerGridX + ex_dx && towerY + dy === towerGridY + ex_dy) {
                            return true;  // Block the tower if any part overlaps with an existing tower
                        }
                    }
                }
            }
        }
    }

    return false;  // No overlap with the path or towers, safe to place the tower
}

// Function to select tower category
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
