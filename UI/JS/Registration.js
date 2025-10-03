 const baseURL = "http://localhost:5138";


document.addEventListener("DOMContentLoaded", () => {
  populateCountryDropdown();

  const countryDropdown = document.getElementById("country");
  countryDropdown.addEventListener("change", () => {
    const selectedCountryId = countryDropdown.value;
    populateCityDropdown(selectedCountryId);
  });
});

/**
 * Fetch all countries from API and populate the Country dropdown
 */
async function populateCountryDropdown() {
  const countryDropdown = document.getElementById("country");
  countryDropdown.innerHTML = `<option value="">--Select Country--</option>`;

  try {
    const response = await fetch(`${baseURL}/api/User/countries`);
    const countries = await response.json();

    countries.forEach(country => {
      const option = document.createElement("option");
      option.value = country.countryId;   // backend ID
      option.textContent = country.name;  // display name
      countryDropdown.appendChild(option);
    });
  } catch (error) {
    console.error("Error fetching countries:", error);
  }
}

/**
 * Fetch cities by countryId and populate the City dropdown
 * @param {number} countryId 
 */
async function populateCityDropdown(countryId) {
  const cityDropdown = document.getElementById("city");
  cityDropdown.innerHTML = `<option value="">--Select City--</option>`;

  if (!countryId) return; // do nothing if no country selected

  try {
    const response = await fetch(`${baseURL}/api/User/cities/${countryId}`);
    const cities = await response.json();

    cities.forEach(city => {
      const option = document.createElement("option");
      option.value = city.cityId;   // backend ID
      option.textContent = city.name; // display name
      cityDropdown.appendChild(option);
    });
  } catch (error) {
    console.error("Error fetching cities:", error);
  }
}


const form = document.getElementById("registration_form");

form.addEventListener("submit", function (e) {
  e.preventDefault(); // form submit 

  let isValid = true;

  // Text Fields Validation
  if (document.getElementById("fullname").value.trim() === "") {
    document.getElementById("error-fullname").style.display = "block";
    isValid = false;
  } else {
    document.getElementById("error-fullname").style.display = "none";
  }

  if (document.getElementById("email").value.trim() === "") {
    document.getElementById("error-email").style.display = "block";
    isValid = false;
  } else {
    document.getElementById("error-email").style.display = "none";
  }

  if (document.getElementById("username").value.trim() === "") {
    document.getElementById("error-username").style.display = "block";
    isValid = false;
  } else {
    document.getElementById("error-username").style.display = "none";
  }

  if (document.getElementById("password").value.trim() === "") {
    document.getElementById("error-password").style.display = "block";
    isValid = false;
  } else {
    document.getElementById("error-password").style.display = "none";
  }

  if (document.getElementById("DOB").value.trim() === "") {
    document.getElementById("error-DOB").style.display = "block";
    isValid = false;
  } else {
    document.getElementById("error-DOB").style.display = "none";
  }

  if (document.getElementById("contact").value.trim() === "") {
    document.getElementById("error-contact_number").style.display = "block";
    isValid = false;
  } else {
    document.getElementById("error-contact_number").style.display = "none";
  }


  // // Dropdown Validation
  // if (document.getElementById("nationality").value === "") {
  //   document.getElementById("error-nationality").style.display = "block";
  //   isValid = false;
  // } else {
  //   document.getElementById("error-nationality").style.display = "none";
  // }

  // if (document.getElementById("country").value === "") {
  //   document.getElementById("error-country").style.display = "block";
  //   isValid = false;
  // } else {
  //   document.getElementById("error-country").style.display = "none";
  // }

  // if (document.getElementById("city").value === "") {
  //   document.getElementById("error-city").style.display = "block";
  //   isValid = false;
  // } else {
  //   document.getElementById("error-city").style.display = "none";
  // }

  // Gender (Radio) Validation
  let genderSelected = document.querySelector('input[name="gender"]:checked');
  if (!genderSelected) {
    document.getElementById("error-gender").style.display = "block";
    isValid = false;
  } else {
    document.getElementById("error-gender").style.display = "none";
  }

  // Language (Checkbox) Validation
  let langSelected = document.querySelectorAll('input[name="language"]:checked');
  if (langSelected.length === 0) {
    document.getElementById("error-LK").style.display = "block";
    isValid = false;
  } else {
    document.getElementById("error-LK").style.display = "none";
  }

  // File Validation
  if (document.getElementById("file").value === "") {
    document.getElementById("error-file").style.display = "block";
    isValid = false;
  } else {
    document.getElementById("error-file").style.display = "none";
  }

  // Submit message if valid
  if (isValid) {
    // // Create a user object from the form inputs
    // const userData = {
    //   fullname: document.getElementById("fullname").value,
    //   email: document.getElementById("email").value,
    //   username: document.getElementById("username").value,
    //   contact: document.getElementById("contact").value,
    //   address: document.getElementById("address").value,
    //   city: document.getElementById("city").value
    // };
  
    // // Get old users from localStorage or create new list
    // let users = JSON.parse(localStorage.getItem("registeredUsers")) || [];
  
    // // Add the new user
    // users.push(userData);
  
    // // Save back to localStorage
    // localStorage.setItem("registeredUsers", JSON.stringify(users));
    


    //Save user details using API
    // Extract gender (radio buttons)
    const genderValue = document.querySelector('input[name="gender"]:checked');
    const gender = genderValue ? (genderValue.value === "Male" ? 1 : genderValue.value === "Female" ? 2 : 3) : null;

    // Extract languages known (checkboxes)
    const languageCheckboxes = document.querySelectorAll('input[name="language"]:checked');

    const languagesKnown = Array.from(languageCheckboxes).map(lang => {
        switch (lang.value) {
            case "English": return 1;
            case "Hindi": return 2;
            case "Marathi": return 3;
            default: return null;
        }
    }).filter(id => id !== null);

    // Get file path (Note: browser only provides fake path, usually not useful for server)
    const photoInput = document.getElementById("file");
    const photoFilePath = photoInput.value; // Browser security blocks full path
    const countryId = parseInt(document.getElementById("country").value, 10) || null;
    const cityId    = parseInt(document.getElementById("city").value, 10) || null;

    const payload = {
        fullName: document.getElementById("fullname").value,
        email: document.getElementById("email").value,
        username: document.getElementById("username").value,
        password: document.getElementById("password").value,
        dateOfBirth: new Date(document.getElementById("DOB").value).toISOString(),
        contactNumber: document.getElementById("contact").value,
        address: document.getElementById("address").value,
        nationalityId: 2, // Hardcoded, you may replace with dynamic selection
        countryId: countryId,     // Same here
        cityId: cityId,        // Same here
        gender: gender,
        profession: document.getElementById("profession").value,
        photoFilePath: photoFilePath,  // Note: might need to upload file separately
        aboutYourself: document.getElementById("about").value,
        languagesKnown: languagesKnown
    };

    fetch(baseURL + "/api/User/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => { throw err; });
        }
        return response.json();
    })
    .then(data => {

       showPopupMessage("Success", "User registered successfully!", true);

        document.getElementById("registration_form").reset();
    })
    .catch(error => {
        showPopupMessage("Error", "Failed to register user." + error, false);
    });
}
else
  {
     showPopupMessage("Error", "Please enter values for required fields", false);
  }

});

// document.getElementById("country").addEventListener("change", function() {
//   let country = this.value;
//   let cityDropdown = document.getElementById("city");

//   // Clear existing city options
//   cityDropdown.innerHTML = "<option value=''>Select City</option>";

//   // Define cities for each country
//   let cities = {
    
//     "India": ["Delhi", "Mumbai", "Bangalore"],
//     "USA": ["New York", "Los Angeles", "Chicago"],
//     "UK": ["London", "Manchester", "Birmingham"]
//   };

//   if (cities[country]) {
//     cities[country].forEach(function(city) {
//       let option = document.createElement("option");
//       option.value = city;
//       option.textContent = city;
//       cityDropdown.appendChild(option);
//     });
//   }
// });