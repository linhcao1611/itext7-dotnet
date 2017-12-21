﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using iText.IO.Image;
using iText.Layout.Element;

namespace iText.Layout
{
    // This test is present only in c#
    // Also this test in only for windows OS 
    class NetWorkPathTest
    {
        [NUnit.Framework.Test]
        public virtual void NetworkPathImageTest()
        {
            var fullImagePath = @"\\someVeryRandomWords\SomeVeryRandomName.img";
            string startOfMsg = null;
#if !NETSTANDARD1_6
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
#else
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
#endif
            try
            {
                Image drawing = new Image(ImageDataFactory.Create(fullImagePath));
            }
            catch (Exception e)
            {
                if (e.InnerException != null && e.InnerException.Message.Length > 18)
                    startOfMsg = e.InnerException.Message.Substring(0, 19);
            }
            NUnit.Framework.Assert.IsNotNull(startOfMsg);
            NUnit.Framework.Assert.AreNotEqual("Could not find file", startOfMsg);
        }
    }
}