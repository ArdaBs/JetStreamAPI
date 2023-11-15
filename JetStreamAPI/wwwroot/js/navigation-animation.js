document.addEventListener("scroll", function (e) {
  const navbar = document.getElementById("navbarid");

  if (scrollY > 30) {
    navbar.classList.remove("expanded");
    console.log("Kleiner geworden!");
  } else {
    navbar.classList.add("expanded");
    console.log("Grösser geworden!");
  }
});
