﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.mysql
{
    /// <summary>
    /// [mysql.connect] slot for connecting to a MySQL server instance.
    /// </summary>
	[Slot(Name = "mysql.connect")]
	public class Connect : ISlot, ISlotAsync
	{
        readonly IConfiguration _configuration;

        /// <summary>
        /// Creates a new instance of your class.
        /// </summary>
        /// <param name="configuration">Configuration for your application.</param>
        public Connect(IConfiguration configuration)
		{
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Handles the signal for the class.
        /// </summary>
        /// <param name="signaler">Signaler used to signal the slot.</param>
        /// <param name="input">Root node for invocation.</param>
		public void Signal(ISignaler signaler, Node input)
		{
            var connectionString = GetConnectionString(input);

            using (var connection = new MySqlConnection(connectionString))
			{
                connection.Open();
                signaler.Scope("mysql.connect", connection, () =>
                {
                    signaler.Signal("eval", input);
                });
                input.Value = null;
            }
        }

        /// <summary>
        /// Handles the signal for the class.
        /// </summary>
        /// <param name="signaler">Signaler used to signal the slot.</param>
        /// <param name="input">Root node for invocation.</param>
        /// <returns>An awaitable task.</returns>
		public async Task SignalAsync(ISignaler signaler, Node input)
        {
            var connectionString = GetConnectionString(input);

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                signaler.Scope("mysql.connect", connection, () =>
                {
                    signaler.Signal("eval", input);
                });
                input.Value = null;
            }
        }

        #region [ -- Private helper methods -- ]

        string GetConnectionString(Node input)
        {
            var connectionString = input.GetEx<string>();

            // Checking if this is a "generic connection string".
            if (connectionString.StartsWith("[", StringComparison.InvariantCulture) &&
                connectionString.EndsWith("]", StringComparison.InvariantCulture))
            {
                var generic = _configuration["databases:mysql:generic"];
                connectionString = generic.Replace("{database}", connectionString.Substring(1, connectionString.Length - 2));
            }
            else if (!connectionString.Contains(";"))
            {
                var generic = _configuration["databases:mysql:generic"];
                connectionString = generic.Replace("{database}", connectionString);
            }
            return connectionString;
        }

        #endregion
    }
}
