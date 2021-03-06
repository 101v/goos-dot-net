﻿using System;
using System.Threading;
using agsXMPP;
using AuctionSniper.Utils;
using AuctionSniper.Xmpp;
using NUnit.Framework;

namespace AuctionSniper.Integration.Tests.Xmpp
{
    [TestFixture]
    public class XmppConnectionTests
    {
        private const string XMPP_HOST = "localhost";

        
        [Test]
        [ExpectedException(typeof(XmppException))]
        public void ThrowsExceptionWhenConnectionCannotBeMade() {
            var connection = new XmppChatClient(new Jid("invalid@invalidhost/resource"), "blah");
            connection.Login();
          

        } 
        
        [Test]
        public void LoinToValidXMPPServer() {
            var connection = new XmppChatClient(new Jid("testuser",XMPP_HOST, "auction"), "pass");
            connection.Login();
        }


        [Test]
        public void LoginWithInvalidUserId()
        {
            try
            {
                var connection = new XmppChatClient(new Jid("invalid", XMPP_HOST, "auction"), "pass");
                connection.Login();
                Assert.Fail("should have thrown xmppexection when loggin with invalid user id");
            }
          
            catch(XmppException e) {Assert.That(e.Message.Contains("Auth error"));}

        }



        [Test]
        public void SendAndReceiveChatMessages() {
            var testuser = new Jid("testuser", XMPP_HOST, "auction");
            var chatClient1 = new XmppChatClient(testuser, "pass");
            var messageReceivedEvent = new ManualResetEvent(false);
            var message = string.Empty;
            chatClient1.OnChatMessageReceived += (sender, msg) => chatClient1.SendMessageTo(msg.From, msg.Body);
            chatClient1.Login();

            var auctionitem1 = new Jid("auction-item1", XMPP_HOST, "auction");
            var chatClient2 = new XmppChatClient(auctionitem1, "auction");
            chatClient2.OnChatMessageReceived += (s, m) => { message = m.Body;
                                                     messageReceivedEvent.Set();
                                                 };
            chatClient2.Login();

            
            chatClient2.SendMessageTo(testuser, "hello");

            TimeSpan timeout = 4.Seconds();
            Assert.That(messageReceivedEvent.WaitOne(timeout), "Did not receive message within {0}",timeout );
            Assert.That(message, Is.EqualTo("hello"));

        }
    }
}
