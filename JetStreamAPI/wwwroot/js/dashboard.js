if (localStorage.getItem('token')) {
    fetch('/api/employees/validate', {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
    })
    .then(response => {
        if (response.ok) {
            // The token is valid
            window.location.href = 'dashboard.html';
        } else {
            // The token is invalid or expired
            localStorage.removeItem('token');
            window.location.href = 'index.html';
        }
    })
    .catch(error => {
        console.error('Network error:', error);
        localStorage.removeItem('token');
        window.location.href = 'index.html';
    });
} else {
    // No token found, redirect to login
    window.location.href = 'index.html';
}
