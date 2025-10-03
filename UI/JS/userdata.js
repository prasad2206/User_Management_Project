//  window.onload = function () {
//       let isLoggedIn = sessionStorage.getItem("isLoggedIn");
//       if (isLoggedIn !== "true") {
//         window.location.href = "Login.html";
//         return;
//       }
//     }

   const baseURL = " http://localhost:5138";

   // Create empty list of user object
   let userList = [];

    //Create a function to call API and get the data
    async function fetchUsers() {
        try {
           
            //Call api and get response
            const response = await fetch(baseURL + "/api/User");

            //log error if response is not ok
            if (!response.ok) {
                //Popup on UI 
                console.log('HTTP error! Status: ${response.status}');
            }


            //Get user list as a JSOn
            userList = await response.json();

            console.log("Users fetched successfully:", userList);

            //Get table - where user data will be added
            const tbody = document.querySelector("table tbody");
            tbody.innerHTML = "";
            
            //get all users and add to html...
            userList.forEach(user => 
                            { 
                                const row = document.createElement("tr");
                                row.innerHTML = `
                                    <td data-label="Name">${user.fullName}</td>
                                    <td data-label="Username">${user.username}</td>
                                    <td data-label="Email">${user.email}</td>
                                    <td data-label="Contact Number">${user.contactNumber}</td>
                                    <td data-label="Address">${user.address}</td>
                                    <td data-label="City">${user.city?.name || ''}</td>
                                    <td>
                                        <a href="UserEdit.html?userId=${user.id}">Edit</a> |
                                        <a href="UserView.html?userId=${user.id}">Details</a> |
                                        <a href="#" onclick="deleteUser('${user.id}')">Delete</a>
                                    </td>`;
                                tbody.appendChild(row);
                            });
           
        } 
        catch (error) 
        {
            console.error("Failed to fetch users:", error);
        }
    }

    // Call the function
    fetchUsers();

    


    // function logoutUser() {
    //   sessionStorage.removeItem("isLoggedIn");
    //   window.location.href = "Login.html";
    // }


    // Function to delete a user
    async function deleteUser(userId) {
 
      showConfirmPopup("Confirm Delete", "Are you sure you want to delete this user?", async function (confirmed) {
        if (!confirmed) return;
    
        try {
            const response = await fetch(baseURL + "/api/User/" + userId, {
                method: 'DELETE'
            });
    
            if (!response.ok) {
              showPopupMessage("Error", "Failed to delete user.", false);
              
                return;
            }
    
            showPopupMessage("Success", "User deleted successfully.", true);

            fetchUsers(); // Refresh the user list

        } catch (error) {
          showPopupMessage("Error while deleting user"+ error, false);
           
        }
      });
  }