// wwwroot/js/participants.js

// Токен не передается через скрипт – он хранится в куки

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

// Функция загрузки списка пустых групп для удаления (используется URL /emptygroups)
async function loadEmptyGroups(projectId) {
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups/emptygroups`);
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

// Функция отрисовки пустых групп в контейнере для удаления
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
        // Для каждой пустой группы создаём чекбокс
        const div = document.createElement("div");
        div.classList.add("form-check");
        div.innerHTML = `<input class="form-check-input delete-group-checkbox" type="checkbox" value="${group.groupName}" id="deleteGroup_${group.groupName}">
                           <label class="form-check-label" for="deleteGroup_${group.groupName}">${group.groupName}</label>`;
        container.appendChild(div);
    });
}

// Функция удаления выбранных пустых групп через API
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
    // Собираем список имен групп для удаления
    const groupsToDelete = Array.from(checkboxes).map(cb => cb.value);
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups/emptygroups`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' },
            // Передаём в теле и projectId, и список групп
            body: JSON.stringify({ projectId: parseInt(projectId), groups: groupsToDelete })
        });
        if (response.ok) {
            alert("Selected groups deleted successfully!");
            // Закрываем модальное окно удаления группы
            const deleteGroupModal = bootstrap.Modal.getInstance(document.getElementById('deleteGroupModal'));
            if (deleteGroupModal) deleteGroupModal.hide();
            // Обновляем списки
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
            const addGroupModal = bootstrap.Modal.getInstance(document.getElementById('addGroupModal'));
            if (addGroupModal) addGroupModal.hide();
            loadParticipantGroups(projectId);
            loadEmptyGroups(projectId);
        } else {
            const error = await response.text();
            // Если сервер возвращает конфликт, выводим красивое сообщение
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

// Инициализация вкладки Participants
function initParticipantsTab() {
    const projectId = getCurrentProjectId();
    if (!projectId) {
        console.warn("Project ID is not defined. Participants tab will be empty.");
        return;
    }
    loadParticipantGroups(projectId);
}

// Инициализация вложенных модальных окон (для добавления/удаления групп)
function initNestedModalEvents() {
    // Обработчик для кнопки "Add Group"
    const addGroupButton = document.getElementById('addGroupButton');
    if (addGroupButton) {
        addGroupButton.addEventListener('click', function (e) {
            e.preventDefault();
            const addGroupModalEl = document.getElementById('addGroupModal');
            if (addGroupModalEl) {
                new bootstrap.Modal(addGroupModalEl, { backdrop: 'static', keyboard: false }).show();
                // Назначаем обработчик для кнопок шаблонов внутри модального окна
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

    // Обработчик для кнопки "Delete Group"
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
                new bootstrap.Modal(deleteGroupModalEl, { backdrop: 'static', keyboard: false }).show();
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

window.initParticipantsTab = initParticipantsTab;
