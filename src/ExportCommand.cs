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
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace AK.PfxTool
{
	internal class ExportCommand : ICommand
	{
		private readonly ParsedToolOptions _options;

		public ExportCommand(ParsedToolOptions options) => _options = options;

		public void Run()
		{
			// TODO: Export chain.

			Console.WriteLine($"Exporting certificate {_options.Thumbprint} from " +
			                  $"{_options.StoreLocation}/{_options.StoreName} to {_options.PfxFilePath}...");

			byte[] pfxBytes;
			using (var store = new X509Store(_options.StoreName, _options.StoreLocation, OpenFlags.ReadOnly | OpenFlags.IncludeArchived))
			{
				var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, _options.Thumbprint, false);
				if (certificates.Count == 0) throw new Exception($"Could not find certificate with thumbprint {_options.Thumbprint}.");

				pfxBytes = _options.Password == null
					? certificates.Export(X509ContentType.Pfx)
					: certificates.Export(X509ContentType.Pfx, _options.Password);

				store.Close();
			}

			File.WriteAllBytes(_options.PfxFilePath, pfxBytes);

			Console.WriteLine("Export complete.");
		}
	}
}