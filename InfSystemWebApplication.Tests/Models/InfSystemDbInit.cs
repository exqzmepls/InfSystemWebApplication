using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfSystemWebApplication.Models;
using System;
using System.Linq;

namespace InfSystemWebApplication.Tests.Models
{
    [TestClass]
    public class InfSystemDbInit
    {
        string connection = "UnitTestDb";

        [TestMethod]
        public void PositionsInitializerTest()
        {
            int expected = 3;

            using (InfSystemContext db = new InfSystemContext(connection))
            {
                db.Positions.RemoveRange(db.Positions);

                PositionsInitializer.Initialize(db);

                db.SaveChanges();

                Assert.AreEqual(expected, db.Positions.Count());
            }
        }

        [TestMethod]
        public void OKEIInitializerTest()
        {
            using (InfSystemContext db = new InfSystemContext(connection))
            {
                db.Units.RemoveRange(db.Units);

                OKEIInitializer.Initialize(db);

                db.SaveChanges();

                Assert.IsTrue(db.Units.Any());
            }
        }

        [TestMethod]
        public void OKPDInitializerTest()
        {
            using (InfSystemContext db = new InfSystemContext(connection))
            {
                db.ProductClasses.RemoveRange(db.ProductClasses);
                db.SaveChanges();

                OKPDInitializer.Initialize(db);

                db.SaveChanges();

                Assert.IsTrue(db.ProductClasses.Any());
                Assert.IsTrue(db.ProductSubClasses.Any());
                Assert.IsTrue(db.ProductGroups.Any());
                Assert.IsTrue(db.ProductSubGroups.Any());
                Assert.IsTrue(db.ProductKinds.Any());
                Assert.IsTrue(db.ProductCategories.Any());
            }
        }
    }
}
