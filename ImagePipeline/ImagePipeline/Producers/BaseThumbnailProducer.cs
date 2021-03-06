﻿using ImagePipeline.Common;

namespace ImagePipeline.Producers
{
    /// <summary>
    /// Base implementation for IThumbnailProducer{T}.
    /// </summary>
    public class BaseThumbnailProducer<T> : IThumbnailProducer<T>
    {
        /// <summary>
        /// Checks whether the producer may be able to produce images
        /// of the specified  size. This makes no promise about being
        /// able to produce images for a particular source, only generally
        /// being able to produce output of the desired resolution.
        /// </summary>
        /// <param name="resizeOptions">
        /// The resize options from the current request.
        /// </param>
        /// <returns>
        /// true if the producer can meet these needs.
        /// </returns>
        public bool CanProvideImageForSize(ResizeOptions resizeOptions)
        {
            return false;
        }

        /// <summary>
        /// Start producing results for given context.
        /// Provided consumer is notified whenever progress is made
        /// (new value is ready or error occurs).
        /// </summary>
        public void ProduceResults(IConsumer<T> consumer, IProducerContext context)
        {
        }
    }
}
