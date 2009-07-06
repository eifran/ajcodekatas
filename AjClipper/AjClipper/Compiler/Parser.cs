﻿namespace AjClipper.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjClipper.Commands;
    using AjClipper.Expressions;

    public class Parser
    {
        private Lexer lexer;

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }

        public Parser(TextReader reader)
            : this(new Lexer(reader))
        {
        }

        public Parser(string text)
            : this(new Lexer(text))
        {
        }

        public ICommand ParseCommand()
        {
            return this.ParseLineCommand();
        }

        private ICommand ParseLineCommand()
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                return null;

            if (token.Value == "?")
                return new PrintLineCommand(this.ParseExpressionList());

            if (token.TokenType == TokenType.Name)
            {
                if (token.Value == "if")
                    return this.ParseIfCommand();

                Token token2 = this.lexer.NextToken();

                if (token2 != null && (token2.Value == ":=" || token2.Value == "="))
                    return new SetVariableCommand(token.Value, this.ParseExpression());
            }

            this.lexer.PushToken(token);

            return null;
        }

        private ICommand ParseIfCommand()
        {
            IfCommand ifCommand = new IfCommand();
            IExpression condition = this.ParseExpression();
            ICommand command = this.ParseCommandList("endif");
            ifCommand.AddConditionAndCommand(condition, command);
            this.lexer.NextToken();
            return ifCommand;
        }

        private ICommand ParseCommandList(params string[] terminators)
        {
            CompositeCommand commands = new CompositeCommand();

            Token token = this.lexer.NextToken();

            while (token != null && !terminators.Contains(token.Value))
            {
                this.lexer.PushToken(token);
                ICommand command = this.ParseLineCommand();
                commands.AddCommand(command);

                token = this.lexer.NextToken();
            }

            if (token != null)
                this.lexer.PushToken(token);

            return commands;
        }

        private List<IExpression> ParseExpressionList()
        {
            List<IExpression> expressions = new List<IExpression>();

            IExpression expression = this.ParseExpression();

            if (expression == null)
                return expressions;

            expressions.Add(expression);

            while (this.TryParse(","))
            {
                expression = this.ParseExpression();

                if (expression == null)
                    throw new ParserException("Invalid list of expressions");

                expressions.Add(expression);
            }

            return expressions;
        }

        private bool TryParse(string value)
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                return false;

            if (token.Value != value)
            {
                this.lexer.PushToken(token);
                return false;
            }

            return true;
        }

        private IExpression ParseExpression()
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                return null;

            switch (token.TokenType)
            {
                case TokenType.String:
                    return new ConstantExpression(token.Value);
                case TokenType.Integer:
                    return new ConstantExpression(Int32.Parse(token.Value));
            }

            throw new ParserException(string.Format("Invalid expression: {0}", token.Value));
        }
    }
}
