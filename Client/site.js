const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:7041/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

let mapWidth = 0;
let mapHeight = 0;
let paths = [];
const blockedPositions = new Set();

let activeTowerCategory = 0;
let activeSelectionDiv = document.getElementById('longDistanceTowerDiv');

const upgradeMap = {
    'DoubleDamage': 0,
    'Burst': 1,
    'DoubleBullet': 2
};

const upgradeCosts = {
    Burst: 30,
    DoubleBullet: 60, 
    DoubleDamage: 90
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

async function joinRoom() {
    const roomCode = document.getElementById('roomCode').value;
    const username = document.getElementById('username').value;

    if (roomCode && username) {
        await connection.invoke("JoinRoom", roomCode, username);

        document.getElementById('loginForm').classList.add('hidden');
        document.getElementById('gameInterface').classList.remove('hidden');
        document.getElementById('resourcesDisplay').classList.remove('hidden');
        document.getElementById('display').classList.remove('hidden');
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

async function startGame() {
    const roomCode = document.getElementById('roomCode').value;
    const username = document.getElementById('username').value;

    connection.invoke("StartGame", roomCode, username);
}

function clearMap() {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = '';
}

connection.on("InitializeMap", function (width, height, map, mapEnemies, newPaths, mainObject, mapBullets) {
    mapWidth = width;
    mapHeight = height;
    paths = newPaths.flat();

    const gameMap = document.getElementById('gameMap');
    gameMap.style.gridTemplateColumns = `repeat(${width}, 10px)`;
    gameMap.style.gridTemplateRows = `repeat(${height}, 10px)`;
    gameMap.style.width = `${width * 10}px`;
    gameMap.style.height = `${height * 10}px`;

    renderMap(map, mapEnemies, mapBullets, mainObject);
});

connection.on("OnTick", function (map, mapEnemies, mapBullets, newPaths, mainObject) {
    paths = newPaths.flat();

    renderMap(map, mapEnemies, mapBullets, mainObject);
});

connection.on("DisplayMessage", function(message) {
    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = `${message}`;
    messageList.appendChild(listItem);
});

connection.on("GameStarted", function (message) {
    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = message;
    messageList.appendChild(listItem);

    document.getElementById('startButton').classList.add('hidden');
    document.getElementById('gameMap').addEventListener('click', handleMapClick);
});

connection.on("OnGameOver", function (GameOverInfo) {
    document.getElementById('roomInfo').classList.add('hidden');
    document.getElementById('display').classList.add('hidden');
    document.getElementById('resourcesDisplay').classList.add('hidden');
    document.getElementById('activePlayersContainer').classList.add('hidden');

    const towerSelectionBar = document.getElementById('towerSelectionBar');
    towerSelectionBar.classList.add('hidden');
    towerSelectionBar.style.display = 'none';
    activeSelectionDiv.classList.remove('active');

    document.getElementById('gameInterface').classList.add('hidden');

    updateResources(0);
    document.getElementById("levelDisplay").textContent = `Level: 1`;

    document.getElementById("gameMap").innerHTML = '';
    document.getElementById('messagesList').innerHTML = '';

    document.getElementById('loginForm').classList.remove('hidden');

    const gameOverDiv = document.createElement('div');
    gameOverDiv.id = 'gameOverInfo';
    gameOverDiv.className = 'game-over-info';

    const header = document.createElement('h3');
    header.textContent = 'Game Results';
    gameOverDiv.appendChild(header);

    const gameOverImage = document.createElement('img');
    gameOverImage.src = GameOverInfo.path;
    gameOverImage.style.width = '100%';
    gameOverImage.alt = 'Game Over Image';

    const gameOverText = document.createElement('p');
    gameOverText.innerHTML = `Health: ${GameOverInfo.health}<br>Level: ${GameOverInfo.level}<br>Resources: ${GameOverInfo.resources}`;
    gameOverText.style.marginTop = '20px';

    gameOverDiv.appendChild(gameOverImage);
    gameOverDiv.appendChild(gameOverText);

    document.body.appendChild(gameOverDiv);

    document.addEventListener('click', function(event) {
        if (!gameOverDiv.contains(event.target)) {
            gameOverDiv.remove();
            document.removeEventListener('click', arguments.callee);
        }
    });
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

connection.on("OnResourceChanged", function (newResourceAmount) {
    updateResources(newResourceAmount);
});

connection.on("LevelChanged", (level) => {
    document.getElementById("levelDisplay").textContent = `Level: ${level}`;
});

function updateActiveUsersList(players) {
    const activeUsersList = document.getElementById('activeUsersList');
    activeUsersList.innerHTML = '';

    players.forEach(player => {
        const userItem = document.createElement('li');
        userItem.textContent = `${player.username} - ${player.towerType}`;
        activeUsersList.appendChild(userItem);
    });
}

function renderMap(map, mapEnemies, mapBullets, mainObject) {
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

        const bulletImage = document.createElement('img');
        bulletImage.src = bullet.path;
        bulletImage.style.width = '100%';
        bulletImage.style.height = '100%';
        bulletElement.appendChild(bulletImage);
    });

    const mainObjectFunction = (mainObject) => {
        const mainObjectHealth = document.getElementById('healthDisplay');
        mainObjectHealth.textContent = `Health: ${mainObject.health}`;

        const mainObjectElement = document.getElementById('mainObject');
        mainObjectElement.innerHTML = '';

        const mainObjectImage = document.createElement('img');
        mainObjectImage.src = mainObject.path;
        mainObjectElement.appendChild(mainObjectImage);
    };

    mainObjectFunction(mainObject);
}

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

function isPositionBlocked(x, y) {
    const isPath = paths.some(pathPoint => pathPoint.x === x && pathPoint.y === y);
    if (isPath) {
        return true;
    }

    return blockedPositions.has(`${x},${y}`);
}

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

function addTowerToBlockedPositions(towerX, towerY) {
    for (let dx = -1; dx <= 1; dx++) {
        for (let dy = -1; dy <= 1; dy++) {
            blockedPositions.add(`${towerX + dx},${towerY + dy}`);
        }
    }
}

function handleMapClick(event) {
    const gameMap = document.getElementById('gameMap');
    const bounds = gameMap.getBoundingClientRect();
    const cellSize = 10;
    const relativeX = event.clientX - bounds.left;
    const relativeY = event.clientY - bounds.top;
    const clickedX = Math.floor(relativeX / cellSize) - 1;
    const clickedY = Math.floor(relativeY / cellSize) - 1;

    const cell = [...gameMap.children].find(child =>
        parseInt(child.style.gridColumnStart) - 1 === clickedX &&
        parseInt(child.style.gridRowStart) - 1 === clickedY &&
        child.classList.contains('tower')
    );

    if (cell) {
        const turretX = parseInt(cell.style.gridColumnStart) - 1;
        const turretY = parseInt(cell.style.gridRowStart) - 1;
        const appliedUpgrades = JSON.parse(cell.dataset.appliedUpgrades || '[]');
        console.log(`Tower found at (${turretX}, ${turretY}). Showing upgrade options.`);
        showUpgradeOptions(turretX, turretY, appliedUpgrades);
        return;
    } 

    let canPlace = true;

    for (let dx = -1; dx <= 1; dx++) {
        for (let dy = -1; dy <= 1; dy++) {
            const checkX = clickedX + dx;
            const checkY = clickedY + dy;

            if (checkX < 0 || checkX >= mapWidth || checkY < 0 || checkY >= mapHeight) {
                canPlace = false;
                console.log("Cannot place tower here: out of bounds.");
                break;
            }

            if (isPositionBlocked(checkX, checkY)) {
                canPlace = false;
                console.log("Cannot place tower here: area is blocked by path or another turret.");
                break;
            }
        }
        if (!canPlace) break;
    }

    if (canPlace) {
        console.log("No obstacles found; placing new tower.");
        const roomCode = document.getElementById('roomCode').value;
        connection.invoke("PlaceTower", roomCode, clickedX, clickedY, activeTowerCategory);
        addTowerToBlockedPositions(clickedX, clickedY);
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
    
    const allUpgrades = Object.keys(upgradeCosts);
    const availableUpgrades = allUpgrades.filter(upgrade => !appliedUpgrades.includes(upgrade));

    if (availableUpgrades.length === 0) {
        const noUpgradesMsg = document.createElement('p');
        noUpgradesMsg.textContent = 'No upgrades available.';
        upgradeDiv.appendChild(noUpgradesMsg);
    } 
    else {
        availableUpgrades.forEach(upgrade => {
            const upgradeButton = document.createElement('button');
            upgradeButton.className = 'upgrade-button';

            upgradeButton.textContent = `${upgrade.replace(/([A-Z])/g, ' $1').trim()} (Cost: ${upgradeCosts[upgrade]})`;

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

connection.on("EffectApplied", function (messageObjects) {
    const [effectType, environmentType] = messageObjects;

    console.log('effectApplied', effectType, environmentType);

    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = `Effect of type ${effectType} has been applied!`;
    messageList.appendChild(listItem);

    if (environmentType) {
        highlightPlayersWithMatchingTowerType(environmentType, "applied");
    }
});

connection.on("EffectEnded", function (messageObjects) {
    const [effectType, environmentType] = messageObjects;

    console.log('effectEnded', effectType, environmentType);

    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = `Effect of type ${effectType} has ended.`;
    messageList.appendChild(listItem);

    if (environmentType) {
        highlightPlayersWithMatchingTowerType(environmentType, "ended");
    }
});

function highlightPlayersWithMatchingTowerType(environmentType, status) {
    const activeUsersList = document.getElementById('activeUsersList');

    [...activeUsersList.children].forEach((userItem) => {
        const [username, towerType] = userItem.textContent.split(" - ");

        const towerTypeCleared = towerType
            .replace(/\s*\(.*\)$/, "")
            .trim();

        if (towerTypeCleared.trim().toLowerCase() === environmentType.toLowerCase()) {
            if (status === "applied") {
                userItem.classList.add('highlight-effect');
                userItem.textContent = `${username} - ${towerTypeCleared} (buffed)`;
            } else if (status === "ended") {
                userItem.classList.remove('highlight-effect');
                userItem.textContent = `${username} - ${towerTypeCleared}`;
            }
        }
    });
}


connection.start().catch(function (err) {
    console.error(err.toString());
});
