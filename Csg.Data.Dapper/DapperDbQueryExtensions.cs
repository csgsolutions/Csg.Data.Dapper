using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csg.Data;
using Csg.Data.Common;
using Csg.Data.Sql;
using Dapper;

namespace Csg.Data
{
    public static class DapperDbQueryExtensions
    {
        /// <summary>
        /// Creates a dapper command definition from the given query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static CommandDefinition ToDapperCommand(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered, System.Threading.CancellationToken cancellationToken = default)
        {
            var transaction = query.Features.Get<IDbTransaction>(query);

            return query.Render().ToDapperCommand(transaction, query.Configuration.CommandTimeout, commandFlags, cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="Dapper.CommandDefinition"/> from the given SQL statement.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandFlags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static CommandDefinition ToDapperCommand(this Sql.SqlStatement statement, System.Data.IDbTransaction transaction = null, int? commandTimeout = null, CommandFlags commandFlags = CommandFlags.Buffered, System.Threading.CancellationToken cancellationToken = default)
        {
            var cmd = new CommandDefinition(statement.CommandText,
                commandType: System.Data.CommandType.Text,
                parameters: statement.Parameters.ToDynamicParameters(),
                transaction: transaction,
                commandTimeout: commandTimeout,
                flags: commandFlags,
                cancellationToken: cancellationToken
            );

            return cmd;
        }

        /// <summary>
        /// Creates a Dapper <see cref="Dapper.DynamicParameters"/> object from the given set of parameter names and values.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DynamicParameters ToDynamicParameters(this IEnumerable<DbParameterValue> parameters)
        {
            var dynpar = new DynamicParameters();

            foreach (var param in parameters)
            {
                dynpar.Add(param.ParameterName, param.Value, param.DbType, System.Data.ParameterDirection.Input, param.Size > 0 ? (int?)param.Size : null);
            }

            return dynpar;
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.Query{T}(IDbConnection, CommandDefinition)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered)
        {
           return SqlMapper.Query<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags));
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.QueryAsync{T}(IDbConnection, CommandDefinition)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered, System.Threading.CancellationToken cancellationToken = default)
        {
            return SqlMapper.QueryAsync<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags, cancellationToken: cancellationToken));
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.QuerySingle(IDbConnection, CommandDefinition)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static T QuerySingle<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered)
        {
            return SqlMapper.QuerySingle<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags));
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.QuerySingleAsync(IDbConnection, CommandDefinition)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static Task<T> QuerySingleAsync<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered, System.Threading.CancellationToken cancellationToken = default)
        {
            return SqlMapper.QuerySingleAsync<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags, cancellationToken: cancellationToken));
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.QuerySingleOrDefault(IDbConnection, CommandDefinition)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static T QuerySingleOrDefault<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered)
        {
            return SqlMapper.QuerySingleOrDefault<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags));
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.QuerySingleOrDefaultAsync(IDbConnection, CommandDefinition)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static Task<T> QuerySingleOrDefaultAsync<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered, System.Threading.CancellationToken cancellationToken = default)
        {
            return SqlMapper.QuerySingleOrDefaultAsync<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags, cancellationToken: cancellationToken));
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.QueryFirst{T}(IDbConnection, CommandDefinition)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static T QueryFirst<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered)
        {
            return SqlMapper.QueryFirst<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags));
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.QueryFirstAsync{T}(IDbConnection, CommandDefinition)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static Task<T> QueryFirstAsync<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered, System.Threading.CancellationToken cancellationToken = default)
        {
            return SqlMapper.QueryFirstAsync<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags, cancellationToken: cancellationToken));
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.QueryFirstOrDefault{T}(IDbConnection, CommandDefinition)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static T QueryFirstOrDefault<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered)
        {
            return SqlMapper.QueryFirstOrDefault<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags));
        }

        /// <summary>
        /// Renders the given query and executes it using <see cref="Dapper.SqlMapper.QueryFirstOrDefaultAsync{T}(IDbConnection, CommandDefinition)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public static Task<T> QueryFirstOrDefaultAsync<T>(this IDbQueryBuilder query, CommandFlags commandFlags = CommandFlags.Buffered, System.Threading.CancellationToken cancellationToken = default)
        {
            return SqlMapper.QueryFirstOrDefaultAsync<T>(query.Features.Get<IDbConnection>(query), ToDapperCommand(query, commandFlags, cancellationToken: cancellationToken));
        }
    }

}