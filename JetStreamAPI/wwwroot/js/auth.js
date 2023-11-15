document.addEventListener('DOMContentLoaded', () => {
    const loginLink = document.getElementById('loginLink');
    const logoutButton = document.getElementById('logoutButton');
  
    // Prüfen, ob der Benutzer eingeloggt ist
    if (localStorage.getItem('token')) {
      loginLink.style.display = 'none';
      logoutButton.style.display = 'block';
    }
  
    logoutButton.addEventListener('click', () => {
      // JWT aus dem Speicher entfernen und den Benutzer abmelden
      localStorage.removeItem('token');
  
      // UI aktualisieren
      logoutButton.style.display = 'none';
      loginLink.style.display = 'block';
  
      // Zurück zur Login-Seite
      window.location.href = 'registration.html';
    });
  });
  