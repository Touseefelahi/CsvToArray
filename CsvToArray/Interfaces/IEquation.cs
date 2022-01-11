namespace ElevationAngleCalculator
{
    /// <summary>
    /// General Equation type for the polynomial or exponential equation
    /// </summary>
    public interface IEquation
    {
        /// <summary>
        /// Check for polynomial or exponential equation
        /// </summary>
        bool IsPolynomial { get; set; }

        /// <summary>
        /// Gets the value for the provided input
        /// </summary>
        /// <param name="forInput"></param>
        /// <returns></returns>
        double GetValue(double forInput);

        /// <summary>
        /// True only if the equation coefficients are set
        /// </summary>
        public bool IsSet { get;  }

        /// <summary>
        /// Sets the coefficients of equation
        /// </summary>
        /// <param name="formattedEquation"></param>
        /// <param name="delimator"></param>
        /// <returns></returns>
        bool SetCoefficients(string formattedEquation, string delimator = "\t");
    }
}