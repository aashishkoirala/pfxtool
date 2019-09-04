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

using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace AK.PfxTool
{
	internal class ToolOptionsParser
	{
		private readonly ToolOptions _options;

		public ToolOptionsParser(ToolOptions options) => _options = options;

		public (bool IsValid, string ErrorMessage, ParsedToolOptions Parsed) Parse()
		{
			Sanitize();
			if (_options.Command == null) return (false, "Invalid command.", null);

			var parsedOptions = new ParsedToolOptions
			{
				Command = Enum.Parse<Commands>(_options.Command),
				PfxFilePath = _options.File,
				CertFilePath = _options.CertFile,
				KeyFilePath = _options.KeyFile,
				Thumbprint = _options.Thumbprint,
				Password = _options.Password
			};

			switch (parsedOptions.Command)
			{
				case Commands.Create:
					if (!ValidateSpecified(nameof(_options.CertFile), _options.CertFile, out var e)) return (false, e, null);
					if (!ValidateSpecified(nameof(_options.KeyFile), _options.KeyFile, out e)) return (false, e, null);
					if (!ValidateSpecified(nameof(_options.File), _options.File, out e)) return (false, e, null);
					if (!ValidateFile(_options.CertFile, out e, out var b)) return (false, e, null);
					parsedOptions.CertFileBytes = b;
					if (!ValidateFile(_options.KeyFile, out e, out b)) return (false, e, null);
					parsedOptions.KeyFileBytes = b;
					break;

				case Commands.Import:
					if (!ValidateSpecified(nameof(_options.File), _options.File, out e)) return (false, e, null);
					if (!ValidateFile(_options.File, out e, out b)) return (false, e, null);
					parsedOptions.PfxFileBytes = b;
					if (!ValidateStore(out var sn, out e)) return (false, e, null);
					parsedOptions.StoreName = sn;
					if (!ValidateScope(out var sl, out e)) return (false, e, null);
					parsedOptions.StoreLocation = sl;
					break;

				case Commands.Explode:
					if (!ValidateSpecified(nameof(_options.CertFile), _options.CertFile, out e)) return (false, e, null);
					if (!ValidateSpecified(nameof(_options.File), _options.File, out _))
					{
						if (!ValidateStore(out sn, out e)) return (false, e, null);
						parsedOptions.StoreName = sn;
						if (!ValidateScope(out sl, out e)) return (false, e, null);
						parsedOptions.StoreLocation = sl;
					}
					else
					{
						if (!ValidateFile(_options.File, out e, out b)) return (false, e, null);
						parsedOptions.PfxFileBytes = b;
					}

					break;

				case Commands.Export:
					if (!ValidateSpecified(nameof(_options.File), _options.File, out e)) return (false, e, null);
					if (!ValidateSpecified(nameof(_options.Thumbprint), _options.Thumbprint, out e)) return (false, e, null);
					if (!ValidateStore(out sn, out e)) return (false, e, null);
					parsedOptions.StoreName = sn;
					if (!ValidateScope(out sl, out e)) return (false, e, null);
					parsedOptions.StoreLocation = sl;
					break;

				case Commands.Remove:
					if (!ValidateSpecified(nameof(_options.Thumbprint), _options.Thumbprint, out e)) return (false, e, null);
					if (!ValidateStore(out sn, out e)) return (false, e, null);
					parsedOptions.StoreName = sn;
					if (!ValidateScope(out sl, out e)) return (false, e, null);
					parsedOptions.StoreLocation = sl;
					break;

				case Commands.List:
					if (!ValidateSpecified(nameof(_options.File), _options.File, out _))
					{
						if (!ValidateStore(out sn, out e)) return (false, e, null);
						parsedOptions.StoreName = sn;
						if (!ValidateScope(out sl, out e)) return (false, e, null);
						parsedOptions.StoreLocation = sl;
					}
					else
					{
						if (!ValidateFile(_options.File, out e, out b)) return (false, e, null);
						parsedOptions.PfxFileBytes = b;
					}

					break;

				case Commands.Show:
					if (!ValidateSpecified(nameof(_options.File), _options.File, out _))
					{
						if (!ValidateStore(out sn, out e)) return (false, e, null);
						parsedOptions.StoreName = sn;
						if (!ValidateScope(out sl, out e)) return (false, e, null);
						parsedOptions.StoreLocation = sl;
					}
					else
					{
						if (!ValidateFile(_options.File, out e, out b)) return (false, e, null);
						parsedOptions.PfxFileBytes = b;
					}

					if (!ValidateSpecified(nameof(_options.Thumbprint), _options.Thumbprint, out e)) return (false, e, null);
					break;

				default:
					return (false, "Invalid command.", null);
			}

			return (true, null, parsedOptions);
		}

		private void Sanitize()
		{
			var commands = Enum.GetNames(typeof(Commands));
			_options.Command = commands.SingleOrDefault(x => x.Equals(_options.Command, StringComparison.OrdinalIgnoreCase));
			if (_options.Command == null) return;

			if (string.IsNullOrWhiteSpace(_options.Scope)) _options.Scope = ToolOptions.MachineScope;
			if (_options.Scope.Equals(ToolOptions.MachineScope, StringComparison.OrdinalIgnoreCase))
				_options.Scope = ToolOptions.MachineScope;
			if (_options.Scope.Equals(ToolOptions.UserScope, StringComparison.OrdinalIgnoreCase)) _options.Scope = ToolOptions.UserScope;

			var stores = Enum.GetNames(typeof(StoreName));
			if (string.IsNullOrWhiteSpace(_options.Store)) _options.Store = StoreName.My.ToString();
			_options.Store = stores.SingleOrDefault(x => x.Equals(_options.Store, StringComparison.OrdinalIgnoreCase));
		}

		private static bool ValidateSpecified(string name, string value, out string errorMessage)
		{
			errorMessage = null;
			if (!string.IsNullOrWhiteSpace(value)) return true;
			errorMessage = $"Missing --{name.ToLower()}.";
			return false;
		}

		private static bool ValidateFile(string path, out string errorMessage, out byte[] bytes)
		{
			errorMessage = null;
			bytes = null;
			try
			{
				bytes = System.IO.File.ReadAllBytes(path);
			}
			catch (Exception ex)
			{
				errorMessage = $"Cannot open requested file {path}: {ex.Message}";
				return false;
			}

			return true;
		}

		private bool ValidateStore(out StoreName storeName, out string errorMessage)
		{
			errorMessage = null;
			storeName = default;
			var names = Enum.GetNames(typeof(StoreName));
			var name = names.SingleOrDefault(x => x == _options.Store);
			if (name != null)
			{
				storeName = Enum.Parse<StoreName>(name);
				return true;
			}

			errorMessage = $"Invalid store: {_options.Store}.";
			return false;
		}

		private bool ValidateScope(out StoreLocation storeLocation, out string errorMessage)
		{
			errorMessage = null;
			storeLocation = default;
			switch (_options.Scope)
			{
				case ToolOptions.MachineScope:
					storeLocation = StoreLocation.LocalMachine;
					return true;
				case ToolOptions.UserScope:
					storeLocation = StoreLocation.CurrentUser;
					return true;
				default:
					errorMessage = $"Invalid scope: {_options.Scope}.";
					return false;
			}
		}
	}
}