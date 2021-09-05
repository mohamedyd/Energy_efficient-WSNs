// Wireless Sensor Network.cs
// Written by David J. Stein, Esq., March 2005
//
// This module contains all of the code for the simulated wireless network. 

#region Using declarations

using System;
using System.Collections;
using System.Threading;

#endregion

namespace Wireless_Sensor_Network_Simulator {

	public class WirelessSensorNetwork	{
		// This class represents the entire wireless sensor network.

		#region Variables and initialization code

		// network data structures
		public ArrayList aSensors;															// array of sensors (network nodes)
		public ArrayList aRadar;																// array of packets that have reached the radar
		public VectorList vectors;															// list of vectors
		// network characteristics
		public int iNetworkSize =10;													// number of nodes in the network
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

		public WirelessSensorNetwork(int iNetworkSize, int iSensorRadius, int iSensorDelay, int iTransmissionRadius, int iTransmitterDelay, float fTransmissionCost, int iReceiveCost, int iSensorCost, bool bDirectedRouting, double x1, double x2, double x3, int iUpdateDelay, int iMaxX, int iMaxY, int iDestinationX)	{
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
			r = new Random((int) System.DateTime.Now.Ticks);

			// build the network
			BuildNetwork();
		}

		#endregion

		#region Network simulation functions

		public void BuildNetwork() {
			aSensors = new ArrayList();
			aRadar = new ArrayList();
			while (aSensors.Count < iNetworkSize) {
				// first, add random sensors to the network (keep sorted by x, and make sure no other nodes with this x/y coordinate are in the list)
				while (aSensors.Count < iNetworkSize) {
					WirelessSensor sensor = new WirelessSensor(r.Next(iMaxX - 10) + 5, r.Next(iMaxY - 10) + 5, iSensorRadius);
					int i = 0;
					// add to list - note that it's sorted according to X coordinate; this provides an unambiguous metric for establishing routing connections (from nodes earlier in the list to nodes further down the list)
					for (; i < aSensors.Count; i++) {
						if (((WirelessSensor) aSensors[i]).x > sensor.x) {
							aSensors.Insert(i, sensor);
							break;
						}
						if ((Math.Abs(((WirelessSensor) aSensors[i]).x - sensor.x) < 14) && (Math.Abs(((WirelessSensor) aSensors[i]).y - sensor.y) < 14))
							break;
					}
					if (i == aSensors.Count)  // not added to list - add at the end
						aSensors.Add(sensor);
				}
				// establish connections (wipe out all sensor connection grids)
				for (int i = 0; i < aSensors.Count; i++) {
					WirelessSensor iSensor = (WirelessSensor) aSensors[i];
					iSensor.aConnections = new ArrayList();
					if (iSensor.x > iDestinationX)
						iSensor.aConnections.Add(new WirelessSensorConnection(iSensor, null, (int) fTransmissionCost, 0, iTransmitterDelay));
					else {
						for (int j = i + 1; j < aSensors.Count; j++) {
							WirelessSensor jSensor = (WirelessSensor) aSensors[j];
							int iRadius = (int) Math.Sqrt(Math.Pow(iSensor.x - jSensor.x, 2) + Math.Pow(iSensor.y - jSensor.y, 2));
							if (iRadius <= iTransmissionRadius)
								iSensor.aConnections.Add(new WirelessSensorConnection(iSensor, jSensor, (int) (fTransmissionCost * iRadius /iTransmissionRadius), iReceiveCost, iTransmitterDelay));
						}
					}
				}
				// finally, weed out all sensors with no downstream connections that aren't in the destination zone
				// the trick here is to ensure that there aren't any dead spots in the network, i.e., clusters of nodes that are connected to each other but not to a node in the
				// data collector/uplink zone - we do this by removing all nodes that have no downstream connections and aren't in the uplink zone.
				ArrayList aRemoveSensors = new ArrayList();
				for (int i = aSensors.Count - 1; i >= 0; i--) {
					WirelessSensor sensor = (WirelessSensor) aSensors[i];
					if ((sensor.x < iDestinationX) && (sensor.aConnections.Count == 0)) {  // dead end - eliminate.
						aRemoveSensors.Add(sensor);
						// scan all upstream nodes, to see if they were connected to this removed node, and delete the connections
						for (int j = i - 1; j >= 0; j--) {
							WirelessSensor sensor2 = (WirelessSensor) aSensors[j];
							ArrayList aRemoveConnections = new ArrayList();
							foreach (WirelessSensorConnection connection in sensor2.aConnections) {
								if (connection.sReceiver == sensor)
									aRemoveConnections.Add(connection);
							}
							foreach (WirelessSensorConnection connection in aRemoveConnections)
								sensor2.aConnections.Remove(connection);
						}
					}
				}
				foreach (WirelessSensor sensor in aRemoveSensors)
					aSensors.Remove(sensor);
			}
		}

		public void Reset(bool bNewSeed) {
			// this function resets the network so that a new simulation can be run - can either be reset with a new seed, or with the previous seed (for replay.)
			this.iProcessTime = 0;
			this.iPacketsDelivered = 0;
			foreach (WirelessSensor sensor in aSensors) {
				sensor.iResidualEnergy = sensor.iInitialEnergy;
				sensor.aPackets = new ArrayList();
				sensor.iSensorRadius = iSensorRadius;
				sensor.iSensorDelay = 0;
				foreach (WirelessSensorConnection connection in sensor.aConnections) {
					connection.iTransmitting = 0;
					connection.packet = null;
				}
			}
			aRadar = new ArrayList();
			if (bDirectedRouting == true)
				SetRoutingInformation();
			iLastUpdated = iUpdateDelay;
			if (bNewSeed == true)
				this.iSeed = (int) System.DateTime.Now.Ticks;
			r = new Random(iSeed);
		}

		public void RunSimulation() {
			// this function is the heart of the visual simulation thread. It runs until bAbort is set to true - this is a faster and cleaner stop mechanism than using thread.Abort().
			bAbort = false;
			bRunningSimulation = true;
			timeStart = System.DateTime.Now;
			this.vectors = new VectorList(iMaxX, iMaxY, r);
			while(bAbort == false) {
				Process(true);
				System.Threading.Thread.Sleep(20);
			}
			bRunningSimulation = false;
		}

		public void RunTest() {
			// this function runs the endurance test.
			bTesting = true;
			iTestResults = new int[100];
			bAbort = false;
			bRunningSimulation = true;
			timeStart = System.DateTime.Now;
			this.vectors = new VectorList(iMaxX, iMaxY, r);
			for (int i = 0; i < 100 && bAbort == false; i++) {
				Reset(true);
				while (Process(false) == true)
					iTestResults[i]++;
			}
			bRunningSimulation = false;
			bTesting = false;
		}

		public bool Process(bool bSleep) {
			// this is the core of a network simulation - all of the network processes for one step of the simulation occur here. returns a bool to indicate that the
			// network is alive (all nodes active) or dead (partitioned by one or more dead nodes.)
			bool bNetworkLives = true;
			iProcessTime++;
			// update vectors (note use of mutex)
			vectors.mutexVector.WaitOne();
			vectors.Update();
			while (vectors.aVectors.Count < 3)
				vectors.AddVector();
			vectors.mutexVector.ReleaseMutex();
			// update sensors (create a new packet if sensor is active and triggered by a vector)
			foreach (WirelessSensor sensor in aSensors) {
				if (sensor.iResidualEnergy > 0) {
					if (sensor.iSensorDelay > 0)
						sensor.iSensorDelay--;
					else {
						foreach (Vector vector in vectors.aVectors) {
							if (Math.Sqrt(Math.Pow(vector.x - sensor.x, 2) + Math.Pow(vector.y - sensor.y, 2)) < sensor.iSensorRadius) {
								if (sensor.iResidualEnergy <= iSensorCost)  // sensing failed due to sensor cost - sensor is now dead
									sensor.iResidualEnergy = 0;
								else {  // create packet
									sensor.iSensorDelay = iSensorDelay;
									sensor.iResidualEnergy -= iSensorCost;
									sensor.aPackets.Add(new Packet(sensor.x, sensor.y, sensor.iSensorDelay));
								}
							}
						}
					}
				}
				else
					bNetworkLives = false;
			}
			// start new transmisions for any pending packets
			foreach (WirelessSensor sensor in aSensors) {
				if (sensor.aPackets.Count > 0) {
					if (sensor.aConnections.Count > 0) {
						if ((bDirectedRouting == true) && (sensor.connectionCurrent != null))
							sensor.connectionCurrent.BeginTransmission();
						else  // random routing
							((WirelessSensorConnection) sensor.aConnections[r.Next(sensor.aConnections.Count)]).BeginTransmission();
					}
				}
			}
			// carry out transmissions across each connection
			foreach (WirelessSensor sensor in aSensors) {
				foreach (WirelessSensorConnection connection in sensor.aConnections) {
					Packet packet = connection.Transmit();
					if (packet != null) {
						aRadar.Add(packet);
						iPacketsDelivered++;
					}
				}
			}
			// if using directed routing, update connection information
			if (bDirectedRouting == true) {
				if ((--iLastUpdated) <= 0) {
					iLastUpdated = iUpdateDelay;
					SetRoutingInformation();
				}
			}
			// note the end of the process - tell the map to paint itself, and sleep for 10ms if this is a timed simulation
			if (bPainting == false)
				bPaint = true;
			if (bSleep == true)
				System.Threading.Thread.Sleep(10);
			return bNetworkLives;
		}

		public void SetRoutingInformation() {
			// this function updates directed-routing selections
			foreach (WirelessSensor sensor in aSensors) {
				if (sensor.iResidualEnergy > 0) {
					// choose best node given current conditions
					sensor.connectionCurrent = null;
					double dBestCost = 0;
					foreach (WirelessSensorConnection connection in sensor.aConnections) {
						if (connection.sReceiver == null) {  // if this is an uplink connection, always select it
							sensor.connectionCurrent = connection;
							break;
						}
						else if (connection.sReceiver.iResidualEnergy > 0) {
							double dCost = Math.Pow(connection.iTransmitCost, x1) * Math.Pow(connection.sSender.iResidualEnergy, -x2) * Math.Pow(connection.sSender.iInitialEnergy, x3) + Math.Pow(connection.iReceiveCost,  x1) * Math.Pow(connection.sReceiver.iResidualEnergy, -x2) * Math.Pow(connection.sReceiver.iInitialEnergy, x3);
							if ((sensor.connectionCurrent == null) || (dCost < dBestCost)) {
								dBestCost = dCost;
								sensor.connectionCurrent = connection;
							}
						}
					}
				}
			}
		}

		#endregion

	}

	public class WirelessSensor {
		// This class represents a single node in a wireless sensor network.

		#region Variables and initialization code

		public ArrayList aPackets = null;																// an array of packets held by this node
		public ArrayList aConnections;																	// an array of connections to downstream nodes (*always* downstream, not upstream) - nodes in the data collector/uplink zone have a connection with a "null" receiver node
		public WirelessSensorConnection connectionCurrent;			// if using directed routing, the currently preferred network connection for transmitting
		public int x, y;																											// the coordinates of the node
		public int iInitialEnergy;																					// the initial power of the node
		public int iResidualEnergy;																			// the current power of the node
		public int iSensorDelay = 0;																			// the timer until the sensor is ready to be tripped again
		public int iSensorRadius;																				// the radius of this sensor

		public WirelessSensor(int x, int y, int iSensorRadius) {
			this.x = x;
			this.y = y;
			this.iSensorRadius = iSensorRadius;
			aConnections = new ArrayList();
			connectionCurrent = null;
			iInitialEnergy = iResidualEnergy = WirelessSensorNetwork.iMaxEnergy;
		}

		#endregion

	}

	public class WirelessSensorConnection {
		// This class represents a communications link between two wireless sensors.

		#region Variables and initialization code

		public WirelessSensor sSender;							// the upstream sensor
		public WirelessSensor sReceiver;						// the downstream sensor - every node in the data collector/uplink zone will have a connection with a NULL sReceiver.
		public Packet packet = null;									// the packet currently being transmitted on this connection (only one at a time, of course)
		public int iTransmitCost, iReceiveCost;			// the energy costs of transmitting and receiving the packet
		public int iTransmitterDelay;									// the total time this node would normally wait to complete delivery of a packet
		public int iTransmitting;												// the timer for completing delivery of a packet

		public WirelessSensorConnection(WirelessSensor sSender, WirelessSensor sReceiver, int iTransmitCost, int iReceiveCost, int iTransmitterDelay) {
			this.sSender = sSender;
			this.sReceiver = sReceiver;
			this.iTransmitCost = iTransmitCost;
			this.iReceiveCost = iReceiveCost;
			this.iTransmitting = 0;
			this.iTransmitterDelay = iTransmitterDelay;
		}

		#endregion

		#region Connection simulation functions

		public void BeginTransmission() {
			// this function begins transmission of a data packet between these nodes.
			if ((sSender.aPackets.Count > 0) && (iTransmitting == 0)) {
				if (sSender.iResidualEnergy <= iTransmitCost)  // transmission failed - sender has run out of energy
					sSender.iResidualEnergy = 0;
				else if ((sReceiver != null) && (sReceiver.iResidualEnergy <= iReceiveCost))  // transmission failed - receiver has run out of energy
					sReceiver.iResidualEnergy = 0;
				else {  // success - accept the packet and start transmitting it.
					iTransmitting = iTransmitterDelay;
					sSender.iResidualEnergy -= iTransmitCost;
					packet = (Packet) sSender.aPackets[0];
					sSender.aPackets.RemoveAt(0);
					if (sReceiver != null)
						sReceiver.iResidualEnergy -= iReceiveCost;
				}
			}
		}

		public Packet Transmit() {
			// this function continues transmission of a previously accepted packet, and completes transmission if appropriate.
			if ((sSender.iResidualEnergy <= 0) || ((sReceiver != null) && (sReceiver.iResidualEnergy <= 0)))  // failed due to depleted energy
				iTransmitting = 0;
			else if (iTransmitting > 0) {  // transmission in progress
				iTransmitting--;
				if (iTransmitting == 0) {
					if (sReceiver != null)
						sReceiver.aPackets.Add(packet);
					else {
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

	public class VectorList {
		// This class represents (and manages) a list of vectors.

		#region Variables and initialization code

		public ArrayList aVectors;			// the array of vectors
		public Mutex mutexVector;		// a mutex to control access to the vector list
		private int iMaxX, iMaxY;				// the maximum X and Y coordinates of a vector
		private Random r;								// a random number generator (the same one used by the WirelessSensorNetwork object that houses this object)

		public VectorList(int iMaxX, int iMaxY, Random r) {
			this.iMaxX = iMaxX;
			this.iMaxY = iMaxY;
			aVectors = new ArrayList();
			mutexVector = new Mutex();
			this.r = r;
		}

		#endregion

		#region Vector list management functions

		public void AddVector() {
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

		public void Update() {
			// this function moves the vectors around, and removes those that have traveled out of bounds
			ArrayList aRemoveVectors = new ArrayList();
			foreach (Vector vector in aVectors) {
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

	public class Vector {
		// A lightweight class - really just a structure - for a vector.

		#region Variables and initialization code

		public int x, y, dx, dy;
		public Vector(int x, int y, int dx, int dy) {
			this.x = x;
			this.y = y;
			this.dx = dx;
			this.dy = dy;
		}

		#endregion

	}

	public class Packet {
		// A lightweight class - really just a data structure - for a packet.

		#region Variables and initialization code

		public int x, y;
		public int timestamp, lifespan;    // the amount of time left to display the packet on the radar (once it gets there), and the total time of displaying it
		public Packet(int x, int y, int timestamp) {
			this.x = x;
			this.y = y;
			this.timestamp = 0;
			this.lifespan = timestamp;
		}

		#endregion

	}
}

// Wireless Sensor Network Simulator v1.0 (and all future versions), together with all accompanying source code and documentation, is copyright (c) David J. Stein, Esq., March 2005. All rights reserved except as set forth in the included Wireless Sensor Network License Agreement.
