// Wireless Sensor Network.cs
// Written by David J. Stein, Esq., March 2005
//
// This module contains all of the code for the simulated wireless network. 

/////////////////////   alpha = 0.8; beta = 0.6;  Tolerance = 0.5; increament += 0.005 ; detection interval = 15

#region Using declarations

using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;

#endregion

namespace Wireless_Sensor_Network_Simulator
{

    public class WirelessSensorNetwork
    {
        // This class represents the entire wireless sensor network.

        #region Variables and initialization code
        private const int DetectionInterval = 20;
        // network data structures
        public ArrayList aSensors;															// array of sensors (network nodes)
        public ArrayList aRadar;																// array of packets that have reached the radar
        public VectorList vectors;															// list of vectors
        // network characteristics
        public int iNetworkSize = 10;													// number of nodes in the network
        public static int iMaxEnergy = 1000;									// the initial energy of every node
        public int iMaxX = 460;																// max X dimension of the map (for randomly placing nodes)
        public int iMaxY = 300;																// max Y dimension of the map (for randomly placing nodes)
        public int iDestinationX = 420;												// the location of the "end zone" (data collection uplink)
        public int iSensorRadius = 10;												// the pixel radius of a sensor for detecting a vector
        public int iSensorDelay = 50;													// the duration of the delay between packets sent by a tripped sensor
        public int iTransmitterDelay = 50;										// the duration of the packet transmission
        public int iTransmissionRadius = 50;								// the maximum communications range of any network node
        public float fTransmissionCost = 0.01f;							// the percent energy cost of sending a packet across (x) pixels - scales with distance, to be realistic
        public int iReceiveCost = 1;														// the energy cost of receiving a packet of information
        public int iSensorCost = 1;														// the energy cost of a tripped sensor
        private bool bDirectedRouting = false;							// specifies whether routing is directed or random
        public int iUpdateDelay = 0;														// the lifetime of directed routing information; i.e., how long the network waits before re-routing
        public double x1, x2, x3;																// the directed-routing network parameters
        // network simulation variables
        public bool bRunningSimulation = false;						// specifies whether or not the visual simulation is running
        public bool bPaint = false, bPainting = false;			// specifies whether or not the map should be painted, and whether or not it is currently being painted
        public bool bAbort = false;														// sepcifies whether or not the simulation thread should be aborted
        public bool bTesting = false;													// specifies whether or not endurance testing is occurring
        public DateTime timeStart;														// the amount of time the network has been running (solely useful for visual simulation)
        public int iLastUpdated = 0;														// a timer for updating routing information
        public int iProcessTime = 0;														// the number of cycles executed by the process (solely useful for endurance testing)
        public int iPacketsDelivered = 0;											// the number of packets delivered by the network
        public Thread tSimulation = null;											// the simulation thread (set to NULL when no simulation is running)
        public Random r;																				// a random number producer
        public int iSeed;																					// the last seed used (retained for the purpose of replaying a simulation, i.e., by re-seeding with this value)
        public int[] iTestResults;																// the results of each test (100-integer array)
        //public bool NormalMode = false;  
        public WirelessSensorNetwork(int iNetworkSize, int iSensorRadius, int iSensorDelay, int iTransmissionRadius, int iTransmitterDelay, float fTransmissionCost, int iReceiveCost, int iSensorCost, bool bDirectedRouting, double x1, double x2, double x3, int iUpdateDelay, int iMaxX, int iMaxY, int iDestinationX)
        {
            // this is, admittedly, a crude way to write a constructor - should be broken out into a network descriptor class - but hey, it works.
            this.iNetworkSize = iNetworkSize;
            this.iSensorRadius = iSensorRadius;
            this.iTransmissionRadius = iTransmissionRadius;
            this.iTransmitterDelay = iTransmitterDelay;
            this.iSensorDelay = iSensorDelay;
            this.fTransmissionCost = fTransmissionCost;
            this.iReceiveCost = iReceiveCost;
            this.iSensorCost = iSensorCost;
            this.bDirectedRouting = bDirectedRouting;
            this.iUpdateDelay = iUpdateDelay;
            this.iMaxX = iMaxX;
            this.iMaxY = iMaxY;
            this.iDestinationX = iDestinationX;
            this.x1 = x1;
            this.x2 = x2;
            this.x3 = x3;
            
            vectors = null;
            r = new Random((int)System.DateTime.Now.Ticks);

            // build the network
            BuildNetwork();
        }
      
        #endregion

        #region Network simulation functions

        public void BuildNetwork()
        {
            aSensors = new ArrayList();
            aRadar = new ArrayList();
            //bSensor = new ArrayList();
            while (aSensors.Count < iNetworkSize)
            {
                #region first, add random sensors to the network (keep sorted by x, and make sure no other nodes with this x/y coordinate are in the list)
                while (aSensors.Count < iNetworkSize)
                {
                    WirelessSensor sensor = new WirelessSensor(r.Next(iMaxX - 10) + 5, r.Next(iMaxY - 10) + 5, iSensorRadius);
                    int i = 0;
                    // add to list - note that it's sorted according to X coordinate; this provides an unambiguous metric for establishing routing connections (from nodes earlier in the list to nodes further down the list)
                    for (; i < aSensors.Count; i++)
                    {
                        if (((WirelessSensor)aSensors[i]).x > sensor.x)
                        {
                            aSensors.Insert(i, sensor);
                            break;
                        }
                        if ((Math.Abs(((WirelessSensor)aSensors[i]).x - sensor.x) < 14) && (Math.Abs(((WirelessSensor)aSensors[i]).y - sensor.y) < 14))
                            break;
                    }
                    if (i == aSensors.Count)  // not added to list - add at the end
                        aSensors.Add(sensor);
                }
                #endregion

                #region establish connections (wipe out all sensor connection grids)
                for (int i = 0; i < aSensors.Count; i++)
                {
                    WirelessSensor iSensor = (WirelessSensor)aSensors[i];
                    iSensor.aConnections = new ArrayList();
                    if (iSensor.x > iDestinationX)
                        iSensor.aConnections.Add(new WirelessSensorConnection(iSensor, null, (int)fTransmissionCost, 0, iTransmitterDelay));
                    else
                    {
                        for (int j = i + 1; j < aSensors.Count; j++)
                        {
                            WirelessSensor jSensor = (WirelessSensor)aSensors[j];
                            int iRadius = (int)Math.Sqrt(Math.Pow(iSensor.x - jSensor.x, 2) + Math.Pow(iSensor.y - jSensor.y, 2));
                            if (iRadius <= iTransmissionRadius)
                                iSensor.aConnections.Add(new WirelessSensorConnection(iSensor, jSensor, (int)(fTransmissionCost * iRadius / iTransmissionRadius), iReceiveCost, iTransmitterDelay));
                        }
                    }
                }
                #endregion
                // finally, weed out all sensors with no downstream connections that aren't in the destination zone
                // the trick here is to ensure that there aren't any dead spots in the network, i.e., clusters of nodes that are connected to each other but not to a node in the
                // data collector/uplink zone - we do this by removing all nodes that have no downstream connections and aren't in the uplink zone.
                ArrayList aRemoveSensors = new ArrayList();
                for (int i = aSensors.Count - 1; i >= 0; i--)
                {
                    WirelessSensor sensor = (WirelessSensor)aSensors[i];
                    if ((sensor.x < iDestinationX) && (sensor.aConnections.Count == 0))
                    {  // dead end - eliminate.
                        aRemoveSensors.Add(sensor);
                        // scan all upstream nodes, to see if they were connected to this removed node, and delete the connections
                        for (int j = i - 1; j >= 0; j--)
                        {
                            WirelessSensor sensor2 = (WirelessSensor)aSensors[j];
                            ArrayList aRemoveConnections = new ArrayList();
                            foreach (WirelessSensorConnection connection in sensor2.aConnections)
                            {
                                if (connection.sReceiver == sensor)
                                    aRemoveConnections.Add(connection);
                            }
                            foreach (WirelessSensorConnection connection in aRemoveConnections)
                                sensor2.aConnections.Remove(connection);
                        }
                    }
                }
                foreach (WirelessSensor sensor in aRemoveSensors)
                {
                    aSensors.Remove(sensor);
                }
            }
        }
        public void Reset(bool bNewSeed)
        {
            // this function resets the network so that a new simulation can be run - can either be reset with a new seed, or with the previous seed (for replay.)
            this.iProcessTime = 0;
            this.iPacketsDelivered = 0;
            foreach (WirelessSensor sensor in aSensors)
            {
                sensor.iResidualEnergy = sensor.iInitialEnergy;
                sensor.aPackets = new ArrayList();
                sensor.iSensorRadius = iSensorRadius;
                sensor.iSensorDelay = 0;
                foreach (WirelessSensorConnection connection in sensor.aConnections)
                {
                    connection.iTransmitting = 0;
                    connection.packet = null;
                }
            }
            aRadar = new ArrayList();
            if (bDirectedRouting == true)
                SetRoutingInformation();
            iLastUpdated = iUpdateDelay;
            if (bNewSeed == true)
                this.iSeed = (int)System.DateTime.Now.Ticks;
            r = new Random(iSeed);
        }

        public void RunSimulation()
        {
            // this function is the heart of the visual simulation thread. It runs until bAbort is set to true - this is a faster and cleaner stop mechanism than using thread.Abort().
            bAbort = false;
            bRunningSimulation = true;
            timeStart = System.DateTime.Now;
            this.vectors = new VectorList(iMaxX, iMaxY, r);
            while (bAbort == false)
            {
                Process(true);
                System.Threading.Thread.Sleep(40); // sensing period
            }
            bRunningSimulation = false;
        }

        public void RunTest()
        {
            // this function runs the endurance test.
            bTesting = true;
            iTestResults = new int[100];
            bAbort = false;
            bRunningSimulation = true;
            timeStart = System.DateTime.Now;
            //this.vectors = new VectorList(iMaxX, iMaxY, r);
            for (int i = 0; i < 100 && bAbort == false; i++)
            {
                Reset(true);
                while (Process(false) == true)
                    iTestResults[i]++;
            }
            bRunningSimulation = false;
            bTesting = false;
        }

        public bool Process(bool bSleep)
        {
            // this is the core of a network simulation - all of the network processes for one step of the simulation occur here. returns a bool to indicate that the
            // network is alive (all nodes active) or dead (partitioned by one or more dead nodes.)
            //ArrayList temperatures = new ArrayList(100);
            bool bNetworkLives = true;
           
            iProcessTime++;
            // update vectors (note use of mutex)
          /*  vectors.mutexVector.WaitOne();
            vectors.Update();
            while (vectors.aVectors.Count < 0)        // no. of vectors
                vectors.AddVector();
            vectors.mutexVector.ReleaseMutex();*/
            #region update sensors (create a new packet if sensor is active and triggered by a vector)
           
           WirelessSensor   B = new WirelessSensor();
            
            foreach (WirelessSensor sensor1 in aSensors)
            {
               
                if (sensor1.iResidualEnergy > 0)
                {
                    if (sensor1.iSensorDelay > 0)
                        sensor1.iSensorDelay--;
                    else
                    {
                  //MessageBox.Show((sensor1.Temperature1 + "   " + sensor1.Prediction).ToString());
                      


                        if (Math.Abs(sensor1.Temperature1 - Math.Abs(sensor1.Prediction)) > .5 || Math.Abs(sensor1.Humidity1 - sensor1.Predicthumidity) > 0.5 || Math.Abs(sensor1.Pressure1 - sensor1.Predictpressure) > 0.5)
                        {
                            /*B.Beta += 0.05;
                            if (B.Beta >= 1)
                                B.Beta = 0.4;*/
                            if (sensor1.iResidualEnergy <= iSensorCost)  // sensing failed due to sensor cost - sensor is now dead
                                sensor1.iResidualEnergy = 0;
                            else
                            {  // create packet
                                sensor1.iSensorDelay = iSensorDelay;
                                sensor1.iResidualEnergy -= iSensorCost;
                                sensor1.aPackets.Add(new Packet(sensor1.x, sensor1.y, sensor1.Temperature1, sensor1.Humidity1,sensor1.Pressure1, sensor1.iSensorDelay));
                                
                            }
                        }
                }
             }  
                    else
                    { bNetworkLives = false;}
            

            }
        

            #region start new transmisions for any pending packets
            foreach (WirelessSensor sensor in aSensors)
            {
                if (sensor.aPackets.Count > 0)
                {
                    if (sensor.aConnections.Count > 0)
                    {
                        if ((bDirectedRouting == true) && (sensor.connectionCurrent != null))
                            sensor.connectionCurrent.BeginTransmission();
                        else  // random routing
                            ((WirelessSensorConnection)sensor.aConnections[r.Next(sensor.aConnections.Count)]).BeginTransmission();
                    }
                }
            }
            #endregion

            #region carry out transmissions across each connection
            foreach (WirelessSensor sensor in aSensors)
            {
                foreach (WirelessSensorConnection connection in sensor.aConnections)
                {
                    Packet packet = connection.Transmit();
                    if (packet != null)
                    {
                        aRadar.Add(packet);
                        iPacketsDelivered++;
                    }
                }
            }
            #endregion

            #region if using directed routing, update connection information
            if (bDirectedRouting == true)
            {
                if ((--iLastUpdated) <= 0)
                {
                    iLastUpdated = iUpdateDelay;
                    SetRoutingInformation();
                }
            }
            #endregion

            // note the end of the process - tell the map to paint itself, and sleep for 10ms if this is a timed simulation
            if (bPainting == false)
                bPaint = true;
            if (bSleep == true)
                System.Threading.Thread.Sleep(100);
            return bNetworkLives;

        }


        public void SetRoutingInformation()
        {
            // this function updates directed-routing selections
            foreach (WirelessSensor sensor in aSensors)
            {
                if (sensor.iResidualEnergy > 0)
                {
                    // choose best node given current conditions
                    sensor.connectionCurrent = null;
                    double dBestCost = 0;
                    foreach (WirelessSensorConnection connection in sensor.aConnections)
                    {
                        if (connection.sReceiver == null)
                        {  // if this is an uplink connection, always select it
                            sensor.connectionCurrent = connection;
                            break;
                        }
                        else if (connection.sReceiver.iResidualEnergy > 0)
                        {
                            double dCost = Math.Pow(connection.iTransmitCost, x1) * Math.Pow(connection.sSender.iResidualEnergy, -x2) * Math.Pow(connection.sSender.iInitialEnergy, x3) + Math.Pow(connection.iReceiveCost, x1) * Math.Pow(connection.sReceiver.iResidualEnergy, -x2) * Math.Pow(connection.sReceiver.iInitialEnergy, x3);
                            if ((sensor.connectionCurrent == null) || (dCost < dBestCost))
                            {
                                dBestCost = dCost;
                                sensor.connectionCurrent = connection;
                            }
                        }
                    }
                }
            }
        }
    }
        #endregion



    public class WirelessSensor
    {
        // This class represents a single node in a wireless sensor network.

        #region Variables and initialization code

        public ArrayList aPackets = null;																// an array of packets held by this node
        public ArrayList aConnections;																	// an array of connections to downstream nodes (*always* downstream, not upstream) - nodes in the data collector/uplink zone have a connection with a "null" receiver node
        public WirelessSensorConnection connectionCurrent;			// if using directed routing, the currently preferred network connection for transmitting
        public int x, y;                                                // the coordinates of the node
        private const int MaxCapcity = 500;
        private probe probe;
        private double temperature1 = 0;
        private double prediction = 0;
        private double humidity1 = 0;
        private double predicthumidity;
        private double pressure1;
        private double predictpressure;
        private double verylow;
        private double low;
        private double high;
        private double veryhigh;
        private double alpha = 0.8;
        private double beta = 0.6;
        private double level1 = Math.Abs(50 * Math.Sin(0));
        private double trend1 = 0;
        private double level2 = Math.Abs(50 * Math.Sin(0));
        private double trend2 = 0;
        private double level3 = Math.Abs(50 * Math.Sin(0));
        private double trend3 = 0;

        public ArrayList levels3 = null;
        public ArrayList trends3 = null;
        public ArrayList levels2 = null;
        public ArrayList trends2 = null;
        public ArrayList levels1 = null;
        public ArrayList trends1 = null;
        public ArrayList predictpressures = null;
        public ArrayList pressures = null;
        public ArrayList predicthumidities = null;
        public ArrayList humidities = null;
        public ArrayList temperatures1 = null;
        public ArrayList predictions = null;
        public int iInitialEnergy;																					// the initial power of the node
        public int iResidualEnergy;																			// the current power of the node
        public int iSensorDelay = 0;																			// the timer until the sensor is ready to be tripped again
        public int iSensorRadius;																				// the radius of this sensor

        // constructor that takes 0 arguments

        public WirelessSensor()
        { }
        public double Beta
        {
            get { return beta; }
            set { beta = value; }
        }
        public double Temperature1
        {
            get { return temperature1; }
        }

        public double Humidity1
        {
            get { return humidity1; }
        }

        public double Prediction
        {
            get { return prediction; }
        }
        public double Predicthumidity
        {
            get { return predicthumidity; }
        }
        public double Predictpressure
        {
            get { return predictpressure; }
        }

        public double Pressure1
        { get { return pressure1; } }

        public double Verylow
        { get { return verylow; } }

        public double Low
        { get { return low; } }

        public double High
        { get { return high; } }

        public double Veryhigh
        { get { return veryhigh; } }


        public WirelessSensor(int x, int y, int iSensorRadius)
        {
            this.x = x;
            this.y = y;

            temperatures1 = ArrayList.Synchronized(new ArrayList(MaxCapcity));
            predictions = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            humidities = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            predicthumidities = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            pressures = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            predictpressures = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            trends1 = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            levels1 = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            trends2 = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            levels2 = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            trends3 = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            levels3 = ArrayList.Synchronized(new ArrayList((MaxCapcity)));
            this.iSensorRadius = iSensorRadius;
            aConnections = new ArrayList();
            connectionCurrent = null;
            iInitialEnergy = iResidualEnergy = WirelessSensorNetwork.iMaxEnergy;

            probe = new probe();
            probe.DetectTemperature += new EventHandler<DetectTemperatureEventArgs>(OnDetectTemperature);

            probe.start();

        }

        public void OnDetectTemperature(object sender, EventArgs args)
        {

            if (args is DetectTemperatureEventArgs)
            {


                DetectTemperatureEventArgs e = args as DetectTemperatureEventArgs;

                // Temperature

                temperature1 = e.Temperature1;
                temperatures1.Add(temperature1);

                double x = trend1;       // trends[i-1]
                double y = level1;        // levels[i-1] 
                trend1 = (alpha * temperature1) + ((1 - alpha) * (y + x));
                level1 = ((trend1 - x) * beta) + ((1 - beta) * y);
                prediction = level1 + trend1;
                predictions.Add(prediction);
                levels1.Add(level1);
                trends1.Add(trend1);

                if (trends1.Capacity == MaxCapcity)
                {
                    trends1.RemoveAt(0);
                }
                if (levels1.Capacity == MaxCapcity)
                {
                    levels1.RemoveAt(0);
                }
                if (temperatures1.Capacity == MaxCapcity)
                {
                    temperatures1.RemoveAt(0);
                }

                if (predictions.Capacity == MaxCapcity)
                {
                    predictions.RemoveAt(0);
                }
     

                // Humidity

                humidity1 = e.Humidity1;
                humidities.Add(humidity1);

                double n = trend2;       // trends[i-1]
                double m = level2;        // levels[i-1] 
                trend2 = (alpha * humidity1) + ((1 - alpha) * (double)(m + n));
                level2 = ((trend2 - n) * beta) + ((1 - beta) * m);
                predicthumidity = level2 + trend2;
                levels2.Add(level2);
                trends2.Add(trend2);
                predicthumidities.Add(predicthumidity);
                if (trends2.Capacity == MaxCapcity)
                {
                    trends2.RemoveAt(0);
                }
                if (levels2.Capacity == MaxCapcity)
                {
                    levels2.RemoveAt(0);
                }

                if (humidities.Capacity == MaxCapcity)
                {
                    humidities.RemoveAt(0);
                }

                if (predicthumidities.Capacity == MaxCapcity)
                {
                    predicthumidities.RemoveAt(0);
                }



                // Pressure

                pressure1 = e.Pressure1;
                pressures.Add(pressure1);

                double p = trend3;       // trends[i-1]
                double q = level3;        // levels[i-1] 
                trend3 = (alpha * pressure1) + ((1 - alpha) * (double)(p + q));
                level3 = ((trend3 - p) * beta) + ((1 - beta) * q);
                predictpressure = level3 + trend3;
                levels3.Add(level3);
                trends3.Add(trend3);
                predictpressures.Add(predictpressure);

                if (trends3.Capacity == MaxCapcity)
                {
                    trends3.RemoveAt(0);
                }
                if (levels3.Capacity == MaxCapcity)
                {
                    levels3.RemoveAt(0);
                }

                if (pressures.Capacity == MaxCapcity)
                {
                    pressures.RemoveAt(0);
                }

                if (predictpressures.Capacity == MaxCapcity)
                {
                    predictpressures.RemoveAt(0);
                }



                // set the linguistic variables

                if (temperature1 >= 0 && temperature1 <= 5)
                    verylow = 1;
                else if (temperature1 > 5 && temperature1 <= 10)
                    verylow = (2 - (temperature1 / 5));
                else if (temperature1 > 10)
                    verylow = 0;


                if (temperature1 < 5)
                    low = 0;
                else if (temperature1 >= 5 && temperature1 < 15)
                    low = (temperature1 / 10) - 0.5;
                else if (temperature1 >= 15 && temperature1 <= 25)
                    low = 2.5 - (0.1 * temperature1);
                else if (temperature1 > 25)
                    low = 0;


                if (temperature1 > 20)
                    high = 0;
                else if (temperature1 >= 20 && temperature1 < 30)
                    high = (0.1 * temperature1) - 2;
                else if (temperature1 >= 30 && temperature1 < 40)
                    high = 4 - (0.1 * temperature1);
                else if (temperature1 >= 40)
                    high = 0;

                if (temperature1 <= 35)
                    veryhigh = 0;
                else if (temperature1 > 35 && temperature1 < 40)
                    veryhigh = (temperature1 / 5) - 7;
                else if (temperature1 >= 40 && temperature1 <= 50)
                    veryhigh = 1;
                else if (temperature1 > 50)
                    veryhigh = 0;



            }


        }

    }
        #endregion



    public class WirelessSensorConnection
    {
        // This class represents a communications link between two wireless sensors.

        #region Variables and initialization code

        public WirelessSensor sSender;							// the upstream sensor
        public WirelessSensor sReceiver;						// the downstream sensor - every node in the data collector/uplink zone will have a connection with a NULL sReceiver.
        public Packet packet = null;									// the packet currently being transmitted on this connection (only one at a time, of course)
        public int iTransmitCost, iReceiveCost;			// the energy costs of transmitting and receiving the packet
        public int iTransmitterDelay;									// the total time this node would normally wait to complete delivery of a packet
        public int iTransmitting;												// the timer for completing delivery of a packet

        public WirelessSensorConnection(WirelessSensor sSender, WirelessSensor sReceiver, int iTransmitCost, int iReceiveCost, int iTransmitterDelay)
        {
            this.sSender = sSender;
            this.sReceiver = sReceiver;
            this.iTransmitCost = iTransmitCost;
            this.iReceiveCost = iReceiveCost;
            this.iTransmitting = 0;
            this.iTransmitterDelay = iTransmitterDelay;
        }

        #endregion

        #region Connection simulation functions

        public void BeginTransmission()
        {
            // this function begins transmission of a data packet between these nodes.
            if ((sSender.aPackets.Count > 0) && (iTransmitting == 0))
            {
                if (sSender.iResidualEnergy <= iTransmitCost)  // transmission failed - sender has run out of energy
                    sSender.iResidualEnergy = 0;
                else if ((sReceiver != null) && (sReceiver.iResidualEnergy <= iReceiveCost))  // transmission failed - receiver has run out of energy
                    sReceiver.iResidualEnergy = 0;
                else
                {  // success - accept the packet and start transmitting it.
                    iTransmitting = iTransmitterDelay;
                    sSender.iResidualEnergy -= iTransmitCost;
                    packet = (Packet)sSender.aPackets[0];
                    sSender.aPackets.RemoveAt(0);
                    if (sReceiver != null)
                        sReceiver.iResidualEnergy -= iReceiveCost;
                }
            }
        }

        public Packet Transmit()
        {
            // this function continues transmission of a previously accepted packet, and completes transmission if appropriate.
            if ((sSender.iResidualEnergy <= 0) || ((sReceiver != null) && (sReceiver.iResidualEnergy <= 0)))  // failed due to depleted energy
                iTransmitting = 0;
            else if (iTransmitting > 0)
            {  // transmission in progress
                iTransmitting--;
                if (iTransmitting == 0)
                {
                    if (sReceiver != null)
                        sReceiver.aPackets.Add(packet);
                    else
                    {
                        Packet returnPacket = packet;
                        packet = null;
                        return returnPacket;
                    }
                }
            }
            return null;
        }

        #endregion

    }

    public class VectorList
    {
        // This class represents (and manages) a list of vectors.

        #region Variables and initialization code

        public ArrayList aVectors;			// the array of vectors
        public Mutex mutexVector;		// a mutex to control access to the vector list
        private int iMaxX, iMaxY;				// the maximum X and Y coordinates of a vector
        private Random r;								// a random number generator (the same one used by the WirelessSensorNetwork object that houses this object)

        public VectorList(int iMaxX, int iMaxY, Random r)
        {
            this.iMaxX = iMaxX;
            this.iMaxY = iMaxY;
            aVectors = new ArrayList();
            mutexVector = new Mutex();
            this.r = r;
        }

        #endregion

        #region Vector list management functions

        public void AddVector()
        {
            // this function creates a new vector and adds it to the vector list
            int iBorder = r.Next(4);
            if (iBorder == 0)  // add to left border, traveling right and up or down
                aVectors.Add(new Vector(-5, r.Next(iMaxY), r.Next(3) + 1, r.Next(7) - 3));
            else if (iBorder == 1)  // add to top border, traveling down and left or right
                aVectors.Add(new Vector(r.Next(iMaxX), -5, r.Next(7) - 3, r.Next(3) + 1));
            else if (iBorder == 2)  // add to right border, traveling left and up or down
                aVectors.Add(new Vector(iMaxX + 5, r.Next(iMaxY), r.Next(3) - 3, r.Next(7) - 3));
            else if (iBorder == 3)  // add to bottom border, traveling up and left or right
                aVectors.Add(new Vector(r.Next(iMaxX), iMaxY + 5, r.Next(7) - 3, r.Next(3) - 3));
        }

        public void Update()
        {
            // this function moves the vectors around, and removes those that have traveled out of bounds
            ArrayList aRemoveVectors = new ArrayList();
            foreach (Vector vector in aVectors)
            {
                vector.x += vector.dx;
                vector.y += vector.dy;
                if ((vector.x < -5) || (vector.y < -5) || (vector.x > iMaxX + 5) || (vector.y > iMaxY + 5))
                    aRemoveVectors.Add(vector);
            }
            foreach (Vector vector in aRemoveVectors)
                aVectors.Remove(vector);
        }

        #endregion

    }

    public class Vector
    {
        // A lightweight class - really just a structure - for a vector.

        #region Variables and initialization code

        public int x, y, dx, dy;
        public Vector(int x, int y, int dx, int dy)
        {
            this.x = x;
            this.y = y;
            this.dx = dx;
            this.dy = dy;
        }

        #endregion

    }

    public class Packet
    {
        // A lightweight class - really just a data structure - for a packet.

        #region Variables and initialization code
        public int x;
        public int y;
        public double pressure;
        public double temperature;
        public double humidity;
        public int timestamp, lifespan;    // the amount of time left to display the packet on the radar (once it gets there), and the total time of displaying it
        public Packet(int x, int y, double temperature,double humidity,double pressure, int timestamp)
        {
            this.temperature = temperature;         //This: To distinguish the instance variables and method parameters or constructor parameters having the same name.
            this.x = x;
            this.y = y;
            this.timestamp = 0;
            this.lifespan = timestamp;
            this.humidity = humidity;
            this.pressure = pressure;
        }

        #endregion

    }

    public class probe
    {
        #region Private Fields
        private const int AbsoluteZero = 0;
        private const int MaxTemperature = 50;
        private const int Maxhum = 70;
        private const int Minhum = 30;
        private const int Minpres = 20;
        private const int Maxpres = 60;
        private const int DetectionInterval = 15;
        private Thread detector;
        private double y = 0;
        private int integer = 18;
        private int counter1 = 0;  // change sign + or -
        private double increament = 2;  // change the constant
        private int constant = 50;
        

        #endregion

        #region Public events
        public event EventHandler<DetectTemperatureEventArgs> DetectTemperature;
        #endregion


        #region public constructor

        public probe()
        { }

       
        #endregion


        #region public methods

        public void start()
        {
            detector = new Thread(new ThreadStart(detect));
            detector.Start();
            detector.IsBackground = true;


        }

        /* public void stop()
         {
             detector.Join(0);
         }   */
        #endregion

        #region private methods

        private void detect()
        {
            double temperature1;
          double humidity1;
          double pressure1; 
            Random random = new Random();
            
            while (true)
            {
                try
                {
                 /*temperature1 = random.Next(AbsoluteZero, MaxTemperature);
                    humidity1 = random.Next(Minhum, Maxhum);
                    pressure1 = random.Next(Minpres, Maxpres);*/
                    
                   temperature1 = random.NextDouble() + Math.Abs( constant* Math.Sin(increament));  // random represent noise
                   humidity1 = random.NextDouble()    + Math.Abs(constant * Math.Sin(increament));
                   pressure1 = random.NextDouble()    + Math.Abs(constant * Math.Sin(increament));
                    increament += 0.005;


              

                    }
                catch (ArgumentOutOfRangeException)
                {
                    temperature1 = MaxTemperature;
                    humidity1 = MaxTemperature;
                    pressure1 = MaxTemperature; 
                }
                DetectTemperatureEventArgs args = new DetectTemperatureEventArgs(temperature1,humidity1,pressure1);


                if (DetectTemperature != null)
                {
                    DetectTemperature(this, args);
                }
                Thread.Sleep(DetectionInterval);
                                  
                
            }

        }
        private double GetRandomValue()
        {

                Random r = new Random();
            for (; ; )
            {
              

                y = integer + r.NextDouble();
                counter1++;
                if (counter1 == 30)
                {
                    y = integer - r.NextDouble();
                    counter1 = 0;
                    //increament++;
                    integer++;
                }

            
                return y;
                }
            }   
        
                #endregion
    }
}
    







    


// Wireless Sensor Network Simulator v2.0 (and all future versions), together with all accompanying source code and documentation, is copyright (c) David J. Stein, Esq., March 2005. All rights reserved except as set forth in the included Wireless Sensor Network License Agreement.
#endregion 