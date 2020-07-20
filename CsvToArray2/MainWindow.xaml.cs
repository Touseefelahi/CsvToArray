using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

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
            const string filePath = @"C:\Users\STIRA\source\repos\ReticleGenerator2.0\Presentation\ReticleGenerator\bin\Debug\netcoreapp3.1\RHA -200_1100 Gap=0.1 DistanceClip=2500.csv";
            const string filePath2 = @"C:\Users\STIRA\source\repos\ReticleGenerator2.0\Presentation\ReticleGenerator\bin\Debug\netcoreapp3.1\RHA -200_1100 Gap=0.1 DistanceClip=2500.csv";
            //GenerateCsvWithDashZeroLevel(filePath);
            GenerateCsvWithDashNegative500(filePath2);
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
                height = Convert.ToInt16((Convert.ToDouble(value.Height) + 500) * 10);
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
                if (count % 1000 == 0)
                {
                    File.AppendAllText("csvDash.txt", stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }
            File.AppendAllText("csvDash.txt", stringBuilder.ToString());
        }

        private void GenerateCsvWithDash2(string filePath)
        {
            var data = new short[2, 2501, 2501];
            int count = 0;
            StringBuilder stringBuilder = new StringBuilder();
            int previousRange = 0;
            int previousHeight = 0;
            int previousAngle = 0;
            int angle = 0;
            int range = 0;
            int height = 0;
            int flightTime = 0;
            int maxRange = 0;
            int maxHeight = 0;
            File.WriteAllText("csvDashHash.txt", "");
            var line = string.Empty;
            var fileStream = File.OpenText(filePath);
            while ((line = fileStream.ReadLine()) != null)
            {
                var value = line.Split(',');
                if (count++ == 0) continue;
                range = Convert.ToInt16(value[0]);
                height = Convert.ToInt16(Convert.ToDouble(value[1]) * 10) + 500;
                angle = Convert.ToInt16(Convert.ToDouble(value[2]) * 10);
                flightTime = Convert.ToInt16(value[3]);
                if (range > maxRange) maxRange = range;
                if (height > maxHeight) maxHeight = height;
                if (angle == previousAngle)
                {
                    if (range - previousRange > 1)
                    {
                        for (int index = previousRange + 1; index < range; index++)
                        {
                            stringBuilder.Append(index).Append(',').Append(height).Append(':').Append(angle).Append('#').Append(flightTime).AppendLine();
                        }
                    }
                    if (Math.Abs(height - previousHeight) > 1)
                    {
                        for (int index = previousHeight + 1; index < height; index++)
                        {
                            stringBuilder.Append(range).Append(',').Append(index).Append(':').Append(angle).Append('#').Append(flightTime).AppendLine();
                        }
                    }
                }
                stringBuilder.Append(range).Append(',').Append(height).Append(':').Append(angle).Append('#').Append(flightTime).AppendLine();
                previousRange = range;
                previousHeight = height;
                previousAngle = angle;
                if (count % 100000 == 0)
                {
                    File.AppendAllText("csvDashHash.txt", stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }
            File.AppendAllText("csvDashHash.txt", stringBuilder.ToString());
        }
        short[,] angleData, angleDataFilled, flightTimeData;
        private void GenerateCsvWithDashZeroLevel(string filePath)
        {
            int rangeLength = 2501;
            int heightLength = 30001;
            angleData = new Int16[rangeLength, heightLength];
            angleDataFilled = new Int16[rangeLength, heightLength];
            flightTimeData = new Int16[rangeLength, heightLength];

            int count = 0;

            int previousRange = 0;
            int previousHeight = 0;
            int previousAngle = 0;
            Int16 angle = 0;
            int range = 0;
            int height = 0;
            Int16 flightTime = 0;
            int maxRange = 0;
            int maxHeight = 0;

            var line = string.Empty;
            var fileStream = File.OpenText(filePath);
            while ((line = fileStream.ReadLine()) != null)
            {
                var value = line.Split(',');
                if (count++ == 0) continue;
                range = Convert.ToInt16(value[0]);
                height = Convert.ToInt16(Convert.ToDouble(value[1]) * 10);
                angle = Convert.ToInt16(Convert.ToDouble(value[2]) * 10);
                flightTime = Convert.ToInt16(value[3]);
                if (range > maxRange) maxRange = range;
                if (height > maxHeight) maxHeight = height;
                if (height < 0 || range < 0) continue;
                //if (angle == previousAngle)
                //{
                //    if (range - previousRange > 1)
                //    {
                //        for (int index = previousRange + 1; index < range; index++)
                //        {
                //            if (index < rangeLength)
                //            {
                //                flightTimeData[index, height] = flightTime;
                //                angleData[index, height] = angle;
                //            }
                //        }
                //    }
                //    if (Math.Abs(height - previousHeight) > 1)
                //    {
                //        for (int index = previousHeight + 1; index < height; index++)
                //        {
                //            if (index < heightLength)
                //            {
                //                angleData[range, index] = angle;
                //                flightTimeData[range, index] = flightTime;
                //            }
                //        }
                //    }
                //}

                angleData[range, height] = angle;
                flightTimeData[range, height] = flightTime;
                previousRange = range;
                previousHeight = height;
                previousAngle = angle;
                if (range == 2400)
                {
                    int ab = 0;
                }
            }

            File.WriteAllText("angleSpaceDelimated.txt", "");
            StringBuilder stringBuilderAngle = new StringBuilder();

            File.WriteAllText("timeSpaceDelimated.txt", "");
            StringBuilder stringBuilderTime = new StringBuilder();

            File.WriteAllText("zerosDelimated.txt", "");
            StringBuilder stringBuilderZero = new StringBuilder();

            count = 0;
            for (int indexRange = 0; indexRange < rangeLength; indexRange++)
            {
                for (int indexHeight = 0; indexHeight < heightLength; indexHeight++)
                {
                    angle = angleData[indexRange, indexHeight];
                    flightTime = flightTimeData[indexRange, indexHeight];
                    if (angle != 0)
                    {
                        stringBuilderAngle.AppendLine($"{indexRange} {indexHeight} {angle}");
                    }
                    if (flightTime != 0)
                    {
                        stringBuilderTime.AppendLine($"{indexRange} {indexHeight} {flightTime}");
                    }
                    if (indexHeight == 5000 && angle != 0 && flightTime > 50)
                    {
                        stringBuilderZero.AppendLine($"{indexRange} {indexHeight} {angle} {flightTime}");
                    }

                    if (count++ % 100000 == 0)
                    {
                        File.AppendAllText("angleSpaceDelimated.txt", stringBuilderAngle.ToString());
                        stringBuilderAngle.Clear();
                        File.AppendAllText("timeSpaceDelimated.txt", stringBuilderTime.ToString());
                        stringBuilderTime.Clear();
                        File.AppendAllText("zerosDelimated.txt", stringBuilderZero.ToString());
                        stringBuilderZero.Clear();
                    }
                }
            }
            File.AppendAllText("angleSpaceDelimated.txt", stringBuilderAngle.ToString());
            File.AppendAllText("timeSpaceDelimated.txt", stringBuilderTime.ToString());
            File.AppendAllText("zerosDelimated.txt", stringBuilderZero.ToString());

            File.WriteAllText("50sDelimated.txt", "");
            StringBuilder stringBuilder50s = new StringBuilder();
            List<int> actualAngle = new List<int>
                (new int[] { 0, 1, 2, 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 19, 22, 30, 34, 39, 44, 49, 55, 62, 68 });
            int indexAngle = 0;
            for (int i = 100; i <= 2500; i += 100)
            {
                var angleValue0 = angleData[i, 0];
                var angleValue1 = angleData[i, 10]; //1 Meter high
                if (angleValue0 == 0)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (angleData[i - j, 0] != 0)
                        {
                            angleValue0 = angleData[i - j, 0];
                            stringBuilder50s.AppendLine($"0 correction {i - j}");
                            break;
                        }
                        else if (angleData[i + j, 0] != 0)
                        {
                            angleValue0 = angleData[i + j, 0];
                            stringBuilder50s.AppendLine($"0 correction {i + j}");
                            break;
                        }
                    }
                }
                if (angleValue1 == 0)
                {
                    for (int j = 0; j < 10; j++)
                    {

                        if (angleData[i - j, 10] != 0)
                        {
                            angleValue1 = angleData[i - j, 10];
                            stringBuilder50s.AppendLine($"1 correction {i + j}");
                            break;
                        }
                        else if (angleData[i + j, 0] != 0)
                        {
                            angleValue1 = angleData[i + j, 10];
                            stringBuilder50s.AppendLine($"1 correction {i + j}");
                            break;
                        }

                    }
                }
                try
                {
                    var error = (angleValue0 - actualAngle[indexAngle] * 10) / 10.0;
                    stringBuilder50s.AppendLine($"{i} {0} {angleValue0} {angleValue1} {error}");
                    indexAngle++;
                }
                catch (Exception)
                {

                }
            }
            File.AppendAllText("50sDelimated.txt", stringBuilder50s.ToString());



            File.WriteAllText("FilledPoints.txt", "");
            StringBuilder filledPoints = new StringBuilder();

            File.WriteAllText("UnFilledPoints.txt", "");
            StringBuilder unfilledPoints = new StringBuilder();
            count = 0;
            int countUnfilled = 0, countR = 0;
            for (int indexRange = 0; indexRange < rangeLength; indexRange++)
            {
                for (int indexHeight = 0; indexHeight < heightLength; indexHeight++)
                {

                    angle = angleData[indexRange, indexHeight];
                    var distance = Math.Sqrt(indexRange * indexRange - ((indexHeight / 10) * (indexHeight / 10)));

                    if (angle == 0 && indexHeight < distance * 24) // distance * 10 * 2.4 (tan 65)
                    {
                        int correctionH = 0;
                        for (int heightCorrectionIndex = 0; heightCorrectionIndex < 10; heightCorrectionIndex++)
                        {
                            try
                            {
                                angle = angleData[indexRange, indexHeight + heightCorrectionIndex];
                                if (angle != 0)
                                {
                                    correctionH = heightCorrectionIndex;
                                    break;
                                }
                                angle = angleData[indexRange, indexHeight - heightCorrectionIndex];
                                if (angle != 0)
                                {
                                    correctionH = -heightCorrectionIndex;
                                    break;
                                }
                            }
                            catch (Exception)
                            {


                            }
                        }
                        if (angle != 0)
                        {
                            flightTime = flightTimeData[indexRange, indexHeight + correctionH];
                            filledPoints.AppendLine($"{indexRange} {indexHeight} {angle} {flightTime} HC {correctionH}");
                            stringBuilderAngle.AppendLine($"{indexRange} {indexHeight} {angle}");
                            stringBuilderTime.AppendLine($"{indexRange} {indexHeight} {flightTime}");
                            if (count++ % 1000 == 0)
                            {
                                File.AppendAllText("FilledPoints.txt", filledPoints.ToString());
                                File.AppendAllText("angleSpaceDelimated.txt", stringBuilderAngle.ToString());
                                File.AppendAllText("timeSpaceDelimated.txt", stringBuilderTime.ToString());
                                stringBuilderAngle.Clear();
                                stringBuilderTime.Clear();
                                filledPoints.Clear();
                            }
                        }
                        else
                        {
                            int correction = 0;
                            for (int rangeCorrectionIndex = 0; rangeCorrectionIndex < 10; rangeCorrectionIndex++)
                            {
                                try
                                {
                                    angle = angleData[indexRange + rangeCorrectionIndex, indexHeight];
                                    if (angle != 0)
                                    {
                                        correction = rangeCorrectionIndex;
                                        break;
                                    }
                                    angle = angleData[indexRange - rangeCorrectionIndex, indexHeight];
                                    if (angle != 0)
                                    {
                                        correction = -rangeCorrectionIndex;
                                        break;
                                    }
                                }
                                catch (Exception)
                                {


                                }
                            }
                            if (angle != 0)
                            {
                                flightTime = flightTimeData[indexRange + correction, indexHeight];

                                filledPoints.AppendLine($"{indexRange} {indexHeight} {angle} {flightTime} Rc {correction}");
                                stringBuilderAngle.AppendLine($"{indexRange} {indexHeight} {angle}");
                                stringBuilderTime.AppendLine($"{indexRange} {indexHeight} {flightTime}");

                                if (countR++ % 1000 == 0)
                                {
                                    File.AppendAllText("FilledPoints.txt", filledPoints.ToString());
                                    File.AppendAllText("angleSpaceDelimated.txt", stringBuilderAngle.ToString());
                                    File.AppendAllText("timeSpaceDelimated.txt", stringBuilderTime.ToString());
                                    stringBuilderAngle.Clear();
                                    stringBuilderTime.Clear();

                                    filledPoints.Clear();
                                }
                            }
                            else
                            {
                                unfilledPoints.AppendLine($"{indexRange} {indexHeight}");
                                if (countUnfilled++ % 1000 == 0)
                                {
                                    File.AppendAllText("UnFilledPoints.txt", unfilledPoints.ToString());
                                    unfilledPoints.Clear();
                                }
                            }

                        }
                        angleData[indexRange, indexHeight] = angle;
                        flightTimeData[indexRange, indexHeight] = flightTime;
                    }
                }
            }

            File.AppendAllText("UnFilledPoints.txt", unfilledPoints.ToString());
            File.AppendAllText("FilledPoints.txt", filledPoints.ToString());

        }

        private void GenerateCsvWithDashNegative500(string filePath)
        {
            int rangeLength = 2501;
            int heightLength = 30001;
            angleData = new Int16[rangeLength, heightLength];
            angleDataFilled = new Int16[rangeLength, heightLength];
            flightTimeData = new Int16[rangeLength, heightLength];

            int count = 0;
            int heightOffset = 5000;
            Int16 angle;
            int range; ;
            int height; ;
            Int16 flightTime = 0;
            int maxRange = 0;
            int maxHeight = 0;
            var fileStream = File.OpenText(filePath);

            string line;
            while ((line = fileStream.ReadLine()) != null)
            {
                var value = line.Split(',');
                if (count++ == 0) continue;
                range = Convert.ToInt16(value[0]);
                height = Convert.ToInt16(Convert.ToDouble(value[1]) * 10) + heightOffset;
                angle = Convert.ToInt16(Convert.ToDouble(value[2]) * 10);
                flightTime = Convert.ToInt16(value[3]);
                if (range > maxRange) maxRange = range;
                if (height > maxHeight) maxHeight = height;
                if (height < 0 || range < 0) continue;

                angleData[range, height] = angle;
                flightTimeData[range, height] = flightTime;
            }

            File.WriteAllText("angleSpaceDelimated.txt", "");
            StringBuilder stringBuilderAngle = new StringBuilder();

            File.WriteAllText("timeSpaceDelimated.txt", "");
            StringBuilder stringBuilderTime = new StringBuilder();

            File.WriteAllText("zerosDelimated.txt", "");
            StringBuilder stringBuilderZero = new StringBuilder();

            count = 0;
            for (int indexRange = 0; indexRange < rangeLength; indexRange++)
            {
                for (int indexHeight = 0; indexHeight < heightLength; indexHeight++)
                {
                    angle = angleData[indexRange, indexHeight];
                    flightTime = flightTimeData[indexRange, indexHeight];
                    if (angle != 0)
                    {
                        stringBuilderAngle.AppendLine($"{indexRange} {indexHeight} {angle}");
                    }
                    if (flightTime != 0)
                    {
                        stringBuilderTime.AppendLine($"{indexRange} {indexHeight} {flightTime}");
                    }
                    if (indexHeight == 5000 && angle != 0 && flightTime > 50)
                    {
                        stringBuilderZero.AppendLine($"{indexRange} {indexHeight} {angle} {flightTime}");
                    }

                    if (count++ % 100000 == 0)
                    {
                        File.AppendAllText("angleSpaceDelimated.txt", stringBuilderAngle.ToString());
                        stringBuilderAngle.Clear();
                        File.AppendAllText("timeSpaceDelimated.txt", stringBuilderTime.ToString());
                        stringBuilderTime.Clear();
                        File.AppendAllText("zerosDelimated.txt", stringBuilderZero.ToString());
                        stringBuilderZero.Clear();
                    }
                }
            }
            File.AppendAllText("angleSpaceDelimated.txt", stringBuilderAngle.ToString());
            File.AppendAllText("timeSpaceDelimated.txt", stringBuilderTime.ToString());
            File.AppendAllText("zerosDelimated.txt", stringBuilderZero.ToString());

            File.WriteAllText("50sDelimated.txt", "");
            StringBuilder stringBuilder50s = new StringBuilder();
            List<int> actualAngle = new List<int>
                (new int[] { 0, 1, 2, 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 19, 22, 30, 34, 39, 44, 49, 55, 62, 68 });
            int indexAngle = 0;
            for (int i = 100; i <= 2500; i += 100)
            {
                var angleValue0 = angleData[i, heightOffset];
                if (angleValue0 == 0)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (angleData[i - j, heightOffset] != 0)
                        {
                            angleValue0 = angleData[i - j, heightOffset];
                            stringBuilder50s.AppendLine($"0 correction {i - j}");
                            break;
                        }
                        else if (angleData[i + j, heightOffset] != 0)
                        {
                            angleValue0 = angleData[i + j, heightOffset];
                            stringBuilder50s.AppendLine($"0 correction {i + j}");
                            break;
                        }
                    }
                }
                try
                {
                    var error = (angleValue0 - actualAngle[indexAngle] * 10) / 10.0;
                    stringBuilder50s.AppendLine($"{i} 0 {angleValue0} {error}");
                    indexAngle++;
                }
                catch (Exception)
                {

                }
            }
            File.AppendAllText("50sDelimated.txt", stringBuilder50s.ToString());



            File.WriteAllText("FilledPoints.txt", "");
            StringBuilder filledPoints = new StringBuilder();

            File.WriteAllText("UnFilledPoints.txt", "");
            StringBuilder unfilledPoints = new StringBuilder();
            count = 0;
            int countUnfilled = 0, countR = 0;
            for (int indexRange = 0; indexRange < rangeLength; indexRange++)
            {
                for (int indexHeight = 0; indexHeight < heightLength; indexHeight++)
                {

                    angle = angleData[indexRange, indexHeight];
                    var distance = Math.Sqrt(indexRange * indexRange - (((indexHeight - heightOffset) / 10) * ((indexHeight - heightOffset) / 10)));
                    var heightLimit = distance * 24; // distance * 10 * 2.4 (tan 65)
                    if (indexHeight < heightOffset) heightLimit = distance * 2; // distance * 10 * 0.2 (tan 11)
                    if (angle == 0 && indexHeight < heightLimit)
                    {
                        int correctionH = 0;
                        for (int heightCorrectionIndex = 0; heightCorrectionIndex < 10; heightCorrectionIndex++)
                        {
                            try
                            {
                                angle = angleData[indexRange, indexHeight + heightCorrectionIndex];
                                if (angle != 0)
                                {
                                    correctionH = heightCorrectionIndex;
                                    break;
                                }
                                if (indexHeight > 9)
                                {
                                    angle = angleData[indexRange, indexHeight - heightCorrectionIndex];
                                    if (angle != 0)
                                    {
                                        correctionH = -heightCorrectionIndex;
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {


                            }
                        }
                        if (angle != 0)
                        {
                            flightTime = flightTimeData[indexRange, indexHeight + correctionH];
                            filledPoints.AppendLine($"{indexRange} {indexHeight} {angle} {flightTime} HC {correctionH}");
                            stringBuilderAngle.AppendLine($"{indexRange} {indexHeight} {angle}");
                            stringBuilderTime.AppendLine($"{indexRange} {indexHeight} {flightTime}");
                            if (count++ % 1000 == 0)
                            {
                                File.AppendAllText("FilledPoints.txt", filledPoints.ToString());
                                File.AppendAllText("angleSpaceDelimated.txt", stringBuilderAngle.ToString());
                                File.AppendAllText("timeSpaceDelimated.txt", stringBuilderTime.ToString());
                                stringBuilderAngle.Clear();
                                stringBuilderTime.Clear();
                                filledPoints.Clear();
                            }
                        }
                        else
                        {
                            int correction = 0;
                            for (int rangeCorrectionIndex = 0; rangeCorrectionIndex < 10; rangeCorrectionIndex++)
                            {
                                try
                                {
                                    angle = angleData[indexRange + rangeCorrectionIndex, indexHeight];
                                    if (angle != 0)
                                    {
                                        correction = rangeCorrectionIndex;
                                        break;
                                    }
                                    if (indexRange > 9)
                                    {
                                        angle = angleData[indexRange - rangeCorrectionIndex, indexHeight];
                                        if (angle != 0)
                                        {
                                            correction = -rangeCorrectionIndex;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception)
                                {


                                }
                            }
                            if (angle != 0)
                            {
                                flightTime = flightTimeData[indexRange + correction, indexHeight];

                                filledPoints.AppendLine($"{indexRange} {indexHeight} {angle} {flightTime} Rc {correction}");
                                stringBuilderAngle.AppendLine($"{indexRange} {indexHeight} {angle}");
                                stringBuilderTime.AppendLine($"{indexRange} {indexHeight} {flightTime}");

                                if (countR++ % 1000 == 0)
                                {
                                    File.AppendAllText("FilledPoints.txt", filledPoints.ToString());
                                    File.AppendAllText("angleSpaceDelimated.txt", stringBuilderAngle.ToString());
                                    File.AppendAllText("timeSpaceDelimated.txt", stringBuilderTime.ToString());
                                    stringBuilderAngle.Clear();
                                    stringBuilderTime.Clear();

                                    filledPoints.Clear();
                                }
                            }
                            else
                            {
                                unfilledPoints.AppendLine($"{indexRange} {indexHeight}");
                                if (countUnfilled++ % 1000 == 0)
                                {
                                    File.AppendAllText("UnFilledPoints.txt", unfilledPoints.ToString());
                                    unfilledPoints.Clear();
                                }
                            }

                        }
                        angleData[indexRange, indexHeight] = angle;
                        flightTimeData[indexRange, indexHeight] = flightTime;
                    }
                }
            }

            File.AppendAllText("UnFilledPoints.txt", unfilledPoints.ToString());
            File.AppendAllText("FilledPoints.txt", filledPoints.ToString());

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int rangeInt = Convert.ToInt32(range.Text);
                float heightActual = Convert.ToSingle(height.Text);
                int heightInt = Convert.ToInt32(heightActual * 10) + 5000;
                double distance = Math.Round(Math.Sqrt(rangeInt * rangeInt - heightActual * heightActual), 1);

                angle.Text = angleData[rangeInt, heightInt].ToString()
                    + " A2=" + angleData[(int)distance, heightInt].ToString()
                    + " D=" + distance.ToString()
                    + " T=" + flightTimeData[rangeInt, heightInt].ToString();



            }
            catch (Exception)
            {

            }
        }
    }
}