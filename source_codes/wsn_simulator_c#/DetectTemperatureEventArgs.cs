using System;
using System.Collections.Generic;
using System.Text;

namespace Wireless_Sensor_Network_Simulator
{
  public  class DetectTemperatureEventArgs : EventArgs
    {
        // fields

        private double temperature1;
        private double humidity1;
        private double pressure1;
        //Properties
        public double Temperature1
        {
            get { return temperature1; }
        }
       public double Humidity1
        {
            get { return humidity1; }
        }
       public double Pressure1
       { get { return pressure1; } }


        //constructor

        public DetectTemperatureEventArgs(double temperature1,double humidity1,double pressure1)
        {
            this.temperature1 = temperature1;
            this.humidity1 = humidity1;
            this.pressure1 = pressure1;
        }
    }
}
