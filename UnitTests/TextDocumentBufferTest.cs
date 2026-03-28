using CsharpPieceTableImplementation;
using System;
using System.Linq;
using Xunit;

namespace UnitTests
{
    public class TextDocumentBufferTest
    {
        private const string Text
            = "During the development of the .NET Framework, the class libraries were originally written using a managed code compiler system called \"Simple Managed C\" (SMC).";

        private TextDocumentBuffer<char> _textDocumentBuffer;

        public TextDocumentBufferTest()
        {
            _textDocumentBuffer
                = new TextDocumentBuffer<char>(
                    Text.ToArray());
        }

        [Fact]
        public void DocumentLength_OriginalText()
        {
            Assert.Equal(Text.Length, _textDocumentBuffer.DocumentLength);
        }

        [Fact]
        public void GetCharacter_OriginalDocumentUnchanged()
        {
            Assert.Equal(Text[0], _textDocumentBuffer[0]);
            Assert.Equal(Text[^1], _textDocumentBuffer[_textDocumentBuffer.DocumentLength - 1]);
        }

        [Fact]
        public void GetText_FullOriginalDocumentUnchanged()
        {
            Assert.Equal(Text, GetFullDocument());
        }

        [Fact]
        public void GetText_ExtractBeginningOfOriginalDocumentUnchanged()
        {
            string documentText = _textDocumentBuffer.GetText(new Span(0, 2)).ToStr();
            Assert.Equal(Text[..2], documentText);
        }

        [Fact]
        public void GetText_ExtractEndOfOriginalDocumentUnchanged()
        {
            string documentText = _textDocumentBuffer.GetText(new Span(_textDocumentBuffer.DocumentLength - 2, 2)).ToStr();
            Assert.Equal(Text.Substring(Text.Length - 2, 2), documentText);
        }

        [Fact]
        public void GetText_ExtractMiddleOfOriginalDocumentUnchanged()
        {
            string documentText = _textDocumentBuffer.GetText(new Span(1, 2)).ToStr();
            Assert.Equal(Text.Substring(1, 2), documentText);
        }

        [Fact]
        public void Insert_EmptyDocument()
        {
            _textDocumentBuffer
                = new TextDocumentBuffer<char>([]);

            string appendText = "TEST!";
            _textDocumentBuffer.Insert(0, appendText.ToCharArray());

            Assert.Equal(appendText.Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal(appendText, GetFullDocument());
        }

        [Fact]
        public void Insert_EndOfDocument()
        {
            string appendText = "TEST!";
            _textDocumentBuffer.Insert(Text.Length, appendText.ToCharArray());

            Assert.Equal(Text.Length + appendText.Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text + appendText, GetFullDocument());
        }

        [Fact]
        public void Insert_BeginningOfDocument()
        {
            string appendText = "TEST!";
            _textDocumentBuffer.Insert(0, appendText.ToCharArray());

            Assert.Equal(appendText.Length + Text.Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal(appendText + Text, GetFullDocument());
        }

        [Fact]
        public void Insert_InsideOfOriginalDocument()
        {
            string appendText = "TEST!";
            _textDocumentBuffer.Insert(2, appendText.ToCharArray());

            Assert.Equal(appendText.Length + Text.Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal(string.Concat(Text.AsSpan(0, 2), appendText, Text.AsSpan(2)), GetFullDocument());
        }

        [Fact]
        public void Insert_BeginningOfAppendBufferPiece()
        {
            string appendText = "TEST!";
            _textDocumentBuffer.Insert(2, appendText.ToCharArray());

            string appendText2 = "HelloThere";
            _textDocumentBuffer.Insert(2, appendText2.ToCharArray());

            Assert.Equal(appendText.Length + appendText2.Length + Text.Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal(string.Concat(Text.AsSpan(0, 2), appendText2, appendText, Text.AsSpan(2)), GetFullDocument());
        }

        [Fact]
        public void Insert_EndOfAppendBufferPiece()
        {
            string appendText = "TEST!";
            _textDocumentBuffer.Insert(2, appendText.ToCharArray());

            string appendText2 = "HelloThere";
            _textDocumentBuffer.Insert(2 + appendText.Length, appendText2.ToCharArray());

            Assert.Equal(appendText.Length + appendText2.Length + Text.Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text[..2] + appendText + appendText2 + Text[2..], GetFullDocument());
        }

        [Fact]
        public void Insert_InsideOfAppendBufferPiece()
        {
            string appendText = "TEST!";
            _textDocumentBuffer.Insert(2, appendText.ToCharArray());

            string appendText2 = "HelloThere";
            _textDocumentBuffer.Insert(4, appendText2.ToCharArray());

            Assert.Equal(appendText.Length + appendText2.Length + Text.Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text[..2] + appendText[..2] + appendText2 + appendText.Substring(2, 3) + Text[2..], GetFullDocument());
        }

        [Fact]
        public void Delete_BeginningOfDocument()
        {
            // Remove the first 2 characters of the document.
            _textDocumentBuffer.Delete(new Span(0, 2));

            Assert.Equal(Text.Length - 2, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text[2..], GetFullDocument());
        }

        [Fact]
        public void Delete_EndOfDocument()
        {
            // Remove the last 2 characters of the document.
            _textDocumentBuffer.Delete(new Span(_textDocumentBuffer.DocumentLength - 2, 2));

            Assert.Equal(Text.Length - 2, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text[0..^2], GetFullDocument());
        }

        [Fact]
        public void Delete_InsideOfOriginalDocument()
        {
            _textDocumentBuffer.Delete(new Span(1, 2));

            Assert.Equal(Text.Length - 2, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text[..1] + Text[3..], GetFullDocument());
        }

        [Fact]
        public void Delete_BeginningOfAppendBufferPiece()
        {
            string appendText = "TEST!";
            _textDocumentBuffer.Insert(2, appendText.ToCharArray());

            // Delete "TE".
            _textDocumentBuffer.Delete(new Span(2, 2));

            Assert.Equal(Text.Length + appendText.Length - 2, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text[..2] + appendText[2..] + Text[2..], GetFullDocument());
        }

        [Fact]
        public void Delete_EndOfAppendBufferPiece()
        {
            string appendText = "TEST!";
            _textDocumentBuffer.Insert(2, appendText.ToCharArray());

            // Delete "ST!".
            _textDocumentBuffer.Delete(new Span(2 + appendText.Length - 3, 3));

            Assert.Equal(Text.Length + appendText.Length - 3, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text[..2] + appendText[..2] + Text[2..], GetFullDocument());
        }

        [Fact]
        public void Delete_InsideOfAppendBufferPiece()
        {
            string appendText = "TEST!";
            _textDocumentBuffer.Insert(2, appendText.ToCharArray());

            // Delete "ES".
            _textDocumentBuffer.Delete(new Span(3, 2));

            Assert.Equal(Text.Length + appendText.Length - 2, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text[..2] + appendText[..1] + appendText[3..] + Text[2..], GetFullDocument());
        }

        [Fact]
        public void Delete_AccrossSeveralAppendBufferPieces()
        {
            string appendText = "Hello_";
            _textDocumentBuffer.Insert(2, appendText.ToCharArray());

            string appendText2 = "World!/";
            _textDocumentBuffer.Insert(2 + appendText.Length, appendText2.ToCharArray());

            string appendText3 = "Boo";
            _textDocumentBuffer.Insert(2 + appendText.Length + appendText2.Length, appendText3.ToCharArray());

            string appendText4 = "Foo Bar";
            _textDocumentBuffer.Insert(2 + appendText.Length + appendText2.Length + appendText3.Length, appendText4.ToCharArray());

            // Delete "_World!/BooFoo", so it forms "Hello Bar".
            _textDocumentBuffer.Delete(new Span(2 + "Hello".Length, "_".Length + appendText2.Length + appendText3.Length + "Foo".Length));

            Assert.Equal(Text.Length + "Hello Bar".Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal(Text[..2] + appendText[..5] + appendText4[3..] + Text[2..], GetFullDocument());
        }

        [Fact]
        public void Delete_AccrossSeveralPiecesOfVariousBuffer()
        {
            string originalText = "Hello!";
            _textDocumentBuffer
                = new TextDocumentBuffer<char>(
                    originalText.ToArray());
            Assert.Equal("Hello!", GetFullDocument());

            string appendText = " I'm testing a PieceTable implementation.";
            _textDocumentBuffer.Insert(originalText.Length, appendText.ToCharArray());
            Assert.Equal("Hello! I'm testing a PieceTable implementation.", GetFullDocument());

            string appendText2 = " there";
            _textDocumentBuffer.Insert("Hello".Length, appendText2.ToCharArray());
            Assert.Equal("Hello there! I'm testing a PieceTable implementation.", GetFullDocument());

            _textDocumentBuffer.Delete(new Span(0, "Hello there! ".Length));

            Assert.Equal("I'm testing a PieceTable implementation.".Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal("I'm testing a PieceTable implementation.", GetFullDocument());
        }

        [Fact]
        public void InsertAndDelete()
        {
            Delete_AccrossSeveralPiecesOfVariousBuffer();
            Assert.Equal("I'm testing a PieceTable implementation.", GetFullDocument());

            _textDocumentBuffer.Insert("I'm testing a".Length, "n implementation of".ToCharArray());
            Assert.Equal("I'm testing an implementation of PieceTable implementation.", GetFullDocument());

            _textDocumentBuffer.Delete(new Span("I'm testing an implementation of PieceTable ".Length, "implementation".Length));
            Assert.Equal("I'm testing an implementation of PieceTable .", GetFullDocument());

            _textDocumentBuffer.Insert("I'm testing an implementation of PieceTable ".Length, "data Structure".ToCharArray());
            Assert.Equal("I'm testing an implementation of PieceTable data Structure.", GetFullDocument());

            _textDocumentBuffer.Delete(new Span("I'm testing an implementation of PieceTable data ".Length, "S".Length));
            Assert.Equal("I'm testing an implementation of PieceTable data tructure.", GetFullDocument());

            _textDocumentBuffer.Insert("I'm testing an implementation of PieceTable data ".Length, 's');

            Assert.Equal("I'm testing an implementation of PieceTable data structure.".Length, _textDocumentBuffer.DocumentLength);
            Assert.Equal("I'm testing an implementation of PieceTable data structure.", GetFullDocument());
        }

        [Fact]
        public void Insert_Pressure()
        {
            int max = 10_000_000;

            string expectedResult = Text + new string('a', max);

            for (int i = 0; i < max; i++)
            {
                _textDocumentBuffer.Insert(_textDocumentBuffer.DocumentLength, 'a');
            }

            Assert.Equal(expectedResult, GetFullDocument());

            for (int i = 0; i < max + Text.Length; i++)
            {
                _textDocumentBuffer.Delete(new Span(0, 1));
            }

            Assert.Equal(string.Empty, GetFullDocument());
        }

        private string GetFullDocument()
        {
            string documentText = _textDocumentBuffer.GetText(new Span(0, _textDocumentBuffer.DocumentLength)).ToStr();
            return documentText;
        }
    }
}
