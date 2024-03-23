using SpreadSheet_Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Unit_SpreadSheet_Engine
{
    internal class HW7Tests
    {
        [Test]
        public void UpdateCellDependenciesTest()
        {
            // Arrange
            var spreadsheet = new MYSpreadSheet(10, 10);
            spreadsheet.ChangeCell(0, 1, "5"); // Set cell B1 to 5
            spreadsheet.ChangeCell(0, 0, "=B1 * 2"); // Set cell A1 formula to "=B1 * 2"

            // Act
            var cellValue = spreadsheet.GetCell(0, 0).CellValue;

            // Assert
            Assert.AreEqual("10", cellValue, "Cell A1 should reflect the formula based on B1's value.");

            // Further testing can include changing B1 and ensuring A1 updates correctly
            spreadsheet.ChangeCell(0, 1, "10"); // Change B1 to 10
            cellValue = spreadsheet.GetCell(0, 0).CellValue;
            Assert.AreEqual("20", cellValue, "Cell A1 should update based on B1's new value.");
        }

    }
}
