// wwwroot/js/users.js

// Открытие модального окна для создания/редактирования пользователя
function openUserModal(userId) {
    const modalEl = document.getElementById('userModal');
    if (!modalEl) {
        console.error("User modal not found");
        return;
    }
    if (userId) {
        loadUserData(userId);
        document.getElementById('userModalLabel').textContent = "Edit User";
    } else {
        const form = document.getElementById('userForm');
        if (form) { form.reset(); }
        document.getElementById('userId').value = "";
        document.getElementById('userModalLabel').textContent = "Create User";
    }
    new bootstrap.Modal(modalEl, { backdrop: 'static', keyboard: false }).show();
}

// Загрузка данных пользователя для редактирования
async function loadUserData(userId) {
    try {
        const response = await fetch(`https://localhost:7200/api/Users/${userId}`);
        if (response.ok) {
            const user = await response.json();
            document.getElementById('userId').value = user.id;
            document.getElementById('username').value = user.username;
            // Поле пароля не заполняется для безопасности
            document.getElementById('fullName').value = user.fullName;
            document.getElementById('email').value = user.email;
            document.getElementById('mobile').value = user.mobile;
            document.getElementById('roleSelect').value = user.roleId;
            document.getElementById('statusSelect').value = user.isActive ? "true" : "false";
        } else {
            alert("Error fetching user data: " + response.statusText);
        }
    } catch (error) {
        console.error("Error fetching user data:", error);
        alert("Error fetching user data. See console for details.");
    }
}

// Загрузка списка пользователей с фильтрами
async function loadUsers() {
    const searchTerm = document.getElementById('userSearchInput')?.value || "";
    const roleId = document.getElementById('roleFilter')?.value || "";
    const statusFilter = document.getElementById('statusFilter')?.value || "all";

    let url = `https://localhost:7200/api/Users?searchTerm=${encodeURIComponent(searchTerm)}`;
    if (roleId) { url += `&roleId=${roleId}`; }
    if (statusFilter && statusFilter !== "all") {
        url += `&isActive=${statusFilter === "active" ? "true" : "false"}`;
    }

    try {
        const response = await fetch(url); // Токен передается через куки
        if (response.ok) {
            const users = await response.json();
            const tbody = document.getElementById('usersTableBody');
            if (!tbody) {
                console.error("Element with id 'usersTableBody' not found.");
                return;
            }
            tbody.innerHTML = "";
            users.forEach(user => {
                const statusText = user.isActive ? "Active" : "Inactive";
                const statusColor = user.isActive ? "green" : "red";
                const row = document.createElement('tr');
                row.innerHTML = `
          <td>${user.id}</td>
          <td>${user.username}</td>
          <td>${user.fullName}</td>
          <td>${user.email}</td>
          <td>${user.mobile}</td>
          <td>${user.roleName}</td>
          <td style="color: ${statusColor};">${statusText}</td>
          <td>
            <button class="btn btn-sm btn-primary" onclick="openUserModal(${user.id})">Edit</button>
          </td>
        `;
                tbody.appendChild(row);
            });
        } else {
            console.error("Error loading users:", response.statusText);
        }
    } catch (err) {
        console.error("Error loading users:", err);
    }
}

// Функция отправки формы для создания/редактирования пользователя
async function submitUser() {
    const userId = document.getElementById('userId').value;
    const userData = {
        id: userId ? parseInt(userId) : 0,
        username: document.getElementById('username').value,
        password: document.getElementById('password').value, // Если пустое, не изменяем (для редактирования)
        fullName: document.getElementById('fullName').value,
        email: document.getElementById('email').value,
        mobile: document.getElementById('mobile').value,
        roleId: parseInt(document.getElementById('roleSelect').value),
        isActive: document.getElementById('statusSelect').value === "true"
    };

    let method = userId ? 'PUT' : 'POST';
    let url = 'https://localhost:7200/api/Users';
    if (userId) { url += `/${userId}`; }

    try {
        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        });
        if (response.ok) {
            alert("User saved successfully!");
            const modalEl = document.getElementById('userModal');
            if (modalEl) { bootstrap.Modal.getInstance(modalEl)?.hide(); }
            loadUsers();
        } else {
            const error = await response.text();
            alert("Error saving user: " + error);
        }
    } catch (error) {
        console.error("Error saving user:", error);
        alert("Error saving user. See console for details.");
    }
}

window.loadUsers = loadUsers;
window.openUserModal = openUserModal;
window.submitUser = submitUser;
