﻿namespace AjPython.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class Parser
    {
        private const char StringChar = '"';
        private const char QuotedStringChar = '\'';
        private const string Operators = "+-/*";
        private const string Separators = "()";

        private TextReader reader;
        private Token lastToken;
        private char lastChar;
        private bool hasChar;

        public Parser(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            this.reader = new StringReader(text);
        }

        public Parser(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            this.reader = reader;
        }

        public Token NextToken()
        {
            if (this.lastToken != null)
            {
                Token t = this.lastToken;
                this.lastToken = null;

                return t;
            }

            char ch;

            try
            {
                ch = this.NextCharSkipBlanks();

                if (char.IsDigit(ch))
                {
                    return this.NextInteger(ch);
                }

                if (char.IsLetter(ch))
                {
                    return this.NextName(ch);
                }

                if (ch == StringChar)
                {
                    return this.NextString();
                }

                if (ch == QuotedStringChar)
                {
                    return this.NextQuotedString();
                }

                if (Separators.Contains(ch))
                {
                    return this.NextSeparator(ch);
                }

                if (Operators.Contains(ch))
                {
                    return this.NextOperator(ch);
                }

                throw new InvalidDataException("Unknown input");
            }
            catch (EndOfInputException)
            {
                return null;
            }
        }

        internal void PushToken(Token token)
        {
            if (this.lastToken != null)
            {
                throw new InvalidOperationException();
            }

            this.lastToken = token;
        }

        private Token NextOperator(char ch)
        {
            return new Token() 
            { 
                TokenType = TokenType.Operator, 
                Value = ch.ToString() 
            };
        }

        private Token NextSeparator(char ch)
        {
            return new Token()
            {
                TokenType = TokenType.Separator,
                Value = ch.ToString()
            };
        }

        private Token NextString()
        {
            StringBuilder sb = new StringBuilder();
            char ch;

            try
            {
                ch = this.NextChar();

                while (ch != StringChar)
                {
                    sb.Append(ch);
                    ch = this.NextChar();
                }
            }
            catch (EndOfInputException)
            {
            }

            Token token = new Token();
            token.Value = sb.ToString();
            token.TokenType = TokenType.String;

            return token;
        }

        private Token NextQuotedString()
        {
            StringBuilder sb = new StringBuilder();
            char ch;

            try
            {
                ch = this.NextChar();

                while (ch != QuotedStringChar)
                {
                    sb.Append(ch);
                    ch = this.NextChar();
                }
            }
            catch (EndOfInputException)
            {
            }

            Token token = new Token();
            token.Value = sb.ToString();
            token.TokenType = TokenType.QuotedString;

            return token;
        }

        private Token NextInteger(char ch)
        {
            string integer = ch.ToString();

            try
            {
                ch = this.NextChar();

                while (char.IsDigit(ch))
                {
                    integer += ch;
                    ch = this.NextChar();
                }

                this.PushChar(ch);
            }
            catch (EndOfInputException)
            {
            }

            Token token = new Token();
            token.Value = integer;
            token.TokenType = TokenType.Integer;

            return token;
        }

        private Token NextName(char ch)
        {
            string name = ch.ToString();

            try
            {
                ch = this.NextChar();

                while (char.IsLetterOrDigit(ch))
                {
                    name += ch;
                    ch = this.NextChar();
                }

                this.PushChar(ch);
            }
            catch (EndOfInputException)
            {
            }

            Token token = new Token();
            token.Value = name;
            token.TokenType = TokenType.Name;

            return token;
        }

        private char NextCharSkipBlanks()
        {
            char ch;

            ch = this.NextChar();

            while (char.IsWhiteSpace(ch))
            {
                ch = this.NextChar();
            }

            return ch;
        }

        private void PushChar(char ch)
        {
            this.lastChar = ch;
            this.hasChar = true;
        }

        private char NextChar()
        {
            if (this.hasChar)
            {
                this.hasChar = false;
                return this.lastChar;
            }

            int ch;

            if (this.reader.Equals(System.Console.In) && this.reader.Peek() < 0)
            {
                Console.Out.Write("> ");
                Console.Out.Flush();
            }

            ch = this.reader.Read();

            if (ch < 0)
            {
                throw new EndOfInputException();
            }

            return Convert.ToChar(ch);
        }
    }
}

