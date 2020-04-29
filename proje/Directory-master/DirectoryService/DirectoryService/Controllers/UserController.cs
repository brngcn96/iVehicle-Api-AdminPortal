using DirectoryService.Dtos;
using DirectoryService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Http;



namespace DirectoryService.Controllers
{

    [Authorize]
    public class UserController : ApiController
    {

        
        private Model1Container context = new Model1Container();


        public ResponseObjectDto CreateChildUser(int parentID,String email,String password,String name,String surname)
        {
            if (!ModelState.IsValid)
                return new ResponseObjectDto(null, 0, "Bad Request", 400);

            try
            {
                byte[] passwordHash;
                byte[] passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                if (passwordHash != null || passwordSalt != null)
                {

                    ChildUser newChildUser = new ChildUser()
                    {
                        Name = name,
                        Surname = surname,
                        Email = email,
                        Parent_ID = parentID,

                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Status = true,

                        CreatorIP = ":::",
                        CreatorRole = "childuser",
                        CreationDate = DateTime.Now,

                    };
                    context.ChildUser.Add(newChildUser);
                    context.SaveChanges();

                    return new ResponseObjectDto(newChildUser, 1, "Accepted", 200);
                }
                return new ResponseObjectDto(null, 0, "Bad Request", 400);

            }
            catch (WebException exc)
            {
                return new ResponseObjectDto(null, 0, exc.Message,(int)(((HttpWebResponse)exc.Response)).StatusCode);
            }

        }







        // POST api/User/CreateRide?
        [AllowAnonymous]
        [HttpPost]
        [ActionName("CreateRide")]

        public RideDto CreateRide(int id)
        {

            RideDto rideDto = new RideDto();
            if (!ModelState.IsValid)
                return null;


            User user = (from c in context.User where c.User_ID == id select c).FirstOrDefault();
            try
            {

                if (user != null)
                {
                    Ride newRide = new Ride()
                    {
                        User_ID = id,
                        StartTime = DateTime.Now,                       
                        Status = true,

                    };

                    rideDto.Ride_ID = newRide.Ride_ID;
                    rideDto.User_ID = newRide.User_ID;
                    rideDto.StartTime = DateTime.Now;
                    rideDto.Status = newRide.Status;
                

                    context.Ride.Add(newRide);
                    context.SaveChanges();
                }


            }
            catch (Exception exc)
            {
                return null;

            }


            return rideDto;
        }



        // POST api/User/Register?
        [AllowAnonymous]
        [HttpPost]
        [ActionName("RegisterUser")]

        public HttpResponseMessage RegisterUser([Required]string name, [Required]string surname, [Required]string email, [Required]string password)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Model State not valid!");

            try
            {
                 User user = (from c in context.User where c.Email == email select c).FirstOrDefault();

                if (user != null)
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Email is already exist!");

                byte[] passwordHash;
                byte[] passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                if (passwordHash != null || passwordSalt != null)
                {
                    User newUser = new User()
                    {
                        Name = name,
                        Surname = surname,
                        Email = email,

                        Credit=50,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Status = true,

                        CreatorIP = ":::",
                        CreatorRole = "user",
                        CreationDate = DateTime.Now,
                        TotalHours = 0,
                        GiftCounter = 10,
                        GiftedHour = 0,

                    };
                    context.User.Add(newUser);
                    context.SaveChanges();
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exc.Message);
                // return Newtonsoft.Json.JsonConvert.SerializeObject(new { status = "reject(IntServerError)" });

            }
            //return Newtonsoft.Json.JsonConvert.SerializeObject(new { status = "accepted" });

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }
        // GET api/User/DisplayGifts?
        [AllowAnonymous]
        [HttpGet]
        [ActionName("DisplayGifts")]

        public int DisplayGifts(int id,[FromBody] string x )
        {
            User user = (from c in context.User where c.User_ID == id select c).FirstOrDefault();
            if(user.GiftCounter == 10)
            {
                user.GiftedHour++;
                context.User.Add(user);
                context.SaveChanges();

            }
            return user.GiftedHour;
        }


        public ResponseObjectDto StopRide(int id)
        {
            if(!ModelState.IsValid)
                return new ResponseObjectDto(null, 0, "Bad Request", 400);

            Ride ride = (from c in context.Ride where c.Ride_ID == id select c).FirstOrDefault();
            if (ride.Status)
            {
                ride.Status = false;
            }
            else
                return new ResponseObjectDto(null, 0, "Bad Request", 400);

            return new ResponseObjectDto(1, 1, "Accepted", 200);
        }

        public RideDto EarlyStartRide(int userID,int veicleID)
        {
            RideDto rideDto = new RideDto();
            if (!ModelState.IsValid)
                return null;

            User user = (from c in context.User where c.User_ID == userID select c).FirstOrDefault();
            try
            {

                if (user != null)
                {
                    Ride newRide = new Ride()
                    {
                        User_ID = userID,
                        StartTime = DateTime.Now,
                        FinishTime = null,
                        Status = true,

                    };

                    rideDto.Ride_ID = newRide.Ride_ID;
                    rideDto.User_ID = newRide.User_ID;
                    rideDto.StartTime = DateTime.Now;
                    rideDto.Status = newRide.Status;


                    Vehicle vehicle = (from c in context.Vehicle where c.Vehicle_ID == veicleID select c).FirstOrDefault();
                    if (vehicle != null)
                        vehicle.isReserved = true;
                    else
                        return null;

                    context.Ride.Add(newRide);
                    context.Vehicle.Add(vehicle);
                    context.SaveChanges();
                }

            }
            catch (Exception exc)
            {
                return null;

            }
            return rideDto;
        }

     
        // GET api/User/DisplayAvaibleVehicles?
        [AllowAnonymous]
        [HttpGet]
        [ActionName("DisplayAvaibleVehicles")]

        public int[] DisplayAvaibleVehicles(int stationID)
        {

            if (stationID<0 && stationID> (from c in context.Station select c).LongCount() )
                return null;

            List<Vehicle> vehiclesInStation = (from c in context.Vehicle where c.Station_ID == stationID select c).ToList();
            int[] avaibleVehicleIDs=new int[vehiclesInStation.Count];

            for(int i = 0; i < vehiclesInStation.Count; i++)
            {
                avaibleVehicleIDs[i] = vehiclesInStation[i].Position_in_Station;
            }
            
            return avaibleVehicleIDs;
        }


    }

    
}
