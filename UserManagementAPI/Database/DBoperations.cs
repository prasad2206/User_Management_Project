using Microsoft.Data.SqlClient;
using UserManagementAPI.Model;
using System.Data;
using System.Data.SqlClient;

 
namespace UserManagementAPI.Database
{
    public class DBOperations
    {
        string connectionString = "Server=PRASAD-PC\\MSSQLSERVER01;Database=UserManagementProject;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true";


        /// <summary>
        /// Use this method to create user in the database
        /// </summary>
        /// <param name="user"></param>
        public void CreateUser(User user)
        {
            //1. Create SQL connection ..
            using (SqlConnection databaseConnection = new SqlConnection(connectionString))
            {

                //2. SQL Query to insert data into user table - with parametrs, or varibales

                string query = @"INSERT INTO Users 
                                   (FullName, Email, Username, Password, DateOfBirth, ContactNumber, Address, NationalityId, CountryId, 
                                    CityId, Gender, Profession, PhotoFilePath, AboutYourself) 
                                VALUES 
                                    (@FullName, @Email, @Username, @Password, @DateOfBirth, @ContactNumber, @Address, @NationalityId, @CountryId,
                                    @CityId, @Gender, @Profession, @PhotoFilePath, @AboutYourself);
                                    
                                   SELECT SCOPE_IDENTITY();";



                //3. Fill parameters or varibales using sql command..

                using (SqlCommand cmd = new SqlCommand(query, databaseConnection))
                {

                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                    cmd.Parameters.AddWithValue("@ContactNumber", user.ContactNumber);
                    cmd.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NationalityId", user.NationalityId);
                    cmd.Parameters.AddWithValue("@CountryId", user.CountryId);
                    cmd.Parameters.AddWithValue("@CityId", user.CityId);
                    cmd.Parameters.AddWithValue("@Gender", user.Gender);
                    cmd.Parameters.AddWithValue("@Profession", user.Profession ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhotoFilePath", user.PhotoFilePath);
                    cmd.Parameters.AddWithValue("@AboutYourself", user.AboutYourself ?? (object)DBNull.Value);


                    //4. Open database connection ..

                    databaseConnection.Open();


                    //5. Execute SQL query to save user to database and get newly created userId from database
                    var excuteQuery = cmd.ExecuteScalar();

                    int userId = Convert.ToInt32(excuteQuery);

                    //6. Insert into LanguageKnown, depends on number of language selection by user..
                    if (user.LanguagesKnown != null && user.LanguagesKnown.Count > 0)
                    {
                        foreach (int languageId in user.LanguagesKnown)
                        {
                            string languageInsertQuery = "INSERT INTO UserLanguages (UserID, LanguageID) VALUES (@UserID, @LanguageID)";

                            using (SqlCommand langCmd = new SqlCommand(languageInsertQuery, databaseConnection))
                            {
                                langCmd.Parameters.AddWithValue("@UserID", userId);

                                langCmd.Parameters.AddWithValue("@LanguageID", languageId);

                                langCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    //7. close connection, this will be handeled by using statement automatically ..
                    // databaseConnection.Close();
                }
            }
        }

        /// <summary>
        /// Get all users data..
        /// </summary>
        /// <returns>Returns List of users</returns>
        public List<UserDetails> GetAllUserDetails()
        {
            List<UserDetails> userDetailList = new List<UserDetails>();

            //Create db connection 
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Prepare query to get the user details..
                string query = @"
                                SELECT 
                                 u.Id as UserId, u.FullName, u.Email, u.Username, u.[Password], u.DateOfBirth,
                                 u.ContactNumber, u.[Address],
                                 u.Gender, u.Profession, u.PhotoFilePath, u.AboutYourself,
     
                                 nat.Id as NationalityId,
                                 nat.[Name],
    
                                 cont.Id as CountryId,
                                 cont.[Name],
     
                                 cty.Id as CityId,
                                 cty.[Name]                                    
 
                        FROM Users as u
 
                         INNER JOIN Nationalities as nat ON u.NationalityId = nat.Id
 
                         INNER JOIN Countries as cont ON u.CountryId = cont.Id
 
                         INNER JOIN Cities as cty ON u.CityId = cty.Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    //Open database connection
                    conn.Open();

                    //Execute query to red data from database
                    SqlDataReader reader = cmd.ExecuteReader();

                    //Open sql  reader and read the data row by row
                    while (reader.Read())
                    {
                        //Assign all user details to user object..
                        var userObj = new UserDetails();


                        userObj.Id = (int)reader["UserId"];
                        userObj.FullName = reader["FullName"].ToString();
                        userObj.Email = reader["Email"].ToString();
                        userObj.Username = reader["Username"].ToString();
                        userObj.Password = reader["Password"].ToString();
                        userObj.DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                        userObj.ContactNumber = reader["ContactNumber"].ToString();
                        userObj.Address = reader["Address"]?.ToString();
                        userObj.Profession = reader["Profession"]?.ToString();
                        userObj.PhotoFilePath = reader["PhotoFilePath"].ToString();
                        userObj.AboutYourself = reader["AboutYourself"]?.ToString();

                        userObj.Nationality = new Nationality
                        {
                            NationalityId = (int)reader["NationalityId"],
                            Name = reader["Name"].ToString()
                        };

                        userObj.Country = new Country
                        {
                            CountryId = (int)reader["CountryId"],
                            Name = reader["Name"].ToString()
                        };

                        userObj.City = new City
                        {
                            CityId = (int)reader["CityId"],
                            Name = reader["Name"].ToString()
                        };

                        userObj.Gender = Enum.TryParse(reader["Gender"].ToString(), true, out Gender genderVal)
                                     ? genderVal : Gender.Other;

                        userObj.LanguagesKnown = new List<Language>(); // Populated later


                        //Add user to the list
                        userDetailList.Add(userObj);


                    }

                    //Close sql  reader
                    reader.Close();


                    // Get languages for all users..

                    foreach (var user in userDetailList)
                    {
                        //SQL query to get languages
                        string langQuery = @"SELECT  ul.UserId, ul.LanguageId, lng.[Name]
 
                                            FROM 
                                                UserLanguages as ul
 
                                        INNER JOIN 
                                                Languages as lng ON ul.LanguageId = lng.Id
 
                                        WHERE ul.UserId = @UserID";


                        using (SqlCommand langCmd = new SqlCommand(langQuery, conn))
                        {
                            //Get user language based on user Id
                            langCmd.Parameters.AddWithValue("@UserID", user.Id);

                            using (SqlDataReader langReader = langCmd.ExecuteReader())
                            {
                                while (langReader.Read())
                                {
                                    //Add user languages to the Users list
                                    user.LanguagesKnown.Add(new Language
                                    {
                                        LanguageId = (int)langReader["LanguageId"],
                                        Name = langReader["Name"].ToString()
                                    });
                                }
                            }
                        }
                    }
                }

                return userDetailList;
            }
        }

        /// <summary>
        /// Get a user deatils
        /// </summary>
        /// <param name="userId"></param>
        /// <returns> Returns user by id</returns>
        public UserDetails GetUserDetailsById(int userId)
        {
            UserDetails userDetails = new UserDetails();

            //Create db connection 
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Prepare query to get the user details..
                string query = @"
                                SELECT 
                                 u.Id as UserId, u.FullName, u.Email, u.Username, u.[Password], u.DateOfBirth,
                                 u.ContactNumber, u.[Address],
                                 u.Gender, u.Profession, u.PhotoFilePath, u.AboutYourself,
     
                                 nat.Id as NationalityId,
                                 nat.[Name],
    
                                 cont.Id as CountryId,
                                 cont.[Name],
     
                                 cty.Id as CityId,
                                 cty.[Name]                                    
 
                        FROM Users as u
 
                         INNER JOIN Nationalities as nat ON u.NationalityId = nat.Id
 
                         INNER JOIN Countries as cont ON u.CountryId = cont.Id
 
                         INNER JOIN Cities as cty ON u.CityId = cty.Id
 
                        WHERE u.Id = @UserId"; //apply where condition to filter user by user id

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    //Append user id
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    //Open database connection
                    conn.Open();

                    //Execute query to red data from database
                    SqlDataReader reader = cmd.ExecuteReader();


                    //Return null if there is no rows found
                    if (!reader.HasRows)
                    {
                        return null;
                    }


                    //Open sql  reader and read the data row by row
                    while (reader.Read())
                    {
                        //Assign all user details to user object..
                        var userObj = new UserDetails();


                        userObj.Id = (int)reader["UserId"];
                        userObj.FullName = reader["FullName"].ToString();
                        userObj.Email = reader["Email"].ToString();
                        userObj.Username = reader["Username"].ToString();
                        userObj.Password = reader["Password"].ToString();
                        userObj.DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                        userObj.ContactNumber = reader["ContactNumber"].ToString();
                        userObj.Address = reader["Address"]?.ToString();
                        userObj.Profession = reader["Profession"]?.ToString();
                        userObj.PhotoFilePath = reader["PhotoFilePath"].ToString();
                        userObj.AboutYourself = reader["AboutYourself"]?.ToString();

                        userObj.Nationality = new Nationality
                        {
                            NationalityId = (int)reader["NationalityId"],
                            Name = reader["Name"].ToString()
                        };

                        userObj.Country = new Country
                        {
                            CountryId = (int)reader["CountryId"],
                            Name = reader["Name"].ToString()
                        };

                        userObj.City = new City
                        {
                            CityId = (int)reader["CityId"],
                            Name = reader["Name"].ToString()
                        };

                        userObj.Gender = Enum.TryParse(reader["Gender"].ToString(), true, out Gender genderVal)
                                     ? genderVal : Gender.Other;

                        userObj.LanguagesKnown = new List<Language>(); // Populated later


                        //Add user to the list
                        userDetails = userObj;


                    }

                    //Close sql  reader
                    reader.Close();


                    // Get languages for all users..
                    //SQL query to get languages
                    string langQuery = @"SELECT  ul.UserId, ul.LanguageId, lng.[Name]
 
                                        FROM 
                                            UserLanguages as ul
 
                                    INNER JOIN 
                                            Languages as lng ON ul.LanguageId = lng.Id
 
                                    WHERE ul.UserId = @UserID";


                    using (SqlCommand langCmd = new SqlCommand(langQuery, conn))
                    {
                        //Get user language based on user Id
                        langCmd.Parameters.AddWithValue("@UserID", userDetails.Id);

                        using (SqlDataReader langReader = langCmd.ExecuteReader())
                        {
                            while (langReader.Read())
                            {
                                //Add user languages to the Users list
                                userDetails.LanguagesKnown.Add(new Language
                                {
                                    LanguageId = (int)langReader["LanguageId"],
                                    Name = langReader["Name"].ToString()
                                });
                            }
                        }
                    }

                }

                return userDetails;
            }
        }

        /// <summary>
        /// Delete a user by their ID
        /// </summary>
        /// <param name="userId">The ID of the user to delete</param>
        /// <returns>True if the user was deleted; false otherwise</returns>
        public bool DeleteUserById(int userId)
        {
            //Connection with DB
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Use a transaction in case of multiple deletions (e.g., child tables)
                // TCL - quries 
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Delete from UserLanguages first to handle FK constraints
                        string deleteLanguagesQuery = "DELETE FROM UserLanguages WHERE UserId = @UserId";
                        using (SqlCommand cmdLanguages = new SqlCommand(deleteLanguagesQuery, conn, transaction))
                        {
                            cmdLanguages.Parameters.AddWithValue("@UserId", userId);
                            cmdLanguages.ExecuteNonQuery();
                        }

                        // Delete the user
                        string deleteUserQuery = "DELETE FROM Users WHERE Id = @UserId";
                        using (SqlCommand cmdUser = new SqlCommand(deleteUserQuery, conn, transaction))
                        {
                            cmdUser.Parameters.AddWithValue("@UserId", userId);
                            int rowsAffected = cmdUser.ExecuteNonQuery();

                            // Commit only if a row was deleted
                            if (rowsAffected > 0)
                            {
                                //Commit the database changes - after all quries runs sucessfully 
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                //Undo the database operation if half execution done (example - data deleted from user language table but not from user table )
                                transaction.Rollback();

                                return false; // No user found with the given ID
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; // Optionally, log and rethrow
                    }
                }
            }
        }

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">The user object containing updated information</param>
        public void UpdateUser(User user, int userId)
        {
            using (SqlConnection databaseConnection = new SqlConnection(connectionString))
            {
                databaseConnection.Open();

                using (SqlTransaction transaction = databaseConnection.BeginTransaction())
                {
                    try
                    {
                        // 1. Update the Users table
                        string updateUserQuery = @"
             UPDATE Users SET 
                 FullName = @FullName,
                 Email = @Email,
                 Username = @Username,
                 Password = @Password,
                 DateOfBirth = @DateOfBirth,
                 ContactNumber = @ContactNumber,
                 Address = @Address,
                 NationalityId = @NationalityId,
                 CountryId = @CountryId,
                 CityId = @CityId,
                 Gender = @Gender,
                 Profession = @Profession,
                 PhotoFilePath = @PhotoFilePath,
                 AboutYourself = @AboutYourself
             WHERE Id = @UserId";

                        using (SqlCommand cmd = new SqlCommand(updateUserQuery, databaseConnection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@FullName", user.FullName);
                            cmd.Parameters.AddWithValue("@Email", user.Email);
                            cmd.Parameters.AddWithValue("@Username", user.Username);
                            cmd.Parameters.AddWithValue("@Password", user.Password);
                            cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                            cmd.Parameters.AddWithValue("@ContactNumber", user.ContactNumber);
                            cmd.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@NationalityId", user.NationalityId);
                            cmd.Parameters.AddWithValue("@CountryId", user.CountryId);
                            cmd.Parameters.AddWithValue("@CityId", user.CityId);
                            cmd.Parameters.AddWithValue("@Gender", user.Gender);
                            cmd.Parameters.AddWithValue("@Profession", user.Profession ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PhotoFilePath", user.PhotoFilePath);
                            cmd.Parameters.AddWithValue("@AboutYourself", user.AboutYourself ?? (object)DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }

                        // 2. Update UserLanguages (Delete existing, then insert new)
                        string deleteLanguagesQuery = "DELETE FROM UserLanguages WHERE UserId = @UserId";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteLanguagesQuery, databaseConnection, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@UserId", userId);
                            deleteCmd.ExecuteNonQuery();
                        }

                        if (user.LanguagesKnown != null && user.LanguagesKnown.Count > 0)
                        {
                            foreach (int languageId in user.LanguagesKnown)
                            {
                                string insertLanguageQuery = "INSERT INTO UserLanguages (UserId, LanguageId) VALUES (@UserId, @LanguageId)";
                                using (SqlCommand insertCmd = new SqlCommand(insertLanguageQuery, databaseConnection, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@UserId", userId);
                                    insertCmd.Parameters.AddWithValue("@LanguageId", languageId);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; // optionally log the error
                    }
                }
            }
        }

        public List<Country> GetAllCountries()
        {
            var list = new List<Country>();
            using (var conn = new SqlConnection(connectionString))
            {
                string CountryQuery = @"SELECT Id AS CountryId, [Name] FROM Countries ORDER BY [Name]";
                using (var CountyryCmd = new SqlCommand(CountryQuery, conn))
                {
                    conn.Open();
                    using (var countryReader = CountyryCmd.ExecuteReader())
                    {
                        while (countryReader.Read())
                        {
                            list.Add(new Country
                            {
                                CountryId = (int)countryReader["CountryId"],
                                Name = countryReader["Name"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<City> GetCitiesByCountryId(int countryId)
        {
            var list = new List<City>();
            using (var conn = new SqlConnection(connectionString))
            {
                string CityQuery = @"SELECT Id AS CityId, [Name], CountryId 
                         FROM Cities 
                         WHERE CountryId = @CountryId
                         ORDER BY [Name]";
                using (var CityCmd = new SqlCommand(CityQuery, conn))
                {
                    CityCmd.Parameters.AddWithValue("@CountryId", countryId);
                    conn.Open();
                    using (var CityReader = CityCmd.ExecuteReader())
                    {
                        while (CityReader.Read())
                        {
                            list.Add(new City
                            {
                                CityId = (int)CityReader["CityId"],
                                Name = CityReader["Name"].ToString(),
                                CountryId = (int)CityReader["CountryId"]
                            });
                        }
                    }
                }
            }
            return list;
        }

    }
}

 
