using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Fabio.SharpTools.Convertion
{

    public delegate object[] ConvertionDataTableDelegate(object obj);

    public sealed class Options
    {
        public Type Type { get; private set; }
        public ReadOnlyCollection<MemberInfo> Members { get; private set; }

        public Options(Type type, IList<MemberInfo> members)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (members == null) throw new ArgumentNullException("members");

            foreach (MemberInfo member in members)
            {
                if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
                    throw new ArgumentException("May only contan Field and Property Infos.", "members");
            }

            Type = type;
            Members = new ReadOnlyCollection<MemberInfo>(members);
        }
    }

    public static class Extented
    {
        #region helper methods
        private static ConvertionDataTableDelegate getConvertionDataTableMethod(Options options)
        {
            DynamicMethod toObjectArray = new DynamicMethod("toObjectArray" + options.Type.Name, typeof(object[]),
                                                            new Type[] { typeof(object) }, options.Type, true);

            // Begin emitting MSIL code
            ILGenerator ilgen = toObjectArray.GetILGenerator();

            ilgen.DeclareLocal(typeof(object[])); // stored at index 0
            ilgen.Emit(OpCodes.Ldc_I4_S, options.Members.Count); // load count of props+fields on stack
            ilgen.Emit(OpCodes.Newarr, typeof(object)); // declare array
            ilgen.Emit(OpCodes.Stloc_0); // Store array in field 0


            for (int i = 0; i < options.Members.Count; i++)
            {
                // Member info is either PropertyInfo or FieldInfo, PropertyInfo is more likely.
                MemberInfo member = options.Members[i];

                PropertyInfo pi = member as PropertyInfo;
                if (pi != null)
                {
                    ilgen.Emit(OpCodes.Ldloc_0); // Load array on evaluation stack
                    ilgen.Emit(OpCodes.Ldc_I4_S, i); // Load array position on eval stack
                    ilgen.Emit(OpCodes.Ldarg_0); // Load ourselves on the eval stack
                    ilgen.Emit(OpCodes.Call, pi.GetGetMethod());

                    // Check if we need to box a value type
                    if (pi.PropertyType.IsValueType)
                    {
                        ilgen.Emit(OpCodes.Box, pi.PropertyType);
                    }

                    // Store value in array, this pops all fields from eval stack that were added this for loop
                    ilgen.Emit(OpCodes.Stelem_Ref);
                }
                else
                {
                    FieldInfo fi = (FieldInfo)member;
                    // makes sure exception is thrown when cast is invalid. Should never happen.

                    ilgen.Emit(OpCodes.Ldloc_0); // Load array on evaluation stack
                    ilgen.Emit(OpCodes.Ldc_I4_S, i); // Load array position on eval stack
                    ilgen.Emit(OpCodes.Ldarg_0); // Load ourselves on the eval stack
                    ilgen.Emit(OpCodes.Ldfld, fi); // Load field on the stack               

                    // Check if we need to box a value type
                    if (fi.FieldType.IsValueType)
                    {
                        ilgen.Emit(OpCodes.Box, fi.FieldType);
                    }

                    // Store value in array, this pops all fields from stack that were added this for loop
                    ilgen.Emit(OpCodes.Stelem_Ref);
                }
            }

            ilgen.Emit(OpCodes.Ldloc_0); // Load objArray on stack 
            ilgen.Emit(OpCodes.Ret); // return it

            return (ConvertionDataTableDelegate)toObjectArray.CreateDelegate(typeof(ConvertionDataTableDelegate));
        }

        private static Options provideConvertionDataTableOptions(Type t)
        {
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);

            List<MemberInfo> members = new List<MemberInfo>();
            members.AddRange(fields.ToList<MemberInfo>());
            members.AddRange(props.ToList<MemberInfo>());

            return new Options(t, members);
        }

        private static void buildTableSchema(DataTable table, Options options)
        {
            foreach (MemberInfo member in options.Members)
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = member.Name;

                // Member info is either PropertyInfo or FieldInfo, PropertyInfo is more likely.
                PropertyInfo p = member as PropertyInfo;
                if (p != null)
                {
                    if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        dc.DataType = p.PropertyType.GetGenericArguments()[0];
                        dc.AllowDBNull = true;
                    }
                    else
                    {
                        dc.DataType = p.PropertyType;
                    }
                }
                else
                {
                    FieldInfo f = (FieldInfo)member; // makes sure exception is thrown when cast is invalid. Should never happen.
                    if (f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        dc.DataType = f.FieldType.GetGenericArguments()[0];
                        dc.AllowDBNull = true;
                    }
                    else
                    {
                        dc.DataType = f.FieldType;
                    }
                }

                // Add column to table
                table.Columns.Add(dc);
            }
        }
        #endregion

        /// <summary>
        /// Converts an IEnumerable object to DataTable
        /// </summary>
        /// <param name="source">IEnumerable object, List or GenericList for example</param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IEnumerable source)
        {
            DataTable table = new DataTable();

            Options m_ConvertionDataTableOptions = null;
            ConvertionDataTableDelegate m_ConvertionDataTableMethod = null;

            // Begin enumeration
            IEnumerator e = source.GetEnumerator();

            // attempt to pick first element
            if (e.MoveNext())
            {
                Type srctype = e.Current.GetType();

                m_ConvertionDataTableOptions = provideConvertionDataTableOptions(srctype);
                m_ConvertionDataTableMethod = getConvertionDataTableMethod(m_ConvertionDataTableOptions);

                // Table must be initialized everytime
                buildTableSchema(table, m_ConvertionDataTableOptions);

                // Load data into table
                table.BeginLoadData();

                do
                {
                    table.Rows.Add(m_ConvertionDataTableMethod.Invoke(e.Current));
                } while (e.MoveNext());

                table.EndLoadData();
            }
            else
            {
                if (m_ConvertionDataTableOptions != null)
                {
                    buildTableSchema(table, m_ConvertionDataTableOptions);
                }
                else
                {
                    // source is empty, attempt to build schema through the generic IEnumerable implementation
                    Type t = e.GetType();

                    if (t.IsGenericType)
                    {
                        m_ConvertionDataTableOptions = provideConvertionDataTableOptions(t);
                        buildTableSchema(table, m_ConvertionDataTableOptions);
                        return table;
                    }
                }


                // cant build any schema, non generic source and empty
                return null;
            }

            return table;
        }

        public static object RowsToDictionary(this DataTable table)
        {

            var columns = table.Columns.Cast<DataColumn>().ToArray();

            return table.Rows.Cast<DataRow>().Select(r => columns.ToDictionary(c => c.ColumnName, c => r[c]));

        }

        public static Dictionary<string, object> ToDictionary(this DataTable table)
        {

            return new Dictionary<string, object>
               {
                   { table.TableName, table.RowsToDictionary() }
               };

        }

        public static Dictionary<string, object> ToDictionary(this DataSet data)
        {

            return data.Tables.Cast<DataTable>().ToDictionary(t => t.TableName, t => t.RowsToDictionary());

        }

    }
}
