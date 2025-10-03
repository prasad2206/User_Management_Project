using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagementAPI.Database;
using UserManagementAPI.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    //varibale/object decration
    private readonly DBOperations dbOperations;

    public UserController()
    {
        //varibale/object intialization... DBOperation .
        dbOperations = new DBOperations();
    }


    [HttpPost]
    public bool PostUser(User userdata)
    {
        try
        {
            dbOperations.CreateUser(userdata);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    [HttpGet]
    public List<UserDetails> GetUser()
    {
        try
        {
            var userDetailList = dbOperations.GetAllUserDetails();

            return userDetailList;
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    [HttpGet("{userId}")]
    public UserDetails GetUser(int userId)
    {
        try
        {
            var userDetail = dbOperations.GetUserDetailsById(userId);


            return userDetail;

        }
        catch (Exception ex)
        {
            return null;
        }
    }

    //Update user data by userId in database
    [HttpDelete("{userId}")]
    public bool DeleteUser(int userId)
    {
        try
        {
            var isDeleted = dbOperations.DeleteUserById(userId);

            return isDeleted;

        }
        catch (Exception ex)
        {
            return false;
        }
    }


    //Update user data by userId in database
    [HttpPut("{userId}")]
    public bool PutUser(User userdata, int userId)
    {
        try
        {
            dbOperations.UpdateUser(userdata, userId);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    //Get countries by Id in database

    [HttpGet("countries")]
    public List<Country> GetCountry()
    {
        try
        {
            var countryList = dbOperations.GetAllCountries();
            return countryList;
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    [HttpGet("cities/{countryId}")]
    public List<City> GetCities(int countryId)
    {
        try
        {
            var cities = dbOperations.GetCitiesByCountryId(countryId);
            return cities;

        }
        catch (Exception ex)
        {
            return null;
        }
    }
   

}