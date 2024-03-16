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
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUserID = new System.Windows.Forms.TextBox();
            this.textBoxUserPW = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.Room = new System.Windows.Forms.GroupBox();
            this.textBoxRelay = new System.Windows.Forms.TextBox();
            this.btnRoomChat = new System.Windows.Forms.Button();
            this.textBoxRoomSendMsg = new System.Windows.Forms.TextBox();
            this.listBoxRoomChatMsg = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxRoomUserList = new System.Windows.Forms.ListBox();
            this.btn_RoomLeave = new System.Windows.Forms.Button();
            this.btn_RoomEnter = new System.Windows.Forms.Button();
            this.textBoxRoomNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox5.SuspendLayout();
            this.Room.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDisconnect.Location = new System.Drawing.Point(408, 25);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(86, 25);
            this.btnDisconnect.TabIndex = 29;
            this.btnDisconnect.Text = "접속 끊기";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConnect.Location = new System.Drawing.Point(321, 26);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(81, 25);
            this.btnConnect.TabIndex = 28;
            this.btnConnect.Text = "접속하기";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxPort);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.textBoxIP);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.btnConnect);
            this.groupBox5.Controls.Add(this.btnDisconnect);
            this.groupBox5.Location = new System.Drawing.Point(14, 13);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(515, 60);
            this.groupBox5.TabIndex = 27;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Socket";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(257, 26);
            this.textBoxPort.MaxLength = 6;
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(58, 22);
            this.textBoxPort.TabIndex = 18;
            this.textBoxPort.Text = "11021";
            this.textBoxPort.WordWrap = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(186, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 14);
            this.label10.TabIndex = 17;
            this.label10.Text = "포트 번호:";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(78, 25);
            this.textBoxIP.MaxLength = 6;
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(99, 22);
            this.textBoxIP.TabIndex = 11;
            this.textBoxIP.Text = "0.0.0.0";
            this.textBoxIP.WordWrap = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 14);
            this.label9.TabIndex = 10;
            this.label9.Text = "서버 주소:";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(11, 623);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(135, 14);
            this.labelStatus.TabIndex = 40;
            this.labelStatus.Text = "서버 접속 상태: ???";
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.HorizontalScrollbar = true;
            this.listBoxLog.Location = new System.Drawing.Point(11, 462);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(559, 147);
            this.listBoxLog.TabIndex = 41;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 14);
            this.label1.TabIndex = 42;
            this.label1.Text = "UserID:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBoxUserID
            // 
            this.textBoxUserID.Location = new System.Drawing.Point(75, 155);
            this.textBoxUserID.MaxLength = 6;
            this.textBoxUserID.Name = "textBoxUserID";
            this.textBoxUserID.Size = new System.Drawing.Size(99, 22);
            this.textBoxUserID.TabIndex = 43;
            this.textBoxUserID.Text = "jacking75";
            this.textBoxUserID.WordWrap = false;
            this.textBoxUserID.TextChanged += new System.EventHandler(this.textBoxUserID_TextChanged);
            // 
            // textBoxUserPW
            // 
            this.textBoxUserPW.Location = new System.Drawing.Point(255, 156);
            this.textBoxUserPW.MaxLength = 6;
            this.textBoxUserPW.Name = "textBoxUserPW";
            this.textBoxUserPW.Size = new System.Drawing.Size(99, 22);
            this.textBoxUserPW.TabIndex = 45;
            this.textBoxUserPW.Text = "jacking75";
            this.textBoxUserPW.WordWrap = false;
            this.textBoxUserPW.TextChanged += new System.EventHandler(this.textBoxUserPW_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(185, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 14);
            this.label2.TabIndex = 44;
            this.label2.Text = "PassWD:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button2.Location = new System.Drawing.Point(365, 151);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(114, 28);
            this.button2.TabIndex = 46;
            this.button2.Text = "Login";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Room
            // 
            this.Room.Controls.Add(this.textBoxRelay);
            this.Room.Controls.Add(this.btnRoomChat);
            this.Room.Controls.Add(this.textBoxRoomSendMsg);
            this.Room.Controls.Add(this.listBoxRoomChatMsg);
            this.Room.Controls.Add(this.label4);
            this.Room.Controls.Add(this.listBoxRoomUserList);
            this.Room.Controls.Add(this.btn_RoomLeave);
            this.Room.Controls.Add(this.btn_RoomEnter);
            this.Room.Controls.Add(this.textBoxRoomNumber);
            this.Room.Controls.Add(this.label3);
            this.Room.Location = new System.Drawing.Point(15, 185);
            this.Room.Name = "Room";
            this.Room.Size = new System.Drawing.Size(566, 270);
            this.Room.TabIndex = 47;
            this.Room.TabStop = false;
            this.Room.Text = "Room";
            // 
            // textBoxRelay
            // 
            this.textBoxRelay.Location = new System.Drawing.Point(350, 26);
            this.textBoxRelay.MaxLength = 6;
            this.textBoxRelay.Name = "textBoxRelay";
            this.textBoxRelay.Size = new System.Drawing.Size(124, 22);
            this.textBoxRelay.TabIndex = 55;
            this.textBoxRelay.Text = "test";
            this.textBoxRelay.WordWrap = false;
            // 
            // btnRoomChat
            // 
            this.btnRoomChat.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRoomChat.Location = new System.Drawing.Point(499, 230);
            this.btnRoomChat.Name = "btnRoomChat";
            this.btnRoomChat.Size = new System.Drawing.Size(57, 28);
            this.btnRoomChat.TabIndex = 53;
            this.btnRoomChat.Text = "chat";
            this.btnRoomChat.UseVisualStyleBackColor = true;
            this.btnRoomChat.Click += new System.EventHandler(this.btnRoomChat_Click);
            // 
            // textBoxRoomSendMsg
            // 
            this.textBoxRoomSendMsg.Location = new System.Drawing.Point(15, 233);
            this.textBoxRoomSendMsg.MaxLength = 32;
            this.textBoxRoomSendMsg.Name = "textBoxRoomSendMsg";
            this.textBoxRoomSendMsg.Size = new System.Drawing.Size(478, 22);
            this.textBoxRoomSendMsg.TabIndex = 52;
            this.textBoxRoomSendMsg.Text = "test1";
            this.textBoxRoomSendMsg.WordWrap = false;
            // 
            // listBoxRoomChatMsg
            // 
            this.listBoxRoomChatMsg.FormattingEnabled = true;
            this.listBoxRoomChatMsg.Location = new System.Drawing.Point(165, 70);
            this.listBoxRoomChatMsg.Name = "listBoxRoomChatMsg";
            this.listBoxRoomChatMsg.Size = new System.Drawing.Size(391, 147);
            this.listBoxRoomChatMsg.TabIndex = 51;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 14);
            this.label4.TabIndex = 50;
            this.label4.Text = "User List:";
            // 
            // listBoxRoomUserList
            // 
            this.listBoxRoomUserList.FormattingEnabled = true;
            this.listBoxRoomUserList.Location = new System.Drawing.Point(15, 72);
            this.listBoxRoomUserList.Name = "listBoxRoomUserList";
            this.listBoxRoomUserList.Size = new System.Drawing.Size(140, 147);
            this.listBoxRoomUserList.TabIndex = 49;
            // 
            // btn_RoomLeave
            // 
            this.btn_RoomLeave.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_RoomLeave.Location = new System.Drawing.Point(247, 21);
            this.btn_RoomLeave.Name = "btn_RoomLeave";
            this.btn_RoomLeave.Size = new System.Drawing.Size(75, 28);
            this.btn_RoomLeave.TabIndex = 48;
            this.btn_RoomLeave.Text = "Leave";
            this.btn_RoomLeave.UseVisualStyleBackColor = true;
            this.btn_RoomLeave.Click += new System.EventHandler(this.btn_RoomLeave_Click);
            // 
            // btn_RoomEnter
            // 
            this.btn_RoomEnter.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_RoomEnter.Location = new System.Drawing.Point(165, 20);
            this.btn_RoomEnter.Name = "btn_RoomEnter";
            this.btn_RoomEnter.Size = new System.Drawing.Size(75, 28);
            this.btn_RoomEnter.TabIndex = 47;
            this.btn_RoomEnter.Text = "Enter";
            this.btn_RoomEnter.UseVisualStyleBackColor = true;
            this.btn_RoomEnter.Click += new System.EventHandler(this.btn_RoomEnter_Click);
            // 
            // textBoxRoomNumber
            // 
            this.textBoxRoomNumber.Location = new System.Drawing.Point(112, 22);
            this.textBoxRoomNumber.MaxLength = 6;
            this.textBoxRoomNumber.Name = "textBoxRoomNumber";
            this.textBoxRoomNumber.Size = new System.Drawing.Size(43, 22);
            this.textBoxRoomNumber.TabIndex = 44;
            this.textBoxRoomNumber.Text = "0";
            this.textBoxRoomNumber.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 14);
            this.label3.TabIndex = 43;
            this.label3.Text = "Room Number:";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 651);
            this.Controls.Add(this.Room);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBoxUserPW);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxUserID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.listBoxLog);
            this.Controls.Add(this.groupBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "mainForm";
            this.Text = "네트워크 테스트 클라이언트";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.Room.ResumeLayout(false);
            this.Room.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.TextBox textBoxUserID;
        private System.Windows.Forms.TextBox textBoxUserPW;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox Room;
        private System.Windows.Forms.Button btn_RoomLeave;
        private System.Windows.Forms.Button btn_RoomEnter;
        private System.Windows.Forms.TextBox textBoxRoomNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRoomChat;
        private System.Windows.Forms.TextBox textBoxRoomSendMsg;
        private System.Windows.Forms.ListBox listBoxRoomChatMsg;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBoxRoomUserList;
        private System.Windows.Forms.TextBox textBoxRelay;
    }
}

