// wwwroot/js/dashboard.js

const yourJwtToken = "YOUR_JWT_TOKEN_HERE"; // Для разработки; в продакшене токен хранится в куках

// Функция загрузки проектов через API
async function loadProjects() {
    try {
        const response = await fetch('https://localhost:7200/api/Projects?limit=30', {
            headers: { 'Authorization': 'Bearer ' + yourJwtToken }
        });
        if (response.ok) {
            const projects = await response.json();
            const tbody = document.querySelector('#projectsTableBody');
            tbody.innerHTML = "";
            projects.forEach(project => {
                const row = document.createElement('tr');
                row.innerHTML = `
          <td>${project.id}</td>
          <td>${project.name}</td>
          <td>${project.client}</td>
          <td>${project.startDate ? project.startDate.split('T')[0] : ''}</td>
          <td>${project.endDate ? project.endDate.split('T')[0] : ''}</td>
          <td>
            <button class="btn btn-sm btn-primary" onclick="openProjectModal(${project.id})">Edit</button>
          </td>
        `;
                tbody.appendChild(row);
            });
        } else {
            console.error("Error loading projects: " + response.statusText);
        }
    } catch (err) {
        console.error("Error loading projects:", err);
    }
}

// Функция переключения содержимого
function loadContent(section) {
    const contentArea = document.getElementById('contentArea');
    if (section === 'projects') {
        contentArea.innerHTML = `
      <h1>Projects</h1>
      <button class="btn btn-primary mb-3" onclick="openProjectModal()">Create Project</button>
      <table class="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Client</th>
            <th>Start Date</th>
            <th>End Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody id="projectsTableBody"></tbody>
      </table>
    `;
        loadProjects();
    } else if (section === 'equipment') {
        contentArea.innerHTML = '<h1>Equipment</h1><p>This is the equipment section.</p>';
    } else if (section === 'users') {
        contentArea.innerHTML = '<h1>Users</h1><p>This is the users section.</p>';
    } else if (section === 'forms') {
        contentArea.innerHTML = '<h1>Forms</h1><p>This is the forms section.</p>';
    } else if (section === 'reports') {
        contentArea.innerHTML = '<h1>Reports</h1><p>This is the reports section.</p>';
    }
}

// Функция загрузки модального окна (HTML из отдельного файла)
async function loadProjectModal() {
    try {
        const response = await fetch('modals/projectModal.html');
        if (response.ok) {
            const modalHtml = await response.text();
            document.body.insertAdjacentHTML('beforeend', modalHtml);
        } else {
            console.error('Error loading project modal:', response.statusText);
        }
    } catch (err) {
        console.error('Error loading project modal:', err);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    loadContent('projects');
    loadProjectModal();
});
