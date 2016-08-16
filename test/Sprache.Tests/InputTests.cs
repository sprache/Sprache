using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Sprache.Tests
{
    public class InputTests
    {
        [Fact]
        public void InputsOnTheSameString_AtTheSamePosition_AreEqual()
        {
            var s = "Nada";
            var p = 2;
            var i1 = new Input(s, p);
            var i2 = new Input(s, p);
            Assert.Equal(i1, i2);
            Assert.True(i1 == i2);
        }

        [Fact]
        public void InputsOnTheSameString_AtDifferentPositions_AreNotEqual()
        {
            var s = "Nada";
            var i1 = new Input(s, 1);
            var i2 = new Input(s, 2);
            Assert.NotEqual(i1, i2);
            Assert.True(i1 != i2);
        }

        [Fact]
        public void InputsOnDifferentStrings_AtTheSamePosition_AreNotEqual()
        {
            var p = 2;
            var i1 = new Input("Algo", p);
            var i2 = new Input("Nada", p);
            Assert.NotEqual(i1, i2);
        }

        [Fact]
        public void InputsAtEnd_CannotAdvance()
        {
            var i = new Input("", 0);
            Assert.True(i.AtEnd);
            Assert.Throws<InvalidOperationException>(() => i.Advance());
        }

        [Fact]
        public void AdvancingInput_MovesForwardOneCharacter()
        {
            var i = new Input("abc", 1);
            var j = i.Advance();
            Assert.Equal(2, j.Position);
        }

        [Fact]
        public void CurrentCharacter_ReflectsPosition()
        {
            var i = new Input("abc", 1);
            Assert.Equal('b', i.Current);
        }

        [Fact]
        public void ANewInput_WillBeAtFirstCharacter()
        {
            var i = new Input("abc");
            Assert.Equal(0, i.Position);
        }

        [Fact]
        public void AdvancingInput_IncreasesColumnNumber()
        {
            var i = new Input("abc", 1);
            var j = i.Advance();
            Assert.Equal(2, j.Column);
        }
        [Fact]
        public void AdvancingInputAtEOL_IncreasesLineNumber()
        {
            var i = new Input("\nabc");
            var j = i.Advance();
            Assert.Equal(2, j.Line);
        }

        [Fact]
        public void AdvancingInputAtEOL_ResetsColumnNumber()
        {
            var i = new Input("\nabc");
            var j = i.Advance();
            Assert.Equal(2, j.Line);
            Assert.Equal(1, j.Column);
        }

        [Fact]
        public void LineCountingSmokeTest()
        {
            IInput i = new Input("abc\ndef");
            Assert.Equal(0, i.Position);
            Assert.Equal(1, i.Line);
            Assert.Equal(1, i.Column);

            i = i.AdvanceAssert((a, b) =>
            {
                Assert.Equal(1, b.Position);
                Assert.Equal(1, b.Line);
                Assert.Equal(2, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.Equal(2, b.Position);
                Assert.Equal(1, b.Line);
                Assert.Equal(3, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.Equal(3, b.Position);
                Assert.Equal(1, b.Line);
                Assert.Equal(4, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.Equal(4, b.Position);
                Assert.Equal(2, b.Line);
                Assert.Equal(1, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.Equal(5, b.Position);
                Assert.Equal(2, b.Line);
                Assert.Equal(2, b.Column);
            });
            i = i.AdvanceAssert((a, b) =>
            {
                Assert.Equal(6, b.Position);
                Assert.Equal(2, b.Line);
                Assert.Equal(3, b.Column);
            });
        }
    }
}
