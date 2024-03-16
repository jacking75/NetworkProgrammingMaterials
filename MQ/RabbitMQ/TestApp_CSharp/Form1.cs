using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp_CSharp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //실행 중 연결 추가
        private void Button3_Click(object sender, EventArgs e)
        {

        }

        //라우팅으로 받는 방식 중 라운드로빈이 되는지 확인(그냥 툴로 테스트 해봐도 된다.DB 서버가 여러대 붙이면 여러게 처리하는지)
        //mq 1:1 에서 QueueDeclare 가 exclusive가 false인 경우 양쪽이 붙은 상태에서 routingkey 다르게 주면 라우팅 방식이 되나?
        //mq 1:1 에서 여러 리시버의 받기 이벤트에 하나의 함수를 연결하고, 이 함수에서 Sleep을 걸어다 리시버간에 영향이 없는지 확인하기

    }
}
