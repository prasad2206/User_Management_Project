

  async function fetchUserById() {

    const urlParams = new URLSearchParams(window.location.search);
    const userId = urlParams.get("userId");

    if (!userId) {
        showPopupMessage("Error", "User ID not found in URL.", false);
      return;
    }

    const baseURL = " http://localhost:5138";
    const apiUrl = baseURL + '/api/User/' + userId;

    try {
      const response = await fetch(apiUrl);
      if (!response.ok) throw new Error("Failed to fetch user");

      const user = await response.json();

      // Fill form fields with data
      document.getElementById("fullName").value = user.fullName || "";
      document.getElementById("email").value = user.email || "";
      document.getElementById("username").value = user.username || "";
      document.getElementById("dob").value = user.dateOfBirth?.split("T")[0] || "";
      document.getElementById("contact").value = user.contactNumber || "";
      document.getElementById("address").value = user.address || "";
      document.getElementById("nationality").value = user.nationality?.name || "";

      document.getElementById("country").value = user.country?.name || "";
      document.getElementById("city").value = user.city?.name || "";

      // Gender
      if (user.gender === 1) document.getElementById("genderMale").checked = true;
      else if (user.gender === 2) document.getElementById("genderFemale").checked = true;
      else if (user.gender === 3) document.getElementById("genderOther").checked = true;

      document.getElementById("profession").value = user.profession || "";

      // Languages Known
      if (user.languagesKnown) {
        const langs = user.languagesKnown.map(l => l.name?.toLowerCase() || l.toLowerCase());
        if (langs.includes("english")) document.getElementById("langEnglish").checked = true;
        if (langs.includes("hindi")) document.getElementById("langHindi").checked = true;
        if (langs.includes("marathi")) document.getElementById("langMarathi").checked = true;
      }

      document.getElementById("photo").src = user.photoFilePath || "../Multimedia/sample-user.jpg";
      document.getElementById("about").value = user.aboutYourself || "";

    } catch (err) {
         showPopupMessage("Error", "Error loading user: " + err.message, false);    
    }
  }

  window.onload = fetchUserById;