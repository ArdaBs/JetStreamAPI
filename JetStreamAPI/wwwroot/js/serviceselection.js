// Dafür da um mit ausgewählten option weitergeleitet zu werden
document.addEventListener("DOMContentLoaded", function() {
    const optionsButtons = document.querySelectorAll(".select-option");

    optionsButtons.forEach(button => {
        button.addEventListener("click", function() {
            const selectedOption = this.getAttribute("data-option");
            localStorage.setItem("selectedOption", selectedOption);

            // Redirect to buy.html with the selected option as a query parameter
            window.location.href = `buy.html?option=${selectedOption}`;
        });
    });
});