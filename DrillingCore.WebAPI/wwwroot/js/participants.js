// wwwroot/js/participants.js

// Глобальная переменная для хранения полного списка доступных пользователей
let allAvailableUsers = [];

// Токен не передаётся через скрипт – он хранится в куках

// Получаем текущий ID проекта из скрытого поля внутри модального окна проекта
function getCurrentProjectId() {
    const modalEl = document.getElementById('projectModal');
    if (!modalEl) {
        console.error("Project modal not found");
        return null;
    }
    const projectIdEl = modalEl.querySelector('#projectId');
    if (!projectIdEl) {
        console.error("Element with id 'projectId' not found in modal");
        return null;
    }
    return projectIdEl.value;
}

// -------------------- Функции работы с группами участников --------------------

// Загрузка групп участников для проекта (для отображения в разделе Participants)
async function loadParticipantGroups(projectId) {
    console.log("Loading participant groups for project ID:", projectId);
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups`, { cache: 'no-cache' });
        if (response.ok) {
            const groups = await response.json();
            console.log("Loaded participant groups:", groups);
            renderParticipantGroups(groups);
        } else {
            alert("Error fetching participant groups: " + response.statusText);
            console.error("Response status:", response.status);
        }
    } catch (error) {
        console.error("Error fetching participant groups:", error);
        alert("Error fetching participant groups. See console for details.");
    }
}

// Отрисовка групп и участников в таблице
function renderParticipantGroups(groups) {
    const tbody = document.getElementById("participantsTableBody");
    if (!tbody) {
        console.error("Element with id 'participantsTableBody' not found.");
        return;
    }
    tbody.innerHTML = "";
    if (!groups || groups.length === 0) {
        const tr = document.createElement("tr");
        tr.innerHTML = `<td colspan="7">No participant groups found.</td>`;
        tbody.appendChild(tr);
        return;
    }
    groups.forEach(group => {
        const headerRow = document.createElement("tr");
        headerRow.classList.add("group-header");
        headerRow.innerHTML = `<td colspan="7">${group.groupName}</td>`;
        tbody.appendChild(headerRow);
        if (group.participants && group.participants.length > 0) {
            group.participants.forEach(participant => {
                const dateAdded = participant.dateAdded ? new Date(participant.dateAdded).toLocaleDateString() : "-";
                const endDate = participant.endDate ? new Date(participant.endDate).toLocaleDateString() : "-";
                const tr = document.createElement("tr");
                tr.innerHTML = `
          <td>${group.groupName}</td>
          <td>${participant.fullName || "-"}</td>
          <td>${participant.mobile || "-"}</td>
          <td>${dateAdded}</td>
          <td>${endDate}</td>
          <td>
            <button class="btn btn-sm btn-warning finish-btn" data-group="${group.groupName}" data-id="${participant.id}">Finish</button>
            <button class="btn btn-sm btn-info equipment-btn" data-group="${group.groupName}" data-id="${participant.id}">Equipment</button>
          </td>
          <td><input type="checkbox" class="participant-checkbox" data-group="${group.groupName}" data-id="${participant.id}"></td>
        `;
                tbody.appendChild(tr);
            });
        } else {
            const tr = document.createElement("tr");
            tr.innerHTML = `<td colspan="7" class="fst-italic text-muted">No participants in this group.</td>`;
            tbody.appendChild(tr);
        }
    });
}

// Загрузка списка пустых групп для удаления
async function loadEmptyGroups(projectId) {
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups/emptygroups`, { cache: 'no-cache' });
        if (response.ok) {
            const emptyGroups = await response.json();
            renderEmptyGroups(emptyGroups);
        } else {
            console.error("Error fetching empty groups: " + response.statusText);
        }
    } catch (error) {
        console.error("Error fetching empty groups:", error);
    }
}

// Отрисовка пустых групп в контейнере для удаления
function renderEmptyGroups(groups) {
    const container = document.getElementById("emptyGroupsContainer");
    if (!container) {
        console.error("Element with id 'emptyGroupsContainer' not found.");
        return;
    }
    container.innerHTML = "";
    if (!groups || groups.length === 0) {
        container.innerHTML = "<p>No empty groups found.</p>";
        return;
    }
    groups.forEach(group => {
        const div = document.createElement("div");
        div.classList.add("form-check");
        div.innerHTML = `<input class="form-check-input delete-group-checkbox" type="checkbox" value="${group.groupName}" id="deleteGroup_${group.groupName}">
                           <label class="form-check-label" for="deleteGroup_${group.groupName}">${group.groupName}</label>`;
        container.appendChild(div);
    });
}

// Удаление выбранных пустых групп через API
async function deleteGroups() {
    const container = document.getElementById('emptyGroupsContainer');
    const checkboxes = container.querySelectorAll('input.delete-group-checkbox:checked');
    if (checkboxes.length === 0) {
        alert("Please select at least one group to delete.");
        return;
    }
    const projectId = getCurrentProjectId();
    if (!projectId) {
        alert("Project ID not found.");
        return;
    }
    const groupsToDelete = Array.from(checkboxes).map(cb => cb.value);
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups/emptygroups`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ projectId: parseInt(projectId), groups: groupsToDelete })
        });
        if (response.ok) {
            alert("Selected groups deleted successfully!");
            const deleteGroupModal = bootstrap.Modal.getInstance(document.getElementById('deleteGroupModal'));
            if (deleteGroupModal) deleteGroupModal.hide();
            loadEmptyGroups(projectId);
            loadParticipantGroups(projectId);
        } else {
            const error = await response.text();
            alert("Error deleting groups: " + error);
        }
    } catch (error) {
        console.error("Error deleting groups:", error);
        alert("Error deleting groups. See console for details.");
    }
}

// Создание группы через API
async function createGroup() {
    const groupNameInput = document.getElementById('groupName');
    if (!groupNameInput || !groupNameInput.value.trim()) {
        alert("Please enter a group name.");
        return;
    }
    const groupName = groupNameInput.value.trim();
    const projectId = getCurrentProjectId();
    if (!projectId) {
        alert("Project ID not found.");
        return;
    }
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ projectId: parseInt(projectId), groupName: groupName })
        });
        if (response.ok) {
            alert("Group created successfully!");
            const addGroupModal = bootstrap.Modal.getInstance(document.getElementById('addGroupModal'));
            if (addGroupModal) addGroupModal.hide();
            loadParticipantGroups(projectId);
            loadEmptyGroups(projectId);
        } else {
            const error = await response.text();
            if (response.status === 409) {
                alert("A group with the same name already exists for this project.");
            } else {
                alert("Error creating group: " + error);
            }
        }
    } catch (error) {
        console.error("Error creating group:", error);
        alert("Error creating group. See console for details.");
    }
}

// ----------------- Функционал добавления участников (Клиентская фильтрация) -----------------

// Загрузка полного списка свободных пользователей с сервера (без фильтрации)
async function loadAllAvailableUsers() {
    const url = `https://localhost:7200/api/Users/available?t=${new Date().getTime()}`;
    try {
        const response = await fetch(url, { cache: 'no-cache' });
        if (response.ok) {
            allAvailableUsers = await response.json();
            console.log("All available users loaded:", allAvailableUsers);
        } else {
            console.error("Error loading available users:", response.statusText);
        }
    } catch (error) {
        console.error("Error loading available users:", error);
    }
}

// Функция отображения пользователей в таблице на основе переданного списка
function renderAvailableUsers(users) {
    const tbody = document.getElementById('availableUsersTableBody');
    if (!tbody) {
        console.error("Element with id 'availableUsersTableBody' not found.");
        return;
    }
    tbody.innerHTML = "";
    if (users.length === 0) {
        tbody.innerHTML = "<tr><td colspan='5'>No users found.</td></tr>";
        return;
    }
    users.forEach(user => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td><input type="checkbox" class="available-user-checkbox" data-id="${user.id}"></td>
            <td>${user.fullName}</td>
            <td>${user.mobile}</td>
            <td>${user.email}</td>
            <td>${user.roleName}</td>
        `;
        tbody.appendChild(row);
    });
}

// Функция, которая фильтрует список пользователей на клиенте по поиску и выбранной роли
function applyClientSideFilters() {
    const searchInput = document.getElementById('userSearchInput');
    const roleSelect = document.getElementById('participantRoleFilterAdd');
    const searchTerm = searchInput ? searchInput.value.trim().toLowerCase() : "";
    const roleFilter = roleSelect ? roleSelect.value : "all";

    // Фильтрация по поиску: ищем совпадения в fullName, email или mobile
    let filtered = allAvailableUsers.filter(user => {
        const fullName = user.fullName ? user.fullName.toLowerCase() : "";
        const email = user.email ? user.email.toLowerCase() : "";
        const mobile = user.mobile ? user.mobile.toLowerCase() : "";
        return fullName.includes(searchTerm) || email.includes(searchTerm) || mobile.includes(searchTerm);
    });

    // Если выбран конкретный фильтр по роли (не "all"), фильтруем по нему
    if (roleFilter !== "all") {
        filtered = filtered.filter(user => String(user.roleId) === roleFilter);
    }
    console.log("Filtered users:", filtered);
    renderAvailableUsers(filtered);
}

// Обновленная функция загрузки свободных пользователей для клиента (без проверки allAvailableUsers.length)
async function loadAvailableUsers() {
    await loadAllAvailableUsers();
    applyClientSideFilters();
}

// Загрузка ролей для фильтра в модальном окне "Add Participant" (без изменений)
async function loadParticipantRoles() {
    try {
        const response = await fetch('https://localhost:7200/api/Roles?t=' + new Date().getTime(), { cache: 'no-cache' });
        if (response.ok) {
            const roles = await response.json();
            const roleSelect = document.getElementById('participantRoleFilterAdd');
            if (roleSelect) {
                roleSelect.innerHTML = '<option value="all">All Roles</option>';
                roles.forEach(role => {
                    const option = document.createElement('option');
                    option.value = role.id;
                    option.textContent = role.name;
                    roleSelect.appendChild(option);
                });
            }
        } else {
            console.error("Error loading roles:", response.statusText);
        }
    } catch (error) {
        console.error("Error loading roles:", error);
    }
}

// Загрузка групп проекта для заполнения выпадающего списка (без изменений)
async function loadProjectGroupsForAddParticipant() {
    const projectId = getCurrentProjectId();
    if (!projectId) {
        console.error("Project ID not found for loading groups");
        return;
    }
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups?t=${new Date().getTime()}`, { cache: 'no-cache' });
        if (response.ok) {
            const groups = await response.json();
            const selectGroup = document.getElementById('selectGroup');
            if (selectGroup) {
                selectGroup.innerHTML = `<option value="">-- No Group (just project) --</option>`;
                groups.forEach(group => {
                    const option = document.createElement('option');
                    option.value = group.id;
                    option.textContent = group.groupName;
                    selectGroup.appendChild(option);
                });
            }
        } else {
            console.error("Error loading project groups:", response.statusText);
        }
    } catch (err) {
        console.error("Error loading project groups: ", err);
    }
}

// Функция добавления выбранных участников; если группа не выбрана – groupId будет null
async function addSelectedParticipants() {
    const projectId = getCurrentProjectId();
    if (!projectId) {
        alert("Project ID not found.");
        return;
    }
    const selectGroupEl = document.getElementById('selectGroup');
    if (!selectGroupEl) {
        alert("Select Group element not found.");
        return;
    }
    const groupValue = selectGroupEl.value;
    const groupId = groupValue ? parseInt(groupValue) : null;
    // Собираем выбранные userIds в массив
    const checkboxes = document.querySelectorAll('#availableUsersTableBody .available-user-checkbox:checked');
    if (checkboxes.length === 0) {
        alert("Please select at least one user to add.");
        return;
    }
    const userIds = Array.from(checkboxes).map(cb => parseInt(cb.getAttribute('data-id')));

    // Получаем значения daily rate и meter rate
    const dailyRateInput = document.getElementById('dailyRate');
    const meterRateInput = document.getElementById('meterRate');
    const dailyRate = dailyRateInput && dailyRateInput.value.trim() !== "" ? parseFloat(dailyRateInput.value) : null;
    const meterRate = meterRateInput && meterRateInput.value.trim() !== "" ? parseFloat(meterRateInput.value) : null;

    const requestBody = {
        projectId: parseInt(projectId),
        groupId: groupId,
        UserIds: userIds,
        dailyRate: dailyRate,
        meterRate: meterRate
    };
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Participants`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestBody)
        });
        if (response.ok) {
            alert("Participants added successfully!");
            // Обновляем группы участников
            loadParticipantGroups(projectId);
            // Обновляем список доступных пользователей после добавления
            await loadAllAvailableUsers();
            applyClientSideFilters();
            // Окно остаётся открытым (если так требуется)
        } else {
            const error = await response.text();
            alert("Error adding participants: " + error);
        }
    } catch (error) {
        console.error("Error adding participants:", error);
        alert("Error adding participants. See console for details.");
    }
}

// ----------------- Инициализация модального окна "Add Participant" -----------------

// Инициализация модального окна "Add Participant" при его открытии (событие show.bs.modal)
// При каждом открытии загружаются fresh данные с сервера (через loadAllAvailableUsers)
function initAddParticipantModalEvents() {
    const addParticipantModalEl = document.getElementById('addParticipantModal');
    if (!addParticipantModalEl) {
        console.error("Add Participant modal not found.");
        return;
    }
    // При открытии модального окна очищаем глобальную переменную, чтобы при следующем открытии данные загрузились заново
    allAvailableUsers = [];
    // Загружаем список пользователей, ролей и групп
    loadAvailableUsers();
    loadParticipantRoles();
    loadProjectGroupsForAddParticipant();

    // Навешиваем обработчики для обновления списка при изменении фильтров (если еще не инициализированы)
    const searchInput = document.getElementById('userSearchInput');
    if (searchInput && !searchInput.dataset.initialized) {
        searchInput.addEventListener('input', applyClientSideFilters);
        searchInput.dataset.initialized = "true";
    }
    const roleSelect = document.getElementById('participantRoleFilterAdd');
    if (roleSelect && !roleSelect.dataset.initialized) {
        roleSelect.addEventListener('change', applyClientSideFilters);
        roleSelect.dataset.initialized = "true";
    }
    const addSelectedBtn = document.getElementById('addSelectedParticipantsBtn');
    if (addSelectedBtn && !addSelectedBtn.dataset.initialized) {
        addSelectedBtn.onclick = addSelectedParticipants;
        addSelectedBtn.dataset.initialized = "true";
    }
}

// Инициализация вкладки Participants (старый функционал групп)
function initParticipantsTab() {
    const projectId = getCurrentProjectId();
    if (!projectId) {
        console.warn("Project ID is not defined. Participants tab will be empty.");
        return;
    }
    loadParticipantGroups(projectId);
}

// Навешивание событий для вложенных модальных окон (Add/Delete Group)
function initNestedModalEvents() {
    const addGroupButton = document.getElementById('addGroupButton');
    if (addGroupButton) {
        addGroupButton.addEventListener('click', function (e) {
            e.preventDefault();
            const addGroupModalEl = document.getElementById('addGroupModal');
            if (addGroupModalEl) {
                const addGroupModal = new bootstrap.Modal(addGroupModalEl, { backdrop: 'static', keyboard: false });
                addGroupModal.show();
                const templateButtons = addGroupModalEl.querySelectorAll('.template-btn');
                templateButtons.forEach(btn => {
                    btn.addEventListener('click', function () {
                        const template = this.getAttribute('data-template');
                        const groupNameInput = document.getElementById('groupName');
                        if (groupNameInput) {
                            groupNameInput.value = template;
                        }
                    });
                });
                const saveGroupBtn = document.getElementById('saveGroupBtn');
                if (saveGroupBtn) {
                    saveGroupBtn.onclick = createGroup;
                } else {
                    console.error("Save Group button not found.");
                }
            }
        });
    } else {
        console.error("Add Group button not found.");
    }
    const deleteGroupButton = document.getElementById('deleteGroupButton');
    if (deleteGroupButton) {
        deleteGroupButton.addEventListener('click', function (e) {
            e.preventDefault();
            const projectId = getCurrentProjectId();
            if (!projectId) {
                alert("Project ID not found.");
                return;
            }
            loadEmptyGroups(projectId);
            const deleteGroupModalEl = document.getElementById('deleteGroupModal');
            if (deleteGroupModalEl) {
                const deleteGroupModal = new bootstrap.Modal(deleteGroupModalEl, { backdrop: 'static', keyboard: false });
                deleteGroupModal.show();
                const confirmDeleteGroupsBtn = document.getElementById('confirmDeleteGroupsBtn');
                if (confirmDeleteGroupsBtn) {
                    confirmDeleteGroupsBtn.onclick = deleteGroups;
                } else {
                    console.error("Confirm Delete Groups button not found.");
                }
            }
        });
    } else {
        console.error("Delete Group button not found.");
    }
}

document.addEventListener('shown.bs.tab', function (event) {
    if (event.target && event.target.id === 'participants-tab') {
        initParticipantsTab();
        initNestedModalEvents();
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const addParticipantModalEl = document.getElementById('addParticipantModal');
    if (addParticipantModalEl) {
        addParticipantModalEl.addEventListener('show.bs.modal', initAddParticipantModalEvents);
        // При закрытии модального окна очищаем список, чтобы при следующем открытии данные обновились
        addParticipantModalEl.addEventListener('hidden.bs.modal', function () {
            allAvailableUsers = [];
        });
    }
});

window.initParticipantsTab = initParticipantsTab;
