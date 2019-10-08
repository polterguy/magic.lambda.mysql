﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using magic.node;
using magic.node.extensions;

namespace magic.lambda.mysql.utilities
{
    /*
     * Helper class for creating an executing a MySQL command.
     */
	internal static class Executor
    {
        /*
         * Creates a MySQL command, by parametrizing it with each
         * child node specified in the invocation node, for then to invoke the
         * lambda callback responsible for actually executing the command.
         */
        public static void Execute(
            Node input,
            MySqlConnection connection,
            Action<MySqlCommand> functor)
        {
            using (var cmd = new MySqlCommand(input.GetEx<string>(), connection))
            {
                foreach (var idxPar in input.Children)
                {
                    cmd.Parameters.AddWithValue(idxPar.Name, idxPar.Get<object>());
                }

                // Making sure we clean nodes before invoking lambda callback.
                input.Value = null;
                input.Clear();

                // Invoking lambda callback supplied by caller.
                functor(cmd);
            }
        }

        /*
         * Creates and parametrizes a SQL command, with the specified parameters,
         * for then to invoke the specified callback with the command.
         */
        public static async Task ExecuteAsync(
            Node input,
            MySqlConnection connection,
            Func<MySqlCommand, Task> functor)
        {
            using (var cmd = new MySqlCommand(input.GetEx<string>(), connection))
            {
                foreach (var idxPar in input.Children)
                {
                    cmd.Parameters.AddWithValue(idxPar.Name, idxPar.Get<object>());
                }

                // Making sure we clean nodes before invoking lambda callback.
                input.Value = null;
                input.Clear();

                // Invoking lambda callback supplied by caller.
                await functor(cmd);
            }
        }
    }
}
