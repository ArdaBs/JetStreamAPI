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
        
        if (selectedService === "Kleiner-Service") {
            totalCost += 34.95;
        } else if (selectedService === "Grosser-Service") {
            totalCost += 59.95;
        } else if (selectedService === "Rennski-Service") {
            totalCost += 74.95;
        } else if (selectedService === "Bindung-montieren-und-einstellen") {
            totalCost += 24.95;
        } else if (selectedService === "Fell-zuschneiden") {
            totalCost += 14.95;
        } else if (selectedService === "Heisswachsen") {
            totalCost += 19.95;
        }

        pricePreview.innerHTML = "Totale Kosten: CHF " + totalCost.toFixed(2);
    }
});
