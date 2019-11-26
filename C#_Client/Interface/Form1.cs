using log4net;
using SmartFactory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interface
{
    
    public partial class Form1 : Form
    {
        private SmartFactory.Udp_Client udpClient;
        private SmartFactory.Udp_Server udpServer;

        DataTable dataTable;

        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Form1()
        {
            InitializeComponent();
            this.tbService.Text = "UDP";
            this.tbIp.Text = "127.0.0.1";
            this.tbPort.Text = "5002";

            udpServer = new Udp_Server();
            udpServer.StartAsServer(tbIp.Text, tbPort.Text);
            udpServer.MessageEvent += OnReceiveMeaasge;
            //udpServer.getForm(this);

        }

        private void TbCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                byte[] resvReponse = null;

                if (tbService.Text.Equals("TCP"))
                {
                    TcpConnect tcpCon = new TcpConnect();
                    //resvReponse = tcpCon.SendRequest_Message(tbIp.Text, tbPort.Text, tbCommand.Text);
                }
                else
                {
                    udpClient = new Udp_Client();
                    udpClient.StartAsClient(tbIp.Text, tbPort.Text);
                    resvReponse = udpClient.SendRequest(Encoding.UTF8.GetBytes(tbCommand.Text));
                }

                if (resvReponse != null)
                {
                    string sResponse = Encoding.UTF8.GetString(resvReponse);

                    if (sResponse.Length < 20)
                    {

                        if (sResponse.Equals("SUCCESS"))
                        {
                            //tbResponse.Text = sResponse;

                            if (tbService.Text.Equals("TCP"))
                            {
                                TcpConnect tcpCon = new TcpConnect();
                                //resvReponse = tcpCon.SendRequest_Message(tbIp.Text, tbPort.Text, "view machinelist");
                            }
                            else
                            {
                                Udp_Client udpClient = new Udp_Client();
                                resvReponse = udpClient.SendRequest(Encoding.UTF8.GetBytes("view machinelist"));
                            }
                        }

                    }

                    /* MachineList Set GRID*/
                    if (resvReponse.Length > 20)
                    {
                        DataTable dataTable = Fomatter.xmlDataToDataTable(Encoding.UTF8.GetString(resvReponse));
                        if (dataTable.Rows.Count > 0) dgvMachinelist.DataSource = null;
                        dgvMachinelist.DataSource = dataTable;
                        //SetDataGeidView(dataTable);
                    }

                    tbCommand.Text = "";

                }
            }
        }
        #region "표만들기"
        private void DataGridViewHeaderSetting()
        {
            dgvMachinelist.AutoGenerateColumns = false;


            DataGridViewTextBoxColumn Col_01 = new DataGridViewTextBoxColumn();
            Col_01.DataPropertyName = "MachineName";
            Col_01.HeaderText = "MachineName";

            DataGridViewTextBoxColumn Col_02 = new DataGridViewTextBoxColumn();
            Col_02.DataPropertyName = "StateName";
            Col_02.HeaderText = "StateName";

            DataGridViewTextBoxColumn Col_03 = new DataGridViewTextBoxColumn();
            Col_03.DataPropertyName = "LastEventName";
            Col_03.HeaderText = "LastEventName";

            DataGridViewTextBoxColumn Col_04 = new DataGridViewTextBoxColumn();
            Col_04.DataPropertyName = "LastEventTime";
            Col_04.HeaderText = "LastEventTime";

            DataGridViewTextBoxColumn Col_05 = new DataGridViewTextBoxColumn();
            Col_05.DataPropertyName = "LastEventComment";
            Col_05.HeaderText = "설   명";

            DataGridViewTextBoxColumn Col_06 = new DataGridViewTextBoxColumn();
            Col_06.DataPropertyName = "ReasonCode";
            Col_06.HeaderText = "ReasonCode";
            /*
            DataGridViewTextBoxColumn Col_07 = new DataGridViewTextBoxColumn();
            Col_07.DataPropertyName = "CompanyTelephone";
            Col_07.HeaderText = "회사 전화번호";

            DataGridViewTextBoxColumn Col_08 = new DataGridViewTextBoxColumn();
            Col_08.DataPropertyName = "Mobile";
            Col_08.HeaderText = "핸드폰번호";

            DataGridViewTextBoxColumn Col_09 = new DataGridViewTextBoxColumn();
            Col_09.DataPropertyName = "Company";
            Col_09.HeaderText = "회사명";

            DataGridViewTextBoxColumn Col_10 = new DataGridViewTextBoxColumn();
            Col_10.DataPropertyName = "RegDate";
            Col_10.HeaderText = "등록일";
            */
            dgvMachinelist.Columns.Add(Col_01);
            dgvMachinelist.Columns.Add(Col_02);
            dgvMachinelist.Columns.Add(Col_03);
            dgvMachinelist.Columns.Add(Col_04);
            dgvMachinelist.Columns.Add(Col_05);
            dgvMachinelist.Columns.Add(Col_06);
            /*
            dataGridView.Columns.Add(Col_07);
            dataGridView.Columns.Add(Col_08);
            dataGridView.Columns.Add(Col_09);
            dataGridView.Columns.Add(Col_10);
            */

            foreach (DataGridViewColumn col in dgvMachinelist.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                // Set the column size automatically
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            // Set the header column BackColor
            dgvMachinelist.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOrange;
            dgvMachinelist.EnableHeadersVisualStyles = false;
        }
        #endregion

        private void dataGridStyle01ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dgvMachinelist.DataSource;
            dgvMachinelist.DataSource = new DataTable();

            DataGridViewHeaderSetting();

            dgvMachinelist.DataSource = dt;
        }

        private void MachineListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "view machinelist";

            KeyEventArgs keyE = new KeyEventArgs(Keys.Enter);
            TbCommand_KeyDown(tbCommand, keyE);
        }

        private void TbCommand_TextChanged(object sender, EventArgs e)
        {
            tbCommand.Focus();
            tbCommand.Select(tbCommand.Text.Length, 0);
        }

        private void OnReceiveMeaasge(object sender, SoketEventArgs e)
        {
            logger.Info(string.Format("[{0}][{1}]{2}" , e.ProtocolType, e.EndPoint, e.Message));
            //tbCommand.Text = string.Format("[{0}][{1}]{2}", e.ProtocolType, e.EndPoint, e.Message);

            //크로스 스레딩 (하나의 작업을 여러개의 스레드가 동시에 실행하려할때 발생) 해결하기위함
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    DataTable dataTable = Fomatter.xmlDataToDataTable(e.Message);
                    if (dataTable.Rows.Count > 0) dgvMachinelist.DataSource = null;
                    dgvMachinelist.DataSource = dataTable;

                    foreach (DataGridViewRow row in dgvMachinelist.Rows)
                    {
                        try
                        {
                            if (row.Cells[2].Value.Equals("WAIT"))
                            {
                                row.DefaultCellStyle.BackColor = Color.Red;
                            }
                        }catch(Exception ex)
                        {
                            break;
                        }
                    }
                }));
            }
            else
            {
                DataTable dataTable = Fomatter.xmlDataToDataTable(e.Message);
                if (dataTable.Rows.Count > 0) dgvMachinelist.DataSource = null;
                dgvMachinelist.DataSource = dataTable;
                tbCommand.Text = "else";

                //foreach (DataGridViewRow row in dgvMachinelist.Rows)
                //{
                //    if (row.Cells[2].Value.Equals("WAIT"))
                //    {
                //        row.DefaultCellStyle.BackColor = Color.Red;
                //    }
                //}
            }
        }

        private void BtnServer_Click(object sender, EventArgs e)
        {
            switch (this.btnServer.Text)
            {
                case "Start":
                    udpServer = new Udp_Server();
                    udpServer.StartAsServer(tbIp.Text, tbPort.Text);
                    udpServer.MessageEvent += OnRecevieViewList;

                    tbCommand.Enabled = false;
                    this.btnServer.Text = "Stop";
                    break;

                case "Stop":
                    udpServer.StopAsServer();
                    tbCommand.Enabled = true;
                    this.btnServer.Text = "Start";
                    break;
            }
        }

        private void OnRecevieViewList(object sender, SoketEventArgs e)
        {
            dataTable = Fomatter.xmlDataToDataTable(e.Message);

            if (InvokeRequired)
            {
                this.Invoke(new Action(
                        delegate ()
                        {
                            dgvMachinelist.DataSource = dataTable;
                        }
                        )
                  );
            }
            else
            {
                dgvMachinelist.DataSource = dataTable;
            }
        }
    }
}
