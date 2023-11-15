document.addEventListener("DOMContentLoaded", function() {
    const optionsButtons = document.querySelectorAll(".select-option");

    optionsButtons.forEach(button => {
        button.addEventListener("click", function(e) {
            e.preventDefault(); // Verhindert das Standardverhalten des Klicks

            const selectedOption = this.getAttribute("data-option");
            localStorage.setItem("selectedOption", selectedOption);

            // Redirect to buy.html with the selected option as a query parameter
            window.location.href = `buy.html?option=${selectedOption}`;
        });
    });

    // After the DOM is loaded, check for the selected option in localStorage
    const selectedOption = localStorage.getItem("selectedOption");
    if (selectedOption) {
        const dropdown = document.getElementById("service");
        dropdown.value = selectedOption;
    }
});
