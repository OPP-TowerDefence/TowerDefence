const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:7041/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

let mapWidth = 0;
let mapHeight = 0;
let paths = []; // Flat array to store all path tiles

let activeTowerCategory = 0;
let activeSelectionDiv = document.getElementById('longDistanceTowerDiv');

const upgradeMap = {
    'DoubleDamage': 0,
    'Burst': 1,
    'DoubleBullet': 2
};

const enemyTypes = {
    STRONG: "Strong",
    FLYING: "Flying",
    FAST: "Fast"
};

const enemyClassMap = {
    [enemyTypes.STRONG]: 'strong-enemy',
    [enemyTypes.FLYING]: 'flying-enemy',
    [enemyTypes.FAST]: 'fast-enemy'
};

// Room join functionality
async function joinRoom() {
    const roomCode = document.getElementById('roomCode').value;
    const username = document.getElementById('username').value;

    if (roomCode && username) {
        await connection.invoke("JoinRoom", roomCode, username);

        document.getElementById('loginForm').classList.add('hidden');
        document.getElementById('gameInterface').classList.remove('hidden');
        document.getElementById('resourcesDisplay').classList.remove('hidden');
        document.getElementById('levelDisplay').classList.remove('hidden');
        document.getElementById('startButton').classList.remove('hidden');

        const roomInfo = document.getElementById('roomInfo');
        roomInfo.textContent = `Room: ${roomCode} | Username: ${username}`;
        roomInfo.classList.remove('hidden');

        const activePlayersContainer = document.getElementById('activePlayersContainer');
        activePlayersContainer.classList.remove('hidden');

        const towerSelectionBar = document.getElementById('towerSelectionBar');
        towerSelectionBar.classList.remove('hidden');
        towerSelectionBar.style.display = 'block';

        activeSelectionDiv.classList.add('active');

        clearMap();
    }
}

// Start game functionality
async function startGame() {
    const roomCode = document.getElementById('roomCode').value;
    const username = document.getElementById('username').value;

    connection.invoke("StartGame", roomCode, username);
}

function clearMap() {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = '';
}

// Map initialization with paths and grid setup
connection.on("InitializeMap", function (width, height, map, mapEnemies, newPaths) {
    mapWidth = width;
    mapHeight = height;
    paths = newPaths.flat();

    const gameMap = document.getElementById('gameMap');
    gameMap.style.gridTemplateColumns = `repeat(${width}, 10px)`;
    gameMap.style.gridTemplateRows = `repeat(${height}, 10px)`;
    gameMap.style.width = `${width * 10}px`;
    gameMap.style.height = `${height * 10}px`;

    renderMap(map, mapEnemies);
});

connection.on("OnTick", function (map, mapEnemies, mapBullets, newPaths) {
    paths = newPaths.flat();

    renderMap(map, mapEnemies, mapBullets);
});

connection.on("GameStarted", function (message) {
    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = message;
    messageList.appendChild(listItem);

    document.getElementById('startButton').classList.add('hidden');
    document.getElementById('gameMap').addEventListener('click', handleMapClick);
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

// Resource update event
connection.on("OnResourceChanged", function (newResourceAmount) {
    updateResources(newResourceAmount);
});

connection.on("LevelChanged", (level) => {
    document.getElementById("levelDisplay").textContent = `Level: ${level}`;
});

// Update active users list
function updateActiveUsersList(players) {
    const activeUsersList = document.getElementById('activeUsersList');
    activeUsersList.innerHTML = '';

    players.forEach(player => {
        const userItem = document.createElement('li');
        userItem.textContent = `${player.username} - ${player.towerType}`;
        activeUsersList.appendChild(userItem);
    });
}

// Render map with towers, paths, enemies, and bullets
function renderMap(map, mapEnemies, mapBullets) {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = '';

    map.forEach(tower => {
        const cell = document.createElement('div');
        cell.className = 'grid-cell tower';
        cell.classList.add(tower.type.toLowerCase());
        cell.classList.add(tower.category.toLowerCase());
        cell.dataset.appliedUpgrades = JSON.stringify(tower.appliedUpgrades);
        gameMap.appendChild(cell);

        cell.style.gridColumnStart = tower.x + 1;
        cell.style.gridRowStart = tower.y + 1;
    });

    paths.forEach(point => {
        renderPathTile(point);
    });

    mapEnemies.forEach(enemy => {
        const cell = document.createElement('div');
        cell.className = 'grid-cell enemy';
        const enemyClass = enemyClassMap[enemy.type];
        if (enemyClass) {
            cell.classList.add(enemyClass);
        }
        gameMap.appendChild(cell);
        cell.style.gridColumnStart = enemy.x + 1;
        cell.style.gridRowStart = enemy.y + 1;
    });

    mapBullets.forEach(bullet => {
        const bulletElement = document.createElement('div');
        bulletElement.className = 'bullet';
        gameMap.appendChild(bulletElement);
        bulletElement.style.gridColumnStart = bullet.x + 1;
        bulletElement.style.gridRowStart = bullet.y + 1;
    });
}

// Render path tile based on point type
function renderPathTile(point) {
    const gameMap = document.getElementById('gameMap');
    const cell = document.createElement('div');
    cell.className = 'grid-cell path';

    switch (point.type) {
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
            cell.classList.add('normal-tile');
            break;
    }

    gameMap.appendChild(cell);
    cell.style.gridColumnStart = point.x + 1;
    cell.style.gridRowStart = point.y + 1;
}

// Event handler for map clicks, checking path and tower placement validity
document.getElementById('gameMap').addEventListener('click', function (event) {
    const bounds = event.target.getBoundingClientRect();
    const x = event.clientX - bounds.left;
    const y = event.clientY - bounds.top;
    const gridX = Math.floor(x / 10);
    const gridY = Math.floor(y / 10);
    const roomCode = document.getElementById('roomCode').value;

    if (!isPathBlocked(gridX, gridY)) {
        connection.invoke("PlaceTower", roomCode, gridX, gridY, activeTowerCategory);
    } else {
        console.log('Cannot place the tower on the path or another tower.');
    }
});

function isPathBlocked(x, y) {
    return paths.some(pathPoint => pathPoint.x === x && pathPoint.y === y);
}

// Tower selection and upgrade functions
function selectTowerCategory(towerCategory) {
    activeTowerCategory = towerCategory;

    if (activeSelectionDiv) {
        activeSelectionDiv.classList.remove('active');
    }

    activeSelectionDiv = document.getElementById(towerCategory === 0 ? 'longDistanceTowerDiv' : 'heavyTowerDiv');
    activeSelectionDiv.classList.add('active');
}

function undoTower() {
    connection.invoke("UndoTower", document.getElementById('roomCode').value);
}

function handleMapClick(event) {
    const gameMap = document.getElementById('gameMap');
    const bounds = gameMap.getBoundingClientRect();
    const x = event.clientX - bounds.left;
    const y = event.clientY - bounds.top;
    const gridX = Math.floor(x / 10);
    const gridY = Math.floor(y / 10);

    if (gridX >= 0 && gridX < mapWidth && gridY >= 0 && gridY < mapHeight) {
        if (event.target.classList.contains('tower')) {
            const appliedUpgrades = JSON.parse(event.target.dataset.appliedUpgrades || '[]');
            showUpgradeOptions(gridX, gridY, appliedUpgrades);
        } else {
            const roomCode = document.getElementById('roomCode').value;
            connection.invoke("PlaceTower", roomCode, gridX, gridY, activeTowerCategory);
        }
    } else {
        console.log('Clicked area is outside the grid boundaries.');
    }
}

function showUpgradeOptions(gridX, gridY, appliedUpgrades) {
    const existingMenu = document.querySelector('.upgrade-options');
    const overlay = document.querySelector('.overlay');

    if (existingMenu) {
        existingMenu.remove();
        overlay.remove();
    }

    const overlayDiv = document.createElement('div');
    overlayDiv.className = 'overlay';

    overlayDiv.onclick = function(event) {
        event.stopPropagation();
        overlayDiv.remove();
        upgradeDiv.remove();
    };

    const upgradeDiv = document.createElement('div');
    upgradeDiv.className = 'upgrade-options';
    upgradeDiv.innerHTML = '<h2>Upgrade</h2>';

    const allUpgrades = ['DoubleDamage', 'Burst', 'DoubleBullet'];
    const availableUpgrades = allUpgrades.filter(upgrade => !appliedUpgrades.includes(upgrade));

    if (availableUpgrades.length === 0) {
        const noUpgradesMsg = document.createElement('p');
        noUpgradesMsg.textContent = 'No upgrades available.';
        upgradeDiv.appendChild(noUpgradesMsg);
    } else {
        availableUpgrades.forEach(upgrade => {
            const upgradeButton = document.createElement('button');
            upgradeButton.className = 'upgrade-button';

            upgradeButton.textContent = upgrade.replace(/([A-Z])/g, ' $1').trim();

            upgradeButton.onclick = function() {
                const upgradeType = upgradeMap[upgrade];
                connection.invoke("UpgradeTower", document.getElementById('roomCode').value, gridX, gridY, upgradeType);
                overlayDiv.remove();
                upgradeDiv.remove();
            };

            upgradeDiv.appendChild(upgradeButton);
        });
    }

    document.body.appendChild(overlayDiv);
    document.body.appendChild(upgradeDiv);

    upgradeDiv.style.position = 'fixed';
    upgradeDiv.style.top = '50%';
    upgradeDiv.style.left = '50%';
    upgradeDiv.style.transform = 'translate(-50%, -50%)';
}

function updateResources(resources) {
    document.getElementById('resourcesDisplay').textContent = `Resources: ${resources}`;
}

// Start connection
connection.start().catch(function (err) {
    console.error(err.toString());
});
