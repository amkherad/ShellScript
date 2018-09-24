using System;
using System.Collections.Generic;
using System.Linq;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionStatement : IStatement, IBlockWrapperStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }

        public string Name { get; }
        public IStatement Statement { get; }
        public FunctionParameterDefinitionStatement[] Parameters { get; }

        public bool IsParams { get; }
        
        public DataTypes DataType { get; }
        
        public IStatement[] TraversableChildren { get; protected set; }


        public FunctionStatement(DataTypes dataType, string name, FunctionParameterDefinitionStatement[] parameters, IStatement statement, StatementInfo info)
        {
            DataType = dataType;
            Name = name;
            Statement = statement;
            Info = info;
            Parameters = parameters;

            if (parameters != null)
            {
                TraversableChildren =
                    StatementHelpers.CreateChildren(new IStatement[] {statement}.Union(parameters).ToArray());
            }
            else
            {
                TraversableChildren =
                    StatementHelpers.CreateChildren(statement);
            }
        }

        public override string ToString()
        {
            if (Parameters != null && Parameters.Length > 0)
            {
                return $"{DataType} {Name}({string.Join(',', Parameters.Select(x => x.ToString()))}){{}}";
            }
            
            return $"{DataType} {Name}(){{}}";
        }
    }
}