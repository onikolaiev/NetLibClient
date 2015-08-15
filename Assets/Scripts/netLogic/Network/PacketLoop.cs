using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using netLogic.Network;
using netLogic.Clients;
using netLogic.Constants;
using netLogic.Shared;

namespace netLogic.Network
{
    public class PacketLoop
    {
         Thread loop;
         uint firstByteSize;
         int dataSize;
         byte[] data;
         ServiceType ServiceStatus;
         LogonSession tClient;
         WorldSession wClient;
         bool Connected = new bool();
         
         Socket tSocket;


         public PacketLoop(LogonSession client, Socket socket)
         {
             tClient = client;
             tSocket = socket;
             ServiceStatus = ServiceType.Logon;
         }

         public PacketLoop(WorldSession client, Socket socket)
         {
             wClient = client;
             tSocket = socket;
             ServiceStatus = ServiceType.World;
         }

         public void Start()
         {
             loop = new Thread(Loop);
             loop.IsBackground = false;
             loop.Start();
         }

         public void Stop()
         {
             if (loop != null)
                 loop.Abort();
         }

         void Loop()
         {
             if (ServiceStatus == ServiceType.Logon)
             {
                 Connected = tClient.Connected;
             }

             else if (ServiceStatus == ServiceType.World)
             {
                 Connected = wClient.Connected;
             }

             while (Connected)
             {
                
                if (ServiceStatus == ServiceType.Logon)
                 {
                     if (!tSocket.Connected)
                     {
                         tClient.Connected = false;
                         Log.WriteLine(LogType.Error, "Disconnected from Logon Server");
                         return;
                     }
                     while  (tSocket.Available > 0)
                     {
                         try
                         {
                             data = OnReceive(tSocket.Available);
                             tClient.HandlePacket(new PacketIn(data, true));
                         }
                         catch (Exception ex)    // Server dc'd us most likely ;P
                         {
                         }
                     }
                 }
                 else if (ServiceStatus == ServiceType.World)
                 {
                     if (!tSocket.Connected)
                     {
                         wClient.Connected = false;
                         Log.WriteLine(LogType.Error, "Disconnected from World Server");
                         return;
                     }
                     try
                     {
                          byte[] sizeBytes = OnReceive(2);
                         dataSize = parseSize(sizeBytes);

                         
                         data = OnReceive(dataSize);
                         decryptData(data);
                         PacketIn packet = new PacketIn(data);
                         //Log.WriteLine(LogType.Network, packet.ToHex());
                         Loom.DispatchToMainThread(() =>
                         {
                             wClient.HandlePacket(packet);
                         });
                         
                     }
                     catch (Exception ex)    // Server dc'd us most likely ;P
                     {
                     }
                 }
             }
         }

         public byte[] OnReceive(int mSize)
         {
             byte[] data = new byte[mSize];

             try
             {
                 int readSoFar = 0;

                 if (ServiceStatus == ServiceType.Logon)
                 {
                     do
                     {
                         tSocket.Poll(10, SelectMode.SelectRead);

                         if (tSocket.Available > 0)
                         {
                             int read = tSocket.Receive(data, readSoFar, mSize - readSoFar, SocketFlags.None);
                             readSoFar += read;
                             Thread.Sleep(10);
                         }
                     }
                     while (readSoFar < mSize);
                 }

                 else if (ServiceStatus == ServiceType.World)
                 {
                     do
                     {
                         tSocket.Poll(10, SelectMode.SelectRead);

                         if (tSocket.Available > 0)
                         {
                             int read = tSocket.Receive(data, readSoFar, mSize - readSoFar, SocketFlags.None);
                             readSoFar += read;
                             Thread.Sleep(10);
                         }
                         else
                         {
                             //Log.WriteLine(LogType.Error, "ouch!");
                         }
                     }
                     while (readSoFar < mSize);

                 }
                 
             }

             catch (Exception ex)
             {
             }

             return data;
             
         }

         private int parseSize(byte[] SizeBytes)
         {
             try
             {
                 if (ServiceStatus == ServiceType.Logon)
                 {
                     tClient.mCrypt.Decrypt(SizeBytes, 2);
                 }
                 else if (ServiceStatus == ServiceType.World)
                 {
                     wClient.mCrypt.Decrypt(SizeBytes, 0, 2);
                 }
                 int size = ((SizeBytes[0] * 256) + SizeBytes[1]);
                 //int size = (SizeBytes[0] + SizeBytes[1]);
                 return size;
             }
             catch(Exception e)
             {
                 return 0;
             }
         }
         private uint parseSizeHdr(byte[] SizeBytes)
         {
             try
             {
                 if (ServiceStatus == ServiceType.Logon)
                 {
                     tClient.mCrypt.Decrypt(SizeBytes, 1);
                 }
                 else if (ServiceStatus == ServiceType.World)
                 {
                     wClient.mCrypt.Decrypt(SizeBytes, 0, 1);
                 }
                 //int size = ((SizeBytes[0] * 256) + SizeBytes[1]);
                 uint size = (SizeBytes[0]);
                 return size;
             }
             catch (Exception e)
             {
                 return 0;
             }
         }

         private void decryptData(byte[] Data)
         {
             if (ServiceStatus == ServiceType.Logon)
             {
                 tClient.mCrypt.Decrypt(Data,2);
             }

             else if (ServiceStatus == ServiceType.World)
             {
                 wClient.mCrypt.Decrypt(Data,0, 2);
             }
                          
         }
    }
}
