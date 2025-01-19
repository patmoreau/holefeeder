using System.Text;

namespace Holefeeder.Infrastructure.SeedWork;

public static class PostgresCommandSplitter
{
    public static IEnumerable<string> SplitCommands(string script)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(script);

        var commands = new List<string>();
        var currentCommand = new StringBuilder();
        bool inSingleQuote = false, inDoubleQuote = false, inDollarQuote = false;
        var dollarQuoteTag = string.Empty;

        for (var i = 0; i < script.Length; i++)
        {
            var current = script[i];
            var next = i < script.Length - 1 ? script[i + 1] : '\0';

            switch (current)
            {
                // Handle single-line comments (--)
                case '-' when next == '-' && !inSingleQuote && !inDoubleQuote && !inDollarQuote:
                {
                    while (i < script.Length && script[i] != '\n')
                    {
                        currentCommand.Append(script[i]);
                        i++;
                    }
                    currentCommand.Append('\n');
                    continue;
                }
                // Handle multi-line comments (/* */)
                case '/' when next == '*' && !inSingleQuote && !inDoubleQuote && !inDollarQuote:
                {
                    currentCommand.Append(current);
                    currentCommand.Append(next);
                    i += 2;

                    while (i < script.Length && !(script[i] == '*' && i + 1 < script.Length && script[i + 1] == '/'))
                    {
                        currentCommand.Append(script[i]);
                        i++;
                    }

                    if (i < script.Length)
                    {
                        currentCommand.Append('*');
                        currentCommand.Append('/');
                        i++;
                    }
                    continue;
                }
                // Handle single quotes (')
                case '\'' when !inDoubleQuote && !inDollarQuote:
                    inSingleQuote = !inSingleQuote;
                    break;
                // Handle double quotes (")
                case '"' when !inSingleQuote && !inDollarQuote:
                    inDoubleQuote = !inDoubleQuote;
                    break;
                // Handle dollar-quoted strings ($$ or $tag$)
                case '$' when !inSingleQuote && !inDoubleQuote:
                {
                    var tagEnd = i + 1;
                    while (tagEnd < script.Length && (char.IsLetterOrDigit(script[tagEnd]) || script[tagEnd] == '_'))
                    {
                        tagEnd++;
                    }

                    var tag = script.Substring(i, tagEnd - i);
                    if (!inDollarQuote)
                    {
                        inDollarQuote = true;
                        dollarQuoteTag = tag;
                    }
                    else if (dollarQuoteTag == tag)
                    {
                        inDollarQuote = false;
                        dollarQuoteTag = string.Empty;
                    }

                    currentCommand.Append(tag);
                    i = tagEnd - 1;
                    continue;
                }
            }

            // Append the current character to the command
            currentCommand.Append(current);

            // Split commands on semicolon if not inside a quoted string or comment
            if (current != ';' || inSingleQuote || inDoubleQuote || inDollarQuote)
            {
                continue;
            }
            commands.Add(currentCommand.ToString().Trim());
            currentCommand.Clear();
        }

        // Add the last command if any
        var command = currentCommand.ToString().Trim();
        if (command.Length > 0)
        {
            commands.Add(command);
        }

        return commands;
    }
}
