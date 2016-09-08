﻿using System;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;

namespace ImageUtils
{
    /// <summary>
    /// Helper class for bitmap
    /// </summary>
    public static class BitmapUtil
    {
        private const int DECODE_BUFFER_SIZE = 16 * 1024;
        private const int POOL_SIZE = 12;
        //private static final Pools.SynchronizedPool<ByteBuffer> DECODE_BUFFERS = new Pools.SynchronizedPool<>(POOL_SIZE);

        /// <summary>
        /// Bytes per pixel (BitmapPixelFormat.Rgba16)
        /// </summary>
        public const int RGBA16_BYTES_PER_PIXEL = 8;

        /// <summary>
        /// Bytes per pixel (BitmapPixelFormat.Rgba8)
        /// </summary>
        public const int RGBA8_BYTES_PER_PIXEL = 4;

        /// <summary>
        /// Bytes per pixel (BitmapPixelFormat.Bgra8)
        /// </summary>
        public const int BGRA8_BYTES_PER_PIXEL = 4;

        /// <summary>
        /// Bytes per pixel (BitmapPixelFormat.Gray16)
        /// </summary>
        public const int GRAY16_BYTES_PER_PIXEL = 2;

        /// <summary>
        /// Bytes per pixel (BitmapPixelFormat.Gray8)
        /// </summary>
        public const int GRAY8_BYTES_PER_PIXEL = 1;

        /// <summary>
        /// Max possible dimension for an image.
        /// </summary>
        public const float MAX_BITMAP_SIZE = 2048f;

        /// <summary>
        /// Get the size in bytes of the underlying bitmap
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <returns>The size in bytes of the underlying bitmap</returns>
        public static uint GetSizeInBytes(SoftwareBitmap bitmap)
        {
            return GetAllocationByteCount(bitmap);
        }

        /// <summary>
        /// Get the size in bytes of the underlying bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>Size in bytes of the underlying bitmap</returns>
        public static unsafe uint GetAllocationByteCount(SoftwareBitmap bitmap)
        {
            uint capacity = default(int);

            using (BitmapBuffer buffer = bitmap.LockBuffer(BitmapBufferAccessMode.Read))
            {
                using (var reference = buffer.CreateReference())
                {
                    byte* dataInBytes;
                    ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacity);
                }
            }

            return capacity;
        }

        /// <summary>
        /// Returns the amount of bytes used by a pixel in a specific
        /// <see cref="Windows.Graphics.Imaging.SoftwareBitmap.BitmapPixelFormat"/>
        /// <param name="bitmapConfig">The <see cref="Windows.Graphics.Imaging.SoftwareBitmap.BitmapPixelFormat"/> for which the size in byte</param>
        /// will be returned
        /// @return
        /// </summary>
        public static int GetPixelSizeForBitmapConfig(BitmapPixelFormat bitmapConfig)
        {
            switch (bitmapConfig)
            {
                case BitmapPixelFormat.Rgba16:
                    return RGBA16_BYTES_PER_PIXEL;

                case BitmapPixelFormat.Rgba8:
                case BitmapPixelFormat.Bgra8:
                    return RGBA8_BYTES_PER_PIXEL;

                case BitmapPixelFormat.Gray16:
                    return GRAY16_BYTES_PER_PIXEL;

                case BitmapPixelFormat.Gray8:
                    return GRAY8_BYTES_PER_PIXEL;
            }

            throw new InvalidOperationException("The provided Bitmap.Config is not supported");
        }

        /// <summary>
        /// Returns the size in byte of an image with specific size
        /// and <see cref="Windows.Graphics.Imaging.SoftwareBitmap.BitmapPixelFormat"/>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="bitmapConfig">The <see cref="Windows.Graphics.Imaging.SoftwareBitmap.BitmapPixelFormat"/> for which the size in byte</param>
        /// will be returned
        /// @return
        /// </summary>
        public static int GetSizeInByteForBitmap(int width, int height, BitmapPixelFormat bitmapConfig)
        {
            return width * height * GetPixelSizeForBitmapConfig(bitmapConfig);
        }
    }

    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }
}