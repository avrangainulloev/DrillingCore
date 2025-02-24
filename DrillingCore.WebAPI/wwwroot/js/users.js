// wwwroot/js/users.js

async function loadUsers() {
    // Получаем значение для поиска и выбранную роль из фильтров
    const searchTerm = document.getElementById('userSearchInput')?.value || "";
    const roleId = document.getElementById('roleFilter')?.value || "";

    let url = `https://localhost:7200/api/Users?searchTerm=${encodeURIComponent(searchTerm)}`;
    if (roleId) {
        url += `&roleId=${roleId}`;
    }

    try {
        const response = await fetch(url); // Токен в куках – заголовок не нужен
        if (response.ok) {
            const users = await response.json();
            const tbody = document.getElementById('usersTableBody');
            if (!tbody) {
                console.error("Element with id 'usersTableBody' not found.");
                return;
            }
            tbody.innerHTML = "";
            users.forEach(user => {
                const row = document.createElement('tr');
                row.innerHTML = `
          <td>${user.id}</td>
          <td>${user.username}</td>
          <td>${user.fullName}</td>
          <td>${user.email}</td>
          <td>${user.mobile}</td>
          <td>${user.roleName}</td>
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

// Если нужно, можно добавить функцию для открытия модального окна редактирования пользователя
function openUserModal(userId) {
    // Реализуйте открытие модального окна для добавления/редактирования пользователя.
    // Например, можно динамически подгрузить HTML из modals/userModal.html.
    alert("Open User Modal for user ID: " + userId);
}

window.loadUsers = loadUsers;
window.openUserModal = openUserModal;
