using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: CLSCompliant(true)]
namespace Fabio.SharpTools.Convertion
{
    public enum MeasurementUnit
    {
        Foot = 22,
        Inch = 20,
        Gram = 17,
        Kilogram = 2,
        Litre = 13,
        Metre = 23,
        Millilitre = 10,
        Ounce = 9,
        Pound = 11
    }

    /// <summary>
    /// Converts different Measurement Units like Kilogram, Pounds, Litres, and so on.
    /// </summary>
    public static class UnitConvertor
    {
        private static decimal getFactor(MeasurementUnit unitFrom)
        {
            switch (unitFrom)
            {
                case MeasurementUnit.Kilogram: //Kg -> g
                    return 1000;
                case MeasurementUnit.Ounce: //oz -> Kg
                    return 0.0283495231m;
                case MeasurementUnit.Pound: //Lb -> Kg
                    return 0.45359237m;
                case MeasurementUnit.Litre: //Litre -> mL
                    return 1000;
                case MeasurementUnit.Foot: //ft -> in
                    return 12;
                case MeasurementUnit.Metre: //m -> ft
                    return 3.2808399m;
                default:
                    throw new UnitConvertorException("Cannot convert " + unitFrom);
            }
        }

        #region 1-level convertions
        public static decimal KilogramToGram(decimal value)
        {
            return value * getFactor(MeasurementUnit.Kilogram);
        }

        public static decimal GramToKilogram(decimal value)
        {
            return value / getFactor(MeasurementUnit.Kilogram);
        }

        public static decimal OunceToKilogram(decimal value)
        {
            return value * getFactor(MeasurementUnit.Ounce);
        }

        public static decimal KilogramToOunce(decimal value)
        {
            return value / getFactor(MeasurementUnit.Ounce);
        }

        public static decimal PoundToKilogram(decimal value)
        {
            return value * getFactor(MeasurementUnit.Pound);
        }

        public static decimal KilogramToPound(decimal value)
        {
            return value / getFactor(MeasurementUnit.Pound);
        }

        public static decimal LitreToMillilitre(decimal value)
        {
            return value * getFactor(MeasurementUnit.Litre);
        }

        public static decimal MillilitreToLitre(decimal value)
        {
            return value / getFactor(MeasurementUnit.Litre);
        }

        public static decimal FootToInch(decimal value)
        {
            return value * getFactor(MeasurementUnit.Foot);
        }

        public static decimal InchToFoot(decimal value)
        {
            return value / getFactor(MeasurementUnit.Foot);
        }

        public static decimal MetreToFoot(decimal value)
        {
            return value * getFactor(MeasurementUnit.Metre);
        }

        public static decimal FootToMetre(decimal value)
        {
            return value / getFactor(MeasurementUnit.Metre);
        }
        #endregion

        #region 2-level convertions
        public static decimal PoundToGram(decimal value)
        {
            return KilogramToGram(PoundToKilogram(value));
        }

        public static decimal GramToPound(decimal value)
        {
            return KilogramToPound(GramToKilogram(value));
        }

        public static decimal OunceToGram(decimal value)
        {
            return KilogramToGram(OunceToKilogram(value));
        }

        public static decimal GramToOunce(decimal value)
        {
            return KilogramToOunce(GramToKilogram(value));
        }

        public static decimal PoundToOunce(decimal value)
        {
            return KilogramToOunce(PoundToKilogram(value));
        }

        public static decimal OunceToPound(decimal value)
        {
            return KilogramToPound(OunceToKilogram(value));
        }

        public static decimal MetreToInch(decimal value)
        {
            return MetreToFoot(FootToInch(value));
        }

        public static decimal InchToMetre(decimal value)
        {
            return InchToFoot(FootToMetre(value));
        }
        #endregion

        #region Convert method
        public static decimal Convert(decimal value, MeasurementUnit unitFrom, MeasurementUnit unitTo)
        {
            bool cantCalculate = false;

            if (value == 0)
                return 0;

            switch (unitFrom)
            {
                case MeasurementUnit.Gram:
                    switch (unitTo)
                    {
                        case MeasurementUnit.Kilogram:
                            value = GramToKilogram(value);
                            break;
                        case MeasurementUnit.Pound:
                            value = GramToPound(value);
                            break;
                        case MeasurementUnit.Ounce:
                            value = GramToOunce(value);
                            break;
                        case MeasurementUnit.Gram:
                            break;
                        default:
                            cantCalculate = true;
                            break;
                    }
                    break;
                case MeasurementUnit.Kilogram:
                    switch (unitTo)
                    {
                        case MeasurementUnit.Gram:
                            value = KilogramToGram(value);
                            break;
                        case MeasurementUnit.Pound:
                            value = KilogramToPound(value);
                            break;
                        case MeasurementUnit.Ounce:
                            value = KilogramToOunce(value);
                            break;
                        case MeasurementUnit.Kilogram:
                            break;
                        default:
                            cantCalculate = true;
                            break;
                    }
                    break;
                case MeasurementUnit.Ounce:
                    switch (unitTo)
                    {
                        case MeasurementUnit.Gram:
                            value = OunceToGram(value);
                            break;
                        case MeasurementUnit.Pound:
                            value = OunceToPound(value);
                            break;
                        case MeasurementUnit.Kilogram:
                            value = OunceToKilogram(value);
                            break;
                        case MeasurementUnit.Ounce:
                            break;
                        default:
                            cantCalculate = true;
                            break;
                    }
                    break;
                case MeasurementUnit.Pound:
                    switch (unitTo)
                    {
                        case MeasurementUnit.Gram:
                            value = PoundToGram(value);
                            break;
                        case MeasurementUnit.Ounce:
                            value = PoundToOunce(value);
                            break;
                        case MeasurementUnit.Kilogram:
                            value = PoundToKilogram(value);
                            break;
                        case MeasurementUnit.Pound:
                            break;
                        default:
                            cantCalculate = true;
                            break;
                    }
                    break;
                case MeasurementUnit.Litre:
                    switch (unitTo)
                    {
                        case MeasurementUnit.Millilitre:
                            value = LitreToMillilitre(value);
                            break;
                        case MeasurementUnit.Litre:
                            break;
                    }
                    break;
                case MeasurementUnit.Millilitre:
                    switch (unitTo)
                    {
                        case MeasurementUnit.Litre:
                            value = MillilitreToLitre(value);
                            break;
                        case MeasurementUnit.Millilitre:
                            break;
                    }
                    break;
                case MeasurementUnit.Foot:
                    switch (unitTo)
                    {
                        case MeasurementUnit.Inch:
                            value = FootToInch(value);
                            break;
                        case MeasurementUnit.Metre:
                            value = FootToMetre(value);
                            break;
                        case MeasurementUnit.Foot:
                            break;
                        default:
                            cantCalculate = true;
                            break;
                    }
                    break;
                case MeasurementUnit.Metre:
                    switch (unitTo)
                    {
                        case MeasurementUnit.Inch:
                            value = MetreToInch(value);
                            break;
                        case MeasurementUnit.Foot:
                            value = MetreToFoot(value);
                            break;
                        case MeasurementUnit.Metre:
                            break;
                        default:
                            cantCalculate = true;
                            break;
                    }
                    break;
                case MeasurementUnit.Inch:
                    switch (unitTo)
                    {
                        case MeasurementUnit.Foot:
                            value = InchToFoot(value);
                            break;
                        case MeasurementUnit.Metre:
                            value = InchToMetre(value);
                            break;
                        case MeasurementUnit.Inch:
                            break;
                        default:
                            cantCalculate = true;
                            break;
                    }
                    break;
                default:
                    cantCalculate = true;
                    break;
            }

            if (cantCalculate)
                throw new UnitConvertorException("Can't convert " + unitFrom + " into " + unitTo);

            return value;
        }
        #endregion
        #region Compare method
        public static bool AlmostEquals(decimal valueA, MeasurementUnit unitA, decimal valueB, MeasurementUnit unitB, int percentageFactor = 20)
        {
            bool equals = false;
            bool cantCalculate = false;

            try
            {
                valueA = Convert(valueA, unitA, unitB);
            }
            catch (UnitConvertorException)
            {
                cantCalculate = true;
            }

            if (!cantCalculate)
            {
                decimal highest = (valueA > valueB) ? valueA : valueB;
                decimal lowest = (valueA < valueB) ? valueA : valueB;
                if (lowest > 0 && highest > 0)
                {
                    int z = Math.Abs(System.Convert.ToInt32(100 - ((lowest * 100) / highest)));
                    equals = z <= percentageFactor;
                }
            }

            return equals;

        }
        public static int Compare(decimal valueA, MeasurementUnit unitA, decimal valueB, MeasurementUnit unitB)
        {
            bool cantCalculate = false;

            try
            {
                valueA = Convert(valueA, unitA, unitB);
            }
            catch (UnitConvertorException)
            {
                cantCalculate = true;
            }

            if (!cantCalculate)
            {
                decimal highest = (valueA > valueB) ? valueA : valueB;
                decimal lowest = (valueA < valueB) ? valueA : valueB;
                int z = Math.Abs(System.Convert.ToInt32(100 - ((lowest * 100) / highest)));

                return z;
            }

            return -1;

        }
        public static bool Equals(decimal valueA, MeasurementUnit unitA, decimal valueB, MeasurementUnit unitB)
        {
            bool equals = false;
            bool cantCalculate = false;

            try
            {
                valueA = Convert(valueA, unitA, unitB);
            }
            catch (UnitConvertorException)
            {
                cantCalculate = true;
            }
            
            if (!cantCalculate)
            {
                equals = valueA == valueB;
            }

            return equals;

        }
        #endregion
    }

    public class UnitConvertorException : Exception
    {
        public UnitConvertorException() : base(){}
        public UnitConvertorException(string message) : base(message){}

    }
}
