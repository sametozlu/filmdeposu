document.addEventListener('DOMContentLoaded', () => {
  const toggle = document.getElementById('navToggle');
  const links = document.getElementById('navLinks');

  if (toggle && links) {
    toggle.addEventListener('click', () => {
      links.classList.toggle('open');
    });
  }

  applyThemeFromCookie();

  const themeSelect = document.getElementById('Theme');
  if (themeSelect) {
    themeSelect.addEventListener('change', (e) => {
      document.documentElement.setAttribute('data-theme', e.target.value);
    });
  }
});

function applyThemeFromCookie() {
  const cookie = document.cookie
    .split('; ')
    .find(row => row.startsWith('FilmSerileriSettings='));

  if (cookie) {
    try {
      const value = decodeURIComponent(cookie.split('=')[1]);
      const settings = JSON.parse(value);
      if (settings.Theme) {
        document.documentElement.setAttribute('data-theme', settings.Theme);
      }
    } catch { /* ignore */ }
  }
}
