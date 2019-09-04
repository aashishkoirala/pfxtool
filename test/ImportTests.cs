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
using Xunit;
using Xunit.Abstractions;

namespace AK.PfxTool.Tests
{
	public class ImportTests : IClassFixture<RemoveTestCertificateFixture>
	{
		public ImportTests(ITestOutputHelper testOutputHelper) => Console.SetOut(new ConsoleWriter(testOutputHelper));

		[Fact]
		public void Import_Existing_Pfx_Works()
		{
			var args = new[]
			{
				"import", $"--file={TestCertificateValues.Path}", $"--password={TestCertificateValues.Password}", "--scope=user"
			};
			var exitCode = Program.Main(args);
			Assert.Equal(0, exitCode);

			var importedCount = 0;
			using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
			{
				var certs = store.Certificates.Find(X509FindType.FindByThumbprint, TestCertificateValues.Thumbprint, false);
				importedCount = certs.Count;
				store.Close();
			}

			Assert.Equal(1, importedCount);
		}

		[Fact]
		public void Import_Existing_Pfx_Bad_Password_Fails()
		{
			var args = new[] {"import", $"--file={TestCertificateValues.Path}", "--password=BadPassword", "--scope=user"};
			var exitCode = Program.Main(args);
			Assert.Equal(1, exitCode);

			var importedCount = 0;
			using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
			{
				var certs = store.Certificates.Find(X509FindType.FindByThumbprint, TestCertificateValues.Thumbprint, false);
				importedCount = certs.Count;
				store.Close();
			}

			Assert.Equal(0, importedCount);
		}

		[Fact]
		public void Import_Non_Existing_Pfx_Fails()
		{
			var args = new[] {"import", "--file=BadPath.pfx", $"--password={TestCertificateValues.Password}", "--scope=user"};
			var exitCode = Program.Main(args);
			Assert.Equal(1, exitCode);

			var importedCount = 0;
			using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
			{
				var certs = store.Certificates.Find(X509FindType.FindByThumbprint, TestCertificateValues.Thumbprint, false);
				importedCount = certs.Count;
				store.Close();
			}

			Assert.Equal(0, importedCount);
		}
	}
}