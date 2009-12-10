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

        public IExpression ParseExpression()
        {
            return this.ParseBinaryExpressionLevel0();
        }

        public ICommand ParseCommandList(params string[] terminators)
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

        private IExpression ParseBinaryExpressionLevel0()
        {
            IExpression expression = this.ParseBinaryExpressionLevel1();

            Token token = this.lexer.NextToken();

            while (token != null && token.TokenType == TokenType.Operator && (token.Value == ">" || token.Value == "<" || token.Value == "<=" || token.Value == ">=" || token.Value == "<>" || token.Value == "!=" || token.Value == "=="))
            {
                if (token.Value == ">")
                    expression = new CompareExpression(expression, this.ParseBinaryExpressionLevel1(), CompareOperator.Greater);
                else if (token.Value == "<")
                    expression = new CompareExpression(expression, this.ParseBinaryExpressionLevel1(), CompareOperator.Less);
                else if (token.Value == ">=")
                    expression = new CompareExpression(expression, this.ParseBinaryExpressionLevel1(), CompareOperator.GreaterEqual);
                else if (token.Value == "<=")
                    expression = new CompareExpression(expression, this.ParseBinaryExpressionLevel1(), CompareOperator.LessEqual);
                else if (token.Value == "<>" || token.Value == "!=")
                    expression = new CompareExpression(expression, this.ParseBinaryExpressionLevel1(), CompareOperator.NotEqual);
                else if (token.Value == "==")
                    expression = new CompareExpression(expression, this.ParseBinaryExpressionLevel1(), CompareOperator.Equal);

                token = this.lexer.NextToken();
            }

            if (token != null)
                this.lexer.PushToken(token);

            return expression;
        }

        private IExpression ParseBinaryExpressionLevel1()
        {
            IExpression expression = this.ParseBinaryExpressionLevel2();

            Token token = this.lexer.NextToken();

            while (token != null && (token.Value == "+" || token.Value == "-"))
            {
                switch (token.Value[0])
                {
                    case '+':
                        expression = new AddExpression(expression, this.ParseBinaryExpressionLevel2());
                        break;
                    case '-':
                        expression = new SubtractExpression(expression, this.ParseBinaryExpressionLevel2
());
                        break;
                }

                token = this.lexer.NextToken();
            }

            if (token != null)
                this.lexer.PushToken(token);

            return expression;
        }

        private IExpression ParseBinaryExpressionLevel2()
        {
            IExpression expression = this.ParseSimpleExpression();

            Token token = this.lexer.NextToken();

            while (token != null && (token.Value == "*" || token.Value == "/"))
            {
                switch (token.Value[0])
                {
                    case '*':
                        expression = new MultiplyExpression(expression, this.ParseSimpleExpression());
                        break;
                    case '/':
                        expression = new DivideExpression(expression, this.ParseSimpleExpression
());
                        break;
                }

                token = this.lexer.NextToken();
            }

            if (token != null)
                this.lexer.PushToken(token);

            return expression;
        }

        private IExpression ParseSimpleExpression()
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
                case TokenType.Name:
                    return new NameExpression(token.Value);
            }

            throw new ParserException(string.Format("Invalid expression: {0}", token.Value));
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

                if (token.Value == "while")
                    return this.ParseWhileCommand();

                if (token.Value == "procedure")
                    return this.ParseProcedureCommand();

                if (token.Value == "do")
                    return this.ParseDoProcedureCommand();

                Token token2 = this.lexer.NextToken();

                if (token2 != null && (token2.Value == ":=" || token2.Value == "="))
                    return new SetVariableCommand(token.Value, this.ParseExpression());
            }

            this.lexer.PushToken(token);

            return null;
        }

        private ICommand ParseWhileCommand()
        {
            IExpression condition = this.ParseExpression();
            ICommand command = this.ParseCommandList("end", "enddo");

            WhileCommand whileCommand = new WhileCommand(condition, command);

            this.lexer.NextToken();
            return whileCommand;
        }

        private ICommand ParseDoProcedureCommand()
        {
            string name = this.ParseName();
            IList<IExpression> arguments = this.ParseArguments();

            return new DoProcedureCommand(name, arguments);
        }

        private ICommand ParseProcedureCommand()
        {
            string name = this.ParseName();
            List<string> parameterNames = this.ParseParameterNameList();
            ICommand command = this.ParseCommandList("return");

            ProcedureCommand procedureCommand = new ProcedureCommand(name, parameterNames, command);

            this.lexer.NextToken();

            return procedureCommand;
        }

        private List<string> ParseParameterNameList()
        {
            List<string> names = new List<string>();

            Token token = this.lexer.NextToken();

            if (token == null)
                return names;

            if (token.Value != "(")
            {
                this.lexer.PushToken(token);
                return names;
            }

            string name;

            name = this.ParseName();
            names.Add(name);

            token = this.lexer.NextToken();

            while (token != null && token.Value == ",")
            {
                name = this.ParseName();
                names.Add(name);

                token = this.lexer.NextToken();
            }

            if (token == null || token.Value != ")")
                throw new ParserException("')' expected");

            return names;
        }

        private List<IExpression> ParseArguments()
        {
            List<IExpression> arguments = new List<IExpression>();

            Token token = this.lexer.NextToken();

            if (token == null)
                return arguments;

            if (token.Value != "(")
            {
                this.lexer.PushToken(token);
                return arguments;
            }

            token = this.lexer.NextToken();

            if (token != null && token.TokenType == TokenType.Delimiter && token.Value != ")")
                this.lexer.PushToken(token);

            while (token != null && token.TokenType == TokenType.Delimiter && token.Value != ")")
            {
                arguments.Add(this.ParseExpression());

                token = this.lexer.NextToken();

                if (token == null || token.TokenType != TokenType.Delimiter || token.Value != ",")
                    break;
            }

            if (token == null || token.TokenType != TokenType.Delimiter || token.Value != ")")
                throw new ParserException("')' expected");

            return arguments;
        }

        private ICommand ParseIfCommand()
        {
            IfCommand ifCommand = new IfCommand();

            Token token = new Token() { TokenType = TokenType.Name, Value = "elseif" };

            while (token != null && token.Value == "elseif")
            {
                IExpression condition = this.ParseExpression();
                ICommand command = this.ParseCommandList("endif", "elseif", "else", "end");
                ifCommand.AddConditionAndCommand(condition, command);
                token = this.lexer.NextToken();
            }

            if (token != null && token.Value == "else")
            {
                ICommand command = this.ParseCommandList("endif", "end");
                ifCommand.AddElseCommand(command);
            }

            this.lexer.NextToken();
            return ifCommand;
        }

        private string ParseName()
        {
            Token token = this.lexer.NextToken();

            if (token == null || token.TokenType != TokenType.Name)
                throw new ParserException("Name expected");

            return token.Value;
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
    }
}