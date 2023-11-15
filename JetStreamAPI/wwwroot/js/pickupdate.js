const prioritySelect = document.getElementById("priority");
prioritySelect.addEventListener("change", calculateCompletionDate);

function calculateCompletionDate() {
    const selectedPriority = prioritySelect.value;
    
    const currentDate = new Date();
    let totalDays = 0;

    if (selectedPriority === "low") {
        totalDays = 12;
    } else if (selectedPriority === "standard") {
        totalDays = 7;
    } else if (selectedPriority === "express") {
        totalDays = 5;
    }
    
    const completionDate = new Date(currentDate);
    completionDate.setDate(currentDate.getDate() + totalDays);
    
    const datePreview = document.getElementById("date-preview");
    const formattedDate = completionDate.toLocaleDateString("de-CH"); // Nutzt schweizerisches Format
    datePreview.innerHTML = "Fertigstellung am: " + formattedDate;
}