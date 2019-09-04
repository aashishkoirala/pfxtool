/*******************************************************************************************************************************
 * PFX Tool
 * Copyright (C) 2019 Aashish Koirala <https://www.aashishkoirala.com>
 *
 * This file is part of PFXTool.
 *
 * PFXTool is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * PFXTool is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with PFXTool.  If not, see <http://www.gnu.org/licenses/>.
 *
 *******************************************************************************************************************************/

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AK.PfxTool
{
	public static class Program
	{
		private static readonly IDictionary<Commands, Func<ParsedToolOptions, ICommand>> _commandMap =
			new Dictionary<Commands, Func<ParsedToolOptions, ICommand>>
			{
				{Commands.Create, o => new CreateCommand(o)},
				{Commands.Import, o => new ImportCommand(o)},
				{Commands.Explode, o => new ExplodeCommand(o)},
				{Commands.Export, o => new ExportCommand(o)},
				{Commands.Remove, o => new RemoveCommand(o)},
				{Commands.List, o => new ListCommand(o)},
				{Commands.Show, o => new ShowCommand(o)}
			};

		public static int Main(string[] args)
		{
			if (!AssignCommand(args))
			{
				ShowUsage();
				return 1;
			}

			var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();
			var options = new ToolOptions();
			configuration.Bind(options);
			Func<ParsedToolOptions, ICommand> commandCreator = null;
			var (isValid, errorMessage, parsedOptions) = new ToolOptionsParser(options).Parse();

			if (isValid)
			{
				if (!_commandMap.TryGetValue(parsedOptions.Command, out commandCreator))
				{
					isValid = false;
					errorMessage = $"Cannot find implementation for command {parsedOptions.Command}.";
				}
			}

			if (!isValid)
			{
				Console.WriteLine(errorMessage);
				Console.WriteLine();
				ShowUsage();
				return 1;
			}

			try
			{
				commandCreator(parsedOptions).Run();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR: {ex.Message}");
				return 1;
			}

			return 0;
		}

		private static bool AssignCommand(string[] args)
		{
			if (args.Length < 1) return false;
			var command = args[0];
			if (command.StartsWith("--") || command.Contains('=')) return false;

			command = $"--command={command}";
			args[0] = command;
			return true;
		}

		private static void ShowUsage()
		{
			var storeNames = string.Join(", ", Enum.GetNames(typeof(StoreName)).Select(x => x.ToLower()));
			var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
			var versionObject = Version.Parse(version);
			version = $"{versionObject.Major}.{versionObject.Minor}.{versionObject.Revision}";

			var sb = new StringBuilder("PFX Tool - Does stuff with and around PFX certificates.")
				.AppendLine()
				.AppendLine()
				.AppendLine("Usage:")
				.AppendLine("------")
				.AppendLine($"<store-name> can be one of {storeNames}; if skipped, defaults to \"my\".")
				.AppendLine("<cert-scope> can either be \"user\" or \"machine\"; if skipped, defaults to \"machine\".")
				.AppendLine("Password is optional but needs to be supplied if operating on PFX with private key that is " +
				            "protected with one, or if exporting a PFX and you want to protect the private key with one.")
				.AppendLine()
				.AppendLine("1. Import a PFX file with or without private key into a certificate store.")
				.AppendLine(
					"\tpfxtool import --file <source-pfx-file-path> [--store <store-name>] [--scope <cert-scope>] [--password <private-key-password>]")
				.AppendLine()
				.AppendLine("2. Export a PFX file with or without private key out of a certificate store.")
				.AppendLine(
					"\tpfxtool export --file <target-pfx-file-path> [--store <store-name>] [--scope <cert-scope>] [--password <private-key-password>]")
				.AppendLine()
				.AppendLine("3. Remove a certificate from a certificate store.")
				.AppendLine("\tpfxtool remove --thumbprint <thumbprint-of-cert-to-remove> [--store <store-name>] [--scope <cert-scope>]")
				.AppendLine()
				.AppendLine("4. List all certificates in a PFX file.")
				.AppendLine("\tpfxtool list --file <source-pfx-file-path>")
				.AppendLine()
				.AppendLine("5. List all certificates in a certificate store.")
				.AppendLine("\tpfxtool list [--store <store-name>] [--scope <cert-scope>]")
				.AppendLine()
				.AppendLine("6. Show details of a certificate in a PFX file.")
				.AppendLine(
					"\tpfxtool show --file <source-pfx-file-path> --thumbprint <thumbprint-of-cert-to-show> [--password <private-key-password>]")
				.AppendLine()
				.AppendLine("7. Show details of a certificate in a certificate store.")
				.AppendLine("\tpfxtool list --thumbprint <thumbprint-of-cert-to-show> [--store <store-name>] [--scope <cert-scope>]")
				.AppendLine()
				.AppendLine("Coming Soon:")
				.AppendLine("------------")
				.AppendLine("8. Create a PFX file out of a public certificate file and optionally a private key file.")
				.AppendLine(
					"\tpfxtool create --cert-file <public-cert-file-path> [--key-file <private-key-file-path>] --file <target-pfx-file-path>")
				.AppendLine()
				.AppendLine("9. Explode a PFX file into its public certificate and private key files.")
				.AppendLine(
					"\tpfxtool explode --cert-file <target-cert-file-path> [--key-file <target-private-key-file-path>] --file <source-pfx-file-path> [--password <private-key-password>]")
				.AppendLine()
				.AppendLine("10. Explode a certificate in a certificate store into its public certificate and private key files.")
				.AppendLine(
					"\tpfxtool explode --cert-file <target-cert-file-path> [--key-file <target-private-key-file-path>] [--store <store-name>] [--scope <cert-scope>] [--password <private-key-password>]")
				.AppendLine()
				.AppendLine($"PFX Tool {version} (c) Aashish Koirala, 2019. https://pfxtool.aashishkoirala.com/");

			Console.WriteLine(sb);
		}
	}
}