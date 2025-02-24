// wwwroot/js/dashboard.js

// Для разработки можно захардкодить токен, но в продакшене сервер будет работать через куки
const yourJwtToken = "YOUR_JWT_TOKEN_HERE";

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
        // HTML для раздела "Users" загружается здесь. Он вызывает функцию loadUsers() из users.js
        contentArea.innerHTML = `
      <h1>Users</h1>
      <!-- Панель фильтрации и поиска -->
      <div class="row mb-3">
        <div class="col-md-4">
          <input type="text" id="userSearchInput" class="form-control" placeholder="Search by name, username, or email">
        </div>
        <div class="col-md-4">
          <select id="roleFilter" class="form-select">
            <option value="">All Roles</option>
            <option value="-1">Administrator</option>
            <option value="-2">ProjectManager</option>
            <option value="-3">Helper</option>
          </select>
        </div>
        <div class="col-md-4 text-end">
          <button class="btn btn-primary" onclick="openUserModal()">Add User</button>
        </div>
      </div>
      <table class="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Username</th>
            <th>Full Name</th>
            <th>Email</th>
            <th>Mobile</th>
            <th>Role</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody id="usersTableBody">
          <!-- Пользователи будут загружаться через users.js -->
        </tbody>
      </table>
    `;
        loadUsers(); // Функция из users.js
        // Вешаем события для фильтрации и поиска
        document.getElementById('userSearchInput').addEventListener('input', loadUsers);
        document.getElementById('roleFilter').addEventListener('change', loadUsers);
    } else if (section === 'forms') {
        contentArea.innerHTML = '<h1>Forms</h1><p>This is the forms section.</p>';
    } else if (section === 'reports') {
        contentArea.innerHTML = '<h1>Reports</h1><p>This is the reports section.</p>';
    }
}

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

// Экспорт JWT-токена в глобальную область, если потребуется (например, для старых вызовов)
window.yourJwtToken = yourJwtToken;
window.loadContent = loadContent;
