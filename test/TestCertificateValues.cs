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

using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AK.PfxTool.Tests
{
	public static class TestCertificateValues
	{
		public const string Path = "TestCertificate.pfx";
		public const string Thumbprint = "f64d033491fbf04ac63f8f04b260e1b11a1ac42b";
		public const string Password = "Password123";
		public const string ExportPath = "Exported.pfx";
	}
}