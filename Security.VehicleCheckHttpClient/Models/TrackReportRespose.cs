﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.VehicleCheckHttpClient.Models
{
    public class TrackReportRespose
    {
        public bool status { get; set; }

        public bool getStatus()
        {
            return status;
        }
    }
}
