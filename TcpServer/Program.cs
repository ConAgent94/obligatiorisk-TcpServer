using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using FootballPlayerLib;

namespace TcpServer
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("This is the server");
            TcpListener listener = new TcpListener(IPAddress.Loopback, 2121);
            listener.Start();
            while (true)
            {
                Console.WriteLine("Server ready");
                TcpClient socket = listener.AcceptTcpClient();
                Console.WriteLine("Incoming client");
               
                Task.Run(() =>
                {
                    DoClient(socket);
                    
                });
                
            }
        }


        public static List<FootballPlayer> FootballPlayers = new List<FootballPlayer>()
        {
            new FootballPlayer(1,"Alex",100,1)
        };

        private static void DoClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);
            while (true)
            {
            
                string message = reader.ReadLine();
                string message2 = reader.ReadLine();
                Console.WriteLine("Server received: " + message);

           
                switch (message)
                {
                    case "GetAll":

                        foreach (FootballPlayer player in FootballPlayers)
                        {
                            Console.WriteLine($"Show all Players: ID: {player.Id}, Name: {player.Name}, Price: {player.Price}, ShirtNumber: {player.ShirtNumber}");
                            writer.WriteLine($"Show all Players: ID: {player.Id}, Name: {player.Name}, Price: {player.Price}, ShirtNumber: {player.ShirtNumber}");
                        }

                        writer.WriteLine("GetAll");
                        writer.Flush();
                        break;

                    case "Get":

                        FootballPlayer person = FootballPlayers.FirstOrDefault(a => a.Id.ToString() == message2);
                        writer.WriteLine($"ID: {person.Id}, Name: {person.Name}, Price: {person.Price}, ShirtNumber: {person.ShirtNumber}");
                        writer.WriteLine("Get");
                        writer.Flush();
                        break;


                    case "Save":
                        FootballPlayer FromJson = JsonSerializer.Deserialize<FootballPlayer>(message2);
                        FootballPlayers.Add(FromJson);
                        Console.WriteLine("FootBallPlayer");

                        writer.WriteLine("Save");
                        writer.Flush();
                        break;

                    case "Close":

                        writer.WriteLine("Bye");
                        writer.Flush();
                        socket.Close();
                        break;

                }
            }
            

        }

        


    }
}

