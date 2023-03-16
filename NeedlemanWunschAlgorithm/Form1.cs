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

        private int CalculateMatchState(int i, int j)
        {
            int matchState = 0;
            int match = Convert.ToInt32(textBox1.Text);
            int mismatch = Convert.ToInt32(textBox2.Text);
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

        private void button1_Click(object sender, EventArgs e)
        {
            label5.Text = "Seq1:  ";
            label6.Text = "Seq2:  ";
            label7.Text = "Score:  ";
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                // dosya islemleri
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

                // tablo boyutu
                int tableColumnLength = seq1Length + 2;
                int tableRowLength = seq2Length + 2;
                dataGridView1.ColumnCount = tableColumnLength;
                dataGridView1.RowCount = tableRowLength;

                // satirlarin yuksekligini tabloya sigdirma
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


                // hesaplama islemlerinin yapilmasi

                // match = +1
                // mismatch = -1;
                // gap = -2
                //
                // T[i,j] = max(   (T[i-1, j-1] + (match || mismatch) )  ||
                //                 (T[i-1, j] + gap) ||
                //                 (T[i, j-1] + gap)

                dataGridView1.Rows[1].Cells[1].Value = 0;
                // ilk satirin hucrelerinin doldurulmasi
                int gapPenalty = Convert.ToInt32(textBox3.Text); ;
                for (int i = 2; i < tableColumnLength; i++)
                {
                    // ikinci formul
                    int cellValue = Convert.ToInt32(dataGridView1.Rows[1].Cells[i - 1].Value);
                    dataGridView1.Rows[1].Cells[i].Value = cellValue + gapPenalty;
                }
                // ilk kolonun hucrelerinin doldurulmasi
                for (int j = 2; j < tableRowLength; j++)
                {
                    // ucuncu formul
                    int cellValue = Convert.ToInt32(dataGridView1.Rows[j - 1].Cells[1].Value);
                    dataGridView1.Rows[j].Cells[1].Value = cellValue + gapPenalty;
                }
                // diger hucrelerin doldurulmasi
                for (int j = 1; j < tableRowLength; j++)
                {
                    for (int i = 1; i < tableColumnLength; i++)
                    {
                        if (i != 1 && j != 1) // ilk satýr ve sutun zaten dolduruldu
                        {
                            int[] results = new int[3];
                            // ilk formul
                            // i == 2 ve j == 2 olduðunda bende tablo bazlari icerdigi icin T[1,1] ama bazlarý içermeyen tabloya göre T[0,0]. almak istedigim bu deðer ise 0 'dýr.
                            int cellValueOfCross = Convert.ToInt32(dataGridView1.Rows[j - 1].Cells[i - 1].Value);
                            int matchState = CalculateMatchState(i, j);
                            results[0] = cellValueOfCross + matchState;
                            // ikinci formul
                            int cellValueOfLeft = Convert.ToInt32(dataGridView1.Rows[j].Cells[i - 1].Value);
                            results[1] = cellValueOfLeft + gapPenalty;
                            // ucuncu formul
                            int cellValueOfTop = Convert.ToInt32(dataGridView1.Rows[j - 1].Cells[i].Value);
                            results[2] = cellValueOfTop + gapPenalty;

                            // max degeri bulma
                            int max = results[0];
                            for (int k = 0; k < results.Length; k++)
                            {
                                if (max < results[k])
                                {
                                    max = results[k];
                                }
                            }

                            // sonucu tabloya isleme
                            dataGridView1.Rows[j].Cells[i].Value = max;
                        }
                    }
                }

                // izlenecek yolun belirlenip indexlerin tutulmasi
                int columnIndex = tableColumnLength - 1;
                int rowIndex = tableRowLength - 1;
                List<string> indexesOfMatchPath = new List<string>();
                string pathIndex = columnIndex + rowIndex.ToString();
                indexesOfMatchPath.Add(pathIndex);
                while (true)
                {
                    // eslesiyorsa capraza git eslesmiyorsa maximumu bul
                    if ((columnIndex != 1 && rowIndex != 1) && CalculateMatchState(columnIndex, rowIndex) == 1)
                    {
                        columnIndex = columnIndex - 1;
                        rowIndex = rowIndex - 1;
                        pathIndex = columnIndex + rowIndex.ToString();
                        indexesOfMatchPath.Add(pathIndex);
                    }
                    else
                    {
                        // int bir deger mi karsilastir eðer int ise degeri guncelle deðilse minimum deðeri alsýn. Cunku en sol ve en ust taraflardan null deðer gelecek.
                        int cellTopValue = -999;
                        if (rowIndex - 1 != 0)
                        {
                            cellTopValue = Convert.ToInt32(dataGridView1.Rows[rowIndex - 1].Cells[columnIndex].Value);
                        }
                        int cellLeftValue = -999;
                        if (columnIndex - 1 != 0)
                        {
                            cellLeftValue = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells[columnIndex - 1].Value);
                        }
                        int cellCrossValue = -999;
                        if (rowIndex - 1 != 0 && columnIndex - 1 != 0)
                        {
                            cellCrossValue = Convert.ToInt32(dataGridView1.Rows[rowIndex - 1].Cells[columnIndex - 1].Value);
                        }

                        // bulunan degeleri karsilastir maximumu bul
                        if (cellCrossValue >= cellLeftValue && cellCrossValue >= cellTopValue) // maximum deger caprazda
                        {
                            columnIndex = columnIndex - 1;
                            rowIndex = rowIndex - 1;
                            pathIndex = columnIndex + rowIndex.ToString();
                            indexesOfMatchPath.Add(pathIndex);
                        }
                        else if (cellLeftValue >= cellCrossValue && cellLeftValue >= cellTopValue) // maximum solda
                        {
                            columnIndex = columnIndex - 1;
                            rowIndex = rowIndex;
                            pathIndex = columnIndex + rowIndex.ToString();
                            indexesOfMatchPath.Add(pathIndex);
                        }
                        else  // maximum yukarda
                        {
                            columnIndex = columnIndex;
                            rowIndex = rowIndex - 1;
                            pathIndex = columnIndex + rowIndex.ToString();
                            indexesOfMatchPath.Add(pathIndex);
                        }
                    }
                    if (columnIndex == 1 && rowIndex == 1)
                    {
                        break;
                    }
                }

                // yollarýn indexleri alinip yeni eslestirmenin yapilmasi 
                indexesOfMatchPath.Reverse();
                string newSeq1 = string.Empty;
                string newSeq2 = string.Empty;
                int tempIndexOfSeq1 = 0;
                int tempIndexOfSeq2 = 0;
                for (int i = 0; i < indexesOfMatchPath.Count(); i++)
                {
                    if (indexesOfMatchPath[i] != "11")
                    {
                        // baz indexini int'e donustur
                        string index = indexesOfMatchPath[i];
                        int seq1BaseIndex = Convert.ToInt32(index[0].ToString());
                        int seq2BaseIndex = Convert.ToInt32(index[1].ToString());

                        string baseOfSeq1 = string.Empty;
                        string baseOfSeq2 = string.Empty;
                        if (dataGridView1.Rows[0].Cells[seq1BaseIndex].Value != null)
                        {
                            baseOfSeq1 = dataGridView1.Rows[0].Cells[seq1BaseIndex].Value.ToString();
                        }
                        if (dataGridView1.Rows[seq2BaseIndex].Cells[0].Value != null)
                        {
                            baseOfSeq2 = dataGridView1.Rows[seq2BaseIndex].Cells[0].Value.ToString();
                        }

                        // eger tekrar eden index varsa tekrar kullanma ve boþ ise - koy deðilse degeri ata
                        if (tempIndexOfSeq1 == seq1BaseIndex || baseOfSeq1 == "")
                        {
                            newSeq1 += "-";
                        }
                        else
                        {
                            newSeq1 += baseOfSeq1;
                        }

                        if (tempIndexOfSeq2 == seq2BaseIndex || baseOfSeq2 == "")
                        {
                            newSeq2 += "-";
                        }
                        else
                        {
                            newSeq2 += baseOfSeq2;
                        }

                        tempIndexOfSeq1 = seq1BaseIndex;
                        tempIndexOfSeq2 = seq2BaseIndex;
                    }
                }
                label5.Text += newSeq1;
                label6.Text += newSeq2;

                // score hesaplanmasý
                int match = Convert.ToInt32(textBox1.Text);
                int mismatch = Convert.ToInt32(textBox2.Text);
                int gapPoint = Convert.ToInt32(textBox3.Text);
                int score = 0;
                for (int i = 0; i < newSeq1.Length; i++)
                {
                    if (newSeq1[i].Equals(newSeq2[i]))
                    {
                        score += match;
                    }
                    else if (newSeq1[i] == '-' || newSeq2[i] == '-')
                    {
                        score += gapPoint;
                    }
                    else
                    {
                        score += mismatch;
                    }
                }
                label7.Text += score.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hatayla karþýlaþýldý...\n{ex}");
            }

            timer.Stop();
            label8.Text = "Geçen Süre: " + timer.Elapsed.ToString() + "  ------>  " + timer.ElapsedMilliseconds.ToString() + " milisaniye";
        }
    }
}