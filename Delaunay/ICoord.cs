using System.Drawing;

namespace Delaunay
{
	/// <summary>
	/// Interface that defines a contract for objects that can provide coordinate information.
	/// Used by geometric objects that need to expose their position as a point.
	/// </summary>
	internal interface ICoord
	{
		/// <summary>
		/// Gets the coordinate position of the object as a PointF.
		/// </summary>
		/// <returns>A PointF representing the object's coordinate position.</returns>
		PointF Coord();
	}
}