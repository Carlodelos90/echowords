document.addEventListener("DOMContentLoaded", () => {
  const acceptButton = document.getElementById("acceptButton");
  const playerNameInput = document.getElementById("playerNameInput");
  const errorMessage = document.getElementById("errorMessage");

  let playerCount = parseInt(localStorage.getItem("playerCount")) || 0;

  acceptButton.addEventListener("click", () => {
    const username = playerNameInput.value.trim();

    if (username === "") {
      errorMessage.style.display = "block";
      errorMessage.textContent = "Please enter a username.";
      return;
    }

    playerCount++;
    localStorage.setItem("playerCount", playerCount);

    errorMessage.style.display = "none";

    addPlayerName().then(() => {
      window.location.href = "joinGame.html";
    });
  });

  async function addPlayerName() {
    const playerName = $('[id="playerNameInput"]').val();
    console.log("PlayerName:", playerName);
    try {
      const response = await fetch("/new-player/", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ word: playerName }),
      });

      console.log("response", response);
      const data = await response.json();
      console.log("data", data);
      $("#message").text(playerName + " lades till i databasen");
    } catch (error) {
      console.error("Error adding player:", error);
    }
  }

  playerNameInput.addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
      event.preventDefault();
      acceptButton.click();
    }
  });

  //Visuals for when the player writes too long name
  document.addEventListener("input", function (event) {
    if (event.target.id === "playerNameInput") {
      if (event.target.value.length >= event.target.maxLength) {
        event.target.classList.add("shakeName");
        setTimeout(() => {
          event.target.classList.remove("shakeName");
        }, 500); // Remove the shake class after 500ms
      }
    }
  });
});

