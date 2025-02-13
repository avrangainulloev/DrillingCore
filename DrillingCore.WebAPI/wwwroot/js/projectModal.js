// wwwroot/js/projectModal.js

// Функция загрузки данных проекта для редактирования
async function loadProjectData(projectId) {
    console.log("Loading project data for ID:", projectId);
    try {
        const response = await fetch(`https://localhost:7200/api/Projects/${projectId}`, {
            headers: { 'Authorization': 'Bearer ' + window.yourJwtToken }
        });
        if (response.ok) {
            const project = await response.json();

            // Получаем элементы формы
            const idEl = document.getElementById('projectId');
            const nameEl = document.getElementById('projectName');
            const startDateEl = document.getElementById('startDate');
            const endDateEl = document.getElementById('endDate');
            const clientEl = document.getElementById('client');
            const hasCampEl = document.getElementById('hasCampOrHotel');
            const modalLabelEl = document.getElementById('projectModalLabel');

            if (!idEl || !nameEl || !startDateEl || !endDateEl || !clientEl || !hasCampEl || !modalLabelEl) {
                console.error("One or more form elements not found. Check IDs in your modal HTML.");
                return;
            }

            // Заполняем поля формы
            idEl.value = project.id;
            nameEl.value = project.name;
            startDateEl.value = project.startDate ? project.startDate.split('T')[0] : "";
            endDateEl.value = project.endDate ? project.endDate.split('T')[0] : "";
            clientEl.value = project.client;
            hasCampEl.checked = project.hasCampOrHotel;
            modalLabelEl.textContent = "Edit Project";
        } else {
            alert("Error fetching project data: " + response.statusText);
            console.error("Response status:", response.status);
        }
    } catch (error) {
        console.error("Error fetching project data:", error);
        alert("Error fetching project data. See console for details.");
    }
}

// Функция открытия модального окна для создания/редактирования проекта
async function openProjectModal(projectId) {
    if (projectId) {
        await loadProjectData(projectId);
    } else {
        const form = document.getElementById('projectInfoForm');
        if (form) {
            form.reset();
        } else {
            console.error("Element with id 'projectInfoForm' not found");
        }
        document.getElementById('projectId').value = "";
        document.getElementById('projectModalLabel').textContent = "Create New Project";
    }
    const modalEl = document.getElementById('projectModal');
    if (modalEl) {
        new bootstrap.Modal(modalEl).show();
    } else {
        console.error("Element with id 'projectModal' not found");
    }
}

// Функция отправки формы (создание или обновление проекта)
async function submitProject() {
    const projectId = document.getElementById('projectId').value;
    const projectData = {
        ...(projectId && { id: parseInt(projectId) }),
        name: document.getElementById('projectName').value,
        startDate: document.getElementById('startDate').value,
        endDate: document.getElementById('endDate').value || null,
        client: document.getElementById('client').value,
        hasCampOrHotel: document.getElementById('hasCampOrHotel').checked
    };

    let method = projectId ? 'PUT' : 'POST';
    let url = projectId ? `https://localhost:7200/api/Projects/${projectId}` : 'https://localhost:7200/api/Projects';

    try {
        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + window.yourJwtToken
            },
            body: JSON.stringify(projectData)
        });
        if (response.ok) {
            alert('Project saved successfully!');
            closeProjectModal();
            if (typeof loadProjects === "function") {
                loadProjects();
            }
        } else {
            const error = await response.text();
            alert('Error saving project: ' + error);
        }
    } catch (err) {
        console.error("Error saving project:", err);
        alert('An error occurred while saving the project.');
    }
}

// Функция закрытия модального окна
function closeProjectModal() {
    const modalEl = document.getElementById('projectModal');
    if (modalEl) {
        bootstrap.Modal.getInstance(modalEl).hide();
        window.history.replaceState({}, document.title, "dashboard.html");
    }
}

window.openProjectModal = openProjectModal;
