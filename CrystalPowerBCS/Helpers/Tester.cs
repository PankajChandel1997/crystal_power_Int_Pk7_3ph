// -----------------------------------------------------------------------
// <copyright file="Tester.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
#define DCU_TEST
namespace CrystalPowerBCS.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gurux.Common;
    using Gurux.DLMS;
    using Gurux.DLMS.Secure;
    using Gurux.DLMS.Enums;
    using Gurux.DLMS.Objects;
    using System.Threading;
    using Gurux.DLMS.Objects.Enums;
    using System.IO;
    using Microsoft.VisualBasic.Logging;
    using System.Threading.Tasks;

    /// <summary>
    /// Tests connection to the meter.
    /// </summary>
    public class Tester
    {
        bool Trace = true;
        IGXMedia media;
        GXDLMSSecureClient cl;
        GXDLMSClient public_client;
        bool isSecureClient = false;
        //GXDLMSSecureClient cl;
        string Lpass, Hpass;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Tester(IGXMedia target, InterfaceType Interface, int Device_ID)
        {
            media = target;
            int logicalname = Convert.ToInt32(Interface == InterfaceType.HDLC);
            byte[] blockCipherKey = ASCIIEncoding.ASCII.GetBytes(MeterKeys.Active_Encryption);

            byte[] authenticationKey = ASCIIEncoding.ASCII.GetBytes(MeterKeys.Active_Authentication);

            cl = new GXDLMSSecureClient(true, 0x30, Gurux.DLMS.GXDLMSClient.GetServerAddress(logicalname, Device_ID), Authentication.High, MeterKeys.Active_Password, Interface, Security.AuthenticationEncryption, ASCIIEncoding.ASCII.GetBytes("GRX12345"), blockCipherKey, authenticationKey);
            public_client = new GXDLMSClient(true, 0x10, 1, Authentication.None, null, Gurux.DLMS.Enums.InterfaceType.HDLC);
        }
        public void update_interface(InterfaceType Interface, int Device_ID)
        {
            int logicalname = Convert.ToInt32(Interface == InterfaceType.HDLC);
            cl.ServerAddress = Gurux.DLMS.GXDLMSClient.GetServerAddress(logicalname, Device_ID);
            cl.InterfaceType = Interface;
        }
        public void update_ClientAddress(int addr)
        {
            cl.ClientAddress = addr;
        }
        List<KeyValuePair<string, UInt16>> GetMeters(object[] data)
        {
            string gw = GXDLMSClient.ChangeType((byte[])data[0], DataType.String) as string;
            Console.WriteLine("GW: : " + gw);
            List<KeyValuePair<string, UInt16>> list = new List<KeyValuePair<string, UInt16>>();
            foreach (object it in (object[])data[1])
            {
                string devName = GXDLMSClient.ChangeType((byte[])((object[])it)[0], DataType.String) as string;
                UInt16 channel = (UInt16)((object[])it)[1];
                list.Add(new KeyValuePair<string, UInt16>(devName, channel));
            }
            return list;
        }
        public void MediaOpen()
        {
            media.Open();
        }
        public void MediaClose()
        {
            media.Close();
        }
        public void echoTest(byte[] data)
        {
            media.Open();
            if (data == null)
            {
                return;
            }
            object eop = (byte)'\n';
            //In network connection terminator is not used.
            if (cl.InterfaceType == InterfaceType.WRAPPER)
            {
                eop = null;
            }
            int pos = 0;
            bool succeeded = false;
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                Eop = eop,
                Count = 1,
                WaitTime = 3000,
            };
            lock (media.Synchronous)
            {
                while (!succeeded && pos != 3)
                {
                    WriteTrace("<- " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(data, true));
                    media.Send(data, null);
                    succeeded = media.Receive(p);
                    if (!succeeded)
                    {
                        //If Eop is not set read one byte at time.
                        if (p.Eop == null)
                        {
                            p.Count = 1;
                        }
                        //Try to read again...
                        if (++pos != 3)
                        {
                            System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                            continue;
                        }
                        throw new Exception("Failed to receive reply from the device in given time.");
                    }
                }
                try
                {
                    //Loop until whole COSEM packet is received.                
                    while (data.Length != p.Reply.Length)
                    {
                        //If Eop is not set read one byte at time.
                        if (p.Eop == null)
                        {
                            p.Count = 1;
                        }
                        while (!media.Receive(p))
                        {
                            //Try to read again...
                            if (++pos != 3)
                            {
                                System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                                continue;
                            }
                            throw new Exception("Failed to receive reply from the device in given time.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public void WriteActionSchedule()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                //////////////////////////////

                //////////////////////////////
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                actionS.LogicalName = "0.0.15.0.0.255";//0, 0,  15, 0, 0, 255
                //actionS.ExecutedScriptLogicalName = "0.0.1.0.0.255";
                actionS.ExecutedScriptSelector = 1;

                actionS.Type = SingleActionScheduleType.SingleActionScheduleType2;
                actionS.ExecutionTime = new GXDateTime[] {
                    new GXDateTime(00, 00, 14, 01, 02, 30, 00)
                };
                Console.WriteLine("Reading bill date\n");
                ReadDataBlock(cl.Read(actionS, 4), reply);
                Console.WriteLine("writing bill date\n");
                ReadDataBlock(cl.Write(actionS, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        public void ReadActionSchedule()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                //////////////////////////////

                //////////////////////////////
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                actionS.LogicalName = "0.0.15.0.0.255";//0, 0,  15, 0, 0, 255
                actionS.ExecutedScriptSelector = 1;

                actionS.Type = SingleActionScheduleType.SingleActionScheduleType2;
                actionS.ExecutionTime = new GXDateTime[] {
                    new GXDateTime(00, 00, 14, 01, 02, 30, 00)
                };
                Console.WriteLine("Reading bill date\n");
                ReadDataBlock(cl.Read(actionS, 4), reply);
                //Console.WriteLine("writing bill date\n");
                //ReadDataBlock(cl.Write(actionS, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        //
        public void SerialWriteActivityCalendar()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();

                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
                //byte[] logicalname = new byte[] {0,0,10,0,0,255 };
                string logicalname = "0.0.10.0.0.255";
                activityC.LogicalName = "0.0.13.0.0.255";
                Console.WriteLine("");
                Console.WriteLine("Reading Passive calendar");
                Console.WriteLine("");
                // ReadDataBlock(cl.Read(activityC, 9), reply);
                // reply.Clear();
                activityC.DayProfileTablePassive = new GXDLMSDayProfile[] { new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[]
                {
                    new GXDLMSDayProfileAction(new GXTime(17, 00, 00, 00), logicalname, 1),
                    new GXDLMSDayProfileAction(new GXTime(22, 00, 00, 00), logicalname, 2),
                    new GXDLMSDayProfileAction(new GXTime(06, 00, 00, 00), logicalname, 3)
                })};
                Console.WriteLine("");
                Console.WriteLine("Writing Passive calendar");
                Console.WriteLine("");


                ReadDataBlock(cl.Write(activityC, 9), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        public void WriteActionSchedule(byte channel)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                Console.WriteLine("Reading meter list.");
                GXDLMSData d = new GXDLMSData("0.128.0.0.0.255");
                ReadDataBlock(cl.Read(d, 2), reply);

                Console.WriteLine("Parsing meter list.");
                List<KeyValuePair<string, UInt16>> meters = GetMeters((object[])reply.Value);
                foreach (var it in meters)
                {
                    Console.WriteLine(it.Value + " " + it.Key);
                }
                Console.WriteLine("Selecting first meter from the list.");
                reply.Clear();

                //Select channel 1.
                d = new GXDLMSData("0.128.1.0.0.255");
                // d.Value = meters[1].Value;
                d.Value = channel;
                Console.WriteLine("Selecting channel: " + d.Value.ToString());
                reply.Clear();
                ReadDataBlock(cl.Write(d, 2), reply);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Channel selected: " + d.Value);

                Console.WriteLine("Reading Device Logical Name.");
                d = new GXDLMSData("0.0.42.0.0.255");
                d.SetUIDataType(2, DataType.String);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Value: " + d.Value.ToString());
                reply.Clear();
                //////////////////////////////

                //////////////////////////////
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                actionS.LogicalName = "0.0.15.0.0.255";//0, 0,  15, 0, 0, 255
                //actionS.ExecutedScriptLogicalName = "0.0.1.0.0.255";
                actionS.ExecutedScriptSelector = 1;

                actionS.Type = SingleActionScheduleType.SingleActionScheduleType2;
                actionS.ExecutionTime = new GXDateTime[] {
                    new GXDateTime(00, 00, 15, 17, 00, 00, 00)
                };
                //ReadDataBlock(cl.Write(actionS, 3), reply);
                ReadDataBlock(cl.Read(actionS, 4), reply);
                // ReadDataBlock(cl.Write(actionS, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        //
        public void WriteActionScheduleEvents()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                actionS.LogicalName = "0.0.128.0.1.255";//0, 0,  15, 0, 0, 255
                //actionS.ExecutedScriptLogicalName = "0.0.1.0.0.255";
                actionS.ExecutedScriptSelector = 1;

                actionS.Type = SingleActionScheduleType.SingleActionScheduleType2;
                actionS.ExecutionTime = new GXDateTime[] {
                    new GXDateTime(00, 00, 00, 00, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 04, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 08, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 12, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 16, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 20, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 00, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 00, 00, 00, 00)
                };
                //ReadDataBlock(cl.Write(actionS, 3), reply);
                ReadDataBlock(cl.Write(actionS, 4), reply);
                // ReadDataBlock(cl.Write(actionS, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public void ReadActionScheduleEvents()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                actionS.LogicalName = "0.0.128.0.1.255";//0, 0,  15, 0, 0, 255                
                ReadDataBlock(cl.Read(actionS, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public void WriteActionScheduleInstantprofile()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                actionS.LogicalName = "1.0.128.8.1.255";//0, 0,  15, 0, 0, 255
                actionS.ExecutedScriptSelector = 1;

                actionS.Type = SingleActionScheduleType.SingleActionScheduleType2;
                actionS.ExecutionTime = new GXDateTime[] {
                    new GXDateTime(00, 00, 00, 00, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 01, 00, 00, 00),
                };
                ReadDataBlock(cl.Write(actionS, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public void ReadActionScheduleInstantprofile()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                actionS.LogicalName = "1.0.128.8.1.255";//0, 0,  15, 0, 0, 255                
                ReadDataBlock(cl.Read(actionS, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public void WriteActionScheduleLoadProfile()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                actionS.LogicalName = "1.0.128.8.2.255";//0, 0,  15, 0, 0, 255
                //actionS.ExecutedScriptLogicalName = "0.0.1.0.0.255";
                actionS.ExecutedScriptSelector = 1;

                actionS.Type = SingleActionScheduleType.SingleActionScheduleType2;
                actionS.ExecutionTime = new GXDateTime[] {
                    new GXDateTime(00, 00, 00, 00, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 04, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 08, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 12, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 16, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 20, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 00, 00, 00, 00),
                    new GXDateTime(00, 00, 00, 00, 00, 00, 00)
                };
                //ReadDataBlock(cl.Write(actionS, 3), reply);
                ReadDataBlock(cl.Write(actionS, 4), reply);
                //ReadDataBlock(cl.Read(actionS, 4), reply);
                // ReadDataBlock(cl.Write(actionS, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public void ReadActionScheduleLoadProfile()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                actionS.LogicalName = "1.0.128.8.2.255";//0, 0,  15, 0, 0, 255                
                ReadDataBlock(cl.Read(actionS, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        //
        public void WriteActivityCalendar(byte channel)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                Console.WriteLine("Reading meter list.");
                GXDLMSData d = new GXDLMSData("0.128.0.0.0.255");
                ReadDataBlock(cl.Read(d, 2), reply);

                Console.WriteLine("Parsing meter list.");
                List<KeyValuePair<string, UInt16>> meters = GetMeters((object[])reply.Value);
                foreach (var it in meters)
                {
                    Console.WriteLine(it.Value + " " + it.Key);
                }
                Console.WriteLine("Selecting first meter from the list.");
                reply.Clear();

                //Select channel 1.
                d = new GXDLMSData("0.128.1.0.0.255");
                // d.Value = meters[1].Value;
                d.Value = channel;
                Console.WriteLine("Selecting channel: " + d.Value.ToString());
                reply.Clear();
                ReadDataBlock(cl.Write(d, 2), reply);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Channel selected: " + d.Value);

                Console.WriteLine("Reading Device Logical Name.");
                d = new GXDLMSData("0.0.42.0.0.255");
                d.SetUIDataType(2, DataType.String);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Value: " + d.Value.ToString());
                reply.Clear();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
                //byte[] logicalname = new byte[] {0,0,10,0,0,255 };
                string logicalname = "0.0.10.0.0.255";
                activityC.LogicalName = "0.0.13.0.0.255";
                Console.WriteLine("");
                Console.WriteLine("Reading Passive calendar");
                Console.WriteLine("");
                // ReadDataBlock(cl.Read(activityC, 9), reply);
                // reply.Clear();
                activityC.DayProfileTablePassive = new GXDLMSDayProfile[] { new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[]
                {
                    new GXDLMSDayProfileAction(new GXTime( 17, 00, 00, 00), logicalname, 1),
                    new GXDLMSDayProfileAction(new GXTime( 22, 00, 00, 00), logicalname, 2),
                    new GXDLMSDayProfileAction(new GXTime( 06, 00, 00, 00), logicalname, 3),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, 00), logicalname, 4),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, 00), logicalname, 5),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, 00), logicalname, 6),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, 00), logicalname, 7),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, 00), logicalname, 8)
                })};
                activityC.Time = new GXDateTime(2016, 11, 15, 17, 1, 2, 3);
                Console.WriteLine("");
                Console.WriteLine("Writing Passive calendar");
                Console.WriteLine("");


                //ReadDataBlock(cl.Write(activityC, 9), reply);
                ReadDataBlock(cl.Write(activityC, 10), reply);

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        //
        public void WriteActivityCalendar()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
                //byte[] logicalname = new byte[] {0,0,10,0,0,255 };
                string logicalname = "0.0.10.0.0.255";
                activityC.LogicalName = "0.0.13.0.0.255";
                Console.WriteLine("");
                Console.WriteLine("Reading Passive calendar");
                Console.WriteLine("");
                // ReadDataBlock(cl.Read(activityC, 9), reply);
                // reply.Clear();
                activityC.DayProfileTablePassive = new GXDLMSDayProfile[] { new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[]
                {
                    new GXDLMSDayProfileAction(new GXTime( 17, 00, 00, -1), logicalname, 1),
                    new GXDLMSDayProfileAction(new GXTime( 22, 00, 00, -1), logicalname, 2),
                    new GXDLMSDayProfileAction(new GXTime( 06, 00, 00, -1), logicalname, 3),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 17, 00, 00, -1), logicalname, 1),
                    new GXDLMSDayProfileAction(new GXTime( 22, 00, 00, -1), logicalname, 2),
                    new GXDLMSDayProfileAction(new GXTime( 06, 00, 00, -1), logicalname, 3),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 17, 00, 00, -1), logicalname, 1),
                    new GXDLMSDayProfileAction(new GXTime( 22, 00, 00, -1), logicalname, 2),
                    new GXDLMSDayProfileAction(new GXTime( 06, 00, 00, -1), logicalname, 3),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 17, 00, 00, -1), logicalname, 1),
                    new GXDLMSDayProfileAction(new GXTime( 22, 00, 00, -1), logicalname, 2),
                    new GXDLMSDayProfileAction(new GXTime( 06, 00, 00, -1), logicalname, 3),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                    new GXDLMSDayProfileAction(new GXTime( 00, 00, 00, -1), logicalname, 0),
                })};
                activityC.Time = new GXDateTime(2016, 11, 15, 17, 1, 2, -1);
                Console.WriteLine("");
                Console.WriteLine("Writing Passive calendar");
                Console.WriteLine("");


                ReadDataBlock(cl.Write(activityC, 9), reply);
                //ReadDataBlock(cl.Write(activityC, 10), reply);

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        //

        public void ReadActivityCalendar()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
                //byte[] logicalname = new byte[] {0,0,10,0,0,255 };

                ReadDataBlock(cl.Read(activityC, 2), reply);
                cl.UpdateValue(activityC, 2, reply.Value);
                reply.Clear();

                ReadDataBlock(cl.Read(activityC, 3), reply);
                cl.UpdateValue(activityC, 3, reply.Value);
                reply.Clear();

                ReadDataBlock(cl.Read(activityC, 4), reply);
                cl.UpdateValue(activityC, 4, reply.Value);
                reply.Clear();

                ReadDataBlock(cl.Read(activityC, 5), reply);
                cl.UpdateValue(activityC, 5, reply.Value);
                reply.Clear();

                ReadDataBlock(cl.Read(activityC, 6), reply);
                cl.UpdateValue(activityC, 6, reply.Value);
                reply.Clear();

                ReadDataBlock(cl.Read(activityC, 7), reply);
                cl.UpdateValue(activityC, 7, reply.Value);
                reply.Clear();

                ReadDataBlock(cl.Read(activityC, 8), reply);
                cl.UpdateValue(activityC, 8, reply.Value);
                reply.Clear();

                ReadDataBlock(cl.Read(activityC, 9), reply);
                cl.UpdateValue(activityC, 9, reply.Value);
                reply.Clear();

                ReadDataBlock(cl.Read(activityC, 10), reply);
                cl.UpdateValue(activityC, 10, reply.Value);
                reply.Clear();
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
                Console.WriteLine("\n________________________________________________________________________________________");
            }
        }

        public void Scriptmethod()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSScriptTable scriptTable = new GXDLMSScriptTable();
                scriptTable.LogicalName = "0.0.10.0.1.255";
                ReadDataBlock(cl.Method(scriptTable, 1, 1, DataType.UInt16), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }
        public void WriteClock(DateTime Time)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSClock Clock = new GXDLMSClock();
                Clock.LogicalName = "0.0.1.0.0.255";//0, 0,  15, 0, 0, 255
                Clock.Time = new GXDateTime(Time);
                // Clock.Time = new GXDateTime();
                reply.Clear();
                ReadDataBlock(cl.Write(Clock, 2), reply);
                //  ReadDataBlock(cl.Read(Clock, 2), reply);
                cl.UpdateValue(Clock, 2, reply.Value);
            }
            finally
            {
                reply.Clear();
            }
        }

        public void ReadClock()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSClock Clock = new GXDLMSClock();
                Clock.LogicalName = "0.0.1.0.0.255";//0, 0,  15, 0, 0, 255

                ReadDataBlock(cl.Read(Clock, 2), reply);
                cl.UpdateValue(Clock, 2, reply.Value);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }
        public byte[] ReadClock2()
        {
            GXReplyData reply = new GXReplyData();
            ushort year = 0;
            byte[] time = null;
            try
            {
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSClock Clock = new GXDLMSClock();
                Clock.LogicalName = "0.0.1.0.0.255";
                ReadDataBlock(cl.Read(Clock, 2), reply);
                if (reply.Value is byte[])
                    time = (byte[])reply.Value;
                cl.UpdateValue(Clock, 2, reply.Value);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                reply.Clear();
            }
            if (time != null)
            {
                //year = ((short)((byte[])time)[0] << 8) | (ushort)((byte[])time)[2];
                year = (ushort)((byte[])time)[0];
                year <<= 8;
                year |= (ushort)((byte[])time)[1];
                if ((((byte[])time)[3] > 13) || (((byte[])time)[5] > 23) || (year != 2022))
                {
                    ((byte[])time)[11] = 1;
                }
                else
                {
                    ((byte[])time)[11] = 0;
                }
                return time;
            }
            return null;
        }

        public ushort coveropen()
        {
            GXReplyData reply = new GXReplyData();
            ushort co = 0;
            try
            {
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSData coverOp = new GXDLMSData();
                coverOp.LogicalName = "0.0.96.11.5.255";
                ReadDataBlock(cl.Read(coverOp, 2), reply);
                if (reply.Value is ushort)
                    co = (ushort)reply.Value;
                cl.UpdateValue(coverOp, 2, reply.Value);
            }
            finally
            {
                reply.Clear();
            }
            return co;
        }
        public GXDateTime Get_BillDate()
        {
            GXReplyData reply = new GXReplyData();
            GXDateTime time2;
            time2 = new GXDateTime();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule Action = new GXDLMSActionSchedule();
                Action.LogicalName = "0.0.15.0.0.255";
                ReadDataBlock(cl.Read(Action, 4), reply);
                cl.UpdateValue(Action, 4, reply.Value);
                time2 = Action.ExecutionTime[0];
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
            if (time2 != null)
                return time2;
            return null;
        }
        public void Set_BillDate(DateTime txtMyDate)
        {
            GXReplyData reply = new GXReplyData();

            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule Action = new GXDLMSActionSchedule();
                Action.LogicalName = "0.0.15.0.0.255";
                Action.ExecutionTime = new GXDateTime[] {
                    new GXDateTime(-1, -1, txtMyDate.Day, txtMyDate.Hour, txtMyDate.Minute, txtMyDate.Second, -1) };
                ReadDataBlock(cl.Write(Action, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }

        public GXDateTime Get_ImageActivationDate()
        {
            GXReplyData reply = new GXReplyData();
            GXDateTime time2;
            time2 = new GXDateTime();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule Action = new GXDLMSActionSchedule();
                Action.LogicalName = "0.0.15.0.2.255";
                ReadDataBlock(cl.Read(Action, 4), reply);
                cl.UpdateValue(Action, 4, reply.Value);
                time2 = Action.ExecutionTime[0];
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
            if (time2 != null)
                return time2;
            return null;
        }
        public void Set_ImageActivationDate(DateTime txtMyDate)
        {
            GXReplyData reply = new GXReplyData();

            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule Action = new GXDLMSActionSchedule();
                Action.LogicalName = "0.0.15.0.2.255";
                Action.ExecutionTime = new GXDateTime[] {
                    new GXDateTime(txtMyDate.Year, txtMyDate.Month, txtMyDate.Day, txtMyDate.Hour, txtMyDate.Minute, txtMyDate.Second, -1) };
                ReadDataBlock(cl.Write(Action, 4), reply);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }
        public void Set_GlobalKey(byte[] key, byte[] Kek)
        {
            GXReplyData reply = new GXReplyData();
            List<KeyValuePair<GlobalKeyType, byte[]>> list = new List<KeyValuePair<GlobalKeyType, byte[]>>();
            list.Add(new KeyValuePair<GlobalKeyType, byte[]>(GlobalKeyType.UnicastEncryption, key));
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSSecuritySetup security = new GXDLMSSecuritySetup();
                security.LogicalName = "0.0.43.0.3.255";
                ReadDataBlock(security.GlobalKeyTransfer(cl, Kek, list), reply);
            }
            finally
            {
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        public void Set_LLSKey(byte[] key)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSAssociationLogicalName association = new GXDLMSAssociationLogicalName();
                association.LogicalName = "0.0.40.0.2.255";
                association.Secret = key;
                // Clock.Time = new GXDateTime();
                ReadDataBlock(cl.Write(association, 7), reply);
                reply.Clear();
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }
        public void Set_HLSKey(byte[] key)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSAssociationLogicalName association = new GXDLMSAssociationLogicalName();
                association.LogicalName = "0.0.40.0.3.255";
                association.Secret = key;
                // Clock.Time = new GXDateTime();
                ReadDataBlock(cl.Method(association, 2, key, DataType.OctetString), reply);
                reply.Clear();
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }
        public void Set_FWKey(byte[] key)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSAssociationLogicalName association = new GXDLMSAssociationLogicalName();
                association.LogicalName = "0.0.40.0.5.255";
                association.Secret = key;
                // Clock.Time = new GXDateTime();
                ReadDataBlock(cl.Method(association, 2, key, DataType.OctetString), reply);
                reply.Clear();
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }
        public void UpdateEncryptionKey(byte[] key)
        {
            if (key.Length == 16)
            {
                cl.update_blockCipherKey(key);
            }
        }
        public void UpdateAuthenticationKey(byte[] key)
        {
            if (key.Length == 16)
            {
                cl.update_authenticationKey(key);
            }
        }
        public void UpdateHLSKey(byte[] key)
        {
            if (key.Length == 16)
            {
                cl.update_HLSKey(key);
            }
        }
        public void ReadClock_TimeZone()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSClock Clock = new GXDLMSClock();
                Clock.LogicalName = "0.0.1.0.0.255";//0, 0,  15, 0, 0, 255
                //Clock.Time = new GXDateTime(2017, 02, 27, 12, 36, 10, 00);
                // Clock.Time = new GXDateTime();
                // ReadDataBlock(cl.Write(Clock, 2), reply);
                ReadDataBlock(cl.Read(Clock, 3), reply);
                cl.UpdateValue(Clock, 3, reply.Value);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }

        public void Read_ImageTransfer()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSImageTransfer Image_Tx = new GXDLMSImageTransfer();
                Image_Tx.LogicalName = "0.0.44.0.0.255";

                ReadDataBlock(cl.Read(Image_Tx, 7), reply);
                cl.UpdateValue(Image_Tx, 7, reply.Value);
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }

        public void DLMS_Condiscon(byte state)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Disconnecting meter");
                    ReadDataBlock(cl.Method(discon_obj, state, 0, DataType.Int8), reply);
                    Thread.Sleep(2000);
                    ReadDataBlock(cl.Read(discon_obj, 2), reply);
                    ReadDataBlock(cl.Read(discon_obj, 3), reply);

                    //  cl.UpdateValue(discon_obj, 3, reply.Value);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);

            }
        }

        //
        public void WriteOverLoadVal(byte channel, ushort val)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                /*
                GXDLMSClock Clock1 = new GXDLMSClock();
                GXDLMSClock Clock2 = new GXDLMSClock();
                Clock1.Time = new GXDateTime(2016, 12, 7, 00, 00, 00, 00);
                Clock2.Time = new GXDateTime(2016, 12, 7, 14, 30, 00, 00);
                */


                /*
                GXDateTime s = new GXDateTime(2016, 12, 21, 11, 00, 00, 00);
                GXDateTime e = new GXDateTime(2016, 12, 21, 12, 00, 00, 00);
                */
                Initialize();

                Console.WriteLine("Reading meter list.");
                GXDLMSData d = new GXDLMSData("0.128.0.0.0.255");
                ReadDataBlock(cl.Read(d, 2), reply);

                Console.WriteLine("Parsing meter list.");
                List<KeyValuePair<string, UInt16>> meters = GetMeters((object[])reply.Value);
                foreach (var it in meters)
                {
                    Console.WriteLine(it.Value + " " + it.Key);
                }
                Console.WriteLine("Selecting first meter from the list.");
                reply.Clear();

                //Select channel 1.
                d = new GXDLMSData("0.128.1.0.0.255");
                // d.Value = meters[1].Value;
                d.Value = channel;
                Console.WriteLine("Selecting channel: " + d.Value.ToString());
                reply.Clear();
                ReadDataBlock(cl.Write(d, 2), reply);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Channel selected: " + d.Value);

                Console.WriteLine("Reading Device Logical Name.");
                d = new GXDLMSData("0.0.42.0.0.255");
                d.SetUIDataType(2, DataType.String);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Value: " + d.Value.ToString());
                reply.Clear();

                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                //GXDLMSClock Clock = new GXDLMSClock();
                GXDLMSData OverLoadVal = new GXDLMSData();
                OverLoadVal.LogicalName = "0.128.128.0.1.255";//0, 0,  15, 0, 0, 255
                OverLoadVal.Value = val;
                //ReadDataBlock(cl.Write(actionS, 3), reply);
                Console.WriteLine("Writing over load value.");
                ReadDataBlock(cl.Write(OverLoadVal, 2), reply);
                ReadDataBlock(cl.Read(OverLoadVal, 2), reply);
                cl.UpdateValue(OverLoadVal, 2, reply.Value);
                Console.WriteLine("Value: " + OverLoadVal.Value.ToString());

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }

        }
        //

        public void connect_disconnect_meter(byte channel, byte state)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                Console.WriteLine("6. Reading meter list.");
                GXDLMSData d = new GXDLMSData("0.128.0.0.0.255");
                ReadDataBlock(cl.Read(d, 2), reply);
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                Console.WriteLine("Parsing meter list.");
                List<KeyValuePair<string, UInt16>> meters = GetMeters((object[])reply.Value);
                foreach (var it in meters)
                {
                    Console.WriteLine(it.Value + " " + it.Key);
                }
                Console.WriteLine("7. Selecting first meter from the list.");
                reply.Clear();

                //Select channel 1.
                d = new GXDLMSData("0.128.1.0.0.255");
                // d.Value = meters[1].Value;
                d.Value = channel;
                Console.WriteLine("Selecting channel: " + d.Value.ToString());
                reply.Clear();
                ReadDataBlock(cl.Write(d, 2), reply);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Channel selected: " + d.Value);

                Console.WriteLine("2. Reading Device Logical Name.");
                d = new GXDLMSData("0.0.42.0.0.255");
                d.SetUIDataType(2, DataType.String);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Value: " + d.Value.ToString());

                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Disconnecting meter");
                    ReadDataBlock(cl.Method(discon_obj, state, 0, DataType.Int8), reply);
                    Thread.Sleep(2000);
                    ReadDataBlock(cl.Read(discon_obj, 2), reply);
                    ReadDataBlock(cl.Read(discon_obj, 3), reply);

                    //  cl.UpdateValue(discon_obj, 3, reply.Value);
                    reply.Clear();
                }


            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }
        public byte[] Read_Meter_LogicalName()
        {
            GXReplyData reply = new GXReplyData();
            GXDLMSData data_object = new GXDLMSData("0.0.42.0.0.255");
            byte[] result = null;
            media.Open();
            try
            {
                Initialize();
                Console.WriteLine("Reading data object value");
                ReadDataBlock(cl.Read(data_object, 2), reply);
                // if(reply.Value is byte[])
                result = (byte[])reply.Value;
                cl.UpdateValue(data_object, 2, reply.Value);
                reply.Clear();
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            media.Close();
            return result;
        }
        public byte[] Read_Meter_LogicalName2()
        {
            GXReplyData reply = new GXReplyData();
            GXDLMSData data_object = new GXDLMSData("0.0.42.0.0.255");
            byte[] result = null;
            try
            {
                Console.WriteLine("Reading data object value");
                ReadDataBlock(cl.Read(data_object, 2), reply);
                // if(reply.Value is byte[])
                result = (byte[])reply.Value;
                cl.UpdateValue(data_object, 2, reply.Value);
                reply.Clear();
            }
            finally
            {
                reply.Clear();
            }
            return result;
        }
        public void Read_data_object(GXDLMSData data_object, byte string_val)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();


                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading data object value");
                    ReadDataBlock(cl.Read(data_object, 2), reply);
                    cl.UpdateValue(data_object, 2, reply.Value);
                    Console.WriteLine("Read data object value" + data_object.Value.ToString());

                    data_object.Value = "123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF 123456789ABCDEF";

                    Console.WriteLine("writing data object value");
                    ReadDataBlock(cl.Write(data_object, 2), reply);
                    Console.WriteLine("Reading data object value");
                    ReadDataBlock(cl.Read(data_object, 2), reply);
                    cl.UpdateValue(data_object, 2, reply.Value);
                    Console.WriteLine("Read data object value" + data_object.Value.ToString());

                    //     if (string_val == 1)
                    //     {
                    //         //data_object.SetUIDataType(2, DataType.String);
                    //     }
                    //     cl.UpdateValue(data_object, 2, reply.Value);
                    //     Console.WriteLine("Value: " + data_object.Value.ToString());
                    reply.Clear();
                }


            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            //Thread.Sleep(1000);
            media.Close();
        }

        public bool serial_disconnect_state()
        {
            GXReplyData reply = new GXReplyData();
            bool state = false;
            media.Open();
            try
            {
                Initialize();
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Disconnecting_meter");
                    ReadDataBlock(cl.Read(discon_obj, 2), reply);
                    cl.UpdateValue(discon_obj, 2, reply.Value);
                    if (reply.Value is bool)
                    {
                        state = (bool)reply.Value;
                    }
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
            return state;
        }
        public bool serial_disconnect_state2()
        {
            GXReplyData reply = new GXReplyData();
            bool state = false;
            try
            {
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Disconnecting_meter");
                    ReadDataBlock(cl.Read(discon_obj, 2), reply);
                    cl.UpdateValue(discon_obj, 2, reply.Value);
                    if (reply.Value is bool)
                    {
                        state = (bool)reply.Value;
                    }
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
            return state;
        }
        public void serial_disconnect_meter()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Disconnecting_meter");
                    ReadDataBlock(cl.Method(discon_obj, 1, 0, DataType.Int8), reply);
                    //
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }

        public void serial_connect_meter()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Connecting_meter 3");
                    ReadDataBlock(cl.Method(discon_obj, 2, 0, DataType.Int8), reply);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }
        public ushort GetDeviceHDLC_ID()
        {
            GXReplyData reply = new GXReplyData();
            ushort HDLC_ID = 0;
            try
            {
                GXDLMSHdlcSetup HdlcSetup = new GXDLMSHdlcSetup("0.0.22.0.0.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Read(HdlcSetup, 9), reply);
                    cl.UpdateValue(HdlcSetup, 9, reply.Value);
                    if (reply.Value is ushort)
                    {
                        HDLC_ID = (ushort)reply.Value;
                    }
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
            return HDLC_ID;
        }

        public void SetDeviceHDLC_ID(UInt16 ID)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSHdlcSetup HdlcSetup = new GXDLMSHdlcSetup("0.0.22.0.0.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    HdlcSetup.DeviceAddress = ID;
                    ReadDataBlock(cl.Write(HdlcSetup, 9), reply);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
        }
        public string GetSerialNo()
        {
            GXReplyData reply = new GXReplyData();
            string SerialNo = null;
            try
            {
                GXDLMSData data_obj = new GXDLMSData("0.0.96.1.0.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Read(data_obj, 2), reply);
                    cl.UpdateValue(data_obj, 2, reply.Value);
                    if (reply.Value is string)
                    {
                        SerialNo = (string)reply.Value;
                    }
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
            return SerialNo;
        }
        public byte GetCT_factor()
        {
            GXReplyData reply = new GXReplyData();
            byte CT_factor = 0;
            try
            {
                GXDLMSData data_obj = new GXDLMSData("1.0.0.4.2.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Read(data_obj, 2), reply);
                    cl.UpdateValue(data_obj, 2, reply.Value);
                    if (reply.Value is byte)
                    {
                        CT_factor = (byte)reply.Value;
                    }
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
            return CT_factor;
        }
        public byte GetMetering_Mode()
        {
            GXReplyData reply = new GXReplyData();
            byte Metring_Mode = 0;
            try
            {
                GXDLMSData data_obj = new GXDLMSData("0.0.94.96.19.255");

                ReadDataBlock(cl.Read(data_obj, 2), reply);
                cl.UpdateValue(data_obj, 2, reply.Value);
                if (reply.Value is byte)
                {
                    Metring_Mode = (byte)reply.Value;
                }
                reply.Clear();
            }
            finally
            {
                reply.Clear();
            }
            return Metring_Mode;
        }

        public void SetCT_factor(byte factor)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSData data_obj = new GXDLMSData("1.0.0.4.2.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    data_obj.Value = factor;
                    ReadDataBlock(cl.Write(data_obj, 2), reply);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
        }
        public void SetMetering_Mode(byte Mode)
        {
            GXReplyData reply = new GXReplyData();
            if (Mode != 0)
            {
                Mode = 1;
            }
            try
            {
                GXDLMSData data_obj = new GXDLMSData("0.0.94.91.19.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    data_obj.Value = Mode;
                    ReadDataBlock(cl.Write(data_obj, 2), reply);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
        }
        public UInt32 Get_DIP()
        {
            GXReplyData reply = new GXReplyData();
            UInt32 DIP_Value = 0;
            try
            {
                GXDLMSData data_obj = new GXDLMSData("1.0.0.8.0.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Read(data_obj, 2), reply);
                    cl.UpdateValue(data_obj, 2, reply.Value);
                    if (reply.Value is UInt32)
                    {
                        DIP_Value = (UInt32)reply.Value;
                    }
                    if (reply.Value is UInt16)
                    {
                        DIP_Value = (UInt16)reply.Value;
                    }
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
            return DIP_Value;
        }
        public void Set_DIP(UInt32 factor)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSData data_obj = new GXDLMSData("1.0.0.8.0.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    data_obj.Value = factor;
                    ReadDataBlock(cl.Write(data_obj, 2), reply);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
        }

        public UInt32 Get_PCP()
        {
            GXReplyData reply = new GXReplyData();
            UInt32 DIP_Value = 0;
            try
            {
                GXDLMSData data_obj = new GXDLMSData("1.0.0.8.4.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Read(data_obj, 2), reply);
                    cl.UpdateValue(data_obj, 2, reply.Value);
                    if (reply.Value is UInt32)
                    {
                        DIP_Value = (UInt32)reply.Value;
                    }
                    if (reply.Value is UInt16)
                    {
                        DIP_Value = (UInt16)reply.Value;
                    }
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
            return DIP_Value;
        }
        public void Set_PCP(UInt32 factor)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSData data_obj = new GXDLMSData("1.0.0.8.4.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    data_obj.Value = factor;
                    ReadDataBlock(cl.Write(data_obj, 2), reply);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
        }

        public UInt32 Get_LoadLimit()
        {
            GXReplyData reply = new GXReplyData();
            UInt32 DIP_Value = 0;
            try
            {
                GXDLMSLimiter data_obj = new GXDLMSLimiter("0.0.17.0.0.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Read(data_obj, 3), reply);
                    cl.UpdateValue(data_obj, 3, reply.Value);
                    if (reply.Value is UInt32)
                    {
                        DIP_Value = (UInt32)reply.Value;
                    }
                    if (reply.Value is UInt16)
                    {
                        DIP_Value = (UInt16)reply.Value;
                    }
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
            return DIP_Value;
        }
        public void Set_LoadLimit(UInt32 factor)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSLimiter data_obj = new GXDLMSLimiter("0.0.17.0.0.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    data_obj.ThresholdActive = factor;
                    ReadDataBlock(cl.Write(data_obj, 3), reply);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
        }
        public Object[] Get_TODData()
        {
            GXReplyData reply = new GXReplyData();
            Object[] ToDData = null;
            try
            {
                GXDLMSActivityCalendar data_obj = new GXDLMSActivityCalendar("0.0.13.0.0.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    cl.NegotiatedConformance = Conformance.Action | Conformance.SelectiveAccess | Conformance.Set | Conformance.Get | Conformance.DataNotification | Conformance.BlockTransferWithSetOrWrite | Conformance.BlockTransferWithGetOrRead;
                    ReadDataBlock(cl.Read(data_obj, 9), reply);
                    cl.UpdateValue(data_obj, 9, reply.Value);
                    if (reply.Value is Object[])
                    {
                        ToDData = (Object[])reply.Value;
                    }
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
            return ToDData;
        }
        public void Set_TODData(DateTime[] Time, ushort[] IDs)
        {
            GXReplyData reply = new GXReplyData();
            GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
            GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
            //byte[] logicalname = new byte[] {0,0,10,0,0,255 };
            string logicalname = "0.0.10.0.0.255";
            activityC.LogicalName = "0.0.13.0.0.255";
            cl.NegotiatedConformance = Conformance.Action | Conformance.SelectiveAccess | Conformance.Set | Conformance.Get | Conformance.DataNotification | Conformance.BlockTransferWithSetOrWrite | Conformance.BlockTransferWithGetOrRead;
            cl.Settings.MaxServerPDUSize = 0x0060;
            cl.Settings.MaxPduSize = 0x0060;
            activityC.DayProfileTablePassive = new GXDLMSDayProfile[] { new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[]
                {
                    new GXDLMSDayProfileAction(new GXTime( Time[0].Hour, Time[0].Minute, Time[0].Second, -1), logicalname, IDs[0]),
                    new GXDLMSDayProfileAction(new GXTime( Time[1].Hour, Time[1].Minute, Time[1].Second, -1), logicalname, IDs[1]),
                    new GXDLMSDayProfileAction(new GXTime( Time[2].Hour, Time[2].Minute, Time[2].Second, -1), logicalname, IDs[2]),
                    new GXDLMSDayProfileAction(new GXTime( Time[3].Hour, Time[3].Minute, Time[3].Second, -1), logicalname, IDs[3]),
                    new GXDLMSDayProfileAction(new GXTime( Time[4].Hour, Time[4].Minute, Time[4].Second, -1), logicalname, IDs[4]),
                    new GXDLMSDayProfileAction(new GXTime( Time[5].Hour, Time[5].Minute, Time[5].Second, -1), logicalname, IDs[5]),
                    new GXDLMSDayProfileAction(new GXTime( Time[6].Hour, Time[6].Minute, Time[6].Second, -1), logicalname, IDs[6]),
                    new GXDLMSDayProfileAction(new GXTime( Time[7].Hour, Time[7].Minute, Time[7].Second, -1), logicalname, IDs[7]),

                })};
            ReadDataBlock(cl.Write(activityC, 9), reply);
        }
        public GXDateTime Get_TODActivationDate()
        {
            GXReplyData reply = new GXReplyData();
            GXDateTime time2;
            time2 = new GXDateTime();
            try
            {
                GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
                //byte[] logicalname = new byte[] {0,0,10,0,0,255 };
                activityC.LogicalName = "0.0.13.0.0.255";

                ReadDataBlock(cl.Read(activityC, 10), reply);
                cl.UpdateValue(activityC, 10, reply.Value);
                time2 = activityC.Time;
            }
            finally
            {
                reply.Clear();
            }
            if (time2 != null)
                return time2;
            return null;
        }
        public void Set_TODActivationDate(DateTime Time)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
                //byte[] logicalname = new byte[] {0,0,10,0,0,255 };
                activityC.LogicalName = "0.0.13.0.0.255";

                activityC.Time = new GXDateTime(Time.Year, Time.Month, Time.Day, Time.Hour, Time.Minute, Time.Second, -1);
                ReadDataBlock(cl.Write(activityC, 10), reply);
            }
            finally
            {
                reply.Clear();
            }
        }
        public string Get_ActivityCalendarName()
        {
            GXReplyData reply = new GXReplyData();
            string Cal_Name;
            try
            {
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
                ReadDataBlock(cl.Read(activityC, 6), reply);
                cl.UpdateValue(activityC, 6, reply.Value);
                Cal_Name = activityC.CalendarNamePassive;
            }
            finally
            {
                reply.Clear();
            }
            return Cal_Name;
        }
        public void Set_ActivityCalendarName(string cal_name)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
                GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
                activityC.CalendarNamePassive = cal_name;
                ReadDataBlock(cl.Write(activityC, 6), reply);
            }
            finally
            {
                reply.Clear();
            }
        }
        public void Set_ESWF(string ESFW)
        {
            GXReplyData reply = new GXReplyData();
            GXDLMSData data_Obj = new GXDLMSData();
            try
            {
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                data_Obj.LogicalName = "0.0.94.91.26.255";
                data_Obj.SetDataType(2, DataType.BitString);
                data_Obj.Value = ESFW;
                ReadDataBlock(cl.Write(data_Obj, 2), reply);
            }
            finally
            {
                reply.Clear();
            }
        }
        public string Get_ESWF()
        {
            string ESFW = null;
            GXReplyData reply = new GXReplyData();
            GXDLMSData data_Obj = new GXDLMSData();
            try
            {
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                data_Obj.LogicalName = "0.0.94.91.26.255";

                ReadDataBlock(cl.Read(data_Obj, 2), reply);
                cl.UpdateValue(data_Obj, 2, reply.Value);
                if (data_Obj.Value is string)
                    ESFW = (string)data_Obj.Value;
            }
            finally
            {
                reply.Clear();
            }
            return ESFW;
        }
        public void HDLC_Disconnect()
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(cl.DisconnectRequest(), reply);
            isSecureClient = false;
        }
        public void serial_disconnect_meter2()
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Disconnecting_meter");
                    ReadDataBlock(cl.Method(discon_obj, 1, 0, DataType.Int8), reply);
                    //
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
        }

        public void serial_connect_meter2()
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Disconnecting_meter 3");
                    ReadDataBlock(cl.Method(discon_obj, 2, 0, DataType.Int8), reply);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
        }
        public void Capture_bill()
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSProfileGeneric profile = new GXDLMSProfileGeneric("1.0.98.1.0.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Disconnecting_meter 3");
                    ReadDataBlock(cl.Method(profile, 2, 0, DataType.Int8), reply);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
            }
        }
        public void SerialAssocaitionList2()
        {
            GXReplyData reply = new GXReplyData();
            List<GXDLMSObject> objects = new List<GXDLMSObject>();
            GXDLMSAssociationLogicalName Association = new GXDLMSAssociationLogicalName("0.0.40.0.0.255");
            objects.Add(Association);
            media.Open();
            try
            {
                Initialize();
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading object list");
                    ReadDataBlock(cl.Read(Association, 2), reply);
                    cl.UpdateValue(Association, 2, reply.Value);
                    reply.Clear();
                }


                /*
                List<GXDLMSObject> objects = new List<GXDLMSObject>();
                GXDLMSAssociationLogicalName ln = new GXDLMSAssociationLogicalName("0.0.40.0.0.255");
                objects.Add(ln);
                for (int pos = 0; pos != 10; ++pos)
                {
                    Console.WriteLine("Association view " + pos);
                    ReadDataBlock(cl.Read(ln, 2), reply);
                }
                foreach (var it in cl.Objects.GetObjects(ObjectType.ProfileGeneric))
                {
                    for (int pos = 0; pos != 10; ++pos)
                    {
                        Console.WriteLine("PG " + it.LogicalName + pos);
                        ReadDataBlock(cl.Read(it, ), reply);
                    }
                }
                 * */


                StringBuilder sb = new StringBuilder();
                foreach (GXDLMSObject row in Association.ObjectList)
                {


                    sb.Append(row.ToString());
                    sb.Append("\n");
                    foreach (var it in row.Attributes)
                    {
                        sb.Append("\n");
                        sb.Append(it.ToString());
                        sb.Append("\t");
                        sb.Append(it.Access.ToString());
                        sb.Append("\t");
                        sb.Append(Convert.ToByte(it.Access));
                    }
                    sb.Append(" ");
                    Console.WriteLine(sb.ToString());
                    sb.Length = 0;

                }

                Console.WriteLine(sb.ToString());

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(20);
            media.Close();
        }
        public void SerialAssocaitionList()
        {
            GXReplyData reply = new GXReplyData();
            List<GXDLMSObject> objects = new List<GXDLMSObject>();
            GXDLMSAssociationLogicalName Association = new GXDLMSAssociationLogicalName("0.0.40.0.0.255");
            objects.Add(Association);
            media.Open();
            try
            {
                Initialize();
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading object list");
                    ReadDataBlock(cl.Read(Association, 4), reply);
                    cl.UpdateValue(Association, 4, reply.Value);
                    reply.Clear();
                }

                StringBuilder sb = new StringBuilder();
                foreach (GXDLMSObject row in Association.ObjectList)
                {


                    sb.Append(row.ToString());
                    sb.Append("\n");
                    foreach (var it in row.Attributes)
                    {
                        sb.Append("\n");
                        sb.Append(it.ToString());
                        sb.Append("\t");
                        sb.Append(it.Access.ToString());
                        sb.Append("\t");
                        sb.Append(Convert.ToByte(it.Access));
                    }
                    sb.Append(" ");
                    Console.WriteLine(sb.ToString());
                    sb.Length = 0;

                }

                Console.WriteLine(sb.ToString());
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(10);
            media.Close();
        }

        public void SerialReadOneEntry(GXDLMSProfileGeneric pg)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading Attribute 3");
                    ReadDataBlock(cl.Read(pg, 3), reply);
                    cl.UpdateValue(pg, 3, reply.Value);
                    reply.Clear();
                    Console.WriteLine("Reading Attribute 2");
                    ReadDataBlock(cl.Read(pg, 2), reply);
                    cl.UpdateValue(pg, 2, reply.Value);
                    reply.Clear();
                }

                int printed = 0;
                foreach (var row in pg.Buffer)
                {
                    foreach (object obj in row)
                    {
                        if (obj is System.Object[])
                        {
                            printed = 1;
                        }
                    }
                }
                StringBuilder sb = new StringBuilder();
                int i = 0;
                foreach (var obj in pg.CaptureObjects)
                {
                    //sb.Append(obj.ToString());
                    List<object> row = new List<object>(pg.Buffer[0]);

                    sb.Append(obj.Key.LogicalName.ToString());
                    sb.Append("\t");
                    sb.Append("\t");
                    sb.Append(obj.Key.ObjectType.ToString());

                    sb.Append("\t");
                    sb.Append("\t");
                    //
                    if (printed != 0)
                    {
                        foreach (object[] it in row)
                        {
                            foreach (object txt in it)
                            {
                                sb.Append(txt.ToString());
                                sb.Append(";");
                            }
                            Console.WriteLine(sb.ToString());
                            sb.Length = 0;
                        }
                    }
                    else
                    {
                        if (row[i] is System.Byte[])
                        {
                            byte[] by = (byte[])(row[i]);
                            foreach (byte it in by)
                            {
                                sb.Append(it.ToString("X2"));
                                sb.Append(" ");
                            }
                        }
                        else
                            sb.Append(row[i].ToString());
                        Console.WriteLine(sb.ToString());
                    }
                    i++;
                    sb.Length = 0;
                }
                Console.WriteLine(sb.ToString());

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }

        public void SerialReadAllEntry(GXDLMSProfileGeneric pg)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading Attribute 3");
                    ReadDataBlock(cl.Read(pg, 3), reply);
                    cl.UpdateValue(pg, 3, reply.Value);
                    reply.Clear();
                    Console.WriteLine("Reading Attribute 2");
                    ReadDataBlock(cl.Read(pg, 2), reply);
                    cl.UpdateValue(pg, 2, reply.Value);
                    reply.Clear();
                }

                int printed = 0;
                foreach (var row in pg.Buffer)
                {
                    foreach (object obj in row)
                    {
                        if (obj is System.Object[])
                        {
                            printed = 1;
                        }
                    }
                }


                StringBuilder sb = new StringBuilder();
                for (ushort i = 0; i < pg.Buffer.Count; i++)
                {
                    int j = 0;
                    foreach (var obj in pg.CaptureObjects)
                    {
                        List<object> row = new List<object>(pg.Buffer[i]);

                        sb.Append(obj.Key.LogicalName.ToString());
                        sb.Append("\t");
                        sb.Append("\t");
                        sb.Append(obj.Key.ObjectType.ToString());

                        sb.Append("\t");
                        sb.Append("\t");
                        //
                        if (printed != 0)
                        {
                            foreach (object[] it in row)
                            {
                                foreach (object txt in it)
                                {
                                    sb.Append(txt.ToString());
                                    sb.Append(";");
                                }
                                Console.WriteLine(sb.ToString());
                                sb.Length = 0;
                            }
                        }
                        else
                        {
                            if (row[j] is System.Byte[])
                            {
                                byte[] by = (byte[])(row[i]);
                                foreach (byte it in by)
                                {
                                    sb.Append(it.ToString("X2"));
                                    sb.Append(" ");
                                }
                            }
                            else
                            {
                                sb.Append(row[j].ToString());
                            }
                            j++;

                            Console.WriteLine(sb.ToString());
                            sb.Length = 0;
                        }

                    }
                    sb.Append("\n");
                }
                Console.WriteLine(sb.ToString());
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }

        public void SerialUpdateLLC_Secret()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSAssociationLogicalName association_obj = new GXDLMSAssociationLogicalName("0.0.40.0.2.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Update_Password LLC");
                    association_obj.Secret = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                    ReadDataBlock(cl.Write(association_obj, 7), reply);
                    //
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }
        public void serial_TOD_update_meter()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Disconnecting_meter");
                    ReadDataBlock(cl.Method(discon_obj, 1, 0, DataType.Int8), reply);
                    //
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }



        public void ReadProfilebyRange(byte channel, GXDateTime s, GXDateTime e, GXDLMSProfileGeneric pg)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {

                Initialize();

                Console.WriteLine("6. Reading meter list.");
                GXDLMSData d = new GXDLMSData("0.128.0.0.0.255");
                ReadDataBlock(cl.Read(d, 2), reply);

                Console.WriteLine("Parsing meter list.");
                List<KeyValuePair<string, UInt16>> meters = GetMeters((object[])reply.Value);
                foreach (var it in meters)
                {
                    Console.WriteLine(it.Value + " " + it.Key);
                }
                Console.WriteLine("7. Selecting first meter from the list.");
                reply.Clear();

                //Select channel 1.
                d = new GXDLMSData("0.128.1.0.0.255");
                // d.Value = meters[1].Value;
                d.Value = channel;
                Console.WriteLine("Selecting channel: " + d.Value.ToString());
                reply.Clear();
                ReadDataBlock(cl.Write(d, 2), reply);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Channel selected: " + d.Value);

                Console.WriteLine("2. Reading Device Logical Name.");
                d = new GXDLMSData("0.0.42.0.0.255");
                d.SetUIDataType(2, DataType.String);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Value: " + d.Value.ToString());
                reply.Clear();
                ReadDataBlock(cl.Read(pg, 3), reply);
                cl.UpdateValue(pg, 3, reply.Value);
                reply.Clear();

                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading Attribute 2");
                    ReadDataBlock(cl.ReadRowsByRange(pg, s, e), reply);
                    cl.UpdateValue(pg, 2, reply.Value);
                    reply.Clear();
                }


                StringBuilder sb = new StringBuilder();
                foreach (var row in pg.Buffer)
                {
                    foreach (var it in row)
                    {
                        sb.Append(it.ToString());
                        sb.Append(";");
                    }
                    Console.WriteLine(sb.ToString());
                    sb.Length = 0;
                }
                Console.WriteLine(sb.ToString());

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }
        public void ReadloadbyEntry(byte channel, int Start_entry, int Count, GXDLMSProfileGeneric pg)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();

                int MeterFound = 0;
                Console.WriteLine("6. Reading meter list.");
                GXDLMSData d = new GXDLMSData("0.128.0.0.0.255");
                ReadDataBlock(cl.Read(d, 2), reply);
                Console.WriteLine("Parsing meter list.");
                List<KeyValuePair<string, UInt16>> meters = GetMeters((object[])reply.Value);
                foreach (var it in meters)
                {
                    Console.WriteLine("Channel no. = " + it.Value + " " + "Meter Logical name = " + it.Key);
                    if (channel == it.Value)
                    {
                        Console.WriteLine("Selecting channel no. = " + it.Value + " " + "Meter Logical name = " + it.Key);
                        MeterFound = channel;
                    }
                    reply.Clear();
                }
                if (MeterFound != 0)
                {
                    //Select channel 1.
                    d = new GXDLMSData("0.128.1.0.0.255");
                    // d.Value = meters[1].Value;


                    d.Value = channel;
                    Console.WriteLine("Selecting channel: " + d.Value.ToString());
                    reply.Clear();
                    ReadDataBlock(cl.Write(d, 2), reply);
                    reply.Clear();
                    ReadDataBlock(cl.Read(d, 2), reply);
                    cl.UpdateValue(d, 2, reply.Value);
                    Console.WriteLine("Channel selected: " + d.Value);

                    Console.WriteLine("2. Reading Device Logical Name.");
                    d = new GXDLMSData("0.0.42.0.0.255");
                    d.SetUIDataType(2, DataType.String);
                    reply.Clear();
                    ReadDataBlock(cl.Read(d, 2), reply);
                    cl.UpdateValue(d, 2, reply.Value);
                    Console.WriteLine("Value: " + d.Value.ToString());
                    reply.Clear();
                    ReadDataBlock(cl.Read(pg, 3), reply);
                    cl.UpdateValue(pg, 3, reply.Value);
                    reply.Clear();
                    ReadDataBlock(cl.Read(pg, 7), reply);
                    cl.UpdateValue(pg, 7, reply.Value);
                    Console.WriteLine("Value: " + reply.Value.ToString());

                    reply.Clear();
                    for (int pos = 0; pos != 1; ++pos)
                    {
                        Console.WriteLine("Reading Attribute 2");
                        ReadDataBlock(cl.ReadRowsByEntry(pg, Start_entry, Count), reply);
                        cl.UpdateValue(pg, 2, reply.Value);
                        reply.Clear();
                    }


                    StringBuilder sb = new StringBuilder();
                    foreach (var row in pg.Buffer)
                    {
                        foreach (var val in row)
                        {
                            sb.Append(val.ToString());
                            sb.Append(";");
                        }
                        Console.WriteLine(sb.ToString());
                        sb.Length = 0;
                    }
                    Console.WriteLine(sb.ToString());
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Meter with channel no. = " + channel + " Not Found! Please check this meter list and select a particular channel");
                    Console.WriteLine("");
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }

        //
        public void ReadloadbyEntryDCU(byte channel, int Start_entry, int Count, GXDLMSProfileGeneric pg)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                {
                    ReadDataBlock(cl.Read(pg, 3), reply);
                    cl.UpdateValue(pg, 3, reply.Value);
                    reply.Clear();
                    ReadDataBlock(cl.Read(pg, 6), reply);
                    cl.UpdateValue(pg, 6, reply.Value);
                    reply.Clear();
                    ReadDataBlock(cl.Read(pg, 7), reply);
                    cl.UpdateValue(pg, 7, reply.Value);
                    Console.WriteLine("Value: " + reply.Value.ToString());

                    reply.Clear();
                    for (int pos = 0; pos != 1; ++pos)
                    {
                        Console.WriteLine("Reading Attribute 2");
                        ReadDataBlock(cl.ReadRowsByEntry(pg, Start_entry, Count), reply);
                        cl.UpdateValue(pg, 2, reply.Value);
                        reply.Clear();
                    }


                    StringBuilder sb = new StringBuilder();
                    foreach (var row in pg.Buffer)
                    {
                        foreach (var val in row)
                        {
                            sb.Append(val.ToString());
                            sb.Append(";");
                        }
                        Console.WriteLine(sb.ToString());
                        sb.Length = 0;
                    }
                    Console.WriteLine(sb.ToString());
                }

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            // Thread.Sleep(1000);
            media.Close();
        }

        public void ReadprofileDCU(GXDLMSProfileGeneric pg)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                {
                    ReadDataBlock(cl.Read(pg, 3), reply);
                    cl.UpdateValue(pg, 3, reply.Value);
                    reply.Clear();

                    ReadDataBlock(cl.Read(pg, 7), reply);
                    cl.UpdateValue(pg, 7, reply.Value);
                    Console.WriteLine("Value: " + reply.Value.ToString());

                    reply.Clear();

                    StringBuilder sb = new StringBuilder();
                    foreach (var row in pg.Buffer)
                    {
                        foreach (var val in row)
                        {
                            sb.Append(val.ToString());
                            sb.Append(";");
                        }
                        Console.WriteLine(sb.ToString());
                        sb.Length = 0;
                    }
                    Console.WriteLine(sb.ToString());
                }

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            media.Close();
        }
        //
        public void ReadProfileOfAllMeter(GXDLMSProfileGeneric pg)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();

                Console.WriteLine("6. Reading meter list.");
                GXDLMSData d = new GXDLMSData("0.128.0.0.0.255");
                ReadDataBlock(cl.Read(d, 2), reply);

                Console.WriteLine("Parsing meter list.");
                List<KeyValuePair<string, UInt16>> meters = GetMeters((object[])reply.Value);
                for (int i = 0; i < 10; i++)
                {
                    foreach (var it in meters)
                    {
                        Console.WriteLine("Channel no. = " + it.Value + " " + "Meter Logical name = " + it.Key);

                        Console.WriteLine("Selecting meter no. = " + it.Key + " from the list.");
                        reply.Clear();

                        //Select channel 1.
                        d = new GXDLMSData("0.128.1.0.0.255");
                        // d.Value = meters[1].Value;
                        d.Value = it.Value;
                        Console.WriteLine("Selecting channel: " + d.Value.ToString());
                        reply.Clear();
                        ReadDataBlock(cl.Write(d, 2), reply);
                        reply.Clear();
                        ReadDataBlock(cl.Read(d, 2), reply);
                        cl.UpdateValue(d, 2, reply.Value);
                        Console.WriteLine("Channel selected: " + d.Value);

                        Console.WriteLine("2. Reading Device Logical Name.");
                        d = new GXDLMSData("0.0.42.0.0.255");
                        d.SetUIDataType(2, DataType.String);
                        reply.Clear();
                        ReadDataBlock(cl.Read(d, 2), reply);
                        cl.UpdateValue(d, 2, reply.Value);
                        Console.WriteLine("Value: " + d.Value.ToString());
                        reply.Clear();

                        reply.Clear();
                        for (int pos = 0; pos != 1; ++pos)
                        {
                            Console.WriteLine("Reading Attribute 2");
                            ReadDataBlock(cl.Read(pg, 2), reply);
                            reply.Clear();
                        }


                        StringBuilder sb = new StringBuilder();
                        foreach (var row in pg.Buffer)
                        {
                            foreach (var val in row)
                            {
                                sb.Append(val.ToString());
                                sb.Append(";");
                            }
                            Console.WriteLine(sb.ToString());
                            sb.Length = 0;
                        }
                        Console.WriteLine(sb.ToString());
                    }
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }
        public void Test31()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                int Num_of_entry = 0;
                string str1;
                Initialize();
                GXDLMSClock Clk = new GXDLMSClock("0.0.1.0.0.255");
                Console.WriteLine("6. Reading meter list.");
                GXDLMSData d = new GXDLMSData("0.128.0.0.0.255");
                ReadDataBlock(cl.Read(d, 2), reply);

                Console.WriteLine("Parsing meter list.");
                List<KeyValuePair<string, UInt16>> meters = GetMeters((object[])reply.Value);
                foreach (var it in meters)
                {
                    Console.WriteLine(it.Value + " " + it.Key);
                }
                Console.WriteLine("7. Selecting first meter from the list.");
                reply.Clear();

                d = new GXDLMSData("0.128.1.0.0.255");
                d.Value = 4;
                Console.WriteLine("Selecting channel: " + d.Value.ToString());
                reply.Clear();
                ReadDataBlock(cl.Write(d, 2), reply);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Channel selected: " + d.Value);

                Console.WriteLine("2. Reading Device Logical Name.");
                d = new GXDLMSData("0.0.42.0.0.255");
                d.SetUIDataType(2, DataType.String);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                Console.WriteLine("Value: " + d.Value.ToString());

                GXDLMSProfileGeneric pg = new GXDLMSProfileGeneric("1.0.94.91.4.255");

                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading Attribute 3");
                    ReadDataBlock(cl.Read(pg, 3), reply);
                    cl.UpdateValue(pg, 3, reply.Value);
                    reply.Clear();
                }
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading Attribute 2");
                    ReadDataBlock(cl.Read(pg, 2), reply);
                    cl.UpdateValue(pg, 2, reply.Value);
                    reply.Clear();
                }

                StringBuilder sb = new StringBuilder();
                foreach (var row in pg.Buffer)
                {
                    foreach (var it in row)
                    {
                        sb.Append(it.ToString());
                        sb.Append(";");
                    }
                    Console.WriteLine(sb.ToString());
                    sb.Length = 0;
                }
                Console.WriteLine(sb.ToString());
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }

        public void TestFW(byte channel)
        {

            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();

                //////////////////////////////
                Console.WriteLine("Updating image.");
                GXDLMSImageTransfer i = new GXDLMSImageTransfer("0.0.44.0.1.255");
                //Update path.
                byte[] data = File.ReadAllBytes("E:/image/CCM603P_img_6_2.bin");
                byte[] identification = { 0x53, 0x69, 0x6e, 0x68, 0x61, 0x6c, 0x20, 0x55, 0x64, 0x79, 0x6f, 0x67, 0x00, 0x20, 0x43, 0x52, 0x59, 0x5f, 0x53, 0x49, 0x5f, 0x36, 0x2e, 0x00, 0x00 };
                ReadDataBlock(cl.Read(i, 7), reply);
                UpdateImage(i, data, "Sinhal Udyog\0 CRY_SI_6.00");
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }

        /// <connectivity test.>
        /// connectivity
        /// </ connectivity test.>
        public void connectivity()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(100);
            media.Close();
        }
        ////////
        //

        /// <summary>
        /// //////////////
        /// cover open check
        /// //////////////
        /// </summary>
        public void cover()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();

                GXDLMSDisconnectControl DC = new GXDLMSDisconnectControl("0.0.99.3.10.255");

                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Read(DC, 1), reply);
                    //cl.UpdateValue(pg, 3, reply.Value);
                    Console.WriteLine("A" + pos);
                    reply.Clear();
                }
                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Method(DC, 2, 1), reply);
                    Console.WriteLine("B" + pos);
                    reply.Clear();
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            Thread.Sleep(1000);
            media.Close();
        }
        //
        ////////////////
        public void Test3()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                List<GXDLMSObject> objects = new List<GXDLMSObject>();
                GXDLMSAssociationLogicalName ln = new GXDLMSAssociationLogicalName("0.0.40.0.0.255");
                objects.Add(ln);
                for (int pos = 0; pos != 3; ++pos)
                {
                    Console.WriteLine("Association view " + pos);
                    ReadDataBlock(cl.Read(ln, 3), reply);
                }
                foreach (var it in cl.Objects.GetObjects(ObjectType.ProfileGeneric))
                {
                    for (int pos = 0; pos != 3; ++pos)
                    {
                        Console.WriteLine("PG " + it.LogicalName + pos);
                        ReadDataBlock(cl.Read(it, 2), reply);
                    }
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        //
        public void Test()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                List<GXDLMSObject> objects = new List<GXDLMSObject>();
                GXDLMSAssociationLogicalName ln = new GXDLMSAssociationLogicalName("0.0.40.0.0.255");

                Console.WriteLine("6. Reading meter list.");
                GXDLMSData d = new GXDLMSData("0.128.0.0.0.255");
                objects.Add(d);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                Thread.Sleep(200);
                Console.WriteLine("Parsing meter list.");
                List<KeyValuePair<string, UInt16>> meters = GetMeters((object[])reply.Value);
                foreach (var it in meters)
                {
                    Console.WriteLine(it.Value + " " + it.Key);
                }
                Console.WriteLine("7. Selecting first meter from the list.");
                reply.Clear();
                d = new GXDLMSData("0.128.1.0.0.255");
                objects.Add(d);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                d.Value = meters[0].Key;
                Console.WriteLine("Selected channel: " + d.Value.ToString());
                reply.Clear();
                ReadDataBlock(cl.Write(d, 2), reply);
                //Read meter Logical name again. It must be same as selected meter.
                d = new GXDLMSData("0.128.1.0.0.255");
                objects.Add(d);
                reply.Clear();
                ReadDataBlock(cl.Read(d, 2), reply);
                cl.UpdateValue(d, 2, reply.Value);
                if (string.Compare(meters[0].Key, d.Value.ToString()) != 0)
                {
                    Console.WriteLine("Expected: " + meters[0].Key + " Actual: " + d.Value.ToString());
                }
                Console.WriteLine("8. Reading load profile 1.");
                GXDLMSProfileGeneric pg = new GXDLMSProfileGeneric("1.0.94.91.10.255");
                objects.Add(pg);
                //Read columns.
                reply.Clear();
                ReadDataBlock(cl.Read(pg, 3), reply);
                cl.UpdateValue(pg, 3, reply.Value);
                //Read buffer (data).
                reply.Clear();
                ReadDataBlock(cl.Read(pg, 2), reply);
                cl.UpdateValue(pg, 2, reply.Value);
                Thread.Sleep(200);

                foreach (var it in objects)
                {
                    bool found = false;
                    foreach (var c in ln.ObjectList)
                    {
                        if (it.GetType() == c.GetType() && it.LogicalName == c.LogicalName)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        throw new Exception("Object is not found from association view.");
                    }
                }
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public void TestConnectDisconnect()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSDisconnectControl push = new GXDLMSDisconnectControl(" 0.0.96.3.10.255");
                Console.WriteLine("connect disconnect.");
                reply.Clear();
                //ReadDataBlock(push.Activate(cl), reply);
                ReadDataBlock(cl.Method(push, 1, (byte)1), reply);
                Thread.Sleep(1000);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        //rakesh en
        public void TestPushProfileGeneric1()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSPushSetup push = new GXDLMSPushSetup("0.1.25.9.0.255");
                Console.WriteLine("Activate Push.");
                reply.Clear();
                ReadDataBlock(cl.Method(push, 1, (byte)1), reply);
                Thread.Sleep(1000);

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public void TestPushProfileGeneric2()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSPushSetup push = new GXDLMSPushSetup("0.2.25.9.0.255");
                Console.WriteLine("Activate Push.");
                reply.Clear();
                //ReadDataBlock(push.Activate(cl), reply);
                ReadDataBlock(cl.Method(push, 1, (byte)1), reply);
                Thread.Sleep(1000);

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public void TestPushEventLog()
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                GXDLMSPushSetup push = new GXDLMSPushSetup("0.3.25.9.0.255");
                Console.WriteLine("Activate Push.");
                reply.Clear();
                //ReadDataBlock(push.Activate(cl), reply);
                ReadDataBlock(cl.Method(push, 1, (byte)1), reply);
                Thread.Sleep(1000);

            }
            finally
            {
                reply.Clear();
                ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }
        void UpdateImage(GXDLMSImageTransfer target, byte[] data, string Identification)
        {
            //Check that image transfer ia enabled.
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(cl.Read(target, 5), reply);
            cl.UpdateValue(target, 5, reply.Value);

            reply.Clear();
            ReadDataBlock(cl.Read(target, 2), reply);
            cl.UpdateValue(target, 2, reply.Value);
            // Step 2: The client initiates the Image transfer process.
            reply.Clear();
            ReadDataBlock(target.ImageTransferInitiate(cl, Identification, data.Length), reply);
            // Step 3: The client transfers ImageBlocks.
            int imageBlockCount;
            reply.Clear();
            Thread.Sleep(5000);
            ReadDataBlock(target.ImageBlockTransfer(cl, data, out imageBlockCount), reply);
            Thread.Sleep(5000);
            //Step 4: The client checks the completeness of the Image in 
            //each server individually and transfers any ImageBlocks not (yet) transferred;
            reply.Clear();
            ReadDataBlock(cl.Read(target, 3), reply);
            cl.UpdateValue(target, 3, reply.Value);

            // Step 5: The Image is verified;
            reply.Clear();
            ReadDataBlock(target.ImageVerify(cl), reply);
            // Step 6: Before activation, the Image is checked;

            //Get list to images to activate.
            reply.Clear();
            ReadDataBlock(cl.Read(target, 7), reply);
            cl.UpdateValue(target, 7, reply.Value);
            bool bFound = false;
            foreach (GXDLMSImageActivateInfo it in target.ImageActivateInfo)
            {
                //if (it.Identification == Identification)
                //{
                //    bFound = true;
                //    break;
                //}
            }
            if (!bFound)
            {
                throw new Exception("Image not found.");
            }
            //Read image transfer status.
            reply.Clear();
            ReadDataBlock(cl.Read(target, 6), reply);
            cl.UpdateValue(target, 6, reply.Value);
            if (target.ImageTransferStatus != ImageTransferStatus.VerificationSuccessful)
            {
                throw new Exception("Image transfer status is " + target.ImageTransferStatus.ToString());
            }

            //Step 7: Activate image.
            reply.Clear();
            ReadDataBlock(target.ImageActivate(cl), reply);
        }

        public void Initialize()
        {
            GXReplyData reply = new GXReplyData();
            byte[] data;

            //////public_Cl
            data = public_client.SNRMRequest();
            if (data != null)
            {
                if (Trace)
                {
                    Console.WriteLine("Send SNRM request." + GXCommon.ToHex(data, true));
                }
                ReadDLMSPacket(data, reply);
                if (Trace)
                {
                    Console.WriteLine("Parsing UA reply." + reply.ToString());
                }
                //Has server accepted cl.
                public_client.ParseUAResponse(reply.Data);
                Console.WriteLine("Parsing UA reply succeeded.");
            }
            //Generate AARQ request.
            //Split requests to multiple packets if needed. 
            //If password is used all data might not fit to one packet.
            foreach (byte[] it in public_client.AARQRequest())
            {
                if (Trace)
                {
                    Console.WriteLine("Send AARQ request", GXCommon.ToHex(it, true));
                }
                reply.Clear();
                ReadDLMSPacket(it, reply);
            }
            if (Trace)
            {
                Console.WriteLine("Parsing AARE reply" + reply.ToString());
            }
            //Parse reply.
            public_client.ParseAAREResponse(reply.Data);

            try
            {

                GXDLMSData data_obj = new GXDLMSData("0.0.43.1.3.255");
                ReadDataBlock(public_client.Read(data_obj, 2), reply);
                if (reply.Value is UInt32)
                {
                    cl.Ciphering.InvocationCounter = (UInt32)reply.Value;
                }
                reply.Clear();
            }
            catch
            {
                Console.WriteLine("Fails to read invocation counter.");
            }
            finally
            {

                ReadDLMSPacket(public_client.DisconnectRequest(), reply);
                reply.Clear();
            }
            reply.Clear();
            //////public_Cl end
            isSecureClient = true;
            data = cl.SNRMRequest();
            if (data != null)
            {
                if (Trace)
                {
                    Console.WriteLine("Send SNRM request." + GXCommon.ToHex(data, true));
                }
                ReadDLMSPacket(data, reply);
                if (Trace)
                {
                    Console.WriteLine("Parsing UA reply." + reply.ToString());
                }
                //Has server accepted cl.
                cl.ParseUAResponse(reply.Data);
                Console.WriteLine("Parsing UA reply succeeded.");
            }
            //Generate AARQ request.
            //Split requests to multiple packets if needed. 
            //If password is used all data might not fit to one packet.
            foreach (byte[] it in cl.AARQRequest())
            {
                if (Trace)
                {
                    Console.WriteLine("Send AARQ request", GXCommon.ToHex(it, true));
                }
                reply.Clear();
                ReadDLMSPacket(it, reply);
            }
            if (Trace)
            {
                Console.WriteLine("Parsing AARE reply" + reply.ToString());
            }
            //Parse reply.
            cl.ParseAAREResponse(reply.Data);
            reply.Clear();
            //Get challenge Is HSL authentication is used.
            if (cl.IsAuthenticationRequired)
            {
                foreach (byte[] it in cl.GetApplicationAssociationRequest())
                {
                    reply.Clear();
                    ReadDLMSPacket(it, reply);
                }
                cl.ParseApplicationAssociationResponse(reply.Data);
            }
        }

        public bool ReadDataBlock(byte[][] data, GXReplyData reply)
        {
            foreach (byte[] it in data)
            {
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            return true;
        }

        /// <summary>
        /// Read data block from the device.
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="text">Progress text.</param>
        /// <param name="multiplier"></param>
        /// <returns>Received data.</returns>
        public void ReadDataBlock(byte[] data, GXReplyData reply)
        {
            ReadDLMSPacket(data, reply);
            while (reply.IsMoreData)
            {
                data = cl.ReceiverReady(reply.MoreData);
                ReadDLMSPacket(data, reply);
            }
        }

        /// <summary>
        /// Read DLMS Data from the device.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <returns>Received data.</returns>
        public void ReadDLMSPacket(byte[] data, GXReplyData reply)
        {
            if (data == null)
            {
                return;
            }
            reply.Error = 0;
            object eop = (byte)0x7E;
            //In network connection terminator is not used.
            if (cl.InterfaceType == InterfaceType.WRAPPER)
            {
                eop = null;
            }
            int pos = 0;
            bool succeeded = false;
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                Eop = eop,
                Count = 5,
#if DCU_TEST
                WaitTime = 105000,
#else
                WaitTime = 5000,
#endif
            };
            lock (media.Synchronous)
            {
                while (!succeeded && pos != 3)
                {
                    if (Trace)
                    {
                        WriteTrace("<- " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(data, true));
                    }
                    media.Send(data, null);
                    succeeded = media.Receive(p);
                    if (!succeeded)
                    {
                        //If Eop is not set read one byte at time.
                        Console.WriteLine("Data get fails");
                        if (p.Eop == null)
                        {
                            p.Count = 1;
                        }
                        //Try to read again...
                        if (++pos != 3)
                        {
                            System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                            continue;
                        }
                        throw new Exception("Failed to receive reply from the device in given time.");
                        //MessageBox.Show("Error - No Ports available");
                    }
                }
                try
                {
                    //Loop until whole COSEM packet is received.        
                    if (isSecureClient)
                    {
                        while (!cl.GetData(p.Reply, reply))
                        {
                            //If Eop is not set read one byte at time.
                            if (p.Eop == null)
                            {
                                p.Count = 1;
                            }
                            while (!media.Receive(p))
                            {
                                //If echo.
                                if (p.Reply.Length == data.Length)
                                {
                                    media.Send(data, null);
                                }
                                //Try to read again...
                                if (++pos != 3)
                                {
                                    System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                                    continue;
                                }
                                throw new Exception("Failed to receive reply from the device in given time.");
                            }
                        }
                    }
                    else
                    {
                        while (!public_client.GetData(p.Reply, reply))
                        {
                            //If Eop is not set read one byte at time.
                            if (p.Eop == null)
                            {
                                p.Count = 1;
                            }
                            while (!media.Receive(p))
                            {
                                //If echo.
                                if (p.Reply.Length == data.Length)
                                {
                                    media.Send(data, null);
                                }
                                //Try to read again...
                                if (++pos != 3)
                                {
                                    System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                                    continue;
                                }
                                throw new Exception("Failed to receive reply from the device in given time.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteTrace("-> " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(p.Reply, true));
                    throw ex;
                }
            }
            if (Trace)
            {
                WriteTrace("-> " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(p.Reply, true));
            }
            if (reply.Error != 0)
            {
                if (reply.Error == (short)ErrorCode.Rejected)
                {
                    Thread.Sleep(1000);
                    ReadDLMSPacket(data, reply);
                }
                else
                {
                    throw new GXDLMSException(Enum.GetName(typeof(ErrorCode), reply.Error));
                }
            }
        }

        void WriteTrace(string message)
        {
            Console.WriteLine(message);
        }

        public GXReplyData ReadMeterData(byte[][] requestPayload)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {
                Initialize();
                ReadDataBlock(requestPayload, reply);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                media.Close();
                reply.Clear();
            }
            return reply;
        }

        public GXDLMSProfileGeneric GetProfileData(string ip)
        {
            GXReplyData reply = new GXReplyData();
            //media.Open();
            try
            {
                GXDLMSProfileGeneric pg = new GXDLMSProfileGeneric(ip);

                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading Attribute 3");
                    ReadDataBlock(cl.Read(pg, 3), reply);
                    cl.UpdateValue(pg, 3, reply.Value);
                    reply.Clear();
                }
                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading Attribute 2");
                    ReadDataBlock(cl.Read(pg, 2), reply);
                    cl.UpdateValue(pg, 2, reply.Value);
                    reply.Clear();
                }
                return pg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reply.Clear();
            }
        }

        //For testing with Start Date and End Date
        public GXDLMSProfileGeneric GetProfileData(string ip, DateTime? startDate, DateTime? endDate)
        {
            GXReplyData reply = new GXReplyData();
            //media.Open();
            try
            {
                GXDLMSProfileGeneric pg = new GXDLMSProfileGeneric(ip);

                for (int pos = 0; pos != 1; ++pos)
                {
                    Console.WriteLine("Reading Attribute 3");
                    ReadDataBlock(cl.Read(pg, 3), reply);
                    cl.UpdateValue(pg, 3, reply.Value);
                    reply.Clear();
                }

                if (startDate != null && endDate != null)
                {
                    for (int pos = 0; pos != 1; ++pos)
                    {
                        Console.WriteLine("Reading Attribute 2");
                        ReadDataBlock(cl.ReadRowsByRange(pg, startDate, endDate), reply);
                        cl.UpdateValue(pg, 2, reply.Value);
                        reply.Clear();
                    }
                }
                else
                {
                    for (int pos = 0; pos != 1; ++pos)
                    {
                        Console.WriteLine("Reading Attribute 2");
                        ReadDataBlock(cl.Read(pg, 2), reply);
                        cl.UpdateValue(pg, 2, reply.Value);
                        reply.Clear();
                    }
                }

                return pg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reply.Clear();
            }
        }

        public void OpenConnection()
        {
            media.Open();
            Initialize();
        }

        public void CloseConnection()
        {
            media.Close();
        }

        public string ReadData(string IP)
        {
            try
            {
                media.Open();
                Initialize();

                try
                {
                    string ESFW = null;
                    GXReplyData reply = new GXReplyData();
                    GXDLMSData data_Obj = new GXDLMSData();
                    try
                    {
                        ///////////////////////////////////////////////////////////////////////
                        //Add action schedule object.
                        data_Obj.LogicalName = IP;

                        ReadDataBlock(cl.Read(data_Obj, 2), reply);
                        cl.UpdateValue(data_Obj, 2, reply.Value);
                        if (data_Obj.Value is string)
                            ESFW = (string)data_Obj.Value;
                    }
                    finally
                    {
                        reply.Clear();
                    }
                    return ESFW;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int ReadGDLMSClock(string IP)
        {
            GXReplyData reply = new GXReplyData();
            media.Open();
            try
            {

                GXDLMSClock Clock = new GXDLMSClock(IP);

                Console.WriteLine("Reading Attribute 2: status");
                ReadDataBlock(cl.Read(Clock, 4), reply);
                cl.UpdateValue(Clock, 4, reply.Value);

                int currentStatus = Convert.ToInt32(reply.Value.ToString());

                reply.Clear();
                return currentStatus;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                reply.Clear();
                //ReadDataBlock(cl.DisconnectRequest(), reply);
            }
            //return reply;
        }

        // For Firmware Version
        public GXDLMSData GetProfileFirmWareVersionTest(string ip)
        {
            GXReplyData reply = new GXReplyData();
            //media.Open();
            try
            {
                GXDLMSData data_Obj = new GXDLMSData(ip);
                try
                {
                    ReadDataBlock(cl.Read(data_Obj, 2), reply);
                    cl.UpdateValue(data_Obj, 2, reply.Value);
                    return data_Obj;
                }
                finally
                {
                    reply.Clear();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                reply.Clear();
            }
        }

        public bool SetBillingDate(DateTime txtMyDate)
        {
            GXReplyData reply = new GXReplyData();

            try
            {
                ///////////////////////////////////////////////////////////////////////
                //Add action schedule object.
                GXDLMSActionSchedule Action = new GXDLMSActionSchedule();
                Action.LogicalName = "0.0.15.0.0.255";
                Action.ExecutionTime = new GXDateTime[] {
                    new GXDateTime(-1, -1, txtMyDate.Day, txtMyDate.Hour, txtMyDate.Minute, txtMyDate.Second, -1) };
                ReadDataBlock(cl.Write(Action, 4), reply);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                reply.Clear();
                //ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public async Task<int> SendKeepAlivePacket()
        {
            try
            {
                //GXReplyData reply = new GXReplyData();
                //ReadDLMSPacket(public_client.GetKeepAlive(), reply);
                //return 1;

                GXReplyData reply = new GXReplyData();
                try
                {
                    //Add action schedule object.
                    GXDLMSClock Clock = new GXDLMSClock();
                    Clock.LogicalName = "0.0.1.0.0.255";//0, 0,  15, 0, 0, 255

                    ReadDataBlock(cl.Read(Clock, 2), reply);
                    cl.UpdateValue(Clock, 2, reply.Value);
                }
                catch (Exception ex)
                {
                    return 1;
                }
                finally
                {
                    reply.Clear();
                    //ReadDataBlock(cl.DisconnectRequest(), reply);
                }
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendConnectCommand()
        {
            GXReplyData reply = new GXReplyData();

            try
            {
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Method(discon_obj, 2, 0, DataType.Int8), reply);
                    reply.Clear();
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reply.Clear();
                //ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public bool SendDisconnectCommand()
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSDisconnectControl discon_obj = new GXDLMSDisconnectControl("0.0.96.3.10.255");
                for (int pos = 0; pos != 1; ++pos)
                {
                    ReadDataBlock(cl.Method(discon_obj, 1, 0, DataType.Int8), reply);
                    reply.Clear();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reply.Clear();
                //ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public bool SendMDResetCommand()
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                GXDLMSScriptTable scriptTable = new GXDLMSScriptTable();
                scriptTable.LogicalName = "0.0.10.0.1.255";
                ReadDataBlock(cl.Method(scriptTable, 1, 1, DataType.UInt16), reply);

                return reply.IsComplete;
            }
            finally
            {
                reply.Clear();
                //ReadDataBlock(cl.DisconnectRequest(), reply);
            }
        }

        public void SetTOD(DateTime[] Time, ushort[] IDs)
        {
            string logicalname = "0.0.10.0.100.255";

            try
            {

                GXReplyData reply = new GXReplyData();

                GXDLMSActivityCalendar activityC = new GXDLMSActivityCalendar();
                activityC.LogicalName = "0.0.13.0.0.255";
                cl.NegotiatedConformance = Conformance.Action | Conformance.SelectiveAccess | Conformance.Set | Conformance.Get | Conformance.DataNotification | Conformance.BlockTransferWithSetOrWrite | Conformance.BlockTransferWithGetOrRead;
                cl.Settings.MaxServerPDUSize = 0x0060;
                cl.Settings.MaxPduSize = 0x0060;
                activityC.DayProfileTablePassive = new GXDLMSDayProfile[] { new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[]
                {
                    new GXDLMSDayProfileAction(new GXTime( Time[0].Hour, Time[0].Minute, Time[0].Second, -1), logicalname, IDs[0]),
                    new GXDLMSDayProfileAction(new GXTime( Time[1].Hour, Time[1].Minute, Time[1].Second, -1), logicalname, IDs[1]),
                    new GXDLMSDayProfileAction(new GXTime( Time[2].Hour, Time[2].Minute, Time[2].Second, -1), logicalname, IDs[2]),
                    new GXDLMSDayProfileAction(new GXTime( Time[3].Hour, Time[3].Minute, Time[3].Second, -1), logicalname, IDs[3]),
                    new GXDLMSDayProfileAction(new GXTime( Time[4].Hour, Time[4].Minute, Time[4].Second, -1), logicalname, IDs[4]),
                    new GXDLMSDayProfileAction(new GXTime( Time[5].Hour, Time[5].Minute, Time[5].Second, -1), logicalname, IDs[5]),
                    new GXDLMSDayProfileAction(new GXTime( Time[6].Hour, Time[6].Minute, Time[6].Second, -1), logicalname, IDs[6]),
                    new GXDLMSDayProfileAction(new GXTime( Time[7].Hour, Time[7].Minute, Time[7].Second, -1), logicalname, IDs[7]),

                })};
                ReadDataBlock(cl.Write(activityC, 9), reply);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
