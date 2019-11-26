using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartFactory;

namespace SMFA_UI
{
    public partial class Form1 : Form
    {
        private SmartFactory.Udp_Client udpClient;
        private SmartFactory.Udp_Server udpServer;
        DataTable dataTable;

        public Form1()
        {
            InitializeComponent();

            this.tbService.Text = "UDP";
            this.tbIP.Text = "127.0.0.1";
            this.tbPort.Text = "5002";

        }

        private void TbCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                byte[] resvReponse;

                if (tbService.Text.Equals("TCP"))
                {
                    TcpConnect tcpCon = new TcpConnect();
                    resvReponse = null;//tcpCon.SendRequest_Message(tbIP.Text, tbPort.Text, tbCommand.Text);
                }
                else
                {

                    Udp_Client udpClient = new Udp_Client();
                    udpClient.StartAsClient(tbIP.Text, tbPort.Text);
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
                                resvReponse = null; //tcpCon.SendRequest_Message(tbIP.Text, tbPort.Text, "view machinelist");
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

                        if (dataTable.Rows.Count > 0) dgvMachineList.DataSource = null;

                        dgvMachineList.DataSource = dataTable;

                        //SetDataGeidView(dataTable);
                    }

                    tbCommand.Text = "";

                }
            }
        }
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "data load";
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "data save";
        }

        private void MachinestateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "set machinestate";
        }

        private void AddMachineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "set machine";
        }

        private void MachineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "view machine";
        }

        private void MachineListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbCommand.Text = "view machinelist";

            KeyEventArgs key = new KeyEventArgs(Keys.Enter);
            TbCommand_KeyDown(this, key);
        }



#region  Grid view 설정
        private void GridStyleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dgvMachineList.DataSource;
            dgvMachineList.DataSource = new DataTable();

            DataGridViewHeaderSetting();

            dgvMachineList.DataSource = dt;
        }

        private void DataGridViewHeaderSetting()
        {
            dgvMachineList.AutoGenerateColumns = false;


                DataGridViewTextBoxColumn Col_01 = new DataGridViewTextBoxColumn();
                Col_01.DataPropertyName = "MachineName";
                Col_01.HeaderText = "MachineName";

                DataGridViewTextBoxColumn Col_02 = new DataGridViewTextBoxColumn();
                Col_02.DataPropertyName = "StateName";
                Col_02.HeaderText = "StateName";
                
                /*
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
                dgvMachineList.Columns.Add(Col_01);
                dgvMachineList.Columns.Add(Col_02);
                /*
                dgvMachineList.Columns.Add(Col_03);
                dgvMachineList.Columns.Add(Col_04);
                dgvMachineList.Columns.Add(Col_05);
                dgvMachineList.Columns.Add(Col_06);
                /*
                dataGridView.Columns.Add(Col_07);
                dataGridView.Columns.Add(Col_08);
                dataGridView.Columns.Add(Col_09);
                dataGridView.Columns.Add(Col_10);
                */

                foreach (DataGridViewColumn col in dgvMachineList.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                  //col.HeaderCell.Style.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                    col.HeaderCell.Style.Font = new Font("맑은 고딕", 12F, GraphicsUnit.Pixel);
                    col.HeaderCell.Style.ForeColor = Color.Black;
                    
                  //Set the column size automatically
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                  //Set the header column BackColor
                    dgvMachineList.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
                    dgvMachineList.EnableHeadersVisualStyles = false; 
        }


        //private void dbView_CellFormatting(object sender, EventArgs e)
        //{
        //    if (e.ColumnIndex == 1)
        //    {
        //        if (e.Value != null)
        //        {
        //            string text = e.Value.ToString();
        //            if (text.Contains("Wait"))
        //            {
        //                e.CellStyle.ForeColor = Color.Yellow;
        //                e.CellStyle.SelectionForeColor = Color.Yellow;
        //            }
        //        }
        //    }
        //}
        #endregion

        private void OnRecevieViewList(object sender, SoketEventArgs e)
        {
            dataTable = Fomatter.xmlDataToDataTable(e.Message);

            if (InvokeRequired)
            {
                this.Invoke(new Action(
                        delegate ()
                        {
                            dgvMachineList.DataSource = dataTable;

                            foreach (DataGridViewRow row in dgvMachineList.Rows)
                            {
                                try
                                {
                                    if (row.Cells[2].Value.Equals("WAIT"))
                                    {
                                        row.DefaultCellStyle.BackColor = Color.Red;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    break;
                                }
                            }
                        }
                        )
                  );
            }
            else
            {
                dgvMachineList.DataSource = dataTable;
            }
        }
        private void BtnServer_Click(object sender, EventArgs e)
        {
            switch (this.btnServer.Text)
            {
                case "Start":
                    udpServer = new Udp_Server();
                    udpServer.StartAsServer(tbIP.Text, tbPort.Text);
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
    }
}