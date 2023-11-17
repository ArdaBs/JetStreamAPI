document.addEventListener("DOMContentLoaded", function() {
    const prioritySelect = document.getElementById("priority");
    const serviceSelect = document.getElementById("service");
    const pricePreview = document.getElementById("price-preview");

    prioritySelect.addEventListener("change", updatePrice);
    serviceSelect.addEventListener("change", updatePrice);

    function updatePrice() {
        const selectedPriority = prioritySelect.value;
        const selectedService = serviceSelect.value;

        let totalCost = 0;

        if (selectedPriority === "low") {
            totalCost -= 5;
        } else if (selectedPriority === "express") {
            totalCost += 10;
        }
        
        if (selectedService === "1") {
            totalCost += 34.95;
        } else if (selectedService === "2") {
            totalCost += 59.95;
        } else if (selectedService === "3") {
            totalCost += 74.95;
        } else if (selectedService === "4") {
            totalCost += 24.95;
        } else if (selectedService === "5") {
            totalCost += 14.95;
        } else if (selectedService === "6") {
            totalCost += 19.95;
        }

        pricePreview.innerHTML = "Totale Kosten: CHF " + totalCost.toFixed(2);
    }
});
