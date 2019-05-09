﻿//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2017-2019  Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.Windows
// 
//  Dapplo.Windows is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.Windows is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.Windows. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region using

using System;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Dapplo.Windows.Gdi32;
using Dapplo.Windows.Gdi32.SafeHandles;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.Windows.Tests
{
    public class SafeHandleTests
    {
        public SafeHandleTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
        }

        [Fact]
        public void TestCreateCompatibleDc()
        {
            using (var desktopDcHandle = SafeWindowDcHandle.FromDesktop())
            {
                Assert.False(desktopDcHandle.IsInvalid);

                // create a device context we can copy to
                using (var safeCompatibleDcHandle = Gdi32Api.CreateCompatibleDC(desktopDcHandle))
                {
                    Assert.False(safeCompatibleDcHandle.IsInvalid);
                }
            }
        }

        [Fact]
        public void TestSafeWindowDcHandle_IntPtrZero()
        {
            using (var safeDctHandle = SafeWindowDcHandle.FromWindow(IntPtr.Zero))
            {
                Assert.Null(safeDctHandle);
            }
        }
    }
}