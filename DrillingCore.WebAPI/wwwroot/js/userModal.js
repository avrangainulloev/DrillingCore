// Пример файла wwwroot/js/userModal.js

function openUserModal(userId) {
    const modalEl = document.getElementById('userModal');
    if (!modalEl) {
        console.error("User modal not found");
        return;
    }

    // Если userId передан (редактирование) – загрузим данные пользователя
    if (userId) {
        loadUserData(userId);
        document.getElementById('userModalLabel').textContent = "Edit User";
    } else {
        // Если userId не передан (создание нового пользователя) – сбросим форму
        const form = document.getElementById('userForm');
        if (form) {
            form.reset();
        }
        document.getElementById('userId').value = ""; // скрытое поле для userId
        document.getElementById('userModalLabel').textContent = "Create User";
    }

    // Открываем модальное окно
    new bootstrap.Modal(modalEl, { backdrop: 'static', keyboard: false }).show();
}

// Пример функции загрузки данных пользователя для редактирования
async function loadUserData(userId) {
    try {
        const response = await fetch(`https://localhost:7200/api/Users/${userId}`);
        if (response.ok) {
            const user = await response.json();
            document.getElementById('userId').value = user.id;
            document.getElementById('username').value = user.username;
            // Не заполняем поле password для безопасности – его вводят заново
            document.getElementById('fullName').value = user.fullName;
            document.getElementById('email').value = user.email;
            document.getElementById('mobile').value = user.mobile;
            document.getElementById('roleSelect').value = user.roleId;
        } else {
            alert("Error fetching user data: " + response.statusText);
        }
    } catch (error) {
        console.error("Error fetching user data:", error);
        alert("Error fetching user data. See console for details.");
    }
}

// Экспорт функции для глобального доступа, если нужно
window.openUserModal = openUserModal;
