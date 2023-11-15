document.addEventListener("DOMContentLoaded", function() {
  const form = document.getElementById("serviceForm");
  const successAlert = document.getElementById("success-alert");
  const alertContainer = document.getElementById("alert-container");

  form.addEventListener("submit", async function(event) {
    event.preventDefault();

    var name = document.getElementById("username").value;
    var email = document.getElementById("email").value;
    var phone = document.getElementById("telnumber").value;
    var priority = document.getElementById("priority").value;
    var service = document.getElementById("service").value;
    var comment = ""

    const creationDate = new Date();
    let pickupDate = new Date(creationDate);

    switch (priority) {
      case "low":
        pickupDate.setDate(creationDate.getDate() + 12);
        break;
      case "standard":
        pickupDate.setDate(creationDate.getDate() + 7);
        break;
      case "express":
        pickupDate.setDate(creationDate.getDate() + 5);
        break;
    }

    var data = {
      name: name,
      email: email,
      phone: phone,
      priority: priority,
      service: service,
      create_date: creationDate.toISOString(), 
      pickup_date: pickupDate.toISOString(),
      comment: comment
    };

    console.log('Daten, die gesendet werden:', data);

    try {
      const response = await fetch('https://localhost:7092/api/Registrations', {
        method: 'POST',
        body: JSON.stringify(data),
        headers: {
          'Content-type': 'application/json; charset=UTF-8',
        },
      });

      const json = await response.json(); // Warten auf die Antwort als JSON

      if (!response.ok) {
        throw new Error('Fehler beim Senden der Daten. Antwort des Servers: ' + JSON.stringify(json));
      }

      // Ausgabe in der Konsole
      console.log('Serverantwort:', json);

      // Ausgabe in einem Alert
      alert('Serverantwort: ' + JSON.stringify(json));

      successAlert.classList.remove("d-none");
    } catch (error) {
      console.error('Error:', error);
    }
  });


});
