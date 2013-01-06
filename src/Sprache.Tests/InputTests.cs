using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Sprache.Tests
{
    [TestFixture]
    public class InputTests
    {
        [Test]
        public void InputsOnTheSameString_AtTheSamePosition_AreEqual()
        {
            var s = "Nada";
            var p = 2;
            var i1 = new Input(s, p);
            var i2 = new Input(s, p);
			Assert.AreEqual(i1, i2);
			Assert.IsTrue(i1 == i2);
		}

        [Test]
        public void InputsOnTheSameString_AtDifferentPositions_AreNotEqual()
        {
            var s = "Nada";
            var i1 = new Input(s, 1);
            var i2 = new Input(s, 2);
            Assert.AreNotEqual(i1, i2);
			Assert.IsTrue(i1 != i2);
		}

        [Test]
        public void InputsOnDifferentStrings_AtTheSamePosition_AreNotEqual()
        {
            var p = 2;
            var i1 = new Input("Algo", p);
            var i2 = new Input("Nada", p);
            Assert.AreNotEqual(i1, i2);
        }

        [Test]
        public void InputsAtEnd_CannotAdvance()
        {
            var i = new Input("", 0);
            Assert.IsTrue(i.AtEnd);
            Assert.Throws<InvalidOperationException>(() => i.Advance());
        }

        [Test]
        public void AdvancingInput_MovesForwardOneCharacter()
        {
            var i = new Input("abc", 1);
            var j = i.Advance();
            Assert.AreEqual(2, j.Position);
        }

        [Test]
        public void CurrentCharacter_ReflectsPosition()
        {
            var i = new Input("abc", 1);
            Assert.AreEqual('b', i.Current);
        }

        [Test]
        public void ANewInput_WillBeAtFirstCharacter()
        {
            var i = new Input("abc");
            Assert.AreEqual(0, i.Position);
        }

        [Test]
        public void AdvancingInput_IncreasesColumnNumber()
        {
            var i = new Input("abc", 1);
            var j = i.Advance();
            Assert.AreEqual(2, j.Column);
        }
        [Test]
        public void AdvancingInputAtEOL_IncreasesLineNumber()
        {
            var i = new Input("\nabc");
            var j = i.Advance();
            Assert.AreEqual(2, j.Line);
        }

        [Test]
        public void AdvancingInputAtEOL_ResetsColumnNumber()
        {
            var i = new Input("\nabc");
            var j = i.Advance();
            Assert.AreEqual(2, j.Line);
            Assert.AreEqual(1, j.Column);
        }

        [Test]
        public void LineCountingSmokeTest()
        {
            var i = new Input("abc\ndef");
            Assert.AreEqual(0, i.Position);
            Assert.AreEqual(1, i.Line);
            Assert.AreEqual(1, i.Column);

            i = i.AdvanceAssert((a, b) =>
            {
                Assert.AreEqual(1, b.Position);
                Assert.AreEqual(1, b.Line);
                Assert.AreEqual(2, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.AreEqual(2, b.Position);
                Assert.AreEqual(1, b.Line);
                Assert.AreEqual(3, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.AreEqual(3, b.Position);
                Assert.AreEqual(1, b.Line);
                Assert.AreEqual(4, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.AreEqual(4, b.Position);
                Assert.AreEqual(2, b.Line);
                Assert.AreEqual(1, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.AreEqual(5, b.Position);
                Assert.AreEqual(2, b.Line);
                Assert.AreEqual(2, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.AreEqual(6, b.Position);
                Assert.AreEqual(2, b.Line);
                Assert.AreEqual(3, b.Column);
            });
        }
    }
}
