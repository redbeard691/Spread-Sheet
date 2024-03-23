using SpreadSheet_Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Unit_SpreadSheet_Engine
{
    public class HW9_HW10
    {

        private MYSpreadSheet spreadSheet;
        private GenericCell cellA;
        private GenericCell cellB;
        private GenericCell cellC;

        [SetUp]
        public void Setup()
        {
            // Initialize the spreadsheet and cells
            spreadSheet = new MYSpreadSheet(3, 3); // Assuming 3x3 spreadsheet for simplicity
            cellA.CellText = "=B1" ; // A1
            cellB.CellText = "=C1"; // B1
            cellC.CellText = "=A1"; // C1

            // Set up dependencies
            spreadSheet.Dependencies[cellA] = new List<GenericCell> { cellB };
            spreadSheet.Dependencies[cellB] = new List<GenericCell> { cellC };
        }

        [Test]
        public void IsCircularReference_WhenCircular_ShouldReturnTrue()
        {
            // Creating a circular dependency: A -> B -> C -> A
            spreadSheet.Dependencies[cellC] = new List<GenericCell> { cellA };

            bool result = spreadSheet.IsCircularReference(cellC, cellA);

            Assert.IsTrue(result, "Circular reference should be detected.");
        }

        [Test]
        public void SaveAndLoadTest()
        {
            // Arrange
            var spreadsheet = new MYSpreadSheet(10, 10);
            var testCellValue = "Test Value";
            spreadsheet.ChangeCell(0, 0, testCellValue); // Change cell at A1

            // Act - Save
            using (var memoryStream = new MemoryStream())
            {
                spreadsheet.Save(memoryStream);
                memoryStream.Position = 0; // Reset the stream position for reading

                // Act - Load
                var loadedSpreadsheet = new MYSpreadSheet(10, 10);
                loadedSpreadsheet.Load(memoryStream);

                // Assert
                var loadedValue = loadedSpreadsheet.GetCell(0, 0).CellText;
                Assert.AreEqual(testCellValue, loadedValue, "Loaded cell value should match the saved value.");
            }
        }
    }
}
