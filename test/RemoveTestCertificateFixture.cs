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

namespace AK.PfxTool.Tests
{
	public class RemoveTestCertificateFixture : IDisposable
	{
		public RemoveTestCertificateFixture()
		{
			using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
			{
				var certs = store.Certificates.Find(X509FindType.FindByThumbprint, TestCertificateValues.Thumbprint, false);
				store.RemoveRange(certs);
				store.Close();
			}
		}

		public void Dispose()
		{
			using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
			{
				var certs = store.Certificates.Find(X509FindType.FindByThumbprint, TestCertificateValues.Thumbprint, false);
				store.RemoveRange(certs);
				store.Close();
			}
		}
	}
}