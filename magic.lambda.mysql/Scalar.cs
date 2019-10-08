﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using magic.node;
using magic.signals.contracts;
using magic.lambda.mysql.utilities;

namespace magic.lambda.mysql
{
    /// <summary>
    /// [mysql.scalar] slot for executing a scalar type of SQL command.
    /// </summary>
    [Slot(Name = "mysql.scalar")]
    public class Scalar : ISlot, ISlotAsync
    {
        /// <summary>
        /// Handles the signal for the class.
        /// </summary>
        /// <param name="signaler">Signaler used to signal the slot.</param>
        /// <param name="input">Root node for invocation.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            Executor.Execute(input, signaler.Peek<MySqlConnection>("mysql.connect"), (cmd) =>
            {
                input.Value = cmd.ExecuteScalar();
            });
        }

        /// <summary>
        /// Handles the signal for the class.
        /// </summary>
        /// <param name="signaler">Signaler used to signal the slot.</param>
        /// <param name="input">Root node for invocation.</param>
        /// <returns>An awaitable task.</returns>
		public async Task SignalAsync(ISignaler signaler, Node input)
        {
            await Executor.ExecuteAsync(input, signaler.Peek<MySqlConnection>("mysql.connect"), async (cmd) =>
            {
                input.Value = await cmd.ExecuteScalarAsync();
            });
        }
    }
}
