﻿// See https://aka.ms/new-console-template for more information
using SimpleTcp;

class Program
{
    private static SimpleTcpServer _tcpServer;

    static void Main(string[] args)
    {
        _tcpServer = new SimpleTcpServer("127.0.0.1", 20007);
        _tcpServer.Events.DataReceived += Events_DataReceived;
        _tcpServer.Start();

        Console.WriteLine("Virtual Terminal ready on 127.0.0.1:20007");
        Console.WriteLine("Wait for connections, press any key for quit");
        Console.ReadLine();

        _tcpServer.Events.DataReceived -= Events_DataReceived;
        _tcpServer.Stop();
        _tcpServer.Dispose();
    }

    private static void Events_DataReceived(object? sender, DataReceivedEventArgs e)
    {
        Console.WriteLine(BitConverter.ToString(e.Data));

        var data = e.Data.AsSpan();

        //Authorization (06 01)
        if (data.StartsWith(new byte[] { 0x06, 0x01 }))
        {
            Thread.Sleep(500);

            //Send Acknowledge
            _tcpServer.Send(e.IpPort, new byte[] { 0x80, 0x00, 0x00 });

            Thread.Sleep(1000);

            //Send Completion
            _tcpServer.Send(e.IpPort, new byte[] { 0x06, 0x0F, 0x00 });
        }
    }
}
