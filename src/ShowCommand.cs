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
using System.Security.Cryptography.X509Certificates;

namespace AK.PfxTool
{
	internal class ShowCommand : ICommand
	{
		private readonly ParsedToolOptions _options;

		public ShowCommand(ParsedToolOptions options) => _options = options;

		public void Run()
		{
			if (_options.PfxFileBytes != null) ShowFromPfx();
			else ShowFromStore();
		}

		private void ShowFromPfx()
		{
			var found = false;
			using (var sourceCertificate = new X509Certificate2(_options.PfxFileBytes, _options.Password ?? string.Empty))
			{
				using (var chain = new X509Chain())
				{
					chain.Build(sourceCertificate);
					foreach (var chainElement in chain.ChainElements)
					{
						var certificate = chainElement.Certificate;
						if (!_options.Thumbprint.Equals(certificate.Thumbprint, StringComparison.OrdinalIgnoreCase)) continue;
						found = true;
						WriteCertificate(certificate);
					}
				}
			}

			if (!found) throw new Exception($"Could not find certificate with thumbprint {_options.Thumbprint}");
		}

		private void ShowFromStore()
		{
			using (var store = new X509Store(_options.StoreName, _options.StoreLocation, OpenFlags.ReadOnly | OpenFlags.IncludeArchived))
			{
				var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, _options.Thumbprint, false);
				if (certificates.Count == 0) throw new Exception($"Could not find certificate with thumbprint {_options.Thumbprint}.");

				foreach (var certificate in certificates) WriteCertificate(certificate);
				store.Close();
			}
		}

		private static void WriteCertificate(X509Certificate2 certificate)
		{
			Console.WriteLine(certificate);
			Console.WriteLine();
		}
	}
}