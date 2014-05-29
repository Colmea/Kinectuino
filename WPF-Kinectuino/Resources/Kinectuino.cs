using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace WPF_Kinectuino.Resources
{
    class Kinectuino
    {
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
    }
}
