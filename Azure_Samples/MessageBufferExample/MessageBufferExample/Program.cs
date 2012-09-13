using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.ServiceBus;
using System.ServiceModel.Channels;

namespace MessageBufferExample
{
    class Program
    {
        static String serviceNamespace = "SERVICE_NAMESPACE";
        static String issuerName = "owner";
        static String issuerKey = "AUTHENTICATION_TOKEN";
        static String whiteBufferName = "White";
        static String blackBufferName = "Black";

        static void Main(string[] args)
        {
            PlayGame();
        }

        private static void PlayGame()
        {
            String[] whiteMoves = { "e4", "Nf3", "d4", "Nxd4", "Nc3" };
            String[] blackMoves = { "c5", "d6", "cxd4", "Nf6", "g6" };

            MessageBufferClient whiteBufferClient = CreateMessageBufferClient(whiteBufferName);
            MessageBufferClient blackBufferClient = CreateMessageBufferClient(blackBufferName);

            Boolean playingBlack = false;
            try
            {
                MakeMove(whiteBufferClient, "BlackOrWhite");
            }
            catch (System.TimeoutException)
            {
                playingBlack = true;
                GetMove(whiteBufferClient);
            }

            Console.WriteLine("Playing {0}", playingBlack ? "black" : "white");
            Console.WriteLine("Press Enter to start");
            Console.ReadLine();

            for (Int32 i = 0; i < whiteMoves.Length; i++)
            {
                if (playingBlack)
                {
                    String lastWhiteMove = GetMove(whiteBufferClient);
                    MakeMove(blackBufferClient, blackMoves[i]);
                }
                else
                {
                    if (i > 0)
                    {
                        String lastBlackMove = GetMove(blackBufferClient);
                    }
                    MakeMove(whiteBufferClient, whiteMoves[i]);
                }
            }

            if (playingBlack)
            {
                whiteBufferClient.DeleteMessageBuffer();
                blackBufferClient.DeleteMessageBuffer();
            }

            Console.WriteLine("Press Enter To Exit");
            Console.ReadLine();
        }

        private static MessageBufferClient CreateMessageBufferClient(String bufferName)
        {
            TransportClientEndpointBehavior credentialBehavior = GetCredentials();
            Uri bufferUri = ServiceBusEnvironment.CreateServiceUri("https", serviceNamespace, bufferName);
            MessageBufferPolicy messageBufferPolicy = GetMessageBufferPolicy();
            MessageBufferClient bufferClient = MessageBufferClient.CreateMessageBuffer(credentialBehavior, bufferUri, messageBufferPolicy);
            return bufferClient;
        }

        private static TransportClientEndpointBehavior GetCredentials()
        {
            TransportClientEndpointBehavior credentialBehavior = new TransportClientEndpointBehavior();
            credentialBehavior.CredentialType = TransportClientCredentialType.SharedSecret;
            credentialBehavior.Credentials.SharedSecret.IssuerName = issuerName;
            credentialBehavior.Credentials.SharedSecret.IssuerSecret = issuerKey;
            return credentialBehavior;
        }

        private static MessageBufferPolicy GetMessageBufferPolicy()
        {
            MessageBufferPolicy messageBufferPolicy = new MessageBufferPolicy()
            {
                ExpiresAfter = TimeSpan.FromMinutes(10d),
                MaxMessageCount = 1
            };
            return messageBufferPolicy;
        }

        private static void MakeMove(MessageBufferClient bufferClient, String move)
        {
            Console.WriteLine("{0} -> {1}", bufferClient.MessageBufferUri.LocalPath[1], move);
            using (Message moveMessage = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "urn:Message", move))
            {
                bufferClient.Send(moveMessage, TimeSpan.FromSeconds(1d));
            }
        }

        private static String GetMove(MessageBufferClient bufferClient)
        {
            String move;
            using (Message moveMessage = bufferClient.Retrieve())
            {
                move = moveMessage.GetBody<String>();
            }
            Console.WriteLine("{0} <- {1}", bufferClient.MessageBufferUri.LocalPath[1], move);
            return move;
        }
    }
}
