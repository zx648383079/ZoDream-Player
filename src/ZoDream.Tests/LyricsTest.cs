using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.Shared.Readers;

namespace ZoDream.Tests
{
    [TestClass]
    public class LyricsTest
    {
        [TestMethod]
        public void TestKrc()
        {
            var file = "F:\\Desktop\\1.lrc";
            var reader = new LrcReader();
            var res = reader.ReadAsync(file).Result;
            Assert.AreNotEqual(res, string.Empty);
        }
    }
}