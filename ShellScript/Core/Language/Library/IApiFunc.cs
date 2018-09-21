using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;

namespace ShellScript.Core.Language.Library
{
    public interface IApiFunc : IApiObject
    {
        DataTypes DataType { get; }
        
        bool IsStatic { get; }
        bool AllowDynamicParams { get; }

        FunctionParameterDefinitionStatement[] Parameters { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// var x = XXX
        /// </remarks>
        /// <param name="p"></param>
        /// <param name="functionCallStatement"></param>
        /// <returns></returns>
        IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement functionCallStatement);
    }
}