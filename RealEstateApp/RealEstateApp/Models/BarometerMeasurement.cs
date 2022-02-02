namespace RealEstateApp.Models
{
    public class BarometerMeasurement
    {
        public double Pressure { get; set; }
        public double Altitude { get; set; }
        public string Label { get; set; }
        public double HeightChange { get; set; }

        public string Display => $"{Label}: {Altitude:N2}m";

        public BarometerMeasurement()
        {
            
        }
        public BarometerMeasurement(double p, double a, string l, double h)
        {
            Pressure = p;
            Altitude = a;
            Label = l;
            if (h != 0) HeightChange = h;
        }
    }
}