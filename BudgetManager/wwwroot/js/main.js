const API_BASE = '/api/user';

// Generic function for showing errors/messages
// Универсальная функция для показа ошибок/сообщений
function showMessage(id, text, isError = true) {
  const el = document.getElementById(id);
  el.textContent = text;
  el.style.color = isError ? 'red' : 'green';
}

// Registration (Регистрация)
async function registerUser(event) {
  event.preventDefault();
  const username = event.target.username.value.trim();
  const password = event.target.password.value.trim();
  showMessage('msg', '');

  if (!username || !password) {
    return showMessage('msg', 'Both fields are required');
  }

  try {
    const res = await fetch(`${API_BASE}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
      credentials: 'include'
    });
    if (res.ok) {
      showMessage('msg', 'Successfully! Redirecting to login...', false);
      setTimeout(() => location.href = 'index.html', 1000);
    } else {
      const txt = await res.text();
      showMessage('msg', txt);
    }
  } catch (e) {
    showMessage('msg', 'Network error');
  }
}

// Login (Вход)
async function loginUser(event) {
  event.preventDefault();
  const username = event.target.username.value.trim();
  const password = event.target.password.value.trim();
  showMessage('msg', '');

  if (!username || !password) {
    return showMessage('msg', 'Both fields are required');
  }

  try {
    const res = await fetch(`${API_BASE}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
      credentials: 'include'
    });
    if (res.ok) {
      // here U can check the body (можно проверить тело: { token } )
      location.href = 'home.html';
    } else {
      const txt = await res.text();
      showMessage('msg', txt);
    }
  } catch {
    showMessage('msg', 'Network error');
  }
}

// Log out (Выход)
async function logoutUser() {
  try {
    await fetch(`${API_BASE}/logout`, {
      method: 'POST',
      credentials: 'include'
    });
  } catch {}
  // even if there is an error, just go to the login page (даже при ошибке, просто уходим на страницу входа)
  location.href = 'index.html';
}

// Change password (Смена пароля)
async function changePassword(event) {
  event.preventDefault();
  const oldPassword = event.target.oldPassword.value.trim();
  const newPassword = event.target.newPassword.value.trim();
  showMessage('msg', '');

  if (!oldPassword || !newPassword) {
    return showMessage('msg', 'Both fields are required');
  }

  try {
    const res = await fetch(`${API_BASE}/change-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: '',           // not required: taken from token (не обязательно: берётся из токена)
        oldPassword,
        newPassword
      }),
      credentials: 'include'
    });
    if (res.ok) {
      showMessage('msg', 'Password changed successfully', false);
      // clearing the fields (можно очистить поля)
      event.target.reset();
    } else {
      const txt = await res.text();
      showMessage('msg', txt);
    }
  } catch {
    showMessage('msg', 'Network error');
  }
}

// When loading each page, we bind the required handlers (при загрузке каждой страницы, мы привязываем нужные обработчики)
document.addEventListener('DOMContentLoaded', () => {
  if (document.getElementById('form-register')) {
    document.getElementById('form-register').addEventListener('submit', registerUser);
  }
  if (document.getElementById('form-login')) {
    document.getElementById('form-login').addEventListener('submit', loginUser);
  }
  if (document.getElementById('btn-logout')) {
    document.getElementById('btn-logout').addEventListener('click', logoutUser);
  }
  if (document.getElementById('form-change')) {
    document.getElementById('form-change').addEventListener('submit', changePassword);
  }
});
