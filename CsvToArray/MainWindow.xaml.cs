﻿using ElevationAngleCalculator;
using Stira.WpfCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CsvToArray
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// This is array length for range index. System m range length = 2501
        /// </summary>
        private const int rangeLength = 2501;

        /// <summary>
        /// This is array length for height index. System m height length = 30001
        /// </summary>
        private const int heightLength = 8001;

        private short[,] angleData, angleDataFilled, flightTimeData;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            SetM56A3();
            string fileAngle = GetFilePath("angle");
            string fileTime = GetFilePath("time");
            // GenerateCsvWithDashNegative500(filePath);
            AngleCalculateFromRangeCommand = new DelegateCommand(CalculateFromRange);
            Task.Run(() => OpenAnglesForTesting(fileAngle));
            Task.Run(() => OpenTimesForTesting(fileTime));
        }

        private string GetFilePath(string textContainingTheFilePath)
        {
            string path = string.Empty;
            string[] filePaths = Directory.GetFiles(Environment.CurrentDirectory, "*.txt");
            foreach (var filePath in filePaths)
            {
                if (filePath.Contains(textContainingTheFilePath))
                {
                    path = filePath;
                }
            }
            return path;
        }

        double velocityMps, latencyMs;
        private readonly Equation windCrossEquation = new();
        private readonly Equation windHeadEquation = new();
        private readonly Equation windTailEquation = new();
        private readonly Equation driftEquation = new();
        private readonly Equation airDensityHighEquation = new();
        private readonly Equation airDensityLowEquation = new();

        private void SetM56A3()
        {
            velocityMps = 1250;
            latencyMs = 104;
            windCrossEquation.SetCoefficients("3.53929168000000E-02	9.93616989000000E-05	4.78831000000000E-07	-8.99000000000000E-11");
            windHeadEquation.SetCoefficients("2.15392916800000E-01	-8.02983539200000E-04	1.15522390000000E-06	-4.78000000000000E-11");
            windTailEquation.SetCoefficients("2.15392916800000E-01	-8.02983539200000E-04	1.15522390000000E-06	-4.78000000000000E-11");
            driftEquation.SetCoefficients("-4.80000000000008E-01	2.61428571428573E-03	-1.67142857142858E-06	6.00000000000002E-10");
            airDensityHighEquation.SetCoefficients("1.56333333720000E+00	-3.39333334590000E-03	6.86333330000000E-06	-1.26000000000000E-09");
            airDensityLowEquation.SetCoefficients("1.56333333720000E+00	-3.39333334590000E-03	6.86333330000000E-06	-1.26000000000000E-09");
        }

        public DelegateCommand AngleCalculateFromDistanceCommand { get; }

        public DelegateCommand AngleCalculateFromRangeCommand { get; }

        private void CalculateFromRange() => buttonCalculateRange_Click(null, null);

        private void OpenAnglesForTesting(string fileAngle)
        {
            angle.Dispatcher.Invoke(() =>
            {
                angle.Text = "Reading Angles";
            });
            int rangeLength = 2501;
            int heightLength = 30001;
            angleData = new Int16[rangeLength, heightLength];

            var lineCount = File.ReadLines(fileAngle).Count();
            var onePercent = lineCount / 100;
            int count = 0;
            using (var streamRdr = new StreamReader(fileAngle))
            {
                var csvReader = new NReco.Csv.CsvReader(streamRdr, " ");
                while (csvReader.Read())
                {
                    angleData[Convert.ToInt16(csvReader[0]), Convert.ToInt16(Convert.ToDouble(csvReader[1]))] = Convert.ToInt16(Convert.ToDouble(csvReader[2]));

                    if (count++ % onePercent == 0)
                    {
                        angle.Dispatcher.Invoke(() =>
                        {
                            angle.Text = $"Reading Angles { Math.Round(((float)count / lineCount) * 100, 1)} % completed";
                        });
                    }
                }
            }
            angle.Dispatcher.Invoke(() =>
            {
                angle.Text = "Angle values data read done";
            });
        }

        private void OpenTimesForTesting(string fileTime)
        {
            angle3.Dispatcher.Invoke(() =>
            {
                angle3.Text = "Reading Time";
            });

            int rangeLength = 2501;
            int heightLength = 30001;
            flightTimeData = new Int16[rangeLength, heightLength];

            var fileStream2 = File.OpenText(fileTime);
            var lineCount = File.ReadLines(fileTime).Count();
            int count = 0;
            var onePercent = lineCount / 100;

            using (var streamRdr = new StreamReader(fileTime))
            {
                var csvReader = new NReco.Csv.CsvReader(streamRdr, " ");
                while (csvReader.Read())
                {
                    flightTimeData[Convert.ToInt16(csvReader[0]), Convert.ToInt16(Convert.ToDouble(csvReader[1]))] = Convert.ToInt16(Convert.ToDouble(csvReader[2]));
                    if (count++ % onePercent == 0)
                    {
                        angle.Dispatcher.Invoke(() =>
                        {
                            angle3.Text = $"Reading Time { Math.Round(((float)count / lineCount) * 100, 1)} % completed";
                        });
                    }
                }
            }

            angle3.Dispatcher.Invoke(() =>
            {
                angle3.Text = "Flight time data read done";
            });
        }

        private void GenerateCsvWithDashZeroLevel(string filePath)
        {
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
                                unfilledPoints.Append(indexRange).Append(' ').Append(indexHeight).AppendLine();
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
            angleData = new Int16[rangeLength, heightLength];
            angleDataFilled = new Int16[rangeLength, heightLength];
            flightTimeData = new Int16[rangeLength, heightLength];

            int count = 0;
            int heightOffset = 5000;
            Int16 angle;
            int range;
            int height;
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
                (new int[] { 0, 1, 2, 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 19, 22, 26, 30, 34, 39, 44, 49, 55, 62, 68 });
            int indexAngle = 0;
            try
            {
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
            }
            catch (Exception)
            {
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



        private void buttonCalculateRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int rangeInt = Convert.ToInt32(rangeText.Text);
                int actualRange = rangeInt;
                float heightActual = Convert.ToSingle(heightText.Text);
                double speedKmph = GetDouble(speedText.Text);
                double windSpeedCross = GetDouble(windSpeedCrossText.Text);
                double windSpeedHead = GetDouble(windSpeedHeadText.Text);
                double airDensity = GetDouble(airDensityText.Text);

                double percentChange = (airDensity - 1.225) * 100 / 1.225;

                var adEffect = airDensityHighEquation.GetValue(rangeInt) * percentChange;
                var headWindEffect = windHeadEquation.GetValue(rangeInt) * windSpeedHead;
                rangeInt = (int)(rangeInt + adEffect - headWindEffect); //Compensated Range



                if (rangeInt > 2500 || heightActual > 2500)
                {
                    angle.Text = "";
                    angle3.Text = "Out of range";
                    return;
                }
                int heightInt = Convert.ToInt32(heightActual * 10) + 5000;
                double distance = Math.Round(Math.Sqrt(rangeInt * rangeInt - heightActual * heightActual), 1);
                int rangeFromDistance = (int)Math.Round(Math.Sqrt(rangeInt * rangeInt + heightActual * heightActual));
                var angleRequired = angleData[rangeInt, heightInt] / 10.0;
                if (angleRequired == 0)
                {
                    angle.Text = $"Distance: {distance} m";
                    angle3.Text = "Out of range";
                    return;
                }

                double speedMps = speedKmph / 3.6;

                var angularSpeed = -(speedMps / actualRange) * (6400 / (2 * Math.PI));
                var movingTargetShift = angularSpeed * ((flightTimeData[rangeInt, heightInt] + 104) / 1000.0);
                var crossWindEffect = windCrossEquation.GetValue(rangeInt) * windSpeedCross;
                var driftEffect = driftEquation.GetValue(rangeInt);
                var totalAzimuthShift = Math.Round(crossWindEffect - driftEffect + movingTargetShift, 2);

                angle.Text = "FA: " + (angleRequired).ToString()
                    + $" mils, Az " + totalAzimuthShift + " mils, T: " + flightTimeData[rangeInt, heightInt].ToString() + " ms";

                double sightAngle = Math.Asin((float)(heightActual / actualRange)) * (6400 / (2 * Math.PI));
                double angleFire = angleData[rangeInt, heightInt] / 10.0;
                double angleFireAt0 = angleData[rangeInt, 5000] / 10.0;
                double angleDifference = angleFire - sightAngle;
                angle3.Text = $"Sight Angle: {sightAngle:N2} mils\n" +
                    $"Firing Angle: {angleFire:N2} mils\n" +
                    $"Elevation at {angleFire:N2} mils: {angleDifference:N2} mils\n" +
                    $"Elevation at 0 mils: {angleFireAt0:N2} mils\n" +
                    $"Elevation Difference: {angleDifference - angleFireAt0:N2} mils";
            }
            catch (Exception)
            {
            }
        }

        private static double GetDouble(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }
            return Convert.ToDouble(text);
        }

        //    private void buttonCalculateDistance_Click(object sender, RoutedEventArgs e)
        //    {
        //        try
        //        {
        //            int distanceInt = Convert.ToInt32(distance.Text);
        //            float heightActual = Convert.ToSingle(heightDistance.Text);
        //            int heightInt = Convert.ToInt32(heightActual * 10) + 5000;
        //            int rangeFromDistance = (int)Math.Round(Math.Sqrt(distanceInt * distanceInt + heightActual * heightActual));
        //            var angleRequired = angleData[rangeFromDistance, heightInt] / 10.0;
        //            if (angleRequired == 0)
        //            {
        //                angle.Text = $"Range: {rangeFromDistance} m";
        //                angle3.Text = "Out of range";
        //                return;
        //            }
        //            angle.Text = "FA: " + (angleRequired).ToString() +
        //                $"mils, {Math.Round((angleData[rangeFromDistance, heightInt] / 10.0) * (2 * Math.PI / 6.4), 1)} mRad, R: " +
        //                rangeFromDistance.ToString() + "m, T: "
        //                + flightTimeData[rangeFromDistance, heightInt].ToString() + " ms";

        //            double sightAngle = Math.Asin((float)(heightActual / rangeFromDistance)) * (6400 / (2 * Math.PI));
        //            double angleFire = angleData[rangeFromDistance, heightInt] / 10.0;
        //            double angleFireAt0 = angleData[rangeFromDistance, 5000] / 10.0;
        //            double angleDifference = angleFire - sightAngle;
        //            angle3.Text = $"Sight Angle: {sightAngle} mils\n" +
        //                $"Firing Angle: {angleFire} mils\n" +
        //                $"Elevation at {angleFire} mils: {angleDifference} mils\n" +
        //                $"Elevation at 0 mils: {angleFireAt0} mils\n" +
        //                $"Elevation Difference: {angleDifference - angleFireAt0} mils";
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //
    }
}