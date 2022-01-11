using System.Collections.Generic;

namespace ElevationAngleCalculator
{
    /// <summary>
    /// Equation Solver Service
    /// </summary>
    public interface IEquationSolver
    {
        /// <summary>
        /// Get the Linear Equation for the provided x and y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="order"></param>
        /// <returns>Equation</returns>
        IEnumerable<double> GetPolynomialEquation(IEnumerable<double> x, IEnumerable<double> y, int order);

        /// <summary>
        /// Solves the equation for the given value
        /// </summary>
        /// <param name="equation">Equation</param>
        /// <param name="atXvalue"></param>
        /// <param name="roundOff"></param>
        /// <param name="precision"></param>
        /// <returns>Result</returns>
        double GetReadingAtPolynomial(IEnumerable<double> equation, double atXvalue, bool roundOff = false, int precision = 5);

        /// <summary>
        /// Solve the equation for the given list
        /// </summary>
        /// <param name="inputEquation"></param>
        /// <param name="atXvalues"></param>
        /// <param name="roundOff"></param>
        /// <param name="precision"></param>
        /// <returns>Multiple readings</returns>
        IEnumerable<double> GetReadingsAtPolynomial(IEnumerable<double> inputEquation, IEnumerable<double> atXvalues, bool roundOff = false, int precision = 5);

        /// <summary>
        /// Get value of Input For the Particular Output value
        /// </summary>
        /// <param name="inputEquation">Polynomial Equation</param>
        /// <param name="atYvalue">Output Value</param>
        /// <param name="accuracy">Accuracy</param>
        /// <returns>Input value for the given desired output</returns>
        public double GetValueOfXAtYPolynomial(double[] inputEquation, double atYvalue, double accuracy = 1e-9);

        /// <summary>
        /// Get the Exponential Equation co-efficients A and r in exponential equation: [ A(e^rX) ]
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y">Output</param>
        /// <returns>A and r co-efficients</returns>
        public IEnumerable<double> GetExponentialEquation(IEnumerable<double> x, IEnumerable<double> y);

        /// <summary>
        /// Get the reading at input for exponential equation
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="atXValue"></param>
        /// <param name="roundOff"></param>
        /// <param name="precision"></param>
        /// <returns>Reading</returns>
        public double GetReadingAtExponential(IEnumerable<double> equation, double atXValue, bool roundOff = false, int precision = 5);

        /// <summary>
        /// Get the reading at input for exponential equation
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="atXValues"></param>
        /// <param name="roundOff"></param>
        /// <param name="precision"></param>
        /// <returns>Readings</returns>
        public IEnumerable<double> GetReadingsAtExponential(IEnumerable<double> equation, IEnumerable<double> atXValues, bool roundOff = false, int precision = 5);
    }
}