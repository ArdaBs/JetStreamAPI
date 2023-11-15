document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.querySelector('.login-form');
  
    loginForm.addEventListener('submit', async (event) => {
      event.preventDefault();
  
      const username = loginForm.querySelector('input[type="text"]').value;
      const password = loginForm.querySelector('input[type="password"]').value;
  
      const loginData = {
        username,
        password
      };
  
      try {
        const response = await fetch('https://localhost:7092/api/Employees/login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(loginData),
        });
  
        if (response.ok) {
          const data = await response.json();
          console.log('Login successful', data);
          localStorage.setItem('token', data.token);
        
          // Redirect to dashboard page
          window.location.href = 'dashboard.html';
        } else {
          // If login is not successful, you might want to inform the user
          alert('Login failed: Invalid username or password.');
        }        
      } catch (error) {
        console.error('Network error:', error);
      }
    });
  });
  