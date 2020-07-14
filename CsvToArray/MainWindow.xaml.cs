using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CsvToArray
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            const string filePath = @"C:\Users\Touseef\Downloads\deep-learning-v2-pytorch-master\deep-learning-v2-pytorch-master\convolutional-neural-networks\mnist-mlp\RHA 5_1100 Gap=0.1 DistanceClip=2000.csv";
            GenerateCsvWithDash(filePath);
            //GenerateArray(filePath);
        }

        private static void GenerateArray(string filePath)
        {
            string[] csvlines = File.ReadAllLines(filePath);
            var query = from csvline in csvlines
                        let data = csvline.Split(',')
                        select new
                        {
                            Range = data[0],
                            Height = data[1],
                            Angle = data[2],
                        };
            int count = 0;
            StringBuilder stringBuilder = new StringBuilder();
            string arrayName = "A";
            int previousRange = 0;
            int previousHeight = 0;
            int previousAngle = 0;
            int angle = 0;
            int range = 0;
            int height = 0;
            foreach (var value in query)
            {
                if (count++ == 0) continue;
                angle = Convert.ToInt16(Convert.ToDouble(value.Angle) * 10);
                range = Convert.ToInt16(value.Range);
                height = Convert.ToInt16(value.Height);
                if (angle == previousAngle)
                {
                    if (range - previousRange > 1)
                    {
                        for (int index = previousRange + 1; index < range; index++)
                        {
                            stringBuilder.Append(arrayName).Append('[').Append(index).Append("][").Append(height).Append("]=").Append(angle).AppendLine(";");
                        }
                    }
                    if (height - previousHeight > 1)
                    {
                        for (int index = previousHeight + 1; index < height; index++)
                        {
                            stringBuilder.Append(arrayName).Append('[').Append(range).Append("][").Append(index).Append("]=").Append(angle).AppendLine(";");
                        }
                    }
                }
                stringBuilder.Append(arrayName).Append('[').Append(range).Append("][").Append(height).Append("]=").Append(angle).AppendLine(";");
                previousRange = Convert.ToInt16(value.Range);
                previousHeight = Convert.ToInt16(value.Height);
                previousAngle = angle;
            }
            File.WriteAllText("arrayName.c", stringBuilder.ToString());
        }

        private void GenerateCsvWithDash(string filePath)
        {
            string[] csvlines = File.ReadAllLines(filePath);
            var query = from csvline in csvlines
                        let data = csvline.Split(',')
                        select new
                        {
                            Range = data[0],
                            Height = data[1],
                            Angle = data[2],
                        };
            int count = 0;
            StringBuilder stringBuilder = new StringBuilder();
            int previousRange = 0;
            int previousHeight = 0;
            int previousAngle = 0;
            int angle = 0;
            int range = 0;
            int height = 0;
            int maxRange = 0;
            int maxHeight = 0;
            File.WriteAllText("csvDash.txt", "");
            foreach (var value in query)
            {
                if (count++ == 0) continue;
                angle = Convert.ToInt16(Convert.ToDouble(value.Angle) * 10);
                range = Convert.ToInt16(value.Range);
                height = Convert.ToInt16(value.Height);
                if (range > maxRange) maxRange = range;
                if (height > maxHeight) maxHeight = height;
                if (angle == previousAngle)
                {
                    if (range - previousRange > 1)
                    {
                        for (int index = previousRange + 1; index < range; index++)
                        {
                            stringBuilder.Append(index).Append(',').Append(height).Append('-').Append(angle).AppendLine();
                        }
                    }
                    if (height - previousHeight > 1)
                    {
                        for (int index = previousHeight + 1; index < height; index++)
                        {
                            stringBuilder.Append(range).Append(',').Append(index).Append('-').Append(angle).AppendLine();
                        }
                    }
                }
                stringBuilder.Append(range).Append(',').Append(height).Append('-').Append(angle).AppendLine();
                previousRange = Convert.ToInt16(value.Range);
                previousHeight = Convert.ToInt16(value.Height);
                previousAngle = angle;
                if (count % 500000 == 0)
                {
                    File.AppendAllText("csvDash.txt", stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }
            File.AppendAllText("csvDash.txt", stringBuilder.ToString());
        }
    }
}