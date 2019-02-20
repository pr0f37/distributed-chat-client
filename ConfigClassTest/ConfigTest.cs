using ChatClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ConfigClassTest
{
    
    
    /// <summary>
    ///This is a test class for ConfigTest and is intended
    ///to contain all ConfigTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConfigTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for LoadConfigFile
        ///</summary>
        [TestMethod()]
        public void LoadConfigFileTest1()
        {
            string fileName = "config.xml"; // TODO: Initialize to an appropriate value
            Config target = new Config(fileName); // TODO: Initialize to an appropriate value
            target.LoadConfigFile();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for LoadConfigFile
        ///</summary>
        [TestMethod()]
        public void LoadConfigFileTest()
        {
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            Config target = new Config(fileName); // TODO: Initialize to an appropriate value
            string fileName1 = string.Empty; // TODO: Initialize to an appropriate value
            target.LoadConfigFile(fileName1);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Config Constructor
        ///</summary>
        [TestMethod()]
        public void ConfigConstructorTest()
        {
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            Config target = new Config(fileName);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
