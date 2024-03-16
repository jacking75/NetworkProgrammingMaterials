namespace TcpDummyClient
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxDummyStartNumber = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDummyCount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxMaxPacketSize = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxDummyStartNumber);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBoxPort);
            this.groupBox1.Controls.Add(this.textBoxIP);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxDummyCount);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(14, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(435, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dummy Info";
            // 
            // textBoxDummyStartNumber
            // 
            this.textBoxDummyStartNumber.Location = new System.Drawing.Point(308, 55);
            this.textBoxDummyStartNumber.Name = "textBoxDummyStartNumber";
            this.textBoxDummyStartNumber.Size = new System.Drawing.Size(50, 22);
            this.textBoxDummyStartNumber.TabIndex = 17;
            this.textBoxDummyStartNumber.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(165, 59);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(143, 14);
            this.label8.TabIndex = 16;
            this.label8.Text = "Dummy Start Number";
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.Location = new System.Drawing.Point(237, 238);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.Size = new System.Drawing.Size(634, 22);
            this.textBoxStatus.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(159, 241);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 14);
            this.label6.TabIndex = 14;
            this.label6.Text = "진행 상황:";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(102, 238);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(45, 22);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 241);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 14);
            this.label4.TabIndex = 7;
            this.label4.Text = "현재 접속 수";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(205, 23);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(69, 22);
            this.textBoxPort.TabIndex = 4;
            this.textBoxPort.Text = "32452";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(110, 23);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(92, 22);
            this.textBoxIP.TabIndex = 2;
            this.textBoxIP.Text = "192.168.0.10";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server IP, Port";
            // 
            // textBoxDummyCount
            // 
            this.textBoxDummyCount.Location = new System.Drawing.Point(105, 55);
            this.textBoxDummyCount.Name = "textBoxDummyCount";
            this.textBoxDummyCount.Size = new System.Drawing.Size(50, 22);
            this.textBoxDummyCount.TabIndex = 6;
            this.textBoxDummyCount.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 14);
            this.label3.TabIndex = 5;
            this.label3.Text = "Dummy Count";
            // 
            // textBoxMaxPacketSize
            // 
            this.textBoxMaxPacketSize.Location = new System.Drawing.Point(345, 70);
            this.textBoxMaxPacketSize.Name = "textBoxMaxPacketSize";
            this.textBoxMaxPacketSize.Size = new System.Drawing.Size(45, 22);
            this.textBoxMaxPacketSize.TabIndex = 12;
            this.textBoxMaxPacketSize.Text = "5000";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(179, 74);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(160, 14);
            this.label12.TabIndex = 11;
            this.label12.Text = "Max Packet Size(byte)";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(90, 17);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(96, 25);
            this.button2.TabIndex = 12;
            this.button2.Text = "Disconnect";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 18);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 25);
            this.button1.TabIndex = 11;
            this.button1.Text = "Connect";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBoxLog
            // 
            this.listBoxLog.DisplayMember = "Message";
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.HorizontalScrollbar = true;
            this.listBoxLog.Location = new System.Drawing.Point(8, 275);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.ScrollAlwaysVisible = true;
            this.listBoxLog.Size = new System.Drawing.Size(863, 355);
            this.listBoxLog.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Location = new System.Drawing.Point(14, 179);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(123, 49);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "접속/끊기 반복";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(61, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(47, 24);
            this.button3.TabIndex = 11;
            this.button3.Text = "Stop";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(8, 18);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(47, 24);
            this.button4.TabIndex = 2;
            this.button4.Text = "Start";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 14);
            this.label7.TabIndex = 9;
            this.label7.Text = "액션 반복 횟수";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBox11);
            this.groupBox5.Controls.Add(this.textBox10);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.button8);
            this.groupBox5.Controls.Add(this.button9);
            this.groupBox5.Location = new System.Drawing.Point(342, 185);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(389, 44);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "echo";
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(228, 15);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(39, 22);
            this.textBox11.TabIndex = 15;
            this.textBox11.Text = "1024";
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(185, 15);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(39, 22);
            this.textBox10.TabIndex = 14;
            this.textBox10.Text = "1024";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(183, 14);
            this.label11.TabIndex = 13;
            this.label11.Text = "데이터 최소최대 길이(byte)";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(329, 14);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(47, 24);
            this.button8.TabIndex = 11;
            this.button8.Text = "Stop";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(276, 15);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(47, 24);
            this.button9.TabIndex = 2;
            this.button9.Text = "Start";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBox14);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.textBox2);
            this.groupBox6.Controls.Add(this.textBoxMaxPacketSize);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.textBox12);
            this.groupBox6.Controls.Add(this.textBox9);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.textBox3);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Location = new System.Drawing.Point(466, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(405, 99);
            this.groupBox6.TabIndex = 8;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Config";
            // 
            // textBox14
            // 
            this.textBox14.Location = new System.Drawing.Point(342, 43);
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new System.Drawing.Size(50, 22);
            this.textBox14.TabIndex = 16;
            this.textBox14.Text = "32";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(188, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(151, 14);
            this.label13.TabIndex = 15;
            this.label13.Text = "더미 간 실행 간격(ms)";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(138, 72);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(28, 22);
            this.textBox2.TabIndex = 14;
            this.textBox2.Text = "30";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 14);
            this.label5.TabIndex = 13;
            this.label5.Text = "액션 반복 시간(초)";
            // 
            // textBox12
            // 
            this.textBox12.Location = new System.Drawing.Point(117, 44);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(50, 22);
            this.textBox12.TabIndex = 12;
            this.textBox12.Text = "1";
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(341, 17);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(50, 22);
            this.textBox9.TabIndex = 11;
            this.textBox9.Text = "6000";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(206, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(132, 14);
            this.label10.TabIndex = 10;
            this.label10.Text = "액션 시간 제한(ms)";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(117, 15);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(50, 22);
            this.textBox3.TabIndex = 7;
            this.textBox3.Text = "10";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "액션 간격(ms)";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.button6);
            this.groupBox7.Controls.Add(this.button5);
            this.groupBox7.Controls.Add(this.button18);
            this.groupBox7.Controls.Add(this.button16);
            this.groupBox7.Location = new System.Drawing.Point(14, 114);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(857, 60);
            this.groupBox7.TabIndex = 13;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Scenario";
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("굴림", 8.830189F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button6.Location = new System.Drawing.Point(786, 22);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(65, 24);
            this.button6.TabIndex = 10;
            this.button6.Text = "Stop";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(127, 22);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(104, 24);
            this.button5.TabIndex = 9;
            this.button5.Text = "RoomInOut";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(244, 22);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(104, 24);
            this.button18.TabIndex = 8;
            this.button18.Text = "RoomChat";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(8, 22);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(104, 24);
            this.button16.TabIndex = 6;
            this.button16.Text = "LoginOut";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Location = new System.Drawing.Point(143, 180);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(194, 49);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "연결만 하기";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 641);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxStatus);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.listBoxLog);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainForm";
            this.Text = "SimpleTcpEchoDummy";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBoxDummyCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxMaxPacketSize;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxDummyStartNumber;
        private System.Windows.Forms.Label label8;
    }
}

