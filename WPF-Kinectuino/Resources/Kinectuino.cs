using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows;

namespace WPF_Kinectuino.Resources
{
    class Kinectuino
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Serial Port to communicate with Arduino
        /// </summary>
        private SerialPort serialPort;

        /// <summary>
        /// Serial port update frequency (in milliseconds)
        /// </summary>
        public int updateFrequency { get; private set; }

        /// <summary>
        /// Time since last update to Serial port
        /// </summary>
        private long timeLastUpdateSerial = DateTime.Now.Ticks / 10000;

        /// <summary>
        /// List of active/not active joints
        /// </summary>
        private SortedList jointsActivation = new SortedList();

        /// <summary>
        /// Initialize Kinecuino class
        /// </summary>
        public Kinectuino()
        {
            Debug.WriteLine("Kinectuino initialize...");

            this.setUpdateFrequency(100);
            this.initDefaultJointsActivation();
        }

        public void setSensor(KinectSensor sensor)
        {
            this.sensor = sensor;

            // Add an event handler to be called whenever there is new color frame data
            this.sensor.SkeletonFrameReady += SensorSkeletonFrameReadyEvent;
        }

        /// <summary>
        /// Update Skeleton and joints position. Send data throught serial port if needed
        /// </summary>
        /// <param name="skeleton">The tracked skeleton</param>
        public void updateSkeleton(Skeleton skeleton)
        {
            long timeStampNow = DateTime.Now.Ticks/10000;

            // Only send data if updateFrequency is reached
            if ((timeStampNow - this.timeLastUpdateSerial) > this.updateFrequency)
            {
                
                // Head
                if (this.isJointEnabled(JointType.Head))
                    this.sendJointToSerial(skeleton.Joints[JointType.Head]);

                this.timeLastUpdateSerial = DateTime.Now.Ticks / 10000;
            }
        }

        private void sendJointToSerial(Joint joint)
        {
            Point position = this.SkeletonPointToScreen(joint.Position);

            Debug.WriteLine("SERIAL: " + joint.JointType + " is at " + position.X + ", " + position.Y);
        }

        public void setUpdateFrequency(int updateFrequency)
        {
            this.updateFrequency = updateFrequency;
            Debug.WriteLine("Kinectuino new update frequendy: " + this.updateFrequency);
        }

        /// <summary>
        /// Set and initialize serial port
        /// </summary>
        /// <param name="port">port name</param>
        /// <returns></returns>
        public SerialPort setSerialPort(string port)
        {
            try
            {
                this.serialPort = new SerialPort(port);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error when init serial port");
            }

            Debug.WriteLine("Serial Port " + port + " initialized");
            return (SerialPort)this.serialPort;
        }

        /// <summary>
        /// Initialize default joins activation sorted list
        /// </summary>
        private void initDefaultJointsActivation()
        {
            this.jointsActivation.Add(JointType.Head,            false);
            this.jointsActivation.Add(JointType.Spine,           false);
            this.jointsActivation.Add(JointType.ShoulderCenter,  false);
            this.jointsActivation.Add(JointType.ShoulderLeft,    false);
            this.jointsActivation.Add(JointType.ShoulderRight,   false);
            this.jointsActivation.Add(JointType.ElbowRight,      false);
            this.jointsActivation.Add(JointType.ElbowLeft,       false);
            this.jointsActivation.Add(JointType.WristLeft,       false);
            this.jointsActivation.Add(JointType.WristRight,      false);
            this.jointsActivation.Add(JointType.HandLeft,        false);
            this.jointsActivation.Add(JointType.HandRight,       false);
            this.jointsActivation.Add(JointType.HipCenter,       false);
            this.jointsActivation.Add(JointType.HipLeft,         false);
            this.jointsActivation.Add(JointType.HipRight,        false);
            this.jointsActivation.Add(JointType.KneeLeft,        false);
            this.jointsActivation.Add(JointType.KneeRight,       false);
            this.jointsActivation.Add(JointType.AnkleLeft,       false);
            this.jointsActivation.Add(JointType.AnkleRight,      false);
            this.jointsActivation.Add(JointType.FootLeft,        false);
            this.jointsActivation.Add(JointType.FootRight,       false);
        }

        public bool isJointEnabled(JointType jointType)
        {
            return (bool)this.jointsActivation[jointType];
        }

        public void setJointEnabled(JointType jointType, Boolean flag)
        {
            this.jointsActivation[jointType] = flag;
            Debug.WriteLine(jointType + " new val: " + this.jointsActivation[jointType]);
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReadyEvent(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }


            if (skeletons.Length != 0)
            {
                bool firstTrackedSkeletonFound = false;

                // Only send data to Kinectuino from first tracked skeleton
                // @TODO Handle more than one tracked skeleton
                foreach (Skeleton skel in skeletons)
                {
                    if (SkeletonTrackingState.Tracked == skel.TrackingState && !firstTrackedSkeletonFound)
                    {
                        this.updateSkeleton(skel);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
    }
}
