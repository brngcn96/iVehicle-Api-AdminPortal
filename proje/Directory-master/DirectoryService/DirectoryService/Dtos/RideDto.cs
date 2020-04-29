using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;

namespace DirectoryService.Dtos
{
    public class RideDto
    {

        public int Ride_ID { get; set; }
        public int User_ID { get; set; }
        public double Cost { get; set; }
        public System.DateTime StartTime { get; set; }
        public bool Status { get; set;}



    }
}