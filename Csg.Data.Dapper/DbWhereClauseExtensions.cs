﻿using Csg.Data.Abstractions;
using Csg.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csg.Data
{
    public static class DbWhereClauseExtensions
    {
        /// <summary>
        /// Adds an equals condition to the given query's where clause for the given field and string value.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fieldName"></param>
        /// <param name="equalsValue"></param>
        /// <returns></returns>
        public static IWhereClause FieldEquals(this IWhereClause query, string fieldName, DbString equalsValue)
        {
            return query.FieldMatch(fieldName, SqlOperator.Equal, equalsValue);
        }

        /// <summary>
        /// Adds a condition to the given query's where clause using the given values.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fieldName">The field name on which to compare.</param>
        /// <param name="operator">The operator to use.</param>
        /// <param name="value">The value that will be compared against the field.</param>
        /// <returns></returns>
        public static IWhereClause FieldMatch(this IWhereClause query, string fieldName, Csg.Data.Sql.SqlOperator @operator, DbString value)
        {
            var filter = new Csg.Data.Sql.SqlCompareFilter<string>(query.Root, fieldName, @operator, value.Value);

            if (value.IsAnsi && value.IsFixedLength)
            {
                filter.DataType = DbType.AnsiStringFixedLength;
            }
            else if (value.IsAnsi)
            {
                filter.DataType = DbType.AnsiString;
            }
            else if (value.IsFixedLength)
            {
                filter.DataType = DbType.StringFixedLength;
            }
            else
            {
                filter.DataType = DbType.String;
            }

            if (value.Length >= 0)
            {
                filter.Size = value.Length;
            }

            query.AddFilter(filter);

            return query;
        }

        /// <summary>
        /// Adds a string pattern matching filter to the query's where clause.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fieldName"></param>
        /// <param name="operator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IWhereClause StringMatch(this IWhereClause query, string fieldName, Csg.Data.Sql.SqlWildcardDecoration @operator, DbString value)
        {
            var filter = new Csg.Data.Sql.SqlStringMatchFilter(query.Root, fieldName, @operator, value.Value);

            if (value.IsAnsi && value.IsFixedLength)
            {
                filter.DataType = DbType.AnsiStringFixedLength;
            }
            else if (value.IsAnsi)
            {
                filter.DataType = DbType.AnsiString;
            }
            else if (value.IsFixedLength)
            {
                filter.DataType = DbType.StringFixedLength;
            }
            else
            {
                filter.DataType = DbType.String;
            }

            if (value.Length >= 0)
            {
                filter.Size = value.Length;
            }

            query.AddFilter(filter);

            return query;
        }

        /// <summary>
        /// Adds a condition to the given query's where clause using the given values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="fieldName"></param>
        /// <param name="operator"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static IWhereClause FieldMatch<T>(this IWhereClause query, string fieldName, SqlOperator @operator, DbDate<T> date) where T : struct
        {
            query.AddFilter(new Csg.Data.Sql.SqlCompareFilter(query.Root, fieldName, @operator, date.GetDbType(), date.Value));

            return query;
        }

        /// <summary>
        /// Adds a range condition to the given query's where clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="fieldName"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IWhereClause FieldBetween<T>(this IWhereClause query, string fieldName, DbDate<T> begin, DbDate<T> end) where T : struct
        {
            query.FieldMatch(fieldName, SqlOperator.GreaterThanOrEqual, begin).FieldMatch(fieldName, SqlOperator.LessThanOrEqual, end);

            return query;
        }
    }
}
