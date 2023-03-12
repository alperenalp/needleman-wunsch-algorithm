using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NeedlemanWunschAlgorithm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                int seq1Length = 0;
                string seq1Text = string.Empty;
                int seq2Length = 0;
                string seq2Text = string.Empty;
                for (int i = 1; i <= 2; i++)
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
                    string path = @$"{currentDirectory}\seq{i}.txt";
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    if (i == 1)
                    {
                        seq1Length = Convert.ToInt32(sr.ReadLine());
                        seq1Text = sr.ReadLine();
                        sr.Close();
                    }
                    else
                    {
                        seq2Length = Convert.ToInt32(sr.ReadLine());
                        seq2Text = sr.ReadLine();
                        sr.Close();
                    }

                }

                int tableColumnLength = seq1Length + 2;
                int tableRowLength = seq2Length + 2;
                dataGridView1.ColumnCount = tableColumnLength;
                dataGridView1.RowCount = tableRowLength;

                // satýrlarýn yüksekliðini tabloya sýðdýrma
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Height = Convert.ToInt32((dataGridView1.ClientRectangle.Height - dataGridView1.ColumnHeadersHeight) / (double)(dataGridView1.Rows.Count - 0.3));
                }


                for (int i = 0; i < seq1Length; i++)
                {
                    dataGridView1.Rows[0].Cells[i + 2].Value = seq1Text[i];
                }

                for (int i = 0; i < seq2Length; i++)
                {
                    dataGridView1.Rows[i + 2].Cells[0].Value = seq2Text[i];
                }


                // hesaplama iþlemleri yapýlmasý
                //
                // match = +1
                // mismatch = -1;
                // gap = -2
                //
                // T[i,j] = max(   (T[i-1, j-1] + (match || mismatch) )  ||
                //                 (T[i-1, j] + gap) ||
                //                 (T[i, j-1] + gap)


                dataGridView1.Rows[1].Cells[1].Value = 0;
                // ilk satýrýn hücrelerinin doldurulmasý
                int gap = -2;
                for (int i = 2; i < tableColumnLength; i++)
                {
                    // ikinci formul
                    int cellValue = Convert.ToInt32(dataGridView1.Rows[1].Cells[i - 1].Value);
                    dataGridView1.Rows[1].Cells[i].Value = cellValue + gap;
                }
                // ilk kolonun hücrelerinin doldurulmasý
                for (int j = 2; j < tableRowLength; j++)
                {
                    // üçüncü formul
                    int cellValue = Convert.ToInt32(dataGridView1.Rows[j - 1].Cells[1].Value);
                    dataGridView1.Rows[j].Cells[1].Value = cellValue + gap;
                }
                // diðer hücrelerin doldurulmasý
                for (int j = 1; j < tableRowLength; j++)
                {
                    for (int i = 1; i < tableColumnLength; i++)
                    {
                        if (i != 1 && j != 1) // ilk satýr ve sütun zaten dolduruldu
                        {
                            int[] results = new int[3];
                            // ilk formul
                            // i == 2 ve j == 2 olduðunda bende tablo bazlarý içerdiði için T[1,1] ama bazlarý içermeyen tabloya göre T[0,0]. almak istediðim bu deðer ise 0 'dýr.
                            int cellValueOfCross = Convert.ToInt32(dataGridView1.Rows[j - 1].Cells[i - 1].Value);
                            int matchState = CalculateMatchState(i, j);
                            results[0] = cellValueOfCross + matchState;
                            // ikinci formul
                            int cellValueOfLeft = Convert.ToInt32(dataGridView1.Rows[j].Cells[i - 1].Value);
                            results[1] = cellValueOfLeft + gap;
                            // üçüncü formul
                            int cellValueOfTop = Convert.ToInt32(dataGridView1.Rows[j - 1].Cells[i].Value);
                            results[2] = cellValueOfTop + gap;

                            // max deðeri bulma
                            int max = results[0];
                            for (int k = 0; k < results.Length; k++)
                            {
                                if (max < results[k])
                                {
                                    max = results[k];
                                }
                            }

                            // sonucu tabloya iþleme
                            dataGridView1.Rows[j].Cells[i].Value = max;
                        }
                    }
                }

                //MessageBox.Show("Geçen Zaman: " + timer.Elapsed.ToString() + "\nYani: " + timer.ElapsedMilliseconds.ToString() + " milisaniye");
                timer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hatayla karþýlaþýldý...\n{ex}");
            }

        }

        private int CalculateMatchState(int i, int j)
        {
            int matchState = 0;
            int match = 1;
            int mismatch = -1;
            if (dataGridView1.Rows[0].Cells[i].Value.Equals(dataGridView1.Rows[j].Cells[0].Value))
            {
                matchState = match;
            }
            else
            {
                matchState = mismatch;
            }
            return matchState;
        }

    }
}