using System.Data;
using System.Dynamic;
using System.Reflection;

namespace App.Utils
{
    public static class CDbData
    {
        public static IEnumerable<dynamic> DataTable2Model(this DataTable dr)
        {
            foreach (DataRow row in dr.Rows)
            {
                dynamic e = new ExpandoObject();
                var d = (IDictionary<string, object>)e;
                var values = row.ItemArray;

                for (int i = 0; i < values.Length; i++)
                {
                    var v = values[i];
                    d.Add(dr.Columns[i].ColumnName, DBNull.Value.Equals(v) ? "" : v);
                }
                yield return d;
            }
            dr.Dispose();
        }

        public static IEnumerable<dynamic> DataReader2Model(this IDataReader rdr)
        {
            while (rdr.Read())
            {
                yield return rdr.RecordToModel();
            }
            rdr.Close();
        }

        public static dynamic RecordToModel(this IDataReader reader)
        {
            dynamic e = new ExpandoObject();
            var d = (IDictionary<string, object>)e;
            object[] values = new object[reader.FieldCount];
            reader.GetValues(values);
            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];
                d.Add(reader.GetName(i), DBNull.Value.Equals(v) ? "" : v);
            }
            return e;
        }

        /// <summary>
        /// Run time Compilered model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>

        public static List<Dictionary<string, object>> ToModel(this IDataReader source)
        {
            try
            {
                int cou = source.FieldCount;
                object[] values = new object[cou];
                string[] names = new string[cou];
                for (int i = 0; i < cou; i++)
                {
                    names[i] = source.GetName(i);
                }

                var res = new List<Dictionary<string, object>>();
                while (source.Read())
                {
                    var model = new Dictionary<string, object>();
                    source.GetValues(values);
                    for (int i = 0; i < cou; i++)
                    {
                        model.Add(names[i], values[i]);
                    }

                    res.Add(model);
                }

                return res;
            }
            finally
            {
                source?.Close();
                source?.Dispose();
            }
        }

        public static void TrySetProperty(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, value, null);
            else
                prop.SetValue(obj, "", null);
        }

        public static DataTable ToDataTable(this object o)
        {
            DataTable dt = new DataTable("ALLDATA");

            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);

            o.GetType().GetProperties().ToList().ForEach(f =>
            {

                f.GetValue(o, null);
                DataColumn dc = new DataColumn();

                Type pt = f.PropertyType;
                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    pt = Nullable.GetUnderlyingType(pt);

                dc.ColumnName = f.Name;
                dc.DataType = pt;

                dt.Columns.Add(dc);
                object value = f.GetValue(o, null);
                dt.Rows[0][f.Name] = value ?? DBNull.Value;
            });

            return dt;
        }

        public static List<T> ToModelList<T>(this DataTable table) where T : class, new()
        {
            List<T> list = new List<T>();
            foreach (var row in table.AsEnumerable())
            {
                T obj = new T();
                foreach (var prop in obj.GetType().GetProperties())
                {
                    PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                    var v = Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType);
                    propertyInfo.SetValue(obj, v == DBNull.Value ? null : v, null);
                }
                list.Add(obj);
            }
            return list;
        }



        public static string GetNullableString(this IDataReader reader, int index)
        {
            object tmp = reader.GetValue(index);
            if (tmp != DBNull.Value)
            {
                return (string)tmp;
            }
            return null;
        }

        public static T? GetNullableValue<T>(this IDataReader reader, int index) where T : struct
        {
            object tmp = reader.GetValue(index);
            if (tmp != DBNull.Value)
            {
                return (T)tmp;
            }
            return null;
        }

        public static T GetValue<T>(this IDataReader reader, int index) where T : struct
        {
            object tmp = reader.GetValue(index);
            if (tmp != DBNull.Value)
            {
                return (T)tmp;
            }
            else
                throw new Exception("Null value");
        }

    }
}

