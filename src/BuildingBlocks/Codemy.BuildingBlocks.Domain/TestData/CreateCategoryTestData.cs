using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.BuildingBlocks.Domain.TestData
{
    public class CreateCategoryTestData
    {
        public string TestCase { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ExpectedStatus { get; set; }
        public bool ExpectedSuccess { get; set; }
        public string ExpectedMessage { get; set; }
        public string Description_TestCase { get; set; }
    }
}
