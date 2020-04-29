using DirectoryService.Dtos;
using DirectoryService.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Text;
using System.Web.Http;


namespace DirectoryService.Controllers
{
    

    [Authorize]
    public class AdminController : ApiController
    {
        private double costRatio = 0.4;
        private Model1Container context = new Model1Container();


        // GET api/Admin/GetOneUserRides?
        [HttpGet]
        [ActionName("GetOneUserRides")]

        public IEnumerable<Ride> GetOneUserRides(int userID)
        {
            List<Ride> rides = (from c in context.Ride where c.User_ID == userID  select c).ToList();

            return rides;


        }

        // GET api/Admin/GetAllPastRidesByPage?
        [HttpGet]
        [ActionName("GetAllPastRidesByPage")]
        public ResponseObjectDto GetAlPastRidesByPage(int pageIndex, int pageSize)
        {

            List<Ride> rideslist = (from c in context.Ride where c.Status == false select c).ToList();


            if (rideslist != null && rideslist.Count > 0)
            {
                return new ResponseObjectDto(rideslist, 1, "Accepted", 200);

            }
            else
                return new ResponseObjectDto(rideslist, 1, "No Content", 202);

        }

        // GET api/Admin/GetAllVehiclesByPage?
        [HttpGet]
        [ActionName("GetAllVehiclesByPage")]
        public ResponseObjectDto GetAllVehiclesByPage(int pageIndex, int pageSize)
        {
            if (!ModelState.IsValid)
                return new ResponseObjectDto(null, 0, "Bad Request", 400);

            List<Vehicle> vehiclelist = (from c in context.Vehicle
                                       where c.Status == true
                                       select c).ToList();
            

            if (vehiclelist != null && vehiclelist.Count > 0)
            {

                return new ResponseObjectDto(vehiclelist, 1, "Accepted", 200);

            }
            else
                return new ResponseObjectDto(vehiclelist, 1, "No Content", 202);

        }


        // GET api/Admin/GetAllRidesByPage?
        [HttpGet]
        [ActionName("GetAllRidesByPage")]
        public ResponseObjectDto GetAllRidesByPage(int pageIndex,int pageSize)
        {

            List<RideDto> rideslist = (from c in context.Ride where c.Status == true select new RideDto
            {
                Ride_ID = c.Ride_ID,
                User_ID=c.User_ID,              
                StartTime=c.StartTime,



            }).ToList();


            if(rideslist!=null && rideslist.Count>0)
            {
                return new ResponseObjectDto(rideslist, 1, "Accepted", 200);

            }
            else
                return new ResponseObjectDto(rideslist, 1, "No Content", 202);

        }

        //POST api/Admin/AddVehicle?
        [HttpPost]
        [ActionName("AddVehicle")]

        public ResponseObjectDto AddVehicle([Required] String name,[Required] int stationID,[Required] int position_in_station,bool status)
        {

            if (!ModelState.IsValid)
                return new ResponseObjectDto(null, 0, "Bad Request", 400);

            try
            {


                Vehicle newVehicle = new Vehicle() {

                    CreationDate = DateTime.Now,
                    isReserved = false,
                    Status = status,
                    Name=name,
                    Station_ID=stationID,
                    Position_in_Station=position_in_station,
                };
                context.Vehicle.Add(newVehicle);
                context.SaveChanges();

                return new ResponseObjectDto(newVehicle, 1, "Accepted", 200);
            }
            catch (Exception exc)
            {
                return new ResponseObjectDto(null, 0, "Bad Request", 400);
            }


          



        }

        // PUT api/Admin/UpdateRide?
        [HttpPut]
        [ActionName("UpdateRide")]
        public void UpdateRide(int id, [FromBody] string value)
        {


        }

        // DELETE api/Admin/StopRide?
        [HttpDelete]
        [ActionName("StopRide")]
        public void StopRide(int id)
        {
            Ride  ride = (from c in context.Ride where c.Status == true select c).FirstOrDefault();
            if (ride != null)
            {
                ride.Status = false;
                context.SaveChanges();
            }
        }

        public Ride FinishRide(int rideID,int userID,double costRatio)
        {

            double cost;
            if (!ModelState.IsValid)
                return null;

            Ride ride = (from c in context.Ride where c.Ride_ID == rideID  select c).FirstOrDefault();
            User user = (from c in context.User where c.User_ID == userID  select c).FirstOrDefault();
            if (ride != null && user!=null)
            {
                cost=CalculateCost(ride.StartTime, DateTime.Now, costRatio);
                ride.FinishTime = DateTime.Now;
                ride.Status = false;
                ride.Cost = cost;
                user.Credit = user.Credit - cost;
            }
            else
                return null;

            context.SaveChanges();
            return ride;
        }


        public double CalculateCost(DateTime startTime,DateTime finishTime,double costRatio)
        {

            double cost;
            TimeSpan duration = finishTime - startTime;

            int durationMinutes = (int)duration.TotalMinutes;

            cost = durationMinutes * costRatio;
            return cost;
        }



    }
}
