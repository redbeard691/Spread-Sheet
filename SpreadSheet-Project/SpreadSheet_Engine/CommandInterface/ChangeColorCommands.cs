namespace SpreadSheet_Engine.CommandInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ChangeColorCommands : ISpreadSheetCommand
    {
        private readonly GenericCell cell;
        private uint oldColor;
        private uint newColor;

        public ChangeColorCommands(GenericCell cell, uint newColor)
        {
            this.cell = cell;
            this.newColor = newColor;
            this.oldColor = cell.BGColor;
        }

        public string Description => "Change Color";

        public void Execute()
        {
            cell.BGColor = this.newColor;
        }

        public void UnExecute()
        {
            cell.BGColor = this.oldColor;
        }

    }
}
