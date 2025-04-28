using System.Globalization;

namespace App.Utils
{
    public static class UtilsObectsException
    {
        public static object Check(object v, int t)
        {
            if (v == null) return DBNull.Value;
            switch (t)
            {
                case 1:
                    {
                        return Convert.ToInt64(v);
                    }
                case 2:
                    {
                        return v.ToString();
                    }
                case 3:
                    {
                        return Convert.ToDateTime(v);
                    }
                case 4:
                    {
                        return Convert.ToDateTime(v).Date;
                    }
                default: break;
            }
            return DBNull.Value;
        }

        public static string ToStr(this object inParam, string dateTimeFormat = "dd.MM.yyyy")
        {
            if (inParam == null)
                return "";
            else
            {
                if (inParam is bool)
                    return inParam.ToString().ToUpper() == "TRUE" ? "1" : "0";
                if (inParam is decimal)
                    return inParam.ToString().Replace('.', ',');
                if (inParam is DateTime)
                    return Convert.ToDateTime(inParam, System.Threading.Thread.CurrentThread.CurrentCulture).ToString(dateTimeFormat);
                if (inParam is byte[])
                    return System.Text.Encoding.Default.GetString(inParam as byte[]);
                return inParam.ToString().Trim();
            }
        }

        public static string ToStr(this object inParam)
        {
            if (inParam == null)
                return "";
            else
            {
                if (inParam is bool)
                    return inParam.ToString().ToUpper() == "TRUE" ? "1" : "0";
                if (inParam is decimal)
                    return inParam.ToString().Replace('.', ',');
                if (inParam is DateTime)
                    return Convert.ToDateTime(inParam, System.Threading.Thread.CurrentThread.CurrentCulture).ToString("dd.MM.yyyy");
                if (inParam is byte[])
                    return System.Text.Encoding.Default.GetString(inParam as byte[]);
                return inParam.ToString().Trim();
            }
        }

        public static long ToInt64(this object inVal)
        {
            var s = inVal.ToStr();
            if (DBNull.Value == inVal || inVal == null || s.Length == 0)
            {
                throw new Exception($"неверно значения => {inVal}");
            }

            s = s.Replace(" ", "");

            return Convert.ToInt64(s);
        }

        public static long? ToNullableInt64(this object inVal)
        {
            if (DBNull.Value == inVal || inVal == null || inVal.ToStr()?.Length == 0)
                return null;
            else
                return Convert.ToInt64(inVal);
        }

        public static int? ToNullableInt(this object inVal)
        {
            if (DBNull.Value == inVal || inVal == null || inVal.ToStr()?.Length == 0)
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(inVal);
            }
        }

        public static Guid? ToNullableGuid(this object inVal)
        {
            if (DBNull.Value == inVal || inVal == null || inVal.ToStr()?.Length == 0)
                return null;
            return Guid.Parse(inVal.ToString());
        }

        public static Guid ToGuid(this object inVal)
        {
            if (DBNull.Value == inVal || inVal == null || inVal.ToStr()?.Length == 0)
                throw new Exception($"неверно значения => {inVal}");
            return Guid.Parse(inVal.ToString());
        }

        public static int ToInt(this object inVal)
        {
            var s = inVal.ToStr().Trim();
            if (DBNull.Value == inVal || inVal == null || s.Length == 0)
            {
                throw new Exception($"неверно значения => {inVal}");
            }

            s = s.Replace(" ", "");

            return Convert.ToInt32(s);
        }

        public static DateTime ToDateTime(this object source)
        {
            if (DBNull.Value == source || source == null || source.ToString() == "")
                return DateTime.MinValue;

            return Convert.ToDateTime(source/*.ToString().Replace("XX.XX", "01.01")*/, CultureInfo.InvariantCulture);
        }

        public static decimal ToDecimal(this object source, int digt = 2)
        {
            if (source == null) return 0;
            var s = source.ToStr().Trim();

            s = s.Replace(" ", "");

            return Math.Round(Convert.ToDecimal(s, CultureInfo.CurrentCulture), digt);
        }

        public static string ToMsSqlDate(this object source, int type = 0)
        {
            var res = "";
            if (source != null)
            {
                switch (type)
                {
                    case -1:
                        res = $"{source:yyyy-MM-dd}";
                        res = "CONVERT(DATETIME, '" + res + "', 102)";
                        break;
                    case 0:
                        res = $"{source:yyyy-MM-dd H:mm:ss}";
                        res = "CONVERT(DATETIME, '" + res + "', 102)";
                        break;
                    case 1:
                        res = $"{source:yyyy-MM-dd}";
                        res = "CONVERT(DATETIME, '" + res + " 00:00:00', 102)";
                        break;
                    case 2:
                        res = $"{source:yyyy-MM-dd}";
                        res = "CONVERT(DATETIME, '" + res + " 23:59:59', 102)";
                        break;

                    case 3:
                        res = $"{source:yyyy-MM-dd HH:mm:ss}";
                        res = "CONVERT(DATETIME, '" + res + "', 102)";
                        break;
                }
            }

            return res;
        }

        //public static string ToClearStr(this object inParam)
        //{
        //    return inParam.ToStr().ToClear();
        //}

        public static bool IsNullorEmpty(this object inParam)
        {
            return string.IsNullOrEmpty(inParam.ToStr().Trim());
        }

        public static bool IsDateTime(this object inParam)
        {
            if (inParam == null) return false;
            try
            {
                DateTimeFormatInfo ruDtfi = new CultureInfo("ru-RU", false).DateTimeFormat;
                var dateTime = Convert.ToDateTime(inParam.ToStr().Replace("XX.XX", "01.01"), ruDtfi);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ToOracleDate(this object source, int type = 0)
        {
            string res = "";
            if (TryConvert.ToDateTime(source, out DateTime d))
            {
                res = source.ToStr();
            }
            else
            {
                throw new Exception($"Киритилган сана хато... {source}");
            }

            switch (type)
            {
                case 0:
                    {
                        res = "to_Date('" + res + "', 'dd.mm.yyyy HH24:MI:SS')";
                        break;
                    }
                case 1:
                    {
                        res = "to_Date('" + res + " 00:00:00', 'dd.mm.yyyy HH24:MI:SS')";
                        break;
                    }
                case 2:
                    {
                        res = "to_Date('" + res + " 23:59:59', 'dd.mm.yyyy HH24:MI:SS')";
                        break;
                    }

            }

            return res;

        }

        public static bool ToBool(this object source)
        {
            try
            {
                return Convert.ToBoolean(source);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}