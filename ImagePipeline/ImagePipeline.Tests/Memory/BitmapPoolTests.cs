﻿using ImagePipeline.Memory;
using ImagePipeline.Testing;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Graphics.Imaging;

namespace ImagePipeline.Tests.Memory
{
    /// <summary>
    /// Basic tests for BitmapPool 
    /// </summary>
    [TestClass]
    public class BitmapPoolTests
    {
        private const int MAX_BUFFER_SIZE = 224;

        private BitmapPool _pool;

        /// <summary>
        /// Initialize
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _pool = new BitmapPool(
                new MockMemoryTrimmableRegistry(),
                new PoolParams(MAX_BUFFER_SIZE, new Dictionary<int, int>()),
                new MockPoolStatsTracker());
        }

        /// <summary>
        /// Tests out the Free method
        /// </summary>
        [TestMethod]
        public void TestFree()
        {
            using (SoftwareBitmap bitmap = _pool.Alloc(12))
            {
                _pool.Free(bitmap);
                bool disposed = false;

                // Check if object has been disposed
                try
                {
                    disposed = (bitmap.PixelWidth == 0);
                }
                catch (ObjectDisposedException e)
                {
                    Debug.WriteLine($"{e.Message} is expected");
                    disposed = true;
                }

                Assert.IsTrue(disposed);
            }
        }

        /// <summary>
        /// Tests out the GetBucketedSize method
        /// </summary>
        [TestMethod]
        public void TestGetBucketedSize()
        {
            Assert.AreEqual(12, _pool.GetBucketedSize(12));
            Assert.AreEqual(56, _pool.GetBucketedSize(56));
        }

        /// <summary>
        /// Tests out the GetBucketedSizeForValue method
        /// </summary>
        [TestMethod]
        public void TestGetBucketedSizeForValue()
        {
            using (SoftwareBitmap bitmap1 = _pool.Alloc(12),
                                  bitmap2 = _pool.Alloc(56),
                                  bitmap3 = MockBitmapFactory.Create(7, 8, BitmapPixelFormat.Rgba8),
                                  bitmap4 = MockBitmapFactory.Create(7, 8, BitmapPixelFormat.Rgba16))
            {
                Assert.AreEqual(12, _pool.GetBucketedSizeForValue(bitmap1));
                Assert.AreEqual(56, _pool.GetBucketedSizeForValue(bitmap2));
                Assert.AreEqual(224, _pool.GetBucketedSizeForValue(bitmap3));
                Assert.AreEqual(448, _pool.GetBucketedSizeForValue(bitmap4));
            }
        }

        /// <summary>
        /// Tests out the GetSizeInBytes method
        /// </summary>
        [TestMethod]
        public void TestGetSizeInBytes()
        {
            Assert.AreEqual(48, _pool.GetSizeInBytes(48));
            Assert.AreEqual(224, _pool.GetSizeInBytes(224));
        }

        /// <summary>
        /// Test out bitmap reusability
        /// </summary>
        [TestMethod]
        public void TestIsReusable()
        {
            using (SoftwareBitmap b1 = _pool.Alloc(12),
                                  b2 = MockBitmapFactory.Create(3, 4, BitmapPixelFormat.Bgra8),
                                  b3 = MockBitmapFactory.Create(3, 4, BitmapPixelFormat.Gray16),
                                  b5 = MockBitmapFactory.Create(3, 4, BitmapPixelFormat.Bgra8).GetReadOnlyView())
            {
                SoftwareBitmap b4 = MockBitmapFactory.Create(3, 4, BitmapPixelFormat.Bgra8);
                Assert.IsTrue(_pool.IsReusable(b1));
                Assert.IsTrue(_pool.IsReusable(b2));
                Assert.IsTrue(_pool.IsReusable(b3));
                b4.Dispose();
                Assert.IsFalse(_pool.IsReusable(b4));
                Assert.IsFalse(_pool.IsReusable(b5));
            }
        }
    }
}
