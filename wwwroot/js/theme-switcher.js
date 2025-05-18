const themeToggle = document.getElementById('themeSwitcher');
const savedTheme = getCookie('theme') || 'light';

setTheme(savedTheme);
themeToggle.checked = savedTheme === 'dark';

themeToggle.addEventListener('change', () => {
    const newTheme = themeToggle.checked ? 'dark' : 'light';
    setTheme(newTheme);
    saveUserTheme(newTheme);
    document.cookie = `theme=${newTheme}; path=/; max-age=31536000`;
});

function setTheme(theme) {
    document.documentElement.setAttribute('data-bs-theme', theme);
}

function getCookie(name) {
    const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    return match ? match[2] : null;
}

function saveUserTheme(theme) {
    
    fetch('/User/SaveTheme', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ theme })
    }).catch(() => {});
}
