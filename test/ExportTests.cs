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
using Xunit;
using Xunit.Abstractions;

namespace AK.PfxTool.Tests
{
	public class ExportTests : IClassFixture<InstallTestCertificateFixture>
	{
		public ExportTests(ITestOutputHelper testOutputHelper)
		{
			Console.SetOut(new ConsoleWriter(testOutputHelper));
			if (File.Exists(TestCertificateValues.ExportPath)) File.Delete(TestCertificateValues.ExportPath);
		}

		[Fact]
		public void Export_Existing_Works()
		{
			var args = new[]
			{
				"export", $"--file={TestCertificateValues.ExportPath}", $"--password={TestCertificateValues.Password}", "--scope=user",
				$"--thumbprint={TestCertificateValues.Thumbprint}"
			};
			var exitCode = Program.Main(args);
			Assert.Equal(0, exitCode);

			using (var cert = new X509Certificate2(TestCertificateValues.ExportPath, TestCertificateValues.Password))
			{
				Assert.Equal(TestCertificateValues.Thumbprint, cert.Thumbprint, StringComparer.OrdinalIgnoreCase);
				Assert.True(cert.HasPrivateKey);
			}
		}

		[Fact]
		public void Export_Non_Existing_Pfx_Fails()
		{
			var args = new[]
			{
				"export", $"--file={TestCertificateValues.ExportPath}", $"--password={TestCertificateValues.Password}", "--scope=user",
				"--thumbprint=BadThumbprint"
			};
			var exitCode = Program.Main(args);
			Assert.Equal(1, exitCode);
			Assert.False(File.Exists(TestCertificateValues.ExportPath));
		}
	}
}