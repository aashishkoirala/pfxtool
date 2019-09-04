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
	internal class ListCommand : ICommand
	{
		private readonly ParsedToolOptions _options;

		public ListCommand(ParsedToolOptions options) => _options = options;

		public void Run()
		{
			if (_options.PfxFileBytes != null) ListFromPfx();
			else ListFromStore();
		}

		private void ListFromPfx()
		{
			Console.WriteLine($"List of certificates in {_options.PfxFilePath}:");
			Console.WriteLine();
			Console.WriteLine("Thumbprint\tSubject");
			Console.WriteLine("----------\t-------");

			using (var certificate = new X509Certificate2(_options.PfxFileBytes, _options.Password ?? string.Empty))
			{
				using (var chain = new X509Chain())
				{
					chain.Build(certificate);
					foreach (var chainElement in chain.ChainElements) WriteRow(chainElement.Certificate);
				}
			}
		}

		private void ListFromStore()
		{
			Console.WriteLine($"List of certificates in {_options.StoreLocation}/{_options.StoreName}:");
			Console.WriteLine();
			Console.WriteLine("Thumbprint\tSubject");
			Console.WriteLine("----------\t-------");

			using (var store = new X509Store(_options.StoreName, _options.StoreLocation, OpenFlags.ReadOnly | OpenFlags.IncludeArchived))
			{
				foreach (var certificate in store.Certificates) WriteRow(certificate);
				store.Close();
			}
		}

		private static void WriteRow(X509Certificate2 certificate)
		{
			var subject = certificate.SubjectName.Format(false);
			Console.WriteLine($"{certificate.Thumbprint}\t{subject}");
		}
	}
}