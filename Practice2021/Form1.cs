using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Xml;


namespace Practice2021
{
    public partial class MainForm : Form
    {
        private DataSet graphDataSet = null;
        private DataTable graphTable = null;
        private string fileName = string.Empty;
        private bool isSaved = true;

        public MainForm()
        {
            InitializeComponent();
        }

        private void CreateDataSet()
        {
            graphTable = new DataTable("graphTable");
            DataColumn xColumn = new DataColumn("DataX", typeof(double));
            graphTable.Columns.Add(xColumn);
            DataColumn yColumn = new DataColumn("DataY", typeof(double));
            graphTable.Columns.Add(yColumn);
            dataGridViewTab.DataSource = graphTable;
            dataGridViewTab.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewTab.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewTab.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTab.RowHeadersWidth = 55;
            DataGridViewColumn gridColumnX = dataGridViewTab.Columns[0];
            gridColumnX.SortMode = DataGridViewColumnSortMode.NotSortable;
            gridColumnX.HeaderText = "x";
            DataGridViewColumn gridColumnY = dataGridViewTab.Columns[1];
            gridColumnY.SortMode = DataGridViewColumnSortMode.NotSortable;
            gridColumnY.HeaderText = "f(x)";
            gridColumnY.ReadOnly = true;
            // Создание DataSet
            graphDataSet = new DataSet("MyDataSet");
            graphDataSet.Tables.Add(graphTable);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CreateDataSet();
            chart1.ChartAreas[0].AxisX.Maximum = 5;
            chart1.ChartAreas[0].AxisX.Minimum = -5;
            chart1.ChartAreas[0].AxisY.Maximum = 10;
            chart1.ChartAreas[0].AxisY.Minimum = -25;
            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisY.Interval = 5;
            chart1.ChartAreas[0].AxisX.Crossing = 0;
            chart1.ChartAreas[0].AxisY.Crossing = 0;
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart1.Legends.Clear();

            chart1.Series[0].ChartType = SeriesChartType.Spline;
            float x = -3F;
            const int N = 1000;
            for (int i = 1; i < N; i++)
            {
                float y = (x * x * x) + 2 * (x * x) - 9 * x - 18;
                chart1.Series[0].Points.AddXY(x, y);
                x = x + 0.01F;

            }
        }
        private void dataGridViewTab_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int index = e.RowIndex;
            if (dataGridViewTab.Rows[index].IsNewRow)
            {
                return;
            }
            string rowIndexStr = (index + 1).ToString();
            object header = dataGridViewTab.Rows[index].HeaderCell.Value;
            if (header == null || !header.Equals(rowIndexStr))
                dataGridViewTab.Rows[index].HeaderCell.Value = rowIndexStr;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout aboutDialog = new FormAbout();
            aboutDialog.ShowDialog();
        }

        private void buttonCalc_Click(object sender, EventArgs e)
        {
            double x;
            double ans;
            for (int i = 0; i < graphTable.Rows.Count; i++)
            {
                if ((graphTable.Rows[i].RowState != DataRowState.Deleted) && !graphTable.Rows[i].IsNull(0))
                {
                    x = (double)graphTable.Rows[i][0];
                    ans = GetY.getY(x);
                    dataGridViewTab.Rows[i].Cells[1].Value = ans;

                    if ((x >= -3.0) && (x <= 3.0))
                    {
                        chart1.Series[1].Points.AddXY(x, ans);
                        chart1.Series[2].ChartType = SeriesChartType.StepLine;
                        chart1.Series[2].BorderDashStyle = ChartDashStyle.Dash;
                        chart1.Series[2].Color = Color.Black;
                        chart1.Series[2].BorderWidth = 1;
                        chart1.Series[2].Points.AddXY(x, 0);
                        chart1.Series[2].Points.AddXY(0, ans);
                        chart1.Series[2].Points.AddXY(x, ans);
                        chart1.Series[2].Points.AddXY(x, 0);
                    }
                }
            }
            isSaved = false;
        }

        private void numericUpDownAcc_ValueChanged(object sender, EventArgs e)
        {
            dataGridViewTab.Columns["DataY"].DefaultCellStyle.Format = "f" + numericUpDownAcc.Value;
        }
        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    chart1.SaveImage(ms, ChartImageFormat.Bmp);
                    Bitmap bm = new Bitmap(ms);
                    Clipboard.SetImage(bm);
                }
                MessageBox.Show("График скопирован", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void сохранитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfDialog = new SaveFileDialog();
                sfDialog.Filter = "Файлы BMP|*.bmp";
                DialogResult dr = sfDialog.ShowDialog();
                if (dr != DialogResult.OK)
                    return;
                chart1.SaveImage(sfDialog.FileName, ImageFormat.Bmp);
                MessageBox.Show("График сохранен", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileName = string.Empty;
            graphDataSet.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            isSaved = true;
        }
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fileName) || (sender == сохранитьКакToolStripMenuItem))
            {
                SaveFileDialog saveFDialog = new SaveFileDialog();
                saveFDialog.Filter = "XML-файлы|*.xml";
                if (saveFDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    graphDataSet.WriteXml(saveFDialog.FileName);
                    graphDataSet.AcceptChanges();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                isSaved = true;
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opDialog = new OpenFileDialog();
            opDialog.Filter = "XML-файл|*.xml";
            if (opDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            graphDataSet.Clear();
            вычислятьАвтоматическиToolStripMenuItem.Checked = false;
            try
            {
                graphDataSet.ReadXml(opDialog.FileName);
                graphDataSet.AcceptChanges();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось открыть файл ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            вычислятьАвтоматическиToolStripMenuItem.Checked = true;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void dataGridViewTab_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Что-то введено неверно, возможно, буква вместо цифры", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void всплывающиеПодсказкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolTipHelp.Active = всплывающиеПодсказкиToolStripMenuItem.Checked;
        }
        private void dataGridViewTab_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (вычислятьАвтоматическиToolStripMenuItem.Checked == true)
            {
                double x;
                double ans;
                for (int i = 0; i < graphTable.Rows.Count; i++)
                {
                    if ((graphTable.Rows[i].RowState != DataRowState.Deleted) && !graphTable.Rows[i].IsNull(0))
                    {
                        x = (double)graphTable.Rows[i][0];
                        ans = GetY.getY(x);
                        dataGridViewTab.Rows[i].Cells[1].Value = ans;

                        if ((x >= -3.0) && (x <= 3.0))
                        {
                            chart1.Series[1].Points.AddXY(x, ans);
                            chart1.Series[2].ChartType = SeriesChartType.StepLine;
                            chart1.Series[2].BorderDashStyle = ChartDashStyle.Dash;
                            chart1.Series[2].Color = Color.Black;
                            chart1.Series[2].BorderWidth = 1;
                            chart1.Series[2].Points.AddXY(x, 0);
                            chart1.Series[2].Points.AddXY(0, ans);
                            chart1.Series[2].Points.AddXY(x, ans);
                            chart1.Series[2].Points.AddXY(x, 0);
                        }
                    }
                }
            }
            if(graphDataSet.HasChanges())
                isSaved = false;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isSaved)
            {
                return;
            }
            DialogResult dr = MessageBox.Show("Есть несохраненные данные, продолжить?", "Данные могут быть потеряны", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (dr == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void dataGridViewTab_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            double x;
            double ans;
            for (int i = 0; i < graphTable.Rows.Count; i++)
            {
                if ((graphTable.Rows[i].RowState != DataRowState.Deleted) && !graphTable.Rows[i].IsNull(0))
                {
                    x = (double)graphTable.Rows[i][0];
                    ans = GetY.getY(x);
                    dataGridViewTab.Rows[i].Cells[1].Value = ans;

                    if ((x >= -3.0) && (x <= 3.0))
                    {
                        chart1.Series[1].Points.AddXY(x, ans);
                        chart1.Series[2].ChartType = SeriesChartType.StepLine;
                        chart1.Series[2].BorderDashStyle = ChartDashStyle.Dash;
                        chart1.Series[2].Color = Color.Black;
                        chart1.Series[2].BorderWidth = 1;
                        chart1.Series[2].Points.AddXY(x, 0);
                        chart1.Series[2].Points.AddXY(0, ans);
                        chart1.Series[2].Points.AddXY(x, ans);
                        chart1.Series[2].Points.AddXY(x, 0);
                    }
                }
            }
            isSaved = false;
        }
    }
}
