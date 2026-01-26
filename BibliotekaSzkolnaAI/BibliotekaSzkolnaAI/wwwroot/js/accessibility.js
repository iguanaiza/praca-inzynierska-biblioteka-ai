//dostepne zmienne
window.fontSizes = ['font-small', 'font-normal', 'font-large'];
window.themes = ['theme-green', 'theme-blue', 'theme-orange', 'theme-gray'];

//Zapis ustawień
window.setTheme = (themeClass) => {
    localStorage.setItem('theme', themeClass);
    window.applyUserPreferences();
};

window.setFontSize = (fontSizeClass) => {
    localStorage.setItem('fontSize', fontSizeClass);
    window.applyUserPreferences();
};

// Osobny tryb wysokiego kontrastu
window.toggleContrast = () => {
    const current = localStorage.getItem('theme');
    if (current === 'theme-contrast') {
        const fallback = localStorage.getItem('lastTheme') || 'theme-green';
        localStorage.setItem('theme', fallback);
    } else {
        localStorage.setItem('lastTheme', current);
        localStorage.setItem('theme', 'theme-contrast');
    }

    window.applyUserPreferences();
};

// Wczytanie ustawień użytkownika
window.applyUserPreferences = () => {
    const root = document.documentElement; 
    if (!root) return;

    const savedTheme = localStorage.getItem('theme') || 'theme-green';
    const savedFontSize = localStorage.getItem('fontSize') || 'font-normal';
    const allThemes = window.themes.concat(['theme-contrast']);

    const allFontSizes = window.fontSizes;

    let currentClasses = root.className.split(' ').filter(cls => cls.length > 0);

    let newClasses = currentClasses.filter(cls => {

        return !allThemes.includes(cls) && !allFontSizes.includes(cls);
    });


    newClasses.push(savedTheme);
    newClasses.push(savedFontSize);

    root.className = newClasses.join(' ');

    document.cookie = `theme=${savedTheme};path=/;max-age=31536000;samesite=lax`;

    console.log('Applied classes:', root.className);
};