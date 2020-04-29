using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirectoryService.Dtos
{
    public class AvaiblesDto
    {
        public List<int> avaibles;
        

        public AvaiblesDto(List<int> avaibleVehicleIDs)
        {
            this.avaibles = avaibleVehicleIDs;
        }
    }
}