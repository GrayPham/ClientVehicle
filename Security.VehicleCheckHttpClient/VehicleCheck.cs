using Newtonsoft.Json;
using Security.VehicleCheckHttpClient.Interface;
using Security.VehicleCheckHttpClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Security.VehicleCheckHttpClient.Common;
using System.Drawing.Imaging;
using Parking.App.Common;

namespace Security.VehicleCheckHttpClient
{
    public class VehicleCheck: IVehicleManagement
    {
        HttpClient client = new HttpClient();
        public string ConvertImageToBase64(Image image)
        {
            MemoryStream ms = new MemoryStream();
            
            image.Save(ms, ImageFormat.Jpeg); // You can use other formats like ImageFormat.Png, ImageFormat.Gif, etc.
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            
        }

        public async Task<string> CheckInVehicleAsync(string platenumber, Image faceImage, Image lpImage, string typeTransport = "car", string typeLP = "2")
        {
            // Convert the byte array to a base64 string
            string base64Image = ConvertImageToBase64(faceImage);
            string base64Imagelp = ConvertImageToBase64(lpImage);
            // Create a JSON payload containing the base64 image data

            // Convert the base64 image to a JSON payload
            var payload = JsonConvert.SerializeObject(new { platenum = platenumber, typeTransport = typeTransport, typeLicensePlate = typeLP, stringFace = base64Image, stringlp = base64Imagelp, siteId= ConfigClass.StoreNo });
            // Send the payload to the FastAPI server using an HTTP POST request
            // Gọi đến API kiểm tra xe ra vào
            using (var client = new HttpClient())
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://26.115.12.45:8005/api/track/trackingVehicle", content);
                //string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    // Read the response as a string
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TrackVehicleRespose>(responseString);
                    if (result.status != "VEHI01")
                        return "Successful";
                    else
                        return "Block";
                }
                else
                {
                    return "Error";
                }
            }
                
        }
        public async Task<bool> TrackReportAsync(Image imagePlate, Image imageFace,string platenum, string typeTransport = "car", string typeLP = "2")
        {
            // Convert the byte array to a base64 string
            string base64Image = ConvertImageToBase64(imageFace);
            string base64Imagelp = ConvertImageToBase64(imagePlate);
            // Convert the base64 image to a JSON payload
            var payload = JsonConvert.SerializeObject(new { platenum = platenum,
                                                            typeTransport = typeTransport, 
                                                            typeLicensePlate = typeLP, 
                                                            stringFace = base64Image, 
                                                            stringlp = base64Imagelp, 
                                                            siteId = ConfigClass.StoreNo});

            using (var client = new HttpClient())
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://26.115.12.45:8005/api/track/trackingReports", content);
                //string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    // Read the response as a string
                    var responseString = await response.Content.ReadAsStringAsync();
                    TrackReportRespose result = JsonConvert.DeserializeObject<TrackReportRespose>(responseString);
                    if (result.getStatus() != false)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }

        }

    }
}
