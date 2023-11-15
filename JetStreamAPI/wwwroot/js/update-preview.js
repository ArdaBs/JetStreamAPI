const serviceSelect = document.getElementById("service");
const prioritySelect = document.getElementById("priority");
const pricePreview = document.getElementById("price-preview");

// Add event listeners for the service and priority selects
serviceSelect.addEventListener("change", updatePricePreview);
prioritySelect.addEventListener("change", updatePricePreview);

function updatePricePreview() {
    const selectedService = serviceSelect.value;
    const selectedPriority = prioritySelect.value;
    const totalPrice = calculatePrice(selectedService, selectedPriority);

    pricePreview.innerHTML = "Totale Kosten: CHF " + totalPrice.toFixed(2); // Update the currency symbol and formatting as needed
}

// Initial update when page loads
updatePricePreview();