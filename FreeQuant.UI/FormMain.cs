using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace FreeQuant.UI {
    public partial class FormMain : Form
    {
        MainPresenter mPresenter;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            mPresenter = new MainPresenter(this);
            showDefault();
        }

        public void showStrategy(DataTable dt)
        {
            dataGridViewStrategy.DataSource = dt;
        }

        public void showPosition(DataTable dt)
        {
            //绑定
            dataGridViewPosition.DataSource = dt;
            if (dataGridViewPosition.Rows.Count > 0)
            {
                //前两列不显示
                dataGridViewPosition.Columns["id"].Visible = false;
                dataGridViewPosition.Columns["strategy_name"].Visible = false;
                //调整时间格式
                dataGridViewPosition.Columns["last_time"].DefaultCellStyle.Format = "yyyy-MM-dd hh:mm:ss";
            }
        }

        public void showOrder(DataTable dt)
        {
            //分表
            DataTable[] dts = new DataTable[dt.Rows.Count / 100 + 1];
            for (int i = 0; i < dts.Length; i++)
            {
                dts[i] = dt.Clone();
                for (int j = 0; j < 100; j++)
                {
                    if (i * 100 + j < dt.Rows.Count)
                    {
                        DataRow r = dt.Rows[i * 100 + j];
                        dts[i].ImportRow(r);
                    }
                }
            }

            //绑定第一页
            dataGridViewOrder.DataSource = dts[0];
            if(dataGridViewOrder.Rows.Count > 0)
            {
                //前两列不显示
                dataGridViewOrder.Columns["id"].Visible = false;
                dataGridViewOrder.Columns["strategy_name"].Visible = false;
                //调整时间格式
                dataGridViewOrder.Columns["order_time"].DefaultCellStyle.Format = "yyyy-MM-dd hh:mm:ss";
            }

            //翻页
            int page = 0;
            labelPage.Text = $"共{dts.Length}页，第{page+1}页";
            buttonPageBefore.Click += (object sender, EventArgs e) => {
                page = page == 0 ? 0 : page - 1;
                labelPage.Text = $"共{dts.Length}页，第{page+1}页";
                dataGridViewOrder.DataSource = dts[page];
            };
            buttonPageAfter.Click += (object sender, EventArgs e) => {
                page = page == dts.Length - 1 ? dts.Length - 1 : page + 1;
                labelPage.Text = $"共{dts.Length}页，第{page+1}页";
                dataGridViewOrder.DataSource = dts[page];
            };
        }

        private void dataGridViewStrategy_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            string strategyName = (string)dataGridViewStrategy.Rows[e.RowIndex].Cells["strategy_name"].Value;
            mPresenter.loadPosition(strategyName);
            mPresenter.loadOrder(strategyName);
        }

        private void dataGridViewStrategy_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    //若行已是选中状态就不再进行设置
                    if (dataGridViewStrategy.Rows[e.RowIndex].Selected == false)
                    {
                        dataGridViewStrategy.ClearSelection();
                        dataGridViewStrategy.Rows[e.RowIndex].Selected = true;
                    }
                    //只选中一行时设置活动单元格
                    if (dataGridViewStrategy.SelectedRows.Count == 1)
                    {
                        dataGridViewStrategy.CurrentCell = dataGridViewStrategy.Rows[e.RowIndex].Cells[0];
                    }
                    //弹出操作菜单
                    contextMenuStripStrategy.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void dataGridViewPosition_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    //若行已是选中状态就不再进行设置
                    if (dataGridViewPosition.Rows[e.RowIndex].Selected == false)
                    {
                        dataGridViewPosition.ClearSelection();
                        dataGridViewPosition.Rows[e.RowIndex].Selected = true;
                    }
                    //只选中一行时设置活动单元格
                    if (dataGridViewPosition.SelectedRows.Count == 1)
                    {
                        dataGridViewPosition.CurrentCell = dataGridViewPosition.Rows[e.RowIndex].Cells[2];
                    }
                    //弹出操作菜单
                    contextMenuStripPosition.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            string strategyName = (string)dataGridViewStrategy.SelectedRows[0].Cells["strategy_name"].Value;
            if (MessageBox.Show($"删除策略{strategyName}！", "删除持仓？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                mPresenter.deleteStrategy(strategyName);
            }
            //
            showDefault();
        }

        private void toolStripMenuItemSet_Click(object sender, EventArgs e)
        {
            string strategyName = (string)dataGridViewStrategy.SelectedRows[0].Cells["strategy_name"].Value;
            string instrumentID = (string)dataGridViewPosition.SelectedRows[0].Cells["instrument_id"].Value;
            string s = Interaction.InputBox($"更改策略{strategyName}，{instrumentID}的持仓", "更改持仓", "", -1, -1);
            try
            {
                int position = Convert.ToInt32(s);
                mPresenter.setPostion(strategyName, instrumentID, position);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

        }

        private void showDefault()
        {
            if (dataGridViewStrategy.Rows.Count > 0)
            {
                string name = (string)dataGridViewStrategy.Rows[0].Cells["strategy_name"].Value;
                mPresenter.loadPosition(name);
                mPresenter.loadOrder(name);
            }
        }

        private void buttonStartStrategy_Click(object sender, EventArgs e)
        {
            Process.Start("MobileQuant.exe");
            Close();
        }
    }
}
