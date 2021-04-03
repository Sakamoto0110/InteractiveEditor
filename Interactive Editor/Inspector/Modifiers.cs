using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class Modifiers
    {
        public delegate object CapFunction(object a, object b);

        public static CapFunction POSITIVE_NUMBERS = (value, args) =>
        {

            switch (value)
            {
                case int _:
                    return (int)value < 0 ? 0 : (int)value;
                case double _:
                    return (double)value < 0 ? 0 : (double)value;
                case float _:
                    return (float)value < 0 ? 0 : (float)value;
                default:
                    return value;
            }
        };
        public static CapFunction ONLY_NUMBERS = (value, args) =>
        {
            string valid = "0123456789.,-";
            string str = $"{value}";
            string str2 = "";
            for (int i = 0; i < str.Length; i++)
                for (int j = 0; j < valid.Length; j++)
                    if (str[i] == valid[j])
                        str2 += valid[j];

            return str2;
        };
        public static CapFunction MAX_SIZE = (value, args) =>
        {
            if (args.GetType() != typeof(string[]))
            {
                Console.WriteLine("Invalid parameter type");
                return value;
            }

            string str = $"{value}";
            string str2 = "";
            int maxSize = Convert.ToInt32(((string[])args)[0]);
            str2 = str;
            if (str.Length > maxSize)
            {
                str2 = str.Substring(0, maxSize);
            }

            return str2;
        };
        public static CapFunction CONTAINS = (value, args) =>
        {
            if (args.GetType() != typeof(string[]))
            {
                Console.WriteLine("Invalid parameter type");
                return value;
            }

            string valid = ((string[])args)[0];
            string str = $"{value}";
            string str2 = "";
            for (int i = 0; i < str.Length; i++)
                for (int j = 0; j < valid.Length; j++)
                    if (str[i] == valid[j])
                        str2 += valid[j];
            return str2;
        };


        [Flags]
        public enum MiscControl
        {
            None = 0,
            NextPage = 1,
            PrevPage = 2,
            Apply = 4,
            Reload = 8,
            Clear = 16,
        }
        [Flags]
        public enum ControlFlags
        {
            None = 0,
            EnablePages = 1,
            EnableApply = 2,
            EnableReload = 4,
            EnableQuestionMark = 8,
            EnableClear = 16,

        }
        [Flags]
        public enum FieldFlags
        {
            None = 0,
            UseHandCursorLabel = 1,
            UseHandCursorField = 2,
            UseHSliderControl = 4,
            IsReadOnly = 8,
            IsNotEnabled = 16,
        }

    }
}
