// <copyright file="ChangeTextCommand.cs" company="PlaceholderCompany">
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
    /// Commands for the changing of cell text.
    /// </summary>
    public class ChangeTextCommand : ISpreadSheetCommand
    {
        private GenericCell cell;
        private string newText;
        private string oldText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTextCommand"/> class.
        /// </summary>
        /// <param name="cell">passes in cell that is being effected.</param>
        /// <param name="newText">New string gets passed in. </param>
        public ChangeTextCommand(GenericCell cell, string newText)
        {
            this.cell = cell;
            if (cell.CellText == string.Empty)
            {
                this.oldText = " ";
            }

            this.oldText = cell.CellText;
            this.newText = newText;
        }

        /// <summary>
        /// Gets texts to be inserted.
        /// </summary>
        public string Description => "Change Text";

        /// <summary>
        /// Undos the celltext change made.
        /// </summary>
        public void Execute()
        {
            this.cell.CellText = this.newText;
        }

        /// <summary>
        /// makes the current text what its old statement was.
        /// </summary>
        public void UnExecute()
        {
            this.cell.CellText = this.oldText;
        }
    }
}
