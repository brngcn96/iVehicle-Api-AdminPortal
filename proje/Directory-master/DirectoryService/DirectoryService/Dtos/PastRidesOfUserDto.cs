using DirectoryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirectoryService.Dtos
{
    public class PastRidesOfUserDto
    {

        private Model1Container context = new Model1Container();

        List<Ride> pastRidesOfUser = new List<Ride>();

        public void FillPastRidesOfUser(int id)
        {
            pastRidesOfUser = (from c in context.Ride where c.User_ID == id select c).ToList();

        }



    }
}