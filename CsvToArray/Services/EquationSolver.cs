using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevationAngleCalculator
{
    /// <summary>
    /// Equation Solver Service
    /// </summary>
    public class EquationSolver : IEquationSolver
    {
        /// <summary>
        /// Get value of Input For the Particular Output value
        /// </summary>
        /// <param name="inputEquation">Polynomial Equation</param>
        /// <param name="atYvalue">Output Value</param>
        /// <param name="accuracy">Accuracy</param>
        /// <returns>Input value for the given desired output</returns>
        public double GetValueOfXAtYPolynomial(double[] inputEquation, double atYvalue, double accuracy = 1e-9)
        {
            double[] cloneEquation = new double[inputEquation.Length];
            inputEquation.CopyTo(cloneEquation, 0);
            cloneEquation[0] -= atYvalue;
            double FunctionGetEquationValueAtX(double atXValue)
            {
                int order = cloneEquation.Length;
                double result = 0.0;
                for (int index = order - 1; index >= 0; index--)
                {
                    result += cloneEquation[index] * Math.Pow(atXValue, index);
                }
                return result;
            }
            Func<double, double> functionX = FunctionGetEquationValueAtX;
            return (double)FindRoots.OfFunction(functionX, 0, 5000, accuracy, 200);
        }

        /// <summary>
        /// Get the Linear Equation for the provided x and y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="order"></param>
        /// <returns>Equation</returns>
        public IEnumerable<double> GetPolynomialEquation(IEnumerable<double> x, IEnumerable<double> y, int order)
        {
            Matrix<double> design = Matrix<double>.Build.Dense(x.Count(), order + 1, (i, j) => Math.Pow(x.ElementAt(i), j));
            Vector<double> yMatrix = Vector<double>.Build.Dense(y.ToArray());

            return MultipleRegression.QR(design, yMatrix).ToArray();
        }

        /// <summary>
        /// Get the Exponential Equation co-efficients A and r in exponential equation: [ A(e^rX) ]
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y">Output</param>
        /// <returns>A and r co-efficients</returns>
        public IEnumerable<double> GetExponentialEquation(IEnumerable<double> x, IEnumerable<double> y)
        {
            const DirectRegressionMethod method = DirectRegressionMethod.QR;
            double[] y_hat = Generate.Map(y.ToArray(), Math.Log);
            double[] p_hat = Fit.LinearCombination(x.ToArray(), y_hat, method, t => 1.0, t => t);
            return new[] { Math.Exp(p_hat[0]), p_hat[1] };
        }

        /// <summary>
        /// Get the reading at input for exponential equation
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="atXValue"></param>
        /// <param name="roundOff"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public double GetReadingAtExponential(IEnumerable<double> equation, double atXValue, bool roundOff = false, int precision = 5)
        {
            double result = equation.ElementAt(0) * Math.Exp(equation.ElementAt(1) * atXValue);
            if (roundOff)
            {
                result = Math.Round(result, precision);
            }
            return result;
        }

        /// <summary>
        /// Get the reading at input for exponential equation
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="atXValues"></param>
        /// <param name="roundOff"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public IEnumerable<double> GetReadingsAtExponential(IEnumerable<double> equation, IEnumerable<double> atXValues, bool roundOff = false, int precision = 5)
        {
            List<double> results = new();

            foreach (double atXvalue in atXValues)
            {
                results.Add(GetReadingAtExponential(equation, atXvalue, roundOff, precision));
            }
            return results.ToArray();
        }

        /// <summary>
        /// Solves the equation for the given value
        /// </summary>
        /// <param name="equation">Equation</param>
        /// <param name="atXvalue"></param>
        /// <param name="roundOff"></param>
        /// <param name="precision"></param>
        /// <returns>Result</returns>
        public double GetReadingAtPolynomial(IEnumerable<double> equation, double atXvalue, bool roundOff = false, int precision = 5)
        {
            int order = equation.Count() - 1;
            double result = 0;
            for (int index = order; index >= 0; index--)
            {
                result += equation.ElementAt(index) * Math.Pow(atXvalue, index);
            }
            if (roundOff)
            {
                result = Math.Round(result, precision);
            }
            return result;
        }

        /// <summary>
        /// Solve the equation for the given list
        /// </summary>
        /// <param name="inputEquation"></param>
        /// <param name="atXvalues"></param>
        /// <param name="roundOff"></param>
        /// <param name="precision"></param>
        /// <returns>Multiple readings</returns>
        public IEnumerable<double> GetReadingsAtPolynomial(IEnumerable<double> inputEquation, IEnumerable<double> atXvalues, bool roundOff = false, int precision = 5)
        {
            List<double> results = new();

            foreach (double atXvalue in atXvalues)
            {
                results.Add(GetReadingAtPolynomial(inputEquation, atXvalue, roundOff, precision));
            }
            return results.ToArray();
        }
    }
}