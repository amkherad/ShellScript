using System;
using System.Globalization;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler
{
    /// <summary>
    /// Compile-time constant folding helpers. Expression trees are folded primarily in
    /// <see cref="Transpiling.BaseImplementations.EvaluationStatementTranspilerBase.ProcessEvaluation"/>.
    /// </summary>
    public static class ConstantFolding
    {
        public static ConstantValueStatement TryFoldCast(
            TypeDescriptor targetType,
            ConstantValueStatement source,
            StatementInfo info,
            IStatement parentStatement = null)
        {
            if (targetType.IsInteger())
            {
                if (source.IsInteger() && long.TryParse(source.Value, NumberStyles.Integer,
                        NumberFormatInfo.InvariantInfo, out var intVal))
                {
                    return Wrap(TypeDescriptor.Integer, intVal.ToString(NumberFormatInfo.InvariantInfo), info,
                        parentStatement);
                }

                if (double.TryParse(source.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var dblVal))
                {
                    var truncated = (long)dblVal;
                    return Wrap(TypeDescriptor.Integer, truncated.ToString(NumberFormatInfo.InvariantInfo), info,
                        parentStatement);
                }
            }

            if (targetType.IsFloat() || targetType.DataType == DataTypes.Numeric)
            {
                if (source.IsNumber() &&
                    double.TryParse(source.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var dblVal))
                {
                    return Wrap(TypeDescriptor.Float, dblVal.ToString(NumberFormatInfo.InvariantInfo), info,
                        parentStatement);
                }
            }

            if (targetType.IsBoolean())
            {
                if (StatementHelpers.TryParseBooleanFromString(source.Value, out var boolVal))
                {
                    return Wrap(TypeDescriptor.Boolean, boolVal.ToString(CultureInfo.InvariantCulture), info,
                        parentStatement);
                }
            }

            return null;
        }

        private static ConstantValueStatement Wrap(TypeDescriptor type, string value, StatementInfo info,
            IStatement parentStatement)
        {
            var result = new ConstantValueStatement(type, value, info) {ParentStatement = parentStatement};
            return result;
        }
    }
}
