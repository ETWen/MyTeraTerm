using System;
using System.Net;
using SnmpSharpNet;

namespace PDUControlLib
{
    /// <summary>
    /// PDU Controller for iPoMan II/III models using SNMP
    /// </summary>
    public class PduController : IDisposable
    {
        private string model;
        private string ipAddress;
        private const string Community = "private";
        private const int SnmpPort = 161;
        private const int SnmpTimeout = 3000; // milliseconds

        public PduController(string ip = "192.168.1.21", string pduModel = "iPoMan II")
        {
            this.ipAddress = ip;
            this.model = pduModel;
        }

        #region SNMP Operations

        /// <summary>
        /// Send SNMP SET command
        /// </summary>
        private bool SnmpSet(string oid, AsnType value)
        {
            try
            {
                // SNMP community name
                OctetString community = new OctetString(Community);

                // Define agent parameters
                AgentParameters param = new AgentParameters(community);
                param.Version = SnmpVersion.Ver1;

                // Construct the agent address object
                IpAddress agent = new IpAddress(ipAddress);

                // Construct target
                UdpTarget target = new UdpTarget((IPAddress)agent, SnmpPort, SnmpTimeout, 1);

                // Pdu class used for all requests
                Pdu pdu = new Pdu(PduType.Set);
                pdu.VbList.Add(new Oid(oid), value);

                // Make SNMP request
                SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);

                // If result is null then agent didn't reply or we couldn't parse the reply.
                if (result != null)
                {
                    // ErrorStatus other than 0 is an error
                    if (result.Pdu.ErrorStatus != 0)
                    {
                        Console.WriteLine($"SNMP SET Error: {result.Pdu.ErrorStatus} at index {result.Pdu.ErrorIndex}");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine($"SNMP SET successful: {oid}");
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("SNMP SET: No response from agent.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SNMP SET Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send SNMP GET command
        /// </summary>
        private string SnmpGet(string oid)
        {
            try
            {
                // SNMP community name
                OctetString community = new OctetString(Community);

                // Define agent parameters
                AgentParameters param = new AgentParameters(community);
                param.Version = SnmpVersion.Ver1;

                // Construct the agent address object
                IpAddress agent = new IpAddress(ipAddress);

                // Construct target
                UdpTarget target = new UdpTarget((IPAddress)agent, SnmpPort, SnmpTimeout, 1);

                // Pdu class used for all requests
                Pdu pdu = new Pdu(PduType.Get);
                pdu.VbList.Add(new Oid(oid));

                // Make SNMP request
                SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);

                // If result is null then agent didn't reply or we couldn't parse the reply.
                if (result != null)
                {
                    // ErrorStatus other than 0 is an error
                    if (result.Pdu.ErrorStatus != 0)
                    {
                        Console.WriteLine($"SNMP GET Error: {result.Pdu.ErrorStatus} at index {result.Pdu.ErrorIndex}");
                        return null;
                    }
                    else
                    {
                        // Get the value
                        foreach (Vb v in result.Pdu.VbList)
                        {
                            return v.Value.ToString();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("SNMP GET: No response from agent.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SNMP GET Exception: {ex.Message}");
                return null;
            }

            return null;
        }

        #endregion

        #region PDU Control Methods

        /// <summary>
        /// Turn on PDU port
        /// </summary>
        public bool SetPduPortOn(int port)
        {
            string oid = GetPortControlOid(port);
            Console.WriteLine($"[PDU] Turning ON port {port}");
            return SnmpSet(oid, new Integer32(3));
        }

        /// <summary>
        /// Turn off PDU port
        /// </summary>
        public bool SetPduPortOff(int port)
        {
            string oid = GetPortControlOid(port);
            Console.WriteLine($"[PDU] Turning OFF port {port}");
            return SnmpSet(oid, new Integer32(4));
        }

        /// <summary>
        /// Set PDU port name
        /// </summary>
        public bool SetPduPortName(int port, string name)
        {
            string oid = GetPortNameOid(port);
            Console.WriteLine($"[PDU] Setting port {port} name to: {name}");
            return SnmpSet(oid, new OctetString(name));
        }

        /// <summary>
        /// Set PDU port location
        /// </summary>
        public bool SetPduPortLocation(int port, string location)
        {
            string oid = GetPortLocationOid(port);
            Console.WriteLine($"[PDU] Setting port {port} location to: {location}");
            return SnmpSet(oid, new OctetString(location));
        }

        /// <summary>
        /// Get PDU port state (true=ON, false=OFF, null=unknown)
        /// </summary>
        public bool? GetPduPortState(int port)
        {
            string oid = GetPortStateOid(port);
            string result = SnmpGet(oid);

            if (result == "3")
                return true;
            else if (result == "2" || result == "4")
                return false;
            else
                return null;
        }

        /// <summary>
        /// Get PDU port name
        /// </summary>
        public string GetPduPortName(int port)
        {
            string oid = GetPortNameOid(port);
            return SnmpGet(oid);
        }

        /// <summary>
        /// Get PDU port location
        /// </summary>
        public string GetPduPortLocation(int port)
        {
            string oid = GetPortLocationOid(port);
            return SnmpGet(oid);
        }

        /// <summary>
        /// Get PDU port current in mA
        /// </summary>
        public int? GetPduPortCurrent(int port)
        {
            string oid = GetPortCurrentOid(port);
            string result = SnmpGet(oid);
            
            if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int current))
            {
                return current;
            }
            
            return null;
        }

        /// <summary>
        /// Get PDU port power in 0.1W (divide by 10 for actual Watts)
        /// </summary>
        public int? GetPduPortPower(int port)
        {
            string oid = GetPortPowerOid(port);
            string result = SnmpGet(oid);
            
            if (int.TryParse(result, out int power))
                return power;
            
            return null;
        }

        /// <summary>
        /// Get PDU port power in Watts
        /// </summary>
        public double? GetPduPortPowerWatts(int port)
        {
            int? power = GetPduPortPower(port);
            if (power.HasValue)
                return power.Value / 10.0;
            
            return null;
        }

        /// <summary>
        /// Get PDU name
        /// </summary>
        public string GetPduName()
        {
            string oid = GetPduNameOid();
            return SnmpGet(oid);
        }

        /// <summary>
        /// Check if PDU is connected and responding
        /// </summary>
        public bool CheckConnection()
        {
            try
            {
                string name = GetPduName();
                return !string.IsNullOrEmpty(name) && name.Contains("PDU");
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region OID Helper Methods

        private string GetPortControlOid(int port)
        {
            return $".1.3.6.1.4.1.2468.1.4.2.1.3.2.4.1.2.{port}";
        }

        private string GetPortNameOid(int port)
        {
            return $".1.3.6.1.4.1.2468.1.4.2.1.3.2.2.1.2.{port}";
        }

        private string GetPortLocationOid(int port)
        {
            return $".1.3.6.1.4.1.2468.1.4.2.1.3.2.2.1.3.{port}";
        }

        private string GetPortStateOid(int port)
        {
            return $".1.3.6.1.4.1.2468.1.4.2.1.3.2.3.1.2.{port}";
        }

        private string GetPortCurrentOid(int port)
        {
            return $".1.3.6.1.4.1.2468.1.4.2.1.3.2.3.1.3.{port}";
        }

        private string GetPortPowerOid(int port)
        {
            return $".1.3.6.1.4.1.2468.1.4.2.1.3.2.3.1.5.{port}";
        }

        private string GetPduNameOid()
        {
            return ".1.3.6.1.4.1.2468.1.4.2.1.1.4";
        }

        #endregion

        #region Properties

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        /// <summary>
        /// Get device identifier (IP address)
        /// </summary>
        public string GetDevice()
        {
            return ipAddress;
        }

        #endregion

        #region IDisposable Implementation

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources if needed
                }
                disposed = true;
            }
        }

        ~PduController()
        {
            Dispose(false);
        }

        #endregion
    }
}