// ----------------------------------------------------------------------
// <copyright file="QPEngineException.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.QPSolver.Exceptions
{
    /// <summary>
    /// QPEngine Exception.
    /// </summary>
    [Serializable]
    public class QPEngineException : Exception
    {
        /// <summary>
        /// Setup exception instance.
        /// </summary>
        public QPEngineException()
        {
        }

        /// <summary>
        /// Setup exception instance.
        /// </summary>
        public QPEngineException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Setup exception instance.
        /// </summary>
        public QPEngineException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}