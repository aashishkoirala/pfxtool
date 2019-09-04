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
	internal class ImportCommand : ICommand
	{
		private readonly ParsedToolOptions _options;

		public ImportCommand(ParsedToolOptions options) => _options = options;

		public void Run()
		{
			Console.WriteLine($"Importing PFX file {_options.PfxFilePath} into {_options.StoreLocation}/{_options.StoreName}...");

			using (var store = new X509Store(_options.StoreName, _options.StoreLocation, OpenFlags.ReadWrite))
			{
				using (var sourceCertificate = new X509Certificate2(_options.PfxFileBytes,
					_options.Password ?? string.Empty,
					X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet))
				{
					using (var chain = new X509Chain())
					{
						chain.Build(sourceCertificate);
						foreach (var chainElement in chain.ChainElements)
						{
							store.Add(chainElement.Certificate);
						}
					}
				}

				store.Close();
			}

			Console.WriteLine("Import complete.");
		}
	}
}