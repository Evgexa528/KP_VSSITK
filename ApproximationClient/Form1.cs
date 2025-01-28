using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ApproximationClient
{
    public partial class Form1 : Form
    {
        ServiceApproximation.ServiceApproximationClient client;

        double[] paramA = null;
        double[] paramB = null;
        double[] paramC = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new ServiceApproximation.ServiceApproximationClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
               
                try
                {
                    dialog.Filter = "Текстовые файлы|*.txt";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {

                        var file = new StreamReader(dialog.FileName);
                        String strAize = file.ReadLine();
                        int size = int.Parse(strAize);
                        paramA = new double[size];
                        paramB = new double[size];
                        paramC = new double[size];
                        dataGridView1.Rows.Clear();
                        foreach (var series in chart1.Series)
                        {
                            series.Points.Clear();
                        }
                        foreach (var series in chart2.Series)
                        {
                            series.Points.Clear();
                        }
                        for (int i = 0; i < size; i++)
                        {
                            String strBufer = file.ReadLine();
                            double bufer = double.Parse(strBufer);
                            paramA[i] = bufer;
                        }
                        for (int i = 0; i < size; i++)
                        {
                            String strBufer = file.ReadLine();
                            double bufer = double.Parse(strBufer);
                            paramB[i] = bufer;
                        }
                        for (int i = 0; i < size; i++)
                        {
                            String strBufer = file.ReadLine();
                            double bufer = double.Parse(strBufer);
                            paramC[i] = bufer;
                        }

                        for (int i = 0; i < size; i++)
                        {
                            dataGridView1.Rows.Add(paramA[i], paramB[i], paramC[i]);
                        }
                        file.Close();
                    }
                }catch (System.FormatException)
                {
                    MessageBox.Show("В файле данные представлены в неправильном формате!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool validation = false;
            do
            {
                try
                {
                    for (int i = 0; i < paramA.Length; i++)
                    {
                        paramA[i] = double.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                        paramB[i] = double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());
                        paramC[i] = double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                        if (paramA[i] < 0)
                        {
                            validation = true;
                            break;
                        }
                        if (paramB[i] < -273)
                        {
                            validation = true;
                            break;
                        }
                    }
                    foreach (var series in chart1.Series)
                    {
                        series.Points.Clear();
                    }
                    foreach (var series in chart2.Series)
                    {
                        series.Points.Clear();
                    }

                    if (validation)
                    {
                        MessageBox.Show("Расход не может быть отрицательным, " +
                                            "отредактируйте данные и попробуйте снова!",
                                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }
                    Axis ax = new Axis();
                    ax.Title = "Расход, кг/сек";
                    chart1.ChartAreas[0].AxisX = ax;
                    Axis ay = new Axis();
                    ay.Title = "Температура, С";
                    chart1.ChartAreas[0].AxisY = ay;

                    Axis ax2 = new Axis();
                    ax2.Title = "Расход, кг/сек";
                    chart2.ChartAreas[0].AxisX = ax2;
                    Axis ay2 = new Axis();
                    ay2.Title = "Отклонение уровня, мм";
                    chart2.ChartAreas[0].AxisY = ay2;
                    this.chart1.Series[0].Points.Clear();
                    var (xResult, yResult) = client.Approximate(paramA, paramB);
                    for (int i = 0; i < paramA.Length; i++)
                    {
                        this.chart1.Series[0].Points.AddXY(paramA[i], paramB[i]);
                    }
                    for (int i = 0; i < xResult.Length; i++)
                    {
                        this.chart1.Series[1].Points.AddXY(xResult[i], yResult[i]);
                    }
                    (xResult, yResult) = client.Approximate(paramA, paramC);
                    for (int i = 0; i < paramA.Length; i++)
                    {
                        this.chart2.Series[0].Points.AddXY(paramA[i], paramC[i]);
                    }
                    for (int i = 0; i < xResult.Length; i++)
                    {
                        this.chart2.Series[1].Points.AddXY(xResult[i], yResult[i]);
                    }
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("Вы не загрузили данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (System.FormatException)
                {
                    MessageBox.Show("Не верный формат данных!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (System.ServiceModel.EndpointNotFoundException)
                {
                    MessageBox.Show("Не удалось подключится к серверу!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                break;
            } while (true);
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Аппроксимация (от лат. proxima — ближайшая) — научный метод, " +
                            "состоящий в замене одних объектов другими, в каком-то смысле " +
                            "близкими к исходным, но более простыми. Аппроксимация позволяет " +
                            "исследовать числовые характеристики и качественные свойства объекта, " +
                            "сводя задачу к изучению более простых или более удобных объектов " +
                            "(например, таких, характеристики которых легко вычисляются или " +
                            "свойства которых уже известны).", "Сведенье", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}