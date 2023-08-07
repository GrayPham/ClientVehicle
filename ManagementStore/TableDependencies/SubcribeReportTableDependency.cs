using ManagementStore.Form.Camera;
using ManagementStore.Model;
using Parking.App.Common.Helper;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using TableDependency.EventArgs;
using TableDependency.SqlClient;

namespace ManagementStore.TableDependencies
{
    class SubcribeReportTableDependency
    {
        SqlTableDependency<tblTrack> tableDependency;
        PictureControl _pictureControl;
        public SubcribeReportTableDependency(PictureControl pictureControl)
        {
            _pictureControl = pictureControl;
        }


        public void SubcribeTableDependency()
        {
            string connectionString = "Data Source=26.115.12.45;Initial Catalog=KIOSK;User ID=parkingai;Password=thien123";
            tableDependency = new SqlTableDependency<tblTrack>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();


        }

        private void TableDependency_OnChanged(object sender, RecordChangedEventArgs<tblTrack> e)
        {
            if (e.ChangeType == TableDependency.Enums.ChangeType.Update && e.Entity.status == "VIHE03")
            {

                // Check Vehicle LP
                string plateNum = GetPlateNumByVehicle(e.Entity.vehicleId);
                string plateNumReport = _pictureControl.label1.Text;
                if(plateNum == plateNumReport)
                {
                    _pictureControl.cEditInVehicle.Checked = true;
                    _pictureControl.cEditInVehicle.ForeColor = Color.Black;
                    Helpers.PlaySound(@"Assets\\DefaultAudio\TrackSuccessful.wav");
                    Task.Delay(5000);
                    _pictureControl.cEditInVehicle.Checked = false;
                    _pictureControl.accReport = false;
                }


                // Notify for user here & event open gate
                Console.WriteLine("Initial dependency", plateNum);
            }
        }

        private void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(tblVehicle)} SQLTableDependency error: {e.Error.Message}");
        }


        public string GetPlateNumByVehicle(int vehicle)
        {
            try
            {
                string connectionString = "Data Source=26.115.12.45;Initial Catalog=KIOSK;User ID=parkingai;Password=thien123";
                string plateNum = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Execute a SQL query
                    string sqlQuery = $"SELECT TOP 1 * FROM tblVehicle WHERE id = {vehicle}";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Access data from the reader
                                plateNum = reader["plateNum"].ToString();

                            }
                        }
                    }

                    connection.Close();
                }

                return plateNum;
            }
            catch
            {
                return "0";
            }

        }
    }
}
