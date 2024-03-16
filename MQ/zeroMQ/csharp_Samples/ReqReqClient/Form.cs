using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReqReqServerClient
{
    public partial class Form : System.Windows.Forms.Form
    {
        RequestSocket ClientSock = null;
        Timer UITimer = new Timer();

        bool IsConnected = false;
        System.Threading.Thread NetThread = null;

        bool IsReqing = false;
                
        public Form()
        {
            InitializeComponent();

            UITimer.Tick += new EventHandler(UITimerFunc);
            UITimer.Interval = 64;
            UITimer.Enabled = true;
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(IsConnected)
            {
                ClientSock.Close();

                //IsRunNetThread = false;
                //NetThread.Join();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(IsReqing)
            {
                return;
            }

            if(ClientSock == null)
            {
                ClientSock = new RequestSocket();
                ClientSock.Connect(textBoxServerAddr.Text);
                IsConnected = true;
                //NetThread = new System.Threading.Thread(new System.Threading.ThreadStart(Run));
            }

            if(string.IsNullOrEmpty(textBoxChatMsg.Text))
            {
                return;
            }

            ClientSock.SendFrame(textBoxChatMsg.Text);
            IsReqing = true;
        }


        void Run()
        {
            while(IsConnected)
            {
                var recvMsg = ClientSock.ReceiveFrameString();
            }            
        }

        public void UITimerFunc(object sender, EventArgs e)
        {
            if (IsConnected && IsReqing)
            {
                if (ClientSock.TryReceiveFrameString(out var recvMsg))
                {
                    IsReqing = false;
                    listBox1.Items.Add(recvMsg);
                }
            }
        }

        
    }
}
