namespace WorldMap.Layers.Interfaces
{
    /// <summary>
    /// Interface that defines a contract for objects that can be reset to their initial state.
    /// Implementing classes should clear their internal data when Reset is called.
    /// </summary>
    public interface IResetable
    {
        /// <summary>
        /// Resets the object to its initial state by clearing all internal data and cached values.
        /// </summary>
        void Reset();
    }
}
