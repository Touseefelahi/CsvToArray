using System.Collections.Generic;

namespace ElevationAngleCalculator
{
    /// <summary>
    /// General Equation type for the polynomial or exponential equation
    /// </summary>
    public class Equation : IEquation
    {
        private static IEquationSolver equationSolver;
        private double[] equationCoefficients;

        /// <summary>
        /// Check for polynomial or exponential equation
        /// </summary>
        public bool IsPolynomial { get; set; }

        /// <summary>
        /// True only if the equation coefficients are set
        /// </summary>
        public bool IsSet { get; private set; }
        public double[] EquationCoefficients
        {
            get { return equationCoefficients; }
            set { equationCoefficients = value; IsSet = true; }
        }
        public Equation(bool isPolynomial = true)
        {
            IsPolynomial = isPolynomial;
            if (equationSolver is null)
            {
                equationSolver = new EquationSolver();
            }
            if (IsPolynomial)
            {
                EquationCoefficients = new double[] { 0 };
                IsSet = false;
                return;
            }
            EquationCoefficients = new double[] { 0, 0 };
            IsSet = false;
        }
        /// <summary>
        /// Gets the value for the provided input
        /// </summary>
        /// <param name="forInput"></param>
        /// <returns></returns>
        public double GetValue(double forInput)
        {
            return IsPolynomial
                ? equationSolver.GetReadingAtPolynomial(EquationCoefficients, forInput)
                : equationSolver.GetReadingAtExponential(EquationCoefficients, forInput);
        }

        /// <summary>
        /// Sets the coefficients of equation
        /// </summary>
        /// <param name="formattedEquation"></param>
        /// <param name="delimator"></param>
        /// <returns>Status</returns>
        public bool SetCoefficients(string formattedEquation, string delimator = "\t")
        {
            List<string> listOfCoefficients = new(formattedEquation.Split(delimator));
            EquationCoefficients = listOfCoefficients.ConvertAll(item => double.Parse(item)).ToArray();
            IsSet = true;
            return true;
        }
    }
}
