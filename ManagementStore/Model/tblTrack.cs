
using System;

namespace ManagementStore.Model
{
    class tblTrack
    {
        public int Id { get; set; }
        public int trackNumber { get; set; }
        public int vehicleId { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public double free { get; set; }
        public int siteId { get; set; }
        public double locationX { get; set; }
        public double locationY { get; set; }
        public int userId { get; set; }
        public string detectInFace { get; set; }
        public string detectOutFace { get; set; }
        public string plateIn { get; set; }
        public string plateOut { get; set; }
        public string status { get; set; }
        public DateTime? currentTime { get; set; }

    }

    class tblVehicle
    {
        public int Id { get; set; }
        public string plateNum { get; set; }
        public string detectOutFace { get; set; }
        public string typePlate { get; set; }
        public string status { get; set; }

        public string userId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
      
        public string vehiclePhotoPath { get; set; }
        public string licensePhotoPath { get; set; }
        public bool useYN { get; set; }
    }
}
