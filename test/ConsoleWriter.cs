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

using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace AK.PfxTool.Tests
{
	public class ConsoleWriter : TextWriter
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public ConsoleWriter(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

		public override Encoding Encoding => Encoding.UTF8;

		public override void WriteLine() => _testOutputHelper.WriteLine(string.Empty);
		public override void WriteLine(string value) => _testOutputHelper.WriteLine(value);
	}
}