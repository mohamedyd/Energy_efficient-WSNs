// Wireless Sensor Network Simulator.cs
// Written by David J. Stein, Esq., March 2005
//
// This module contains all of the code for the simulator interface. This class (WirelessSensorNetworkSimulator) hosts the WirelessNetwork object, which is where all of the
// interesting simulation bits occur. Reference is therefore made to the module Wireless Sensor Network.cs for more interesting information.

#region Using declarations

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;


#endregion

namespace Wireless_Sensor_Network_Simulator {

	public class WirelessSensorNetworkSimulator : System.Windows.Forms.Form {

		#region Variables, window management code, and Main

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnDeploy;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.PictureBox picNetwork;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TrackBar trackReceiveCost;
		private System.Windows.Forms.TrackBar trackTransmissionCost;
		private System.Windows.Forms.TrackBar trackTransmissionRadius;
		private System.Windows.Forms.TrackBar trackNetworkSize;
		private System.Windows.Forms.TrackBar trackSensorRadius;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TrackBar trackResidualEnergy;
		private System.Windows.Forms.TrackBar trackInitialEnergy;
		private System.Windows.Forms.TrackBar trackEnergyCost;
		private System.ComponentModel.IContainer components;
		private WirelessSensorNetwork network = null;
		private System.Timers.Timer timerUpdate;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.PictureBox picRadar;
		private System.Windows.Forms.Button btnReplay;
		private System.Windows.Forms.TrackBar trackUpdateFrequency;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label lblTime;
		private System.Windows.Forms.Label lblPower;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.TrackBar trackSensorDelay;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar trackTransmitterDelay;
		private System.Windows.Forms.TrackBar trackSensorCost;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label lblSensors;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.RadioButton radioRandom;
		private System.Windows.Forms.RadioButton radioDirected;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label lblLivePackets;
		private System.Windows.Forms.Label lblRecdPackets;
        private System.Windows.Forms.Button bMeanLifetimeTest;
		private int iSetupDisplay = -1;
        private bool NormalMode;
		public WirelessSensorNetworkSimulator() {
			InitializeComponent();
		}

		#region Uninteresting Windows Form Designer generated code
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WirelessSensorNetworkSimulator));
            this.picNetwork = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trackSensorRadius = new System.Windows.Forms.TrackBar();
            this.label9 = new System.Windows.Forms.Label();
            this.trackReceiveCost = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.trackTransmissionCost = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.trackNetworkSize = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.btnDeploy = new System.Windows.Forms.Button();
            this.trackSensorDelay = new System.Windows.Forms.TrackBar();
            this.label11 = new System.Windows.Forms.Label();
            this.trackTransmitterDelay = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.trackSensorCost = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.trackTransmissionRadius = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.radioDirected = new System.Windows.Forms.RadioButton();
            this.radioRandom = new System.Windows.Forms.RadioButton();
            this.trackUpdateFrequency = new System.Windows.Forms.TrackBar();
            this.label10 = new System.Windows.Forms.Label();
            this.trackResidualEnergy = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.trackInitialEnergy = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.trackEnergyCost = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblPower = new System.Windows.Forms.Label();
            this.lblLivePackets = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnReplay = new System.Windows.Forms.Button();
            this.timerUpdate = new System.Timers.Timer();
            this.picRadar = new System.Windows.Forms.PictureBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bMeanLifetimeTest = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblRecdPackets = new System.Windows.Forms.Label();
            this.lblSensors = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picNetwork)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackSensorRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackReceiveCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTransmissionCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackNetworkSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSensorDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTransmitterDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSensorCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTransmissionRadius)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackUpdateFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackResidualEnergy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackInitialEnergy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackEnergyCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timerUpdate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRadar)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // picNetwork
            // 
            this.picNetwork.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picNetwork.Location = new System.Drawing.Point(152, 8);
            this.picNetwork.Name = "picNetwork";
            this.picNetwork.Size = new System.Drawing.Size(738, 520);
            this.picNetwork.TabIndex = 0;
            this.picNetwork.TabStop = false;
            this.picNetwork.Paint += new System.Windows.Forms.PaintEventHandler(this.picNetwork_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trackSensorRadius);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.trackReceiveCost);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.trackTransmissionCost);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.trackNetworkSize);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnDeploy);
            this.groupBox1.Controls.Add(this.trackSensorDelay);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.trackTransmitterDelay);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.trackSensorCost);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.trackTransmissionRadius);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(136, 520);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Network Configuration";
            // 
            // trackSensorRadius
            // 
            this.trackSensorRadius.AutoSize = false;
            this.trackSensorRadius.Location = new System.Drawing.Point(8, 136);
            this.trackSensorRadius.Maximum = 120;
            this.trackSensorRadius.Minimum = 20;
            this.trackSensorRadius.Name = "trackSensorRadius";
            this.trackSensorRadius.Size = new System.Drawing.Size(120, 30);
            this.trackSensorRadius.TabIndex = 2;
            this.trackSensorRadius.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trackSensorRadius, "The size of the area within which a sensor can detect movement.");
            this.trackSensorRadius.Value = 45;
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(8, 120);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(120, 16);
            this.label9.TabIndex = 16;
            this.label9.Text = "Sensor Radius";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackReceiveCost
            // 
            this.trackReceiveCost.AutoSize = false;
            this.trackReceiveCost.Location = new System.Drawing.Point(8, 480);
            this.trackReceiveCost.Maximum = 100;
            this.trackReceiveCost.Minimum = 1;
            this.trackReceiveCost.Name = "trackReceiveCost";
            this.trackReceiveCost.Size = new System.Drawing.Size(120, 30);
            this.trackReceiveCost.TabIndex = 8;
            this.trackReceiveCost.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trackReceiveCost, "The energy consumed by receiving a packet.");
            this.trackReceiveCost.Value = 15;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(8, 464);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(120, 16);
            this.label8.TabIndex = 14;
            this.label8.Text = "Receive Cost";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackTransmissionCost
            // 
            this.trackTransmissionCost.AutoSize = false;
            this.trackTransmissionCost.Location = new System.Drawing.Point(8, 424);
            this.trackTransmissionCost.Maximum = 10001;
            this.trackTransmissionCost.Minimum = 1;
            this.trackTransmissionCost.Name = "trackTransmissionCost";
            this.trackTransmissionCost.Size = new System.Drawing.Size(120, 30);
            this.trackTransmissionCost.TabIndex = 7;
            this.trackTransmissionCost.TickFrequency = 1000;
            this.toolTip.SetToolTip(this.trackTransmissionCost, "The energy consumed by sending a packet.");
            this.trackTransmissionCost.Value = 200;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(8, 408);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 16);
            this.label7.TabIndex = 12;
            this.label7.Text = "Transmit Cost";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackNetworkSize
            // 
            this.trackNetworkSize.AutoSize = false;
            this.trackNetworkSize.Location = new System.Drawing.Point(8, 80);
            this.trackNetworkSize.Maximum = 410;
            this.trackNetworkSize.Minimum = 10;
            this.trackNetworkSize.Name = "trackNetworkSize";
            this.trackNetworkSize.Size = new System.Drawing.Size(120, 30);
            this.trackNetworkSize.TabIndex = 1;
            this.trackNetworkSize.TickFrequency = 40;
            this.toolTip.SetToolTip(this.trackNetworkSize, "The number of nodes in the network.");
            this.trackNetworkSize.Value = 35;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(8, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Network Size";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDeploy
            // 
            this.btnDeploy.Location = new System.Drawing.Point(8, 24);
            this.btnDeploy.Name = "btnDeploy";
            this.btnDeploy.Size = new System.Drawing.Size(120, 23);
            this.btnDeploy.TabIndex = 0;
            this.btnDeploy.Text = "Deploy Network";
            this.toolTip.SetToolTip(this.btnDeploy, "Click to deploy a network with these settings.");
            this.btnDeploy.Click += new System.EventHandler(this.btnDeploy_Click);
            // 
            // trackSensorDelay
            // 
            this.trackSensorDelay.AutoSize = false;
            this.trackSensorDelay.Location = new System.Drawing.Point(8, 192);
            this.trackSensorDelay.Maximum = 101;
            this.trackSensorDelay.Minimum = 1;
            this.trackSensorDelay.Name = "trackSensorDelay";
            this.trackSensorDelay.Size = new System.Drawing.Size(120, 30);
            this.trackSensorDelay.TabIndex = 3;
            this.trackSensorDelay.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trackSensorDelay, "How long a tripped sensor waits before sending a subsequent event.");
            this.trackSensorDelay.Value = 15;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(8, 176);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(120, 16);
            this.label11.TabIndex = 14;
            this.label11.Text = "Sensor Period";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackTransmitterDelay
            // 
            this.trackTransmitterDelay.AutoSize = false;
            this.trackTransmitterDelay.Location = new System.Drawing.Point(8, 368);
            this.trackTransmitterDelay.Maximum = 101;
            this.trackTransmitterDelay.Minimum = 1;
            this.trackTransmitterDelay.Name = "trackTransmitterDelay";
            this.trackTransmitterDelay.Size = new System.Drawing.Size(120, 30);
            this.trackTransmitterDelay.TabIndex = 6;
            this.trackTransmitterDelay.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trackTransmitterDelay, "The amount of time required to send a packet.");
            this.trackTransmitterDelay.Value = 10;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(8, 352);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 16;
            this.label1.Text = "Transmitter Period";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Location = new System.Drawing.Point(8, 232);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 16);
            this.label12.TabIndex = 18;
            this.label12.Text = "Sensor Cost";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackSensorCost
            // 
            this.trackSensorCost.AutoSize = false;
            this.trackSensorCost.Location = new System.Drawing.Point(8, 256);
            this.trackSensorCost.Maximum = 100;
            this.trackSensorCost.Minimum = 1;
            this.trackSensorCost.Name = "trackSensorCost";
            this.trackSensorCost.Size = new System.Drawing.Size(120, 30);
            this.trackSensorCost.TabIndex = 4;
            this.trackSensorCost.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trackSensorCost, "The energy consumed by a sensor activation.");
            this.trackSensorCost.Value = 20;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(8, 296);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Transmission Radius";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackTransmissionRadius
            // 
            this.trackTransmissionRadius.AutoSize = false;
            this.trackTransmissionRadius.Location = new System.Drawing.Point(8, 312);
            this.trackTransmissionRadius.Maximum = 230;
            this.trackTransmissionRadius.Minimum = 30;
            this.trackTransmissionRadius.Name = "trackTransmissionRadius";
            this.trackTransmissionRadius.Size = new System.Drawing.Size(120, 30);
            this.trackTransmissionRadius.TabIndex = 5;
            this.trackTransmissionRadius.TickFrequency = 20;
            this.toolTip.SetToolTip(this.trackTransmissionRadius, "The maximum distance between two connected nodes.");
            this.trackTransmissionRadius.Value = 130;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.radioDirected);
            this.groupBox2.Controls.Add(this.radioRandom);
            this.groupBox2.Controls.Add(this.trackUpdateFrequency);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.trackResidualEnergy);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.trackInitialEnergy);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.trackEnergyCost);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(8, 536);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(256, 136);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Routing Parameters";
            // 
            // label14
            // 
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(8, 24);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(80, 16);
            this.label14.TabIndex = 15;
            this.label14.Text = "Routing Method";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // radioDirected
            // 
            this.radioDirected.Checked = true;
            this.radioDirected.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioDirected.Location = new System.Drawing.Point(16, 56);
            this.radioDirected.Name = "radioDirected";
            this.radioDirected.Size = new System.Drawing.Size(72, 24);
            this.radioDirected.TabIndex = 0;
            this.radioDirected.TabStop = true;
            this.radioDirected.Text = "Directed";
            // 
            // radioRandom
            // 
            this.radioRandom.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioRandom.Location = new System.Drawing.Point(16, 88);
            this.radioRandom.Name = "radioRandom";
            this.radioRandom.Size = new System.Drawing.Size(72, 24);
            this.radioRandom.TabIndex = 1;
            this.radioRandom.Text = "Random";
            // 
            // trackUpdateFrequency
            // 
            this.trackUpdateFrequency.AutoSize = false;
            this.trackUpdateFrequency.Location = new System.Drawing.Point(176, 96);
            this.trackUpdateFrequency.Maximum = 100;
            this.trackUpdateFrequency.Minimum = 1;
            this.trackUpdateFrequency.Name = "trackUpdateFrequency";
            this.trackUpdateFrequency.Size = new System.Drawing.Size(72, 30);
            this.trackUpdateFrequency.TabIndex = 5;
            this.trackUpdateFrequency.TickFrequency = 10;
            this.toolTip.SetToolTip(this.trackUpdateFrequency, "The amount of time between packet-routing decision updates.");
            this.trackUpdateFrequency.Value = 1;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(176, 80);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 16);
            this.label10.TabIndex = 12;
            this.label10.Text = "Routing Period";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackResidualEnergy
            // 
            this.trackResidualEnergy.AutoSize = false;
            this.trackResidualEnergy.Location = new System.Drawing.Point(176, 40);
            this.trackResidualEnergy.Name = "trackResidualEnergy";
            this.trackResidualEnergy.Size = new System.Drawing.Size(72, 30);
            this.trackResidualEnergy.TabIndex = 3;
            this.toolTip.SetToolTip(this.trackResidualEnergy, "The emphasis placed on the residual energy of the nodes in making routing decisio" +
                    "ns.");
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(176, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 10;
            this.label6.Text = "Residual Power";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackInitialEnergy
            // 
            this.trackInitialEnergy.AutoSize = false;
            this.trackInitialEnergy.Location = new System.Drawing.Point(96, 96);
            this.trackInitialEnergy.Name = "trackInitialEnergy";
            this.trackInitialEnergy.Size = new System.Drawing.Size(72, 30);
            this.trackInitialEnergy.TabIndex = 4;
            this.toolTip.SetToolTip(this.trackInitialEnergy, "The emphasis placed on the initial energy of the nodes in making routing decision" +
                    "s.");
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(96, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Initial Power";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackEnergyCost
            // 
            this.trackEnergyCost.AutoSize = false;
            this.trackEnergyCost.Location = new System.Drawing.Point(96, 40);
            this.trackEnergyCost.Maximum = 1;
            this.trackEnergyCost.Name = "trackEnergyCost";
            this.trackEnergyCost.Size = new System.Drawing.Size(72, 30);
            this.trackEnergyCost.TabIndex = 2;
            this.toolTip.SetToolTip(this.trackEnergyCost, "Whether or not the cost of transmitting packets is taken into account in determin" +
                    "ing routing.");
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(96, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Exchange Cost";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(0, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 23);
            this.label13.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(8, 24);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(112, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Simulation";
            this.toolTip.SetToolTip(this.btnStart, "Starts the simulation.");
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblTime
            // 
            this.lblTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.Location = new System.Drawing.Point(4, 35);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(116, 18);
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "Time: 00:00.000";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPower
            // 
            this.lblPower.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPower.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPower.Location = new System.Drawing.Point(4, 54);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(116, 18);
            this.lblPower.TabIndex = 2;
            this.lblPower.Text = "Power: 0";
            this.lblPower.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLivePackets
            // 
            this.lblLivePackets.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLivePackets.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLivePackets.Location = new System.Drawing.Point(4, 92);
            this.lblLivePackets.Name = "lblLivePackets";
            this.lblLivePackets.Size = new System.Drawing.Size(116, 18);
            this.lblLivePackets.TabIndex = 4;
            this.lblLivePackets.Text = "Live Packets: 0";
            this.lblLivePackets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(4, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(116, 18);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Status: Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnReplay
            // 
            this.btnReplay.Location = new System.Drawing.Point(8, 64);
            this.btnReplay.Name = "btnReplay";
            this.btnReplay.Size = new System.Drawing.Size(112, 23);
            this.btnReplay.TabIndex = 1;
            this.btnReplay.Text = "Replay Simulation";
            this.toolTip.SetToolTip(this.btnReplay, "Replays the previous simulation.");
            this.btnReplay.Click += new System.EventHandler(this.btnReplay_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 20;
            this.timerUpdate.SynchronizingObject = this;
            this.timerUpdate.Elapsed += new System.Timers.ElapsedEventHandler(this.timerUpdate_Elapsed);
            // 
            // picRadar
            // 
            this.picRadar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picRadar.Location = new System.Drawing.Point(4, 20);
            this.picRadar.Name = "picRadar";
            this.picRadar.Size = new System.Drawing.Size(330, 112);
            this.picRadar.TabIndex = 7;
            this.picRadar.TabStop = false;
            this.picRadar.Paint += new System.Windows.Forms.PaintEventHandler(this.picRadar_Paint);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.picRadar);
            this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox4.Location = new System.Drawing.Point(550, 536);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(340, 136);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Radar";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bMeanLifetimeTest);
            this.groupBox3.Controls.Add(this.btnStart);
            this.groupBox3.Controls.Add(this.btnReplay);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox3.Location = new System.Drawing.Point(274, 536);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(128, 136);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Simulation Control";
            // 
            // bMeanLifetimeTest
            // 
            this.bMeanLifetimeTest.Location = new System.Drawing.Point(8, 104);
            this.bMeanLifetimeTest.Name = "bMeanLifetimeTest";
            this.bMeanLifetimeTest.Size = new System.Drawing.Size(112, 23);
            this.bMeanLifetimeTest.TabIndex = 2;
            this.bMeanLifetimeTest.Text = "Mean Lifetime Test";
            this.toolTip.SetToolTip(this.bMeanLifetimeTest, "Runs 1,000 trials and determines the average network lifetime.");
            this.bMeanLifetimeTest.Click += new System.EventHandler(this.bMeanLifetimeTest_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblRecdPackets);
            this.groupBox5.Controls.Add(this.lblSensors);
            this.groupBox5.Controls.Add(this.lblPower);
            this.groupBox5.Controls.Add(this.lblTime);
            this.groupBox5.Controls.Add(this.lblStatus);
            this.groupBox5.Controls.Add(this.lblLivePackets);
            this.groupBox5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox5.Location = new System.Drawing.Point(412, 536);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(128, 136);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Simulation Status";
            // 
            // lblRecdPackets
            // 
            this.lblRecdPackets.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblRecdPackets.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecdPackets.Location = new System.Drawing.Point(4, 111);
            this.lblRecdPackets.Name = "lblRecdPackets";
            this.lblRecdPackets.Size = new System.Drawing.Size(116, 18);
            this.lblRecdPackets.TabIndex = 7;
            this.lblRecdPackets.Text = "Rec\'d Packets: 0";
            this.lblRecdPackets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSensors
            // 
            this.lblSensors.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSensors.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSensors.Location = new System.Drawing.Point(4, 73);
            this.lblSensors.Name = "lblSensors";
            this.lblSensors.Size = new System.Drawing.Size(116, 18);
            this.lblSensors.TabIndex = 6;
            this.lblSensors.Text = "Sensors: 0";
            this.lblSensors.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WirelessSensorNetworkSimulator
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(901, 674);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.picNetwork);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WirelessSensorNetworkSimulator";
            this.Text = "Wireless Sensor Network Simulator";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.WirelessSensorNetworkSimulator_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.picNetwork)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackSensorRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackReceiveCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTransmissionCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackNetworkSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSensorDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTransmitterDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSensorCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTransmissionRadius)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackUpdateFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackResidualEnergy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackInitialEnergy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackEnergyCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timerUpdate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRadar)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#endregion

		private void WirelessSensorNetworkSimulator_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if ((network != null) && (network.tSimulation != null))
				network.tSimulation.Abort();
		}

		[STAThread]
		static void Main() {
			Application.Run(new WirelessSensorNetworkSimulator());
		}

		#endregion

		#region Button-click event handlers

		private void btnDeploy_Click(object sender, System.EventArgs e) {
			network = new WirelessSensorNetwork((int) trackNetworkSize.Value, (int) trackSensorRadius.Value, (int) trackSensorDelay.Value, (int) trackTransmissionRadius.Value, (int) trackTransmitterDelay.Value, (float) trackTransmissionCost.Value / 100.0f, (int) trackReceiveCost.Value, (int) trackSensorCost.Value, radioDirected.Checked, trackEnergyCost.Value, trackResidualEnergy.Value, trackInitialEnergy.Value, 1, picNetwork.Width - 5, picNetwork.Height - 5, picNetwork.Width - 45);
			iSetupDisplay = 0;  // initiate the "deploying network" display
		}

		private void btnStart_Click(object sender, System.EventArgs e) {
			if (network != null) {
				if (network.tSimulation == null) {  // no simulation running - start a new simulation (by spawning a new thread)
					network.Reset(true);
					network.tSimulation = new Thread(new ThreadStart(network.RunSimulation));
					network.tSimulation.Start();
					btnStart.Text = "Stop Simulation";
					btnStart.Refresh();
					lblStatus.Text = "Status: Operating";
					lblStatus.Refresh();
				}
				else {  // simulation running - tell the thread to stop running and relabel buttons
					network.bAbort = true;
					network.tSimulation = null;
					btnStart.Text = "Start Simulation";
					btnStart.Refresh();
					lblStatus.Text = "Status: Ready";
					lblStatus.Refresh();
				}
			}
		}

		private void btnReplay_Click(object sender, System.EventArgs e) {
			if (network != null) {
				if (network.tSimulation != null)  // immediately stop simulation if it's running, so that we can start a new one
					network.tSimulation.Abort();
				else {
					btnStart.Text = "Stop Simulation";
					btnStart.Refresh();
					lblStatus.Text = "Status: Operating";
					lblStatus.Refresh();
				}
				network.Reset(false);  // "false" indicates that we're restarting without picking a new seed (i.e., use the one from the last run)
				network.tSimulation = new Thread(new ThreadStart(network.RunSimulation));
				network.tSimulation.Start();
			}
		}

		private void bMeanLifetimeTest_Click(object sender, System.EventArgs e) {
			if (network != null) {
				if (network.tSimulation != null) {  // abort any running simulation
					network.tSimulation.Abort();
					network.tSimulation = null;
					btnStart.Text = "Start Simulation";
					btnStart.Refresh();
					lblStatus.Text = "Status: Ready";
					lblStatus.Refresh();
				}
				// start a new simulation and display the tester window
				Thread thread = new Thread(new ThreadStart(network.RunTest));
				thread.Start();
				WirelessSensorNetworkTest test = new WirelessSensorNetworkTest(network);
				test.ShowDialog();
			}
		}

		#endregion

		#region Paint and Timer event handlers

		private void picNetwork_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			// note that we're painting, so that the timer thread doesn't send another Paint message
			if (network != null) {
				network.bPainting = true;
				network.bPaint = false;
			}
			// create drawing tools
			Font font = new Font("Times New Roman", 7.0f);
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			// draw background
			g.FillRectangle(System.Drawing.Brushes.BlanchedAlmond, e.ClipRectangle);
			// draw network objects (if network has been deployed)
			if (network != null) {
				// draw sensor background
				if (iSetupDisplay == -1) {
					ArrayList activatedSensors = new ArrayList();
					foreach (WirelessSensor sensor in network.aSensors) {
						if (sensor.iSensorDelay <= 0)
							g.FillEllipse(new SolidBrush(Color.FromArgb(196, 196, 196)), sensor.x - sensor.iSensorRadius, sensor.y - sensor.iSensorRadius, sensor.iSensorRadius * 2, sensor.iSensorRadius * 2);
						else
							activatedSensors.Add(sensor);
					}
					foreach (WirelessSensor sensor in activatedSensors)
						g.FillEllipse(new SolidBrush(Color.FromArgb(196, 196, 196 + 48 * sensor.iSensorDelay / network.iSensorDelay)), sensor.x - sensor.iSensorRadius, sensor.y - sensor.iSensorRadius, sensor.iSensorRadius * 2, sensor.iSensorRadius * 2);
				}
				// draw end zone
				g.DrawLine(Pens.Black, picNetwork.Width - 44, 0, picNetwork.Width - 44, picNetwork.Height);
				for (int i = 0; i < picNetwork.Height; i += 20)
					g.DrawLine(Pens.Black, picNetwork.Width - 44, i + 20, picNetwork.Width, i);
				// draw connections
				foreach (WirelessSensor sensor in network.aSensors) {
					foreach (WirelessSensorConnection connection in sensor.aConnections) {
						if ((connection.sReceiver != null) && ((iSetupDisplay == -1) ||  (iSetupDisplay > connection.sReceiver.x)) && (connection.sSender.iResidualEnergy > 0) && (connection.sReceiver.iResidualEnergy > 0))
							g.DrawLine(connection.iTransmitting > 0 ? Pens.Red : connection == sensor.connectionCurrent ? Pens.Blue : Pens.Black, connection.sSender.x, connection.sSender.y, connection.sReceiver.x, connection.sReceiver.y);
					}
				}
				// draw sensors
				Brush sensorBrush = Brushes.DarkGray;
				Pen sensorPen = Pens.Red;
				foreach (WirelessSensor sensor in network.aSensors) {
					if ((iSetupDisplay == -1) || (iSetupDisplay > sensor.x)) {
						int color = sensor.iResidualEnergy <= 0 ? 0 : (int) (255 * sensor.iResidualEnergy / WirelessSensorNetwork.iMaxEnergy);
						g.FillEllipse(new SolidBrush(Color.FromArgb(color, color, color)), sensor.x - 4, sensor.y - 4, 9, 9);
						if (sensor.iResidualEnergy <= 0) {
							if (sensor.iSensorRadius > 0)
								sensor.iSensorRadius--;
						}
						g.DrawEllipse(sensor.iResidualEnergy <= 0 ? Pens.Black : Pens.Red, sensor.x - 4, sensor.y - 4, 9, 9);
					}
				}
				// draw vectors
				if ((network.bRunningSimulation == true) && (network.vectors != null)) {
					network.vectors.mutexVector.WaitOne();
					foreach (Vector vector in network.vectors.aVectors) {
						g.FillRectangle(Brushes.Green, vector.x - 1, vector.y - 1, 3, 3);
					}
					network.vectors.mutexVector.ReleaseMutex();
				}
				// draw setup line, if network is being deployed
				if (iSetupDisplay != -1)
					g.DrawLine(Pens.Black, iSetupDisplay, 0, iSetupDisplay, picNetwork.Height);
			}
			// finish painting
			if (network != null)
				network.bPainting = false;
		}

		private void picRadar_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			// create drawing tools
			Font font = new Font("Times New Roman", 7.0f);
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			// draw background
			g.FillRectangle(System.Drawing.Brushes.Black, e.ClipRectangle);
			// draw network radar objects
			if ((network != null) && (network.vectors != null)) {
				// draw sensors
				int iRadius = network.iSensorRadius * picRadar.Width / picNetwork.Width;
				foreach (WirelessSensor sensor in network.aSensors) {
					if (sensor.iResidualEnergy > 0)
						g.DrawEllipse(Pens.Green, sensor.x * picRadar.Width / picNetwork.Width - iRadius, sensor.y * picRadar.Height / picNetwork.Height - iRadius, iRadius * 2, iRadius * 2);
				}
				// draw received packets
				for (int i = 0; i < network.aRadar.Count;) {
					if ((++((Packet) network.aRadar[i]).timestamp) >= ((Packet) network.aRadar[i]).lifespan)
						network.aRadar.RemoveAt(i);
					else {
						int green = 255 - 191 * ((Packet) network.aRadar[i]).timestamp / (((Packet) network.aRadar[i]).lifespan + 1);
						g.FillEllipse(new SolidBrush(Color.FromArgb(0, green, 0)), ((Packet) network.aRadar[i]).x * picRadar.Width / picNetwork.Width - iRadius, ((Packet) network.aRadar[i]).y * picRadar.Height / picNetwork.Height - iRadius, iRadius * 2, iRadius * 2);
						i++;
					}
				}
				// draw vectors
				network.vectors.mutexVector.WaitOne();
				foreach (Vector vector in network.vectors.aVectors)
					g.DrawRectangle(Pens.White, vector.x * picRadar.Width  / picNetwork.Width, vector.y * picRadar.Height / picNetwork.Height, 2, 2);
				network.vectors.mutexVector.ReleaseMutex();
				// draw radar line
				int xRadarLine = picRadar.Width - Math.Abs((int) (System.DateTime.Now.Ticks / 500000) % picRadar.Width);
				for (int i = 0; i < 6; i++)
					g.DrawLine(new Pen(Color.FromArgb(0, 255 - (255 * i / 7), 0)), xRadarLine - i, 0, xRadarLine - i, picRadar.Height);
			}
		}

		private void timerUpdate_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			// check to see if the network is being deployed
			if (iSetupDisplay != -1) {
				iSetupDisplay += 10;
				if (iSetupDisplay >= picNetwork.Width)
					iSetupDisplay = -1;
				picNetwork.Refresh();
			}
			// check to see if network simulation is running
			else if ((network != null) && (network.tSimulation != null) && (network.bPaint == true) && (network.bPainting == false)) {
				// refresh objects
				picNetwork.Refresh();
				picRadar.Refresh();
				// display information in textboxes
				if (network.bRunningSimulation == true) { 
					TimeSpan counter = new TimeSpan(System.DateTime.Now.Ticks - network.timeStart.Ticks);
					lblTime.Text = "Time: " + counter.Minutes.ToString("d2") + ":" + counter.Seconds.ToString("d2") + "." + counter.Milliseconds.ToString("d3");
					lblTime.Refresh();
					lblRecdPackets.Text = "Rec'd Packets: " + network.iPacketsDelivered;
					lblRecdPackets.Refresh();
					int iPower = 0;int iSensors = 0;
					int iLivePackets = 0;
					foreach (WirelessSensor sensor in network.aSensors) {
						iPower += sensor.iResidualEnergy;
						if (sensor.iResidualEnergy > 0) {
							iSensors++;
							iLivePackets += sensor.aPackets.Count;
						}
					}
					lblSensors.Text = "Sensors: " + iSensors + "/" + network.aSensors.Count;
					lblSensors.Refresh();
					lblPower.Text = "Power: " + iPower;
					lblPower.Refresh();
					lblLivePackets.Text = "Live Packets: " + iLivePackets;
					lblLivePackets.Refresh();
				}
				else {   // network has stopped running - throw away the completed thread and relabel buttons
					network.tSimulation = null;
					btnStart.Text = "Start Simulation";
					btnStart.Refresh();
					lblStatus.Text = "Status: Ready";
					lblStatus.Refresh();
				}
			}
		}

		#endregion



      

	}
}

// Wireless Sensor Network Simulator v1.0 (and all future versions), together with all accompanying source code and documentation, is copyright (c) David J. Stein, Esq., March 2005. All rights reserved except as set forth in the included Wireless Sensor Network License Agreement.
