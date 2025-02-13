// wwwroot/js/participants.js

// Токен не передаётся – он хранится в куках

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

// Загрузка групп участников для проекта через API
async function loadParticipantGroups(projectId) {
    console.log("Loading participant groups for project ID:", projectId);
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups`);
        if (response.ok) {
            const groups = await response.json();
            console.log("Loaded participant groups:", groups);
            renderParticipantGroups(groups);
            updateEmptyGroupsList();
            // Сохраняем группы глобально для использования в удалении (если требуется)
            window.projectGroupsArray = groups;
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
        // Строка-заголовок группы
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
    attachParticipantSearchHandler();
}

// Обработчик поиска участников по имени (фильтрация)
function attachParticipantSearchHandler() {
    const searchInput = document.getElementById('participantSearchInput');
    if (!searchInput) return;
    searchInput.addEventListener('input', function () {
        const filter = this.value.toLowerCase();
        const tbody = document.getElementById("participantsTableBody");
        const rows = tbody.getElementsByTagName("tr");
        for (let row of rows) {
            if (row.classList.contains('group-header')) continue;
            const fullNameCell = row.cells[1];
            if (fullNameCell) {
                const txtValue = fullNameCell.textContent || fullNameCell.innerText;
                row.style.display = txtValue.toLowerCase().indexOf(filter) > -1 ? "" : "none";
            }
        }
    });
}

// Обновление списка пустых групп для модального окна удаления
function updateEmptyGroupsList() {
    const container = document.getElementById('emptyGroupsContainer');
    if (!container) {
        console.error("Element with id 'emptyGroupsContainer' not found.");
        return;
    }
    container.innerHTML = "";
    if (window.projectGroupsArray && window.projectGroupsArray.length > 0) {
        window.projectGroupsArray.forEach(group => {
            if (!group.participants || group.participants.length === 0) {
                const div = document.createElement("div");
                div.classList.add("form-check");
                div.innerHTML = `<input class="form-check-input delete-group-checkbox" type="checkbox" value="${group.groupName}" id="deleteGroup_${group.groupName}">
                                   <label class="form-check-label" for="deleteGroup_${group.groupName}">${group.groupName}</label>`;
                container.appendChild(div);
            }
        });
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

// Инициализация вложённых модальных окон (Add Group и Delete Group)
function initNestedModalEvents() {
    // Кнопка "Add Group"
    const addGroupButton = document.getElementById('addGroupButton');
    if (addGroupButton) {
        addGroupButton.addEventListener('click', function (e) {
            e.preventDefault();
            const addGroupModalEl = document.getElementById('addGroupModal');
            if (addGroupModalEl) {
                new bootstrap.Modal(addGroupModalEl, { backdrop: 'static', keyboard: false }).show();
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
    // Кнопка "Delete Group"
    const deleteGroupButton = document.getElementById('deleteGroupButton');
    if (deleteGroupButton) {
        deleteGroupButton.addEventListener('click', function (e) {
            e.preventDefault();
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

// При переключении на вкладку Participants – инициализируем вкладку и вложённые модальные окна
document.addEventListener('shown.bs.tab', function (event) {
    if (event.target && event.target.id === 'participants-tab') {
        initParticipantsTab();
        initNestedModalEvents();
    }
});

window.initParticipantsTab = initParticipantsTab;

// Функция создания группы через API
async function createGroup() {
    const groupNameInput = document.getElementById('groupName');
    if (!groupNameInput || !groupNameInput.value.trim()) {
        displayGroupError("Please enter a group name.");
        return;
    }
    const groupName = groupNameInput.value.trim();
    const projectId = getCurrentProjectId();
    if (!projectId) {
        displayGroupError("Project ID not found.");
        return;
    }
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ projectId: parseInt(projectId), groupName: groupName })
        });
        if (response.ok) {
            // Если успешно, закрываем модальное окно и обновляем список групп
            const addGroupModalEl = document.getElementById('addGroupModal');
            const modalInstance = bootstrap.Modal.getInstance(addGroupModalEl);
            if (modalInstance) modalInstance.hide();
            loadParticipantGroups(projectId);
        } else if (response.status === 409) {
            // Если группа уже существует, выводим сообщение
            displayGroupError("A group with this name already exists.");
        } else {
            const error = await response.text();
            displayGroupError("Error creating group: " + error);
        }
    } catch (error) {
        console.error("Error creating group:", error);
        displayGroupError("Error creating group. See console for details.");
    }
}

// Функция для вывода сообщения об ошибке в модальном окне Add Group
function displayGroupError(message) {
    const container = document.getElementById('groupErrorContainer');
    if (container) {
        container.innerHTML = `<div class="alert alert-danger" role="alert">${message}</div>`;
        // Скрыть сообщение через 5 секунд
        setTimeout(() => { container.innerHTML = ""; }, 10000);
    } else {
        alert(message);
    }
}

 

// Функция удаления групп через API
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
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}/Groups`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ groups: groupsToDelete })
        });
        if (response.ok) {
            alert("Selected groups deleted successfully!");
            const deleteGroupModalEl = document.getElementById('deleteGroupModal');
            if (deleteGroupModalEl) {
                const modalInstance = bootstrap.Modal.getInstance(deleteGroupModalEl);
                if (modalInstance) modalInstance.hide();
            }
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

document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('template-btn')) {
            const template = e.target.getAttribute('data-template');
            const groupNameField = document.getElementById('groupName');
            if (groupNameField) {
                groupNameField.value = template;
            } else {
                console.error("Element with id 'groupName' not found.");
            }
        }
    });
});
