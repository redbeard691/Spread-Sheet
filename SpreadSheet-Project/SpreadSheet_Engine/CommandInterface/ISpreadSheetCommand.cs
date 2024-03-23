// <copyright file="ISpreadSheetCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheet_Engine.CommandInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstract interface for changing commands.
    /// </summary>
    public interface ISpreadSheetCommand
    {
        /// <summary>
        /// Gets a description of what the command does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Will execute a undo command.
        /// </summary>
        void Execute();

        /// <summary>
        /// Executes a undo command.
        /// </summary>
        void UnExecute();
    }
}
