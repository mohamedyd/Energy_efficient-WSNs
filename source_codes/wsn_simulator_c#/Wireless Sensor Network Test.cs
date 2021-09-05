// Wireless Sensor Network Test.cs
// Written by David J. Stein, Esq., March 2005
//
// This module contains the simple code for showing the bar-chart progress of a 100-run endurance test. It's not very interesting; the testing is actually carried out in the
// wireless sensor network object, which hosts a function called RunTrials. The only real work performed by this application is in the bar chart, which is completely
// routine.

#region Using declarations

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace Wireless_Sensor_Network_Simulator {

	public class WirelessSensorNetworkTest : System.Windows.Forms.Form	{

		#region Variables and window management code

		private System.Windows.Forms.PictureBox picGraph;
		private System.Windows.Forms.Button btnOK;
		private System.Timers.Timer timerUpdate;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox txtStatus;
		WirelessSensorNetwork network;

		public WirelessSensorNetworkTest(WirelessSensorNetwork network)	{
			InitializeComponent();
			this.network = network;
		}

	#region Uninteresting Windows Form Designer generated code
		protected override void Dispose(bool disposing)		{
			if (disposing) {
				if (components != null) {
				components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

	private void InitializeComponent()	{
        this.picGraph = new System.Windows.Forms.PictureBox();
        this.btnOK = new System.Windows.Forms.Button();
        this.timerUpdate = new System.Timers.Timer();
        this.txtStatus = new System.Windows.Forms.TextBox();
        ((System.ComponentModel.ISupportInitialize)(this.picGraph)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.timerUpdate)).BeginInit();
        this.SuspendLayout();
        // 
        // picGraph
        // 
        this.picGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.picGraph.Location = new System.Drawing.Point(8, 8);
        this.picGraph.Name = "picGraph";
        this.picGraph.Size = new System.Drawing.Size(280, 96);
        this.picGraph.TabIndex = 0;
        this.picGraph.TabStop = false;
        this.picGraph.Click += new System.EventHandler(this.picGraph_Click);
        this.picGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.picGraph_Paint);
        // 
        // btnOK
        // 
        this.btnOK.Enabled = false;
        this.btnOK.Location = new System.Drawing.Point(213, 112);
        this.btnOK.Name = "btnOK";
        this.btnOK.Size = new System.Drawing.Size(75, 23);
        this.btnOK.TabIndex = 1;
        this.btnOK.Text = "OK";
        this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
        // 
        // timerUpdate
        // 
        this.timerUpdate.Enabled = true;
        this.timerUpdate.SynchronizingObject = this;
        this.timerUpdate.Elapsed += new System.Timers.ElapsedEventHandler(this.timerUpdate_Elapsed);
        // 
        // txtStatus
        // 
        this.txtStatus.Location = new System.Drawing.Point(8, 112);
        this.txtStatus.Name = "txtStatus";
        this.txtStatus.ReadOnly = true;
        this.txtStatus.Size = new System.Drawing.Size(200, 20);
        this.txtStatus.TabIndex = 2;
        this.txtStatus.Text = "Status: Initiating endurance test";
        // 
        // WirelessSensorNetworkTest
        // 
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        this.ClientSize = new System.Drawing.Size(292, 142);
        this.Controls.Add(this.txtStatus);
        this.Controls.Add(this.btnOK);
        this.Controls.Add(this.picGraph);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
        this.Name = "WirelessSensorNetworkTest";
        this.Text = "Wireless Sensor Network Test";
        ((System.ComponentModel.ISupportInitialize)(this.picGraph)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.timerUpdate)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

	}
		#endregion

		#endregion

		#region Button-click event handlers

		private void btnOK_Click(object sender, System.EventArgs e) {
			Close();
		}

		#endregion
		
		#region Paint and Timer event function
		
		private void picGraph_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			// draw background
			g.FillRectangle(System.Drawing.Brushes.BlanchedAlmond, e.ClipRectangle);
			// calculate max value, so that we can scale appropriately
			int i, iMax = 0;
			for (i = 0; i < 100; i++)
				iMax = Math.Max(iMax, network.iTestResults[i]);
			// draw the graph
			for (i = 0; i < 100; i++) {
				int iHeight = (int) (picGraph.Height * network.iTestResults[i] / iMax);
				if (network.iTestResults[i] > 0)
					g.FillRectangle(Brushes.Blue, picGraph.Width * i / 100, picGraph.Height - iHeight, picGraph.Width * (i + 1) / 100 - picGraph.Width * i / 100, iHeight);
				else {  // we've reached the last updated value - break out of the loop
					txtStatus.Text = "Status: " + (i - 1) + " / 100";
					txtStatus.Refresh();
					break;
				}
			}
			if (i == 100) {  // we didn't break out of the loop above, so assume we're on the last test or simply finished - calculate and display results
				int iTotal = 0;
				for (i = 0; i < 100; i++)
					iTotal += network.iTestResults[i];
				iTotal /= 100;
				txtStatus.Text = "Status: Complete. Average steps: " + iTotal;
				txtStatus.Refresh();
			}
		}

		private void timerUpdate_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			if ((network.bTesting == false)  && (btnOK.Enabled == false)) // testing is done - enable button
				btnOK.Enabled = true;
			picGraph.Refresh();
		}

		#endregion

        private void picGraph_Click(object sender, EventArgs e)
        {

        }
	
	}
}

// Wireless Sensor Network Simulator v1.0 (and all future versions), together with all accompanying source code and documentation, is copyright (c) David J. Stein, Esq., March 2005. All rights reserved except as set forth in the included Wireless Sensor Network License Agreement.
