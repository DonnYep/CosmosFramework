namespace Quark.Editor
{
    public class EditorWaitForSeconds
    {
        /// <summary>
        /// The time to wait in seconds.
        /// </summary>
        public float WaitTime { get; }

        /// <summary>
        /// Creates a instruction object for yielding inside a generator function.
        /// </summary>
        /// <param name="time">The amount of time to wait in seconds.</param>
        public EditorWaitForSeconds(float time)
        {
            WaitTime = time;
        }
    }
}