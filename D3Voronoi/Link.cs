namespace D3Voronoi
{
    /// <summary>
    /// Represents a connection or link between two points in a Voronoi diagram.
    /// Used to define edges and relationships between vertices in the diagram structure.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// The source point of the link connection.
        /// </summary>
        public Point Source;
        
        /// <summary>
        /// The target point of the link connection.
        /// </summary>
        public Point Target;
    }
}
