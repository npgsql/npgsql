using System.Collections.Generic;
using System.Diagnostics;

namespace Npgsql;

class ParsedQuery
{
    public ParsedQuery(string queryText, List<NpgsqlParameter> queryParameters)
    {
        Query = queryText;

        var parameters = new List<ParsedQueryParameter>(queryParameters.Count);
        for (var i = 0; i < queryParameters.Count; i++)
        {
            Debug.Assert(!string.IsNullOrEmpty(queryParameters[i].ParameterName));
            parameters.Add(new ParsedQueryParameter(queryParameters[i].ParameterName));
        }
        Parameters = parameters.AsReadOnly();
    }   
    
    internal string Query { get; }
    
    internal IReadOnlyList<ParsedQueryParameter> Parameters { get; }

    internal bool GenerateCommand(NpgsqlCommand command)
    {
        var internalBatchCommands = command.InternalBatchCommands;
        NpgsqlBatchCommand internalBatchCommand;
        if (internalBatchCommands.Count > 0)
        {
            internalBatchCommand = internalBatchCommands[0];
            internalBatchCommand.Reset();
            if (internalBatchCommands.Count > 1)
                internalBatchCommands.RemoveRange(1, internalBatchCommands.Count - 1);
        }
        else
        {
            internalBatchCommand = new NpgsqlBatchCommand();
            internalBatchCommands.Add(internalBatchCommand);
        }

        return GenerateCommand(internalBatchCommand, command.Parameters);
    }

    internal bool GenerateCommand(NpgsqlBatchCommand command, NpgsqlParameterCollection commandParameters)
    {
        command.FinalCommandText = Query;
        for (var i = 0; i < Parameters.Count; i++)
        {
            var parameterFound = false;
            var positionalParameter = Parameters[i];
            for (var j = 0; j < commandParameters.Count; j++)
            {
                var namedParameter = commandParameters[j];
                if (namedParameter.ParameterName == positionalParameter.Name)
                {
                    command.PositionalParameters.Add(namedParameter);
                    parameterFound = true;
                    break;
                }
            }

            if (!parameterFound)
                return false;
        }

        return true;
    }
}

class ParsedQueryParameter
{
    public ParsedQueryParameter(string name) => Name = name;
    
    internal string Name { get; }
}
