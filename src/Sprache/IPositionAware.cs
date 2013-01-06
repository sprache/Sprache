using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sprache
{
    /// <summary>
    /// An interface for objects that have a source position.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPositionAware<out T>
    {
        /// <summary>
        /// Set the start pos and the matched length.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        T SetPos(Position startPos, int length);
    }
}
