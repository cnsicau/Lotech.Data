using System;

namespace Lotech.Data.Utility
{
    static class EnumUtility
    {
        static public bool TryParse<TEnum>(string enumText, out TEnum value)
            where TEnum : struct
        {
            try
            {
                value = (TEnum)Enum.Parse(typeof(TEnum), enumText, true);
                return true;
            }
            catch
            {
                value = default(TEnum);
                return false;
            }
        }
    }
}
