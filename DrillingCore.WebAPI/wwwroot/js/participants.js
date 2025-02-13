// wwwroot/js/participants.js

// Токен больше не передается через скрипт – он хранится в куках

// Получаем текущий ID проекта из скрытого поля внутри модального окна
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

// Функция загрузки групп участников для проекта через API
async function loadParticipantGroups(projectId) {
    console.log("Loading participant groups for project ID:", projectId);
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups`);
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

// Функция отрисовки групп и участников в таблице
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
        // Добавляем строку-заголовок группы
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

// Функция создания группы через API
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
            // Закрываем модальное окно Add Group
            const addGroupModal = bootstrap.Modal.getInstance(document.getElementById('addGroupModal'));
            if (addGroupModal) addGroupModal.hide();
            // Обновляем список групп
            loadParticipantGroups(projectId);
        } else {
            const error = await response.text();
            alert("Error creating group: " + error);
        }
    } catch (error) {
        console.error("Error creating group:", error);
        alert("Error creating group. See console for details.");
    }
}

// Функция удаления группы через API (например, для пустых групп)
async function deleteGroups() {
    // Предполагается, что в контейнере emptyGroupsContainer находятся чекбоксы с value равным имени группы
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
    // Собираем список групп для удаления
    const groupsToDelete = Array.from(checkboxes).map(cb => cb.value);
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ groups: groupsToDelete })
        });
        if (response.ok) {
            alert("Selected groups deleted successfully!");
            // Закрываем модальное окно Delete Group
            const deleteGroupModal = bootstrap.Modal.getInstance(document.getElementById('deleteGroupModal'));
            if (deleteGroupModal) deleteGroupModal.hide();
            // Обновляем список групп
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

// Функция инициализации вкладки Participants
function initParticipantsTab() {
    const projectId = getCurrentProjectId();
    if (!projectId) {
        console.warn("Project ID is not defined. Participants tab will be empty.");
        return;
    }
    loadParticipantGroups(projectId);
}

// Обработчик события переключения вкладок – при открытии вкладки Participants
document.addEventListener('shown.bs.tab', function (event) {
    if (event.target && event.target.id === 'participants-tab') {
        initParticipantsTab();
    }
});

// Назначаем обработчик для кнопки Save Group
document.addEventListener('DOMContentLoaded', function () {
    const saveGroupBtn = document.getElementById('saveGroupBtn');
    if (saveGroupBtn) {
        saveGroupBtn.addEventListener('click', createGroup);
    } else {
        console.error("Save Group button not found.");
    }
    // Назначьте аналогично обработчик для удаления группы
    const confirmDeleteGroupsBtn = document.getElementById('confirmDeleteGroupsBtn');
    if (confirmDeleteGroupsBtn) {
        confirmDeleteGroupsBtn.addEventListener('click', deleteGroups);
    } else {
        console.error("Confirm Delete Groups button not found.");
    }
});

window.initParticipantsTab = initParticipantsTab;
