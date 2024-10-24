const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:7041/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

let mapWidth = 0;
let mapHeight = 0;

let activeTowerCategory = 0;
let activeSelectionDiv = document.getElementById('longDistanceTowerDiv');

const upgradeMap = {
    'DoubleDamage': 0,
    'Burst': 1,
    'DoubleBullet': 2
};

async function joinRoom() {
    const roomCode = document.getElementById('roomCode').value;
    const username = document.getElementById('username').value;

    if (roomCode && username) {
        await connection.invoke("JoinRoom", roomCode, username);

        document.getElementById('loginForm').classList.add('hidden');
        document.getElementById('gameInterface').classList.remove('hidden');
        document.getElementById('resourcesDisplay').classList.remove('hidden');
        document.getElementById('startButton').classList.remove('hidden');

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

async function startGame(){
    const roomCode = document.getElementById('roomCode').value;
    const username = document.getElementById('username').value;

    connection.invoke("StartGame", roomCode, username);
}

function clearMap() {
    const gameMap = document.getElementById('gameMap');
    gameMap.innerHTML = '';
}

connection.on("InitializeMap", function (width, height, map, mapEnemies) {
    mapWidth = width;
    mapHeight = height;

    const gameMap = document.getElementById('gameMap');

    gameMap.style.gridTemplateColumns = `repeat(${width}, 50px)`;
    gameMap.style.gridTemplateRows = `repeat(${height}, 50px)`;

    gameMap.style.width = `${width * 50}px`;
    gameMap.style.height = `${height * 50}px`;

    renderMap(map, mapEnemies);
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

    const activeUsersList = document.getElementById('activeUsersList');
    activeUsersList.innerHTML = '';

    players.forEach(player => {
        const userItem = document.createElement('li');
        userItem.textContent = `${player.username} - ${player.towerType}`;
        activeUsersList.appendChild(userItem);
    });
});

connection.on("UserLeft", function (username, players) {
    const messageList = document.getElementById('messagesList');
    const listItem = document.createElement('li');
    listItem.textContent = `${username} has left the room!`;
    messageList.appendChild(listItem);

    const activeUsersList = document.getElementById('activeUsersList');
    activeUsersList.innerHTML = '';

    players.forEach(player => {
        const userItem = document.createElement('li');
        userItem.textContent = `${player.username} - ${player.towerType}`;
        activeUsersList.appendChild(userItem);
    });

    const towerSelectionBar = document.getElementById('towerSelectionBar');
    towerSelectionBar.style.display = 'none';
});

connection.on("OnResourceChanged", function (newResourceAmount) {
    updateResources(newResourceAmount);
});

connection.on("OnTick", function (map, mapEnemies, mapBullets) {
    renderMap(map, mapEnemies, mapBullets);
});

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
    
    mapEnemies.forEach(enemy => {
        const cell = document.createElement('div');
        cell.className = 'grid-cell enemy';

        if (enemy.speed == 1) {
            cell.classList.add('strong-enemy');
        } else if (enemy.speed == 2) {
            cell.classList.add('flying-enemy');
        }
        else{
            cell.classList.add('fast-enemy');
        }

        gameMap.appendChild(cell);

        cell.style.gridColumnStart = enemy.x + 1;
        cell.style.gridRowStart = enemy.y + 1;
    });

    mapBullets.forEach(bullet => {
        const bulletElement = document.createElement('div');
        bulletElement.className = 'bullet'; // Add a class for bullets
        gameMap.appendChild(bulletElement);

        bulletElement.style.gridColumnStart = bullet.x + 1;
        bulletElement.style.gridRowStart = bullet.y + 1;
    });
}

function updateResources(resources) {
    const resourcesDisplay = document.getElementById('resourcesDisplay');
    resourcesDisplay.textContent = `Resources: ${resources}`;
}

connection.start().catch(function (err) {
    console.error(err.toString());
});

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

function handleMapClick(event) {
    const gameMap = document.getElementById('gameMap');
    const bounds = gameMap.getBoundingClientRect();
    const x = event.clientX - bounds.left;
    const y = event.clientY - bounds.top;
    const gridX = Math.floor(x / 50);
    const gridY = Math.floor(y / 50);

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
            // Format the upgrade name for display
            const upgradeText = upgrade.replace(/([A-Z])/g, ' $1').trim();
            upgradeButton.textContent = upgradeText;
            upgradeButton.className = 'upgrade-button';
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

document.addEventListener('click', function(event) {
    const upgradeDiv = document.querySelector('.upgrade-options');
    const overlay = document.querySelector('.overlay');
    if (overlay && event.target === overlay) {
        if (upgradeDiv) {
            upgradeDiv.remove();
        }
        overlay.remove();
    }
}, true);






