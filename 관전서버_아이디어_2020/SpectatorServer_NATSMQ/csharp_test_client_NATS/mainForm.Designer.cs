namespace csharp_test_client
{
    partial class mainForm
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
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.listBoxStartType = new System.Windows.Forms.ListBox();
            this.textBoxClientID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxClusterID = new System.Windows.Forms.TextBox();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDisconnect.Location = new System.Drawing.Point(340, 22);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(49, 23);
            this.btnDisconnect.TabIndex = 29;
            this.btnDisconnect.Text = "끊기";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConnect.Location = new System.Drawing.Point(287, 22);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(47, 23);
            this.btnConnect.TabIndex = 28;
            this.btnConnect.Text = "접속";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBox4);
            this.groupBox5.Controls.Add(this.textBoxPort);
            this.groupBox5.Controls.Add(this.button2);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.labelStatus);
            this.groupBox5.Controls.Add(this.textBox1);
            this.groupBox5.Controls.Add(this.textBoxIP);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.listBoxStartType);
            this.groupBox5.Controls.Add(this.btnConnect);
            this.groupBox5.Controls.Add(this.textBoxClientID);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.btnDisconnect);
            this.groupBox5.Controls.Add(this.textBoxClusterID);
            this.groupBox5.Location = new System.Drawing.Point(16, 11);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(453, 188);
            this.groupBox5.TabIndex = 27;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "NATS MQ Sub";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(372, 76);
            this.textBox4.MaxLength = 6;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(73, 21);
            this.textBox4.TabIndex = 54;
            this.textBox4.Text = "0";
            this.textBox4.WordWrap = false;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(214, 24);
            this.textBoxPort.MaxLength = 6;
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(51, 21);
            this.textBoxPort.TabIndex = 18;
            this.textBoxPort.Text = "4222";
            this.textBoxPort.WordWrap = false;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button2.Location = new System.Drawing.Point(298, 149);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(65, 23);
            this.button2.TabIndex = 53;
            this.button2.Text = "종료";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(152, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 12);
            this.label10.TabIndex = 17;
            this.label10.Text = "포트 번호:";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Location = new System.Drawing.Point(228, 149);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(65, 23);
            this.button1.TabIndex = 52;
            this.button1.Text = "시작";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(297, 53);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(111, 12);
            this.labelStatus.TabIndex = 40;
            this.labelStatus.Text = "서버 접속 상태: ???";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(63, 149);
            this.textBox1.MaxLength = 6;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(159, 21);
            this.textBox1.TabIndex = 50;
            this.textBox1.Text = "test1";
            this.textBox1.WordWrap = false;
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(68, 23);
            this.textBoxIP.MaxLength = 6;
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(79, 21);
            this.textBoxIP.TabIndex = 11;
            this.textBoxIP.Text = "127.0.0.1";
            this.textBoxIP.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 12);
            this.label3.TabIndex = 49;
            this.label3.Text = "Subject:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 12);
            this.label9.TabIndex = 10;
            this.label9.Text = "서버 주소:";
            // 
            // listBoxStartType
            // 
            this.listBoxStartType.FormattingEnabled = true;
            this.listBoxStartType.ItemHeight = 12;
            this.listBoxStartType.Items.AddRange(new object[] {
            "StartAtSequence: 일련 번호를 지정하여 이 위치부터 시작한다.",
            "StartAtTime: 절대 시간 기준",
            "StartAtTimeDelta: 시작 시간을 기준으로 상대 시간(TimeSpan)",
            "StartWithLastReceived: 마지막에서 시작한다.",
            "한번에 모두 다 받기"});
            this.listBoxStartType.Location = new System.Drawing.Point(8, 76);
            this.listBoxStartType.Name = "listBoxStartType";
            this.listBoxStartType.Size = new System.Drawing.Size(359, 64);
            this.listBoxStartType.TabIndex = 49;
            // 
            // textBoxClientID
            // 
            this.textBoxClientID.Location = new System.Drawing.Point(214, 49);
            this.textBoxClientID.MaxLength = 6;
            this.textBoxClientID.Name = "textBoxClientID";
            this.textBoxClientID.Size = new System.Drawing.Size(75, 21);
            this.textBoxClientID.TabIndex = 45;
            this.textBoxClientID.Text = "jacking75-1";
            this.textBoxClientID.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 12);
            this.label1.TabIndex = 42;
            this.label1.Text = "Cluster ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(157, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 12);
            this.label2.TabIndex = 44;
            this.label2.Text = "Client ID:";
            // 
            // textBoxClusterID
            // 
            this.textBoxClusterID.Location = new System.Drawing.Point(68, 49);
            this.textBoxClusterID.MaxLength = 6;
            this.textBoxClusterID.Name = "textBoxClusterID";
            this.textBoxClusterID.Size = new System.Drawing.Size(79, 21);
            this.textBoxClusterID.TabIndex = 43;
            this.textBoxClusterID.Text = "pvp";
            this.textBoxClusterID.WordWrap = false;
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.HorizontalScrollbar = true;
            this.listBoxLog.ItemHeight = 12;
            this.listBoxLog.Location = new System.Drawing.Point(10, 453);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(459, 136);
            this.listBoxLog.TabIndex = 41;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.textBox6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.textBox7);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(16, 216);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(453, 197);
            this.groupBox2.TabIndex = 49;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Streamming Pub";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(299, 56);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(111, 12);
            this.label11.TabIndex = 50;
            this.label11.Text = "서버 접속 상태: ???";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(218, 26);
            this.textBox2.MaxLength = 6;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(51, 21);
            this.textBox2.TabIndex = 59;
            this.textBox2.Text = "4222";
            this.textBox2.WordWrap = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(156, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 12);
            this.label5.TabIndex = 58;
            this.label5.Text = "포트 번호:";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(73, 25);
            this.textBox5.MaxLength = 6;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(79, 21);
            this.textBox5.TabIndex = 57;
            this.textBox5.Text = "127.0.0.1";
            this.textBox5.WordWrap = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 12);
            this.label6.TabIndex = 56;
            this.label6.Text = "서버 주소:";
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button5.Location = new System.Drawing.Point(288, 26);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(52, 23);
            this.button5.TabIndex = 60;
            this.button5.Text = "접속";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(218, 51);
            this.textBox6.MaxLength = 6;
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(75, 21);
            this.textBox6.TabIndex = 65;
            this.textBox6.Text = "jacking75-2";
            this.textBox6.WordWrap = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 12);
            this.label7.TabIndex = 62;
            this.label7.Text = "Cluster ID:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(161, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 12);
            this.label8.TabIndex = 64;
            this.label8.Text = "Client ID:";
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button6.Location = new System.Drawing.Point(346, 26);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(52, 23);
            this.button6.TabIndex = 61;
            this.button6.Text = "끊기";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(73, 51);
            this.textBox7.MaxLength = 6;
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(79, 21);
            this.textBox7.TabIndex = 63;
            this.textBox7.Text = "pvp";
            this.textBox7.WordWrap = false;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button4.Location = new System.Drawing.Point(346, 85);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(69, 23);
            this.button4.TabIndex = 55;
            this.button4.Text = "추가";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button3.Location = new System.Drawing.Point(202, 85);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(138, 23);
            this.button3.TabIndex = 53;
            this.button3.Text = "더미 데이터 추가 1";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(65, 86);
            this.textBox3.MaxLength = 6;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(131, 21);
            this.textBox3.TabIndex = 52;
            this.textBox3.Text = "test1";
            this.textBox3.WordWrap = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 12);
            this.label4.TabIndex = 51;
            this.label4.Text = "Subject:";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 601);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.listBoxLog);
            this.Controls.Add(this.groupBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "mainForm";
            this.Text = "네트워크 테스트 클라이언트";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxClusterID;
        private System.Windows.Forms.TextBox textBoxClientID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxStartType;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TextBox textBox7;
    }
}

