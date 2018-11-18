using System;
using System.Collections.Generic;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Core.Language.Library
{
    public interface IApi
    {
        IApiVariable[] Variables { get; }
        IApiFunc[] Functions { get; }
        IApiClass[] Classes { get; }

        IDictionary<string, IThirdPartyUtility> Utilities { get; }

        string Name { get; }


        void InitializeContext(Context context);


        bool TryGetClass(string className, out IApiClass result);
        bool TryGetClass(Type apiClassType, out IApiClass result);

        bool TryGetFunction(string functionName, out IApiFunc result);
        bool TryGetFunction(Type apiFunctionType, out IApiFunc result);

        bool TryGetVariable(string variableName, out IApiVariable result);
        bool TryGetVariable(Type apiVariableType, out IApiVariable result);
    }
}